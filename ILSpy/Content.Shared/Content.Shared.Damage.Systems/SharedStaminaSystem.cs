using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Armor;
using Content.Shared.CCVar;
using Content.Shared.CombatMode;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Rounding;
using Content.Shared.Stunnable;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared.Damage.Systems;

public abstract class SharedStaminaSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private MetaDataSystem _metadata;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private SharedStunSystem _stunSystem;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private IConfigurationManager _config;

	private static readonly TimeSpan StamCritBufferTime = TimeSpan.FromSeconds(3.0);

	public float UniversalStaminaDamageModifier { get; private set; } = 1f;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeModifier();
		InitializeResistance();
		((EntitySystem)this).SubscribeLocalEvent<StaminaComponent, ComponentStartup>((ComponentEventHandler<StaminaComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaComponent, ComponentShutdown>((ComponentEventHandler<StaminaComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaComponent, AfterAutoHandleStateEvent>((ComponentEventRefHandler<StaminaComponent, AfterAutoHandleStateEvent>)OnStamHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaComponent, DisarmedEvent>((ComponentEventRefHandler<StaminaComponent, DisarmedEvent>)OnDisarmed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaComponent, RejuvenateEvent>((ComponentEventHandler<StaminaComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaDamageOnEmbedComponent, EmbedEvent>((ComponentEventRefHandler<StaminaDamageOnEmbedComponent, EmbedEvent>)OnProjectileEmbed, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaDamageOnCollideComponent, ProjectileHitEvent>((ComponentEventRefHandler<StaminaDamageOnCollideComponent, ProjectileHitEvent>)OnProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaDamageOnCollideComponent, ThrowDoHitEvent>((ComponentEventHandler<StaminaDamageOnCollideComponent, ThrowDoHitEvent>)OnThrowHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaDamageOnHitComponent, MeleeHitEvent>((ComponentEventHandler<StaminaDamageOnHitComponent, MeleeHitEvent>)OnMeleeHit, (Type[])null, (Type[])null);
		EntitySystemSubscriptionExt.CVar<float>(((EntitySystem)this).Subs, _config, CCVars.PlaytestStaminaDamageModifier, (Action<float>)delegate(float value)
		{
			UniversalStaminaDamageModifier = value;
		}, true);
	}

	private void OnStamHandleState(EntityUid uid, StaminaComponent component, ref AfterAutoHandleStateEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (component.Critical)
		{
			EnterStamCrit(uid, component);
			return;
		}
		if (component.StaminaDamage > 0f)
		{
			((EntitySystem)this).EnsureComp<ActiveStaminaComponent>(uid);
		}
		ExitStamCrit(uid, component);
	}

	private void OnShutdown(EntityUid uid, StaminaComponent component, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)((EntitySystem)this).MetaData(uid).EntityLifeStage < 4)
		{
			((EntitySystem)this).RemCompDeferred<ActiveStaminaComponent>(uid);
		}
		_alerts.ClearAlert(uid, component.StaminaAlert);
	}

	private void OnStartup(EntityUid uid, StaminaComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetStaminaAlert(uid, component);
	}

	public float GetStaminaDamage(EntityUid uid, StaminaComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StaminaComponent>(uid, ref component, true))
		{
			return 0f;
		}
		TimeSpan curTime = _timing.CurTime;
		TimeSpan pauseTime = _metadata.GetPauseTime(uid, (MetaDataComponent)null);
		return MathF.Max(0f, component.StaminaDamage - MathF.Max(0f, (float)(curTime - (component.NextUpdate + pauseTime)).TotalSeconds * component.Decay));
	}

	private void OnRejuvenate(EntityUid uid, StaminaComponent component, RejuvenateEvent args)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (component.StaminaDamage >= component.CritThreshold)
		{
			ExitStamCrit(uid, component);
		}
		component.StaminaDamage = 0f;
		AdjustSlowdown(Entity<StaminaComponent>.op_Implicit(uid));
		((EntitySystem)this).RemComp<ActiveStaminaComponent>(uid);
		SetStaminaAlert(uid, component);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void OnDisarmed(EntityUid uid, StaminaComponent component, ref DisarmedEvent args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled && !component.Critical)
		{
			float damage = args.PushProbability * component.CritThreshold;
			TakeStaminaDamage(uid, damage, component, args.Source);
			args.PopupPrefix = "disarm-action-shove-";
			args.IsStunned = component.Critical;
			args.Handled = true;
		}
	}

	private void OnMeleeHit(EntityUid uid, StaminaDamageOnHitComponent component, MeleeHitEvent args)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		if (!args.IsHit || !args.HitEntities.Any() || component.Damage <= 0f)
		{
			return;
		}
		StaminaDamageOnHitAttemptEvent ev = default(StaminaDamageOnHitAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>(uid, ref ev, false);
		if (ev.Cancelled)
		{
			return;
		}
		EntityQuery<StaminaComponent> stamQuery = ((EntitySystem)this).GetEntityQuery<StaminaComponent>();
		List<(EntityUid, StaminaComponent)> toHit = new List<(EntityUid, StaminaComponent)>();
		StaminaComponent stam = default(StaminaComponent);
		foreach (EntityUid ent in args.HitEntities)
		{
			if (stamQuery.TryGetComponent(ent, ref stam))
			{
				toHit.Add((ent, stam));
			}
		}
		StaminaMeleeHitEvent hitEvent = new StaminaMeleeHitEvent(toHit);
		((EntitySystem)this).RaiseLocalEvent<StaminaMeleeHitEvent>(uid, hitEvent, false);
		if (((HandledEntityEventArgs)hitEvent).Handled)
		{
			return;
		}
		float damage = component.Damage;
		damage *= hitEvent.Multiplier;
		damage += hitEvent.FlatModifier;
		foreach (var (ent2, comp) in toHit)
		{
			TakeStaminaDamage(ent2, damage / (float)toHit.Count, comp, args.User, args.Weapon, visual: true, component.Sound);
		}
	}

	private void OnProjectileHit(EntityUid uid, StaminaDamageOnCollideComponent component, ref ProjectileHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OnCollide(uid, component, args.Target);
	}

	private void OnProjectileEmbed(EntityUid uid, StaminaDamageOnEmbedComponent component, ref EmbedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		StaminaComponent stamina = default(StaminaComponent);
		if (((EntitySystem)this).TryComp<StaminaComponent>(args.Embedded, ref stamina))
		{
			TakeStaminaDamage(args.Embedded, component.Damage, stamina, uid);
		}
	}

	private void OnThrowHit(EntityUid uid, StaminaDamageOnCollideComponent component, ThrowDoHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OnCollide(uid, component, args.Target);
	}

	private void OnCollide(EntityUid uid, StaminaDamageOnCollideComponent component, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<StaminaComponent>(target))
		{
			StaminaDamageOnHitAttemptEvent ev = default(StaminaDamageOnHitAttemptEvent);
			((EntitySystem)this).RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>(uid, ref ev, false);
			if (!ev.Cancelled)
			{
				float damage = component.Damage;
				EntityUid? source = uid;
				SoundSpecifier sound = component.Sound;
				TakeStaminaDamage(target, damage, null, source, null, visual: true, sound);
			}
		}
	}

	private void SetStaminaAlert(EntityUid uid, StaminaComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StaminaComponent>(uid, ref component, false) && !((Component)component).Deleted)
		{
			if (!component.ShowAlert)
			{
				_alerts.ClearAlert(uid, component.StaminaAlert);
				return;
			}
			int severity = ContentHelpers.RoundToLevels(MathF.Max(0f, component.CritThreshold - component.StaminaDamage), component.CritThreshold, 7);
			_alerts.ShowAlert(uid, component.StaminaAlert, (short)severity);
		}
	}

	public bool TryTakeStamina(EntityUid uid, float value, StaminaComponent? component = null, EntityUid? source = null, EntityUid? with = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StaminaComponent>(uid, ref component, false))
		{
			return true;
		}
		if (component.StaminaDamage + value > component.CritThreshold || component.Critical)
		{
			return false;
		}
		TakeStaminaDamage(uid, value, component, source, with, visual: false);
		return true;
	}

	public void TakeStaminaDamage(EntityUid uid, float value, StaminaComponent? component = null, EntityUid? source = null, EntityUid? with = null, bool visual = true, SoundSpecifier? sound = null, bool ignoreResist = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StaminaComponent>(uid, ref component, false))
		{
			return;
		}
		BeforeStaminaDamageEvent ev = new BeforeStaminaDamageEvent(value);
		((EntitySystem)this).RaiseLocalEvent<BeforeStaminaDamageEvent>(uid, ref ev, false);
		if (ev.Cancelled)
		{
			return;
		}
		if (!ignoreResist)
		{
			value = ev.Value;
		}
		value = UniversalStaminaDamageModifier * value;
		if (component.Critical)
		{
			return;
		}
		float oldDamage = component.StaminaDamage;
		component.StaminaDamage = MathF.Max(0f, component.StaminaDamage + value);
		if (oldDamage < component.StaminaDamage)
		{
			TimeSpan nextUpdate = _timing.CurTime + TimeSpan.FromSeconds(component.Cooldown);
			if (component.NextUpdate < nextUpdate)
			{
				component.NextUpdate = nextUpdate;
			}
		}
		AdjustSlowdown(Entity<StaminaComponent>.op_Implicit(uid));
		SetStaminaAlert(uid, component);
		if (component.AfterCritical && oldDamage > component.StaminaDamage && component.StaminaDamage <= 0f)
		{
			component.AfterCritical = false;
		}
		if (!component.Critical)
		{
			if (component.StaminaDamage >= component.CritThreshold)
			{
				EnterStamCrit(uid, component);
			}
		}
		else if (component.StaminaDamage < component.CritThreshold)
		{
			ExitStamCrit(uid, component);
		}
		((EntitySystem)this).EnsureComp<ActiveStaminaComponent>(uid);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		if (!(value <= 0f))
		{
			if (source.HasValue)
			{
				ISharedAdminLogManager adminLogger = _adminLogger;
				LogStringHandler handler = new LogStringHandler(27, 4);
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(source.Value)), "user", "ToPrettyString(source.Value)");
				handler.AppendLiteral(" caused ");
				handler.AppendFormatted(value, "value");
				handler.AppendLiteral(" stamina damage to ");
				handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
				handler.AppendFormatted(with.HasValue ? $" using {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(with.Value)):using}" : "");
				adminLogger.Add(LogType.Stamina, ref handler);
			}
			else
			{
				ISharedAdminLogManager adminLogger2 = _adminLogger;
				LogStringHandler handler2 = new LogStringHandler(21, 2);
				handler2.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "target", "ToPrettyString(uid)");
				handler2.AppendLiteral(" took ");
				handler2.AppendFormatted(value, "value");
				handler2.AppendLiteral(" stamina damage");
				adminLogger2.Add(LogType.Stamina, ref handler2);
			}
			if (visual)
			{
				_color.RaiseEffect(Color.Aqua, new List<EntityUid> { uid }, Filter.Pvs(uid, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null));
			}
			if (_net.IsServer)
			{
				_audio.PlayPvs(sound, uid, (AudioParams?)null);
			}
		}
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQuery<StaminaComponent> stamQuery = ((EntitySystem)this).GetEntityQuery<StaminaComponent>();
		EntityQueryEnumerator<ActiveStaminaComponent> query = ((EntitySystem)this).EntityQueryEnumerator<ActiveStaminaComponent>();
		TimeSpan curTime = _timing.CurTime;
		EntityUid uid = default(EntityUid);
		ActiveStaminaComponent activeStaminaComponent = default(ActiveStaminaComponent);
		StaminaComponent comp = default(StaminaComponent);
		while (query.MoveNext(ref uid, ref activeStaminaComponent))
		{
			if (!stamQuery.TryGetComponent(uid, ref comp) || (comp.StaminaDamage <= 0f && !comp.Critical))
			{
				((EntitySystem)this).RemComp<ActiveStaminaComponent>(uid);
			}
			else if (!(comp.NextUpdate > curTime))
			{
				if (comp.Critical)
				{
					ExitStamCrit(uid, comp);
				}
				comp.NextUpdate += TimeSpan.FromSeconds(1.0);
				TakeStaminaDamage(uid, comp.AfterCritical ? ((0f - comp.Decay) * comp.AfterCritDecayMultiplier) : (0f - comp.Decay), comp);
				((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			}
		}
	}

	private void EnterStamCrit(EntityUid uid, StaminaComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StaminaComponent>(uid, ref component, true) && !component.Critical)
		{
			component.Critical = true;
			component.StaminaDamage = component.CritThreshold;
			_stunSystem.TryParalyze(uid, component.StunTime, refresh: true);
			component.NextUpdate = _timing.CurTime + component.StunTime + StamCritBufferTime;
			((EntitySystem)this).EnsureComp<ActiveStaminaComponent>(uid);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(21, 1);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "user", "ToPrettyString(uid)");
			handler.AppendLiteral(" entered stamina crit");
			adminLogger.Add(LogType.Stamina, LogImpact.Medium, ref handler);
		}
	}

	private void ExitStamCrit(EntityUid uid, StaminaComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<StaminaComponent>(uid, ref component, true) && component.Critical)
		{
			component.Critical = false;
			component.AfterCritical = true;
			component.NextUpdate = _timing.CurTime;
			SetStaminaAlert(uid, component);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(28, 1);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "user", "ToPrettyString(uid)");
			handler.AppendLiteral(" recovered from stamina crit");
			adminLogger.Add(LogType.Stamina, LogImpact.Low, ref handler);
		}
	}

	private void AdjustSlowdown(Entity<StaminaComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StaminaComponent>(Entity<StaminaComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			return;
		}
		FixedPoint2 closest = FixedPoint2.Zero;
		foreach (KeyValuePair<FixedPoint2, float> thres in ent.Comp.StunModifierThresholds)
		{
			float key = thres.Key.Float();
			if (ent.Comp.StaminaDamage >= key && key > closest && closest < ent.Comp.CritThreshold)
			{
				closest = thres.Key;
			}
		}
		_stunSystem.UpdateStunModifiers(ent, ent.Comp.StunModifierThresholds[closest]);
	}

	private void InitializeModifier()
	{
		((EntitySystem)this).SubscribeLocalEvent<StaminaModifierComponent, ComponentStartup>((ComponentEventHandler<StaminaModifierComponent, ComponentStartup>)OnModifierStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaModifierComponent, ComponentShutdown>((ComponentEventHandler<StaminaModifierComponent, ComponentShutdown>)OnModifierShutdown, (Type[])null, (Type[])null);
	}

	private void OnModifierStartup(EntityUid uid, StaminaModifierComponent comp, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StaminaComponent stamina = default(StaminaComponent);
		if (((EntitySystem)this).TryComp<StaminaComponent>(uid, ref stamina))
		{
			stamina.CritThreshold *= comp.Modifier;
		}
	}

	private void OnModifierShutdown(EntityUid uid, StaminaModifierComponent comp, ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		StaminaComponent stamina = default(StaminaComponent);
		if (((EntitySystem)this).TryComp<StaminaComponent>(uid, ref stamina))
		{
			stamina.CritThreshold /= comp.Modifier;
		}
	}

	public void SetModifier(EntityUid uid, float modifier, StaminaComponent? stamina = null, StaminaModifierComponent? comp = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<StaminaModifierComponent>(uid, ref comp, true))
		{
			return;
		}
		float old = comp.Modifier;
		if (!old.Equals(modifier))
		{
			comp.Modifier = modifier;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)comp, (MetaDataComponent)null);
			if (((EntitySystem)this).Resolve<StaminaComponent>(uid, ref stamina, false))
			{
				stamina.CritThreshold *= modifier / old;
			}
		}
	}

	private void InitializeResistance()
	{
		((EntitySystem)this).SubscribeLocalEvent<StaminaResistanceComponent, BeforeStaminaDamageEvent>((EntityEventRefHandler<StaminaResistanceComponent, BeforeStaminaDamageEvent>)OnGetResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaResistanceComponent, InventoryRelayedEvent<BeforeStaminaDamageEvent>>((EntityEventRefHandler<StaminaResistanceComponent, InventoryRelayedEvent<BeforeStaminaDamageEvent>>)RelayedResistance, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StaminaResistanceComponent, ArmorExamineEvent>((EntityEventRefHandler<StaminaResistanceComponent, ArmorExamineEvent>)OnArmorExamine, (Type[])null, (Type[])null);
	}

	private void OnGetResistance(Entity<StaminaResistanceComponent> ent, ref BeforeStaminaDamageEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Value *= ent.Comp.DamageCoefficient;
	}

	private void RelayedResistance(Entity<StaminaResistanceComponent> ent, ref InventoryRelayedEvent<BeforeStaminaDamageEvent> args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Worn)
		{
			OnGetResistance(ent, ref args.Args);
		}
	}

	private void OnArmorExamine(Entity<StaminaResistanceComponent> ent, ref ArmorExamineEvent args)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		float value = MathF.Round((1f - ent.Comp.DamageCoefficient) * 100f, 1);
		if (value != 0f)
		{
			args.Msg.PushNewline();
			args.Msg.AddMarkupOrThrow(base.Loc.GetString(LocId.op_Implicit(ent.Comp.Examine), (ValueTuple<string, object>)("value", value)));
		}
	}
}
