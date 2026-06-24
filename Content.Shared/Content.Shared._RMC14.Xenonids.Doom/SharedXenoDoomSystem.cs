using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Xenonids.Doom;

public abstract class SharedXenoDoomSystem : EntitySystem
{
	[Dependency]
	private SharedPointLightSystem _pointLight;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedRMCActionsSystem _rmcAction;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	protected EntityLookupSystem _entityLookup;

	[Dependency]
	protected SharedTransformSystem _transform;

	[Dependency]
	private ExamineSystemShared _examine;

	[Dependency]
	private RMCDazedSystem _daze;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private StatusEffectsSystem _status;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCCameraShakeSystem _cameraShake;

	[Dependency]
	private SharedSolutionContainerSystem _solution;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoDoomComponent, XenoDoomActionEvent>((EntityEventRefHandler<XenoDoomComponent, XenoDoomActionEvent>)OnXenoDoomAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightDoomedComponent, ComponentStartup>((EntityEventRefHandler<LightDoomedComponent, ComponentStartup>)OnDoomedLightAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightDoomedComponent, ComponentShutdown>((EntityEventRefHandler<LightDoomedComponent, ComponentShutdown>)OnDoomedLightRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightDoomedComponent, AttemptPointLightToggleEvent>((EntityEventRefHandler<LightDoomedComponent, AttemptPointLightToggleEvent>)OnDoomedLightAttemptToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightDoomedComponent, PointLightToggleEvent>((EntityEventRefHandler<LightDoomedComponent, PointLightToggleEvent>)OnDoomedLightToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<LightDoomedComponent, ItemToggleActivateAttemptEvent>((EntityEventRefHandler<LightDoomedComponent, ItemToggleActivateAttemptEvent>)OnDoomedLightItemToggle, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobDoomedComponent, ComponentStartup>((EntityEventRefHandler<MobDoomedComponent, ComponentStartup>)OnDoomedAdded, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MobDoomedComponent, ComponentShutdown>((EntityEventRefHandler<MobDoomedComponent, ComponentShutdown>)OnDoomedRemoved, (Type[])null, (Type[])null);
	}

	protected virtual void OnDoomedAdded(Entity<MobDoomedComponent> ent, ref ComponentStartup args)
	{
	}

	protected virtual void OnDoomedRemoved(Entity<MobDoomedComponent> ent, ref ComponentShutdown args)
	{
	}

	protected virtual void OnDoomedLightAdded(Entity<LightDoomedComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (!_pointLight.TryGetLight(Entity<LightDoomedComponent>.op_Implicit(ent), ref light))
		{
			((EntitySystem)this).RemCompDeferred<LightDoomedComponent>(Entity<LightDoomedComponent>.op_Implicit(ent));
			return;
		}
		_pointLight.SetEnabled(ent.Owner, false, (SharedPointLightComponent)null, (MetaDataComponent)null);
		ent.Comp.EndsAt = _timing.CurTime + ent.Comp.Duration;
	}

	private void OnDoomedLightRemoved(Entity<LightDoomedComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		SharedPointLightComponent light = default(SharedPointLightComponent);
		if (_pointLight.TryGetLight(Entity<LightDoomedComponent>.op_Implicit(ent), ref light))
		{
			_pointLight.SetEnabled(ent.Owner, ent.Comp.WasEnabled, (SharedPointLightComponent)null, (MetaDataComponent)null);
		}
	}

	private void OnDoomedLightItemToggle(Entity<LightDoomedComponent> ent, ref ItemToggleActivateAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ItemTogglePointLightComponent>(Entity<LightDoomedComponent>.op_Implicit(ent)))
		{
			args.Popup = base.Loc.GetString("rmc-doomed-fail");
			args.Cancelled = true;
		}
	}

	private void OnDoomedLightAttemptToggle(Entity<LightDoomedComponent> ent, ref AttemptPointLightToggleEvent args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		TimeSpan curTime = _timing.CurTime;
		TimeSpan? endsAt = ent.Comp.EndsAt;
		if (!(curTime >= endsAt))
		{
			if (ent.Comp.DoomActivated)
			{
				ent.Comp.WasEnabled = ((AttemptPointLightToggleEvent)(ref args)).Enabled;
			}
			if (((AttemptPointLightToggleEvent)(ref args)).Enabled)
			{
				args.Cancelled = true;
			}
		}
	}

	private void OnDoomedLightToggle(Entity<LightDoomedComponent> ent, ref PointLightToggleEvent args)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if (args.Enabled)
		{
			TimeSpan curTime = _timing.CurTime;
			TimeSpan? endsAt = ent.Comp.EndsAt;
			if (!(curTime >= endsAt))
			{
				ent.Comp.DoomActivated = true;
				_pointLight.SetEnabled(ent.Owner, false, (SharedPointLightComponent)null, (MetaDataComponent)null);
			}
		}
	}

	protected virtual void OnXenoDoomAction(Entity<XenoDoomComponent> xeno, ref XenoDoomActionEvent args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcAction.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoDoomComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoDoomComponent>.op_Implicit(xeno), (AudioParams?)null);
		((EntitySystem)this).PredictedSpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		((EntitySystem)this).PredictedSpawnAtPosition(EntProtoId.op_Implicit(xeno.Comp.Smoke), xeno.Owner.ToCoordinates(), (ComponentRegistry)null);
		_mobs.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(((EntitySystem)this).Transform(Entity<XenoDoomComponent>.op_Implicit(xeno)).Coordinates, xeno.Comp.Range, _mobs, (LookupFlags)110);
		foreach (Entity<MobStateComponent> mob in _mobs)
		{
			if (!_examine.InRangeUnOccluded(Entity<XenoDoomComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(mob)) || !_xeno.CanAbilityAttackTarget(Entity<XenoDoomComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(mob)))
			{
				continue;
			}
			_status.TryAddStatusEffect<RMCBlindedComponent>(Entity<MobStateComponent>.op_Implicit(mob), "Blinded", xeno.Comp.DazeTime, true, (StatusEffectsComponent?)null, false);
			_daze.TryDaze(Entity<MobStateComponent>.op_Implicit(mob), xeno.Comp.DazeTime);
			_slow.TrySuperSlowdown(Entity<MobStateComponent>.op_Implicit(mob), xeno.Comp.SlowTime, refresh: true, ignoreDurationModifier: true);
			if (!_solution.TryGetSolution(Entity<SolutionContainerManagerComponent>.op_Implicit(mob.Owner), xeno.Comp.TargetSolution, out Entity<SolutionComponent>? solEnt, out Solution solu))
			{
				if (solu == null || !solEnt.HasValue)
				{
					break;
				}
				foreach (ReagentPrototype chemical in solu.GetReagentPrototypes(_prototypeManager).Keys)
				{
					_solution.RemoveReagent(solEnt.Value, chemical.ID, xeno.Comp.RemovalPerReagent);
				}
			}
			_cameraShake.ShakeCamera(Entity<MobStateComponent>.op_Implicit(mob), 6, xeno.Comp.CameraShakeStrength);
			((EntitySystem)this).EnsureComp<MobDoomedComponent>(Entity<MobStateComponent>.op_Implicit(mob)).EndsAt = _timing.CurTime + xeno.Comp.OverlayTime;
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<LightDoomedComponent> query = ((EntitySystem)this).EntityQueryEnumerator<LightDoomedComponent>();
		EntityUid uid = default(EntityUid);
		LightDoomedComponent doomed = default(LightDoomedComponent);
		while (query.MoveNext(ref uid, ref doomed))
		{
			if (doomed.EndsAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? endsAt = doomed.EndsAt;
				if (value < endsAt)
				{
					continue;
				}
			}
			((EntitySystem)this).RemCompDeferred<LightDoomedComponent>(uid);
		}
		EntityQueryEnumerator<WaitingDoomComponent> queryWait = ((EntitySystem)this).EntityQueryEnumerator<WaitingDoomComponent>();
		EntityUid uid2 = default(EntityUid);
		WaitingDoomComponent wait = default(WaitingDoomComponent);
		LightDoomedComponent doom = default(LightDoomedComponent);
		while (queryWait.MoveNext(ref uid2, ref wait))
		{
			if (!(time < wait.DoomAt))
			{
				((EntitySystem)this).EnsureComp<LightDoomedComponent>(uid2, ref doom);
				((EntitySystem)this).RemCompDeferred<WaitingDoomComponent>(uid2);
			}
		}
		EntityQueryEnumerator<MobDoomedComponent> queryDoom = ((EntitySystem)this).EntityQueryEnumerator<MobDoomedComponent>();
		EntityUid uid3 = default(EntityUid);
		MobDoomedComponent doom2 = default(MobDoomedComponent);
		while (queryDoom.MoveNext(ref uid3, ref doom2))
		{
			if (doom2.EndsAt.HasValue)
			{
				TimeSpan value = time;
				TimeSpan? endsAt = doom2.EndsAt;
				if (value < endsAt)
				{
					continue;
				}
			}
			((EntitySystem)this).RemCompDeferred<MobDoomedComponent>(uid3);
		}
	}
}
