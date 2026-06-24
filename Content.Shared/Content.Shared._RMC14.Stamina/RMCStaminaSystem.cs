using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Stun;
using Content.Shared.Administration.Logs;
using Content.Shared.Alert;
using Content.Shared.Damage.Events;
using Content.Shared.Database;
using Content.Shared.Effects;
using Content.Shared.Movement.Systems;
using Content.Shared.Projectiles;
using Content.Shared.Rejuvenate;
using Content.Shared.Speech.EntitySystems;
using Content.Shared.StatusEffect;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Wieldable.Components;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Stamina;

public sealed class RMCStaminaSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private SharedStutteringSystem _stutter;

	[Dependency]
	private RMCDazedSystem _daze;

	[Dependency]
	private MovementSpeedModifierSystem _speed;

	[Dependency]
	private TemporarySpeedModifiersSystem _temporarySpeed;

	[Dependency]
	private SharedColorFlashEffectSystem _color;

	[Dependency]
	private ISharedAdminLogManager _adminLogger;

	[Dependency]
	private AlertsSystem _alerts;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<RMCStaminaComponent, ComponentStartup>((EntityEventRefHandler<RMCStaminaComponent, ComponentStartup>)OnStaminaStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStaminaComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<RMCStaminaComponent, RefreshMovementSpeedModifiersEvent>)OnStaminaMovementSpeedModify, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStaminaComponent, RejuvenateEvent>((EntityEventRefHandler<RMCStaminaComponent, RejuvenateEvent>)OnStaminaRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStaminaDamageOnHitComponent, MeleeHitEvent>((EntityEventRefHandler<RMCStaminaDamageOnHitComponent, MeleeHitEvent>)OnStaminaOnHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStaminaDamageOnCollideComponent, ProjectileHitEvent>((EntityEventRefHandler<RMCStaminaDamageOnCollideComponent, ProjectileHitEvent>)OnStaminaOnProjectileHit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCStaminaDamageOnCollideComponent, ThrowDoHitEvent>((EntityEventRefHandler<RMCStaminaDamageOnCollideComponent, ThrowDoHitEvent>)OnStaminaOnThrowHit, (Type[])null, (Type[])null);
	}

	private void OnStaminaStartup(Entity<RMCStaminaComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetStaminaAlert(ent);
	}

	private void OnStaminaRejuvenate(Entity<RMCStaminaComponent> ent, ref RejuvenateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit((Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp)), 0.0 - ent.Comp.Max, visual: false);
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<RMCStaminaComponent> query = ((EntitySystem)this).EntityQueryEnumerator<RMCStaminaComponent>();
		EntityUid uid = default(EntityUid);
		RMCStaminaComponent stamina = default(RMCStaminaComponent);
		while (query.MoveNext(ref uid, ref stamina))
		{
			if (stamina.Current != stamina.Max)
			{
				if (time >= stamina.NextRegen)
				{
					DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit((uid, stamina)), -stamina.RegenPerTick);
				}
				else if (time >= stamina.NextCheck)
				{
					ProcessEffects(Entity<RMCStaminaComponent>.op_Implicit((uid, stamina)));
				}
			}
		}
	}

	public void DoStaminaDamage(Entity<RMCStaminaComponent?> ent, double amount, bool visual = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<RMCStaminaComponent>(Entity<RMCStaminaComponent>.op_Implicit(ent), ref ent.Comp, false))
		{
			ent.Comp.Current = Math.Clamp(ent.Comp.Current - amount, 0.0, ent.Comp.Max);
			if (visual && amount > 0.0 && _timing.IsFirstTimePredicted)
			{
				_color.RaiseEffect(Color.Aqua, new List<EntityUid> { Entity<RMCStaminaComponent>.op_Implicit(ent) }, Filter.Pvs(Entity<RMCStaminaComponent>.op_Implicit(ent), 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null));
			}
			ProcessEffects(Entity<RMCStaminaComponent>.op_Implicit((Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp)));
			ent.Comp.NextRegen = _timing.CurTime + ((amount > 0.0) ? ent.Comp.RestPeriod : ent.Comp.TimeBetweenChecks);
			SetStaminaAlert(Entity<RMCStaminaComponent>.op_Implicit((Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp)));
		}
	}

	private void ProcessEffects(Entity<RMCStaminaComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextCheck = _timing.CurTime + ent.Comp.TimeBetweenChecks;
		int newLevel;
		for (newLevel = 0; newLevel < ent.Comp.TierThresholds.Length && !(ent.Comp.Current >= (double)ent.Comp.TierThresholds[newLevel]); newLevel++)
		{
		}
		if (newLevel >= 2)
		{
			_status.TryAddStatusEffect<RMCBlindedComponent>(Entity<RMCStaminaComponent>.op_Implicit(ent), "Blinded", ent.Comp.EffectTime, true, (StatusEffectsComponent?)null, false);
			_stutter.DoStutter(Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp.EffectTime, refresh: true);
		}
		if (newLevel >= 3 && newLevel != ent.Comp.Level)
		{
			_daze.TryDaze(Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp.EffectTime, refresh: true, null, stutter: true);
		}
		if (newLevel >= 4)
		{
			_sizeStun.TryKnockOut(Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp.EffectTime);
		}
		int oldLevel = ent.Comp.Level;
		ent.Comp.Level = newLevel;
		((EntitySystem)this).Dirty<RMCStaminaComponent>(ent, (MetaDataComponent)null);
		if (newLevel != oldLevel)
		{
			_speed.RefreshMovementSpeedModifiers(Entity<RMCStaminaComponent>.op_Implicit(ent));
		}
	}

	private void OnStaminaMovementSpeedModify(Entity<RMCStaminaComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Level <= ent.Comp.SpeedModifiers.Length && ent.Comp.Level >= 0)
		{
			float? multiplier = _temporarySpeed.CalculateSpeedModifier(Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp.SpeedModifiers[ent.Comp.Level]);
			if (multiplier.HasValue)
			{
				args.ModifySpeed(multiplier.Value, multiplier.Value);
			}
		}
	}

	private void OnStaminaOnHit(Entity<RMCStaminaDamageOnHitComponent> ent, ref MeleeHitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		WieldableComponent wieldable = default(WieldableComponent);
		if ((ent.Comp.RequiresWield && ((EntitySystem)this).TryComp<WieldableComponent>(ent.Owner, ref wieldable) && !wieldable.Wielded) || !args.IsHit || !args.HitEntities.Any() || ent.Comp.Damage <= 0.0)
		{
			return;
		}
		StaminaDamageOnHitAttemptEvent ev = default(StaminaDamageOnHitAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<StaminaDamageOnHitAttemptEvent>(Entity<RMCStaminaDamageOnHitComponent>.op_Implicit(ent), ref ev, false);
		if (ev.Cancelled)
		{
			return;
		}
		EntityQuery<RMCStaminaComponent> stamQuery = ((EntitySystem)this).GetEntityQuery<RMCStaminaComponent>();
		List<(EntityUid, RMCStaminaComponent)> toHit = new List<(EntityUid, RMCStaminaComponent)>();
		RMCStaminaComponent stam = default(RMCStaminaComponent);
		foreach (EntityUid hit in args.HitEntities)
		{
			if (stamQuery.TryGetComponent(hit, ref stam))
			{
				toHit.Add((hit, stam));
			}
		}
		double damage = ent.Comp.Damage;
		foreach (var item in toHit)
		{
			EntityUid hit2 = item.Item1;
			DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit(hit2), damage / (double)toHit.Count);
			ISharedAdminLogManager adminLogger = _adminLogger;
			LogStringHandler handler = new LogStringHandler(39, 4);
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(hit2)), "target", "ToPrettyString(hit)");
			handler.AppendLiteral(" was dealt ");
			handler.AppendFormatted(damage, "damage");
			handler.AppendLiteral(" stamina damage from ");
			handler.AppendFormatted<EntityUid>(args.User, "args.User");
			handler.AppendLiteral(" with ");
			handler.AppendFormatted<EntityUid>(args.Weapon, "args.Weapon");
			handler.AppendLiteral(".");
			adminLogger.Add(LogType.Stamina, ref handler);
		}
	}

	private void OnStaminaOnProjectileHit(Entity<RMCStaminaDamageOnCollideComponent> ent, ref ProjectileHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		OnCollide(ent, args.Target);
	}

	private void OnStaminaOnThrowHit(Entity<RMCStaminaDamageOnCollideComponent> ent, ref ThrowDoHitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		OnCollide(ent, args.Target);
	}

	private void OnCollide(Entity<RMCStaminaDamageOnCollideComponent> ent, EntityUid target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		RMCStaminaComponent stam = default(RMCStaminaComponent);
		if (((EntitySystem)this).TryComp<RMCStaminaComponent>(target, ref stam))
		{
			DoStaminaDamage(Entity<RMCStaminaComponent>.op_Implicit((target, stam)), ent.Comp.Damage);
		}
	}

	private void SetStaminaAlert(Entity<RMCStaminaComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (!ent.Comp.ShowAlert)
		{
			_alerts.ClearAlert(Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp.StaminaAlert);
		}
		else
		{
			_alerts.ShowAlert(Entity<RMCStaminaComponent>.op_Implicit(ent), ent.Comp.StaminaAlert, (short)(ent.Comp.TierThresholds.Length - 1 - ent.Comp.Level));
		}
	}
}
