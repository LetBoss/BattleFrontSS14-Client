using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Content.Shared.Administration.Logs;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Prototypes;
using Content.Shared.Database;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Components;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Anomaly;

public abstract class SharedAnomalySystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	protected IRobustRandom Random;

	[Dependency]
	protected ISharedAdminLogManager AdminLog;

	[Dependency]
	protected SharedAudioSystem Audio;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	[Dependency]
	private SharedPhysicsSystem _physics;

	[Dependency]
	protected SharedPopupSystem Popup;

	[Dependency]
	private IPrototypeManager _prototype;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _map;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<AnomalyComponent, MeleeThrowOnHitStartEvent>((EntityEventRefHandler<AnomalyComponent, MeleeThrowOnHitStartEvent>)OnAnomalyThrowStart, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<AnomalyComponent, LandEvent>((EntityEventRefHandler<AnomalyComponent, LandEvent>)OnLand, (Type[])null, (Type[])null);
	}

	private void OnAnomalyThrowStart(Entity<AnomalyComponent> ent, ref MeleeThrowOnHitStartEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Invalid comparison between Unknown and I4
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		CorePoweredThrowerComponent corePowered = default(CorePoweredThrowerComponent);
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<CorePoweredThrowerComponent>(args.Weapon, ref corePowered) && ((EntitySystem)this).TryComp<PhysicsComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref body))
		{
			PhysicsComponent physics = default(PhysicsComponent);
			if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref physics) && (int)physics.BodyType == 4)
			{
				_physics.SetBodyType(Entity<AnomalyComponent>.op_Implicit(ent), (BodyType)8, (FixturesComponent)null, body, (TransformComponent)null);
			}
			ChangeAnomalyStability(Entity<AnomalyComponent>.op_Implicit(ent), Random.NextFloat(corePowered.StabilityPerThrow.X, corePowered.StabilityPerThrow.Y), ent.Comp);
		}
	}

	private void OnLand(Entity<AnomalyComponent> ent, ref LandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		PhysicsComponent body = default(PhysicsComponent);
		if (((EntitySystem)this).TryComp<PhysicsComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref body) && (int)body.BodyType == 8)
		{
			_physics.SetBodyType(Entity<AnomalyComponent>.op_Implicit(ent), (BodyType)4, (FixturesComponent)null, (PhysicsComponent)null, (TransformComponent)null);
		}
	}

	public void DoAnomalyPulse(EntityUid uid, AnomalyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true) && Timing.IsFirstTimePredicted)
		{
			RefreshPulseTimer(uid, component);
			if (_net.IsServer)
			{
				((EntitySystem)this).Log.Info($"Performing anomaly pulse. Entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			if (component.Stability > component.GrowthThreshold)
			{
				ChangeAnomalySeverity(uid, GetSeverityIncreaseFromGrowth(component), component);
			}
			float minStability = component.PulseStabilityVariation.X * component.Severity;
			float maxStability = component.PulseStabilityVariation.Y * component.Severity;
			float stability = Random.NextFloat(minStability, maxStability);
			ChangeAnomalyStability(uid, stability, component);
			ISharedAdminLogManager adminLog = AdminLog;
			LogStringHandler handler = new LogStringHandler(31, 2);
			handler.AppendLiteral("Anomaly ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			handler.AppendLiteral(" pulsed with severity ");
			handler.AppendFormatted(component.Severity, "component.Severity");
			handler.AppendLiteral(".");
			adminLog.Add(LogType.Anomaly, LogImpact.Medium, ref handler);
			if (_net.IsServer)
			{
				Audio.PlayPvs(component.PulseSound, uid, (AudioParams?)null);
			}
			AnomalyPulsingComponent pulse = ((EntitySystem)this).EnsureComp<AnomalyPulsingComponent>(uid);
			pulse.EndTime = Timing.CurTime + pulse.PulseDuration;
			Appearance.SetData(uid, (Enum)AnomalyVisuals.IsPulsing, (object)true, (AppearanceComponent)null);
			float powerMod = 1f;
			if (component.CurrentBehavior.HasValue)
			{
				IPrototypeManager prototype = _prototype;
				ProtoId<AnomalyBehaviorPrototype>? currentBehavior = component.CurrentBehavior;
				powerMod = prototype.Index<AnomalyBehaviorPrototype>(currentBehavior.HasValue ? ProtoId<AnomalyBehaviorPrototype>.op_Implicit(currentBehavior.GetValueOrDefault()) : null).PulsePowerModifier;
			}
			AnomalyPulseEvent ev = new AnomalyPulseEvent(uid, component.Stability, component.Severity, powerMod);
			((EntitySystem)this).RaiseLocalEvent<AnomalyPulseEvent>(uid, ref ev, true);
		}
	}

	public void RefreshPulseTimer(EntityUid uid, AnomalyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true))
		{
			float variation = Random.NextFloat(0f - component.PulseVariation, component.PulseVariation) + 1f;
			component.NextPulseTime = Timing.CurTime + GetPulseLength(component) * variation;
		}
	}

	public void StartSupercriticalEvent(Entity<AnomalyComponent?> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<AnomalySupercriticalComponent>(Entity<AnomalyComponent>.op_Implicit(ent)) && ((EntitySystem)this).Resolve<AnomalyComponent>(Entity<AnomalyComponent>.op_Implicit(ent), ref ent.Comp, true))
		{
			ISharedAdminLogManager adminLog = AdminLog;
			LogStringHandler handler = new LogStringHandler(35, 1);
			handler.AppendLiteral("Anomaly ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner)), "ToPrettyString(ent.Owner)");
			handler.AppendLiteral(" began to go supercritical.");
			adminLog.Add(LogType.Anomaly, LogImpact.High, ref handler);
			if (_net.IsServer)
			{
				((EntitySystem)this).Log.Info($"Anomaly is going supercritical. Entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(ent.Owner))}");
			}
			Audio.PlayPvs(ent.Comp.SupercriticalSoundAtAnimationStart, ((EntitySystem)this).Transform(Entity<AnomalyComponent>.op_Implicit(ent)).Coordinates, (AudioParams?)null);
			AnomalySupercriticalComponent super = ((EntitySystem)this).AddComp<AnomalySupercriticalComponent>(Entity<AnomalyComponent>.op_Implicit(ent));
			super.EndTime = Timing.CurTime + super.SupercriticalDuration;
			Appearance.SetData(Entity<AnomalyComponent>.op_Implicit(ent), (Enum)AnomalyVisuals.Supercritical, (object)true, (AppearanceComponent)null);
			((EntitySystem)this).Dirty(Entity<AnomalyComponent>.op_Implicit(ent), (IComponent)(object)super, (MetaDataComponent)null);
		}
	}

	public void DoAnomalySupercriticalEvent(EntityUid uid, AnomalyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true) && Timing.IsFirstTimePredicted)
		{
			if (_net.IsServer)
			{
				Audio.PlayPvs(component.SupercriticalSound, ((EntitySystem)this).Transform(uid).Coordinates, (AudioParams?)null);
				((EntitySystem)this).Log.Info($"Raising supercritical event. Entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			float powerMod = 1f;
			if (component.CurrentBehavior.HasValue)
			{
				IPrototypeManager prototype = _prototype;
				ProtoId<AnomalyBehaviorPrototype>? currentBehavior = component.CurrentBehavior;
				powerMod = prototype.Index<AnomalyBehaviorPrototype>(currentBehavior.HasValue ? ProtoId<AnomalyBehaviorPrototype>.op_Implicit(currentBehavior.GetValueOrDefault()) : null).PulsePowerModifier;
			}
			AnomalySupercriticalEvent ev = new AnomalySupercriticalEvent(uid, powerMod);
			((EntitySystem)this).RaiseLocalEvent<AnomalySupercriticalEvent>(uid, ref ev, true);
			EndAnomaly(uid, component, supercritical: true, spawnCore: true, logged: true);
		}
	}

	public void EndAnomaly(EntityUid uid, AnomalyComponent? component = null, bool supercritical = false, bool spawnCore = true, bool logged = false)
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (logged)
		{
			if (_net.IsServer)
			{
				((EntitySystem)this).Log.Info($"Ending anomaly. Entity: {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			ISharedAdminLogManager adminLog = AdminLog;
			int impact = (supercritical ? 1 : (-1));
			LogStringHandler handler = new LogStringHandler(10, 2);
			handler.AppendLiteral("Anomaly ");
			handler.AppendFormatted<EntityStringRepresentation>(((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid)), "ToPrettyString(uid)");
			handler.AppendLiteral(" ");
			handler.AppendFormatted(supercritical ? "went supercritical" : "decayed");
			handler.AppendLiteral(".");
			adminLog.Add(LogType.Anomaly, (LogImpact)impact, ref handler);
		}
		if (!((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true))
		{
			return;
		}
		AnomalyShutdownEvent ev = new AnomalyShutdownEvent(uid, supercritical);
		((EntitySystem)this).RaiseLocalEvent<AnomalyShutdownEvent>(uid, ref ev, true);
		if (!((EntitySystem)this).Terminating(uid, (MetaDataComponent)null) && !_net.IsClient)
		{
			if (spawnCore)
			{
				EntProtoId? val = (supercritical ? component.CorePrototype : component.CoreInertPrototype);
				EntityUid core = ((EntitySystem)this).Spawn(val.HasValue ? EntProtoId.op_Implicit(val.GetValueOrDefault()) : null, ((EntitySystem)this).Transform(uid).Coordinates);
				_transform.PlaceNextTo(Entity<TransformComponent>.op_Implicit(core), Entity<TransformComponent>.op_Implicit(uid));
			}
			if (component.DeleteEntity)
			{
				((EntitySystem)this).QueueDel((EntityUid?)uid);
			}
			else
			{
				((EntitySystem)this).RemCompDeferred<AnomalySupercriticalComponent>(uid);
			}
		}
	}

	public void ChangeAnomalyStability(EntityUid uid, float change, AnomalyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true))
		{
			float newVal = component.Stability + change;
			component.Stability = Math.Clamp(newVal, 0f, 1f);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			AnomalyStabilityChangedEvent ev = new AnomalyStabilityChangedEvent(uid, component.Stability, component.Severity);
			((EntitySystem)this).RaiseLocalEvent<AnomalyStabilityChangedEvent>(uid, ref ev, true);
		}
	}

	public void ChangeAnomalySeverity(EntityUid uid, float change, AnomalyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true))
		{
			float newVal = component.Severity + change;
			if (newVal >= 1f)
			{
				StartSupercriticalEvent(Entity<AnomalyComponent>.op_Implicit((uid, component)));
			}
			component.Severity = Math.Clamp(newVal, 0f, 1f);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			AnomalySeverityChangedEvent ev = new AnomalySeverityChangedEvent(uid, component.Stability, component.Severity);
			((EntitySystem)this).RaiseLocalEvent<AnomalySeverityChangedEvent>(uid, ref ev, true);
		}
	}

	public void ChangeAnomalyHealth(EntityUid uid, float change, AnomalyComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<AnomalyComponent>(uid, ref component, true))
		{
			float newVal = component.Health + change;
			if (newVal < 0f)
			{
				EndAnomaly(uid, component, supercritical: false, spawnCore: true, logged: true);
				return;
			}
			component.Health = Math.Clamp(newVal, 0f, 1f);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			AnomalyHealthChangedEvent ev = new AnomalyHealthChangedEvent(uid, component.Health);
			((EntitySystem)this).RaiseLocalEvent<AnomalyHealthChangedEvent>(uid, ref ev, true);
		}
	}

	public TimeSpan GetPulseLength(AnomalyComponent component)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		float modifier = Math.Clamp((component.Stability - component.GrowthThreshold) / component.GrowthThreshold, 0f, 1f);
		TimeSpan lenght = (component.MaxPulseLength - component.MinPulseLength) * modifier + component.MinPulseLength;
		if (component.CurrentBehavior.HasValue)
		{
			AnomalyBehaviorPrototype behavior = _prototype.Index<AnomalyBehaviorPrototype>(component.CurrentBehavior.Value);
			lenght *= (double)behavior.PulseFrequencyModifier;
		}
		return lenght;
	}

	private float GetSeverityIncreaseFromGrowth(AnomalyComponent component)
	{
		return (1f + Math.Max(component.Stability - component.GrowthThreshold, 0f) * 10f) * component.SeverityGrowthCoefficient;
	}

	public override void Update(float frameTime)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		EntityQueryEnumerator<AnomalyComponent> anomalyQuery = ((EntitySystem)this).EntityQueryEnumerator<AnomalyComponent>();
		EntityUid ent = default(EntityUid);
		AnomalyComponent anomaly = default(AnomalyComponent);
		while (anomalyQuery.MoveNext(ref ent, ref anomaly))
		{
			if (anomaly.Stability < anomaly.DecayThreshold)
			{
				ChangeAnomalyHealth(ent, anomaly.HealthChangePerSecond * frameTime, anomaly);
			}
			if (Timing.CurTime > anomaly.NextPulseTime)
			{
				DoAnomalyPulse(ent, anomaly);
			}
		}
		EntityQueryEnumerator<AnomalyPulsingComponent> pulseQuery = ((EntitySystem)this).EntityQueryEnumerator<AnomalyPulsingComponent>();
		EntityUid ent2 = default(EntityUid);
		AnomalyPulsingComponent pulse = default(AnomalyPulsingComponent);
		while (pulseQuery.MoveNext(ref ent2, ref pulse))
		{
			if (Timing.CurTime > pulse.EndTime)
			{
				Appearance.SetData(ent2, (Enum)AnomalyVisuals.IsPulsing, (object)false, (AppearanceComponent)null);
				((EntitySystem)this).RemComp(ent2, (IComponent)(object)pulse);
			}
		}
		EntityQueryEnumerator<AnomalySupercriticalComponent, AnomalyComponent> supercriticalQuery = ((EntitySystem)this).EntityQueryEnumerator<AnomalySupercriticalComponent, AnomalyComponent>();
		EntityUid ent3 = default(EntityUid);
		AnomalySupercriticalComponent super = default(AnomalySupercriticalComponent);
		AnomalyComponent anom = default(AnomalyComponent);
		while (supercriticalQuery.MoveNext(ref ent3, ref super, ref anom))
		{
			if (!(Timing.CurTime <= super.EndTime))
			{
				DoAnomalySupercriticalEvent(ent3, anom);
			}
		}
	}

	public List<TileRef>? GetSpawningPoints(EntityUid uid, float stability, float severity, AnomalySpawnSettings settings, float powerModifier = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Invalid comparison between Unknown and I4
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		TransformComponent xform = ((EntitySystem)this).Transform(uid);
		MapGridComponent grid = default(MapGridComponent);
		if (!((EntitySystem)this).TryComp<MapGridComponent>(xform.GridUid, ref grid))
		{
			return null;
		}
		int amount = (int)(MathHelper.Lerp((float)settings.MinAmount, (float)settings.MaxAmount, severity * stability * powerModifier) + 0.5f);
		Vector2 worldPos = _transform.GetWorldPosition(uid);
		List<TileRef> tilerefs = _map.GetTilesIntersecting(xform.GridUid.Value, grid, new Box2(worldPos + new Vector2(0f - settings.MaxRange), worldPos + new Vector2(settings.MaxRange)), true, (Predicate<TileRef>)null).ToList();
		if (tilerefs.Count == 0)
		{
			return null;
		}
		EntityQuery<PhysicsComponent> physQuery = ((EntitySystem)this).GetEntityQuery<PhysicsComponent>();
		List<TileRef> resultList = new List<TileRef>();
		PhysicsComponent body = default(PhysicsComponent);
		while (resultList.Count < amount && tilerefs.Count != 0)
		{
			TileRef tileref = RandomExtensions.Pick<TileRef>(Random, (IReadOnlyList<TileRef>)tilerefs);
			float distance = Vector2.Distance(_map.GridTileToWorldPos(xform.GridUid.Value, grid, tileref.GridIndices), worldPos);
			if (distance > settings.MaxRange || distance < settings.MinRange)
			{
				tilerefs.Remove(tileref);
				continue;
			}
			if (!settings.CanSpawnOnEntities)
			{
				bool valid = true;
				foreach (EntityUid ent in _map.GetAnchoredEntities(xform.GridUid.Value, grid, tileref.GridIndices))
				{
					if (physQuery.TryGetComponent(ent, ref body) && (int)body.BodyType == 4 && body.Hard && (body.CollisionLayer & 2) != 0)
					{
						valid = false;
						break;
					}
				}
				if (!valid)
				{
					tilerefs.Remove(tileref);
					continue;
				}
			}
			resultList.Add(tileref);
		}
		return resultList;
	}
}
