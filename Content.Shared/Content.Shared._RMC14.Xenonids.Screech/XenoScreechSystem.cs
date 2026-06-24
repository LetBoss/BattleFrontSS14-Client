using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Deafness;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Screech;

public sealed class XenoScreechSystem : EntitySystem
{
	[Dependency]
	private MobStateSystem _mobState;

	[Dependency]
	private XenoPlasmaSystem _xenoPlasma;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private ExamineSystemShared _examineSystem;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private SharedDeafnessSystem _deaf;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private XenoSystem _xeno;

	private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

	private readonly HashSet<Entity<MobStateComponent>> _closeMobs = new HashSet<Entity<MobStateComponent>>();

	private readonly HashSet<Entity<XenoParasiteComponent>> _parasites = new HashSet<Entity<XenoParasiteComponent>>();

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<XenoScreechComponent, XenoScreechActionEvent>((EntityEventRefHandler<XenoScreechComponent, XenoScreechActionEvent>)OnXenoScreechAction, (Type[])null, (Type[])null);
	}

	private void OnXenoScreechAction(Entity<XenoScreechComponent> xeno, ref XenoScreechActionEvent args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoScreechAttemptEvent attempt = default(XenoScreechAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoScreechAttemptEvent>(Entity<XenoScreechComponent>.op_Implicit(xeno), ref attempt, false);
		TransformComponent xform = default(TransformComponent);
		if (attempt.Cancelled || !_xenoPlasma.TryRemovePlasmaPopup(Entity<XenoPlasmaComponent>.op_Implicit(xeno.Owner), xeno.Comp.PlasmaCost) || !((EntitySystem)this).TryComp(Entity<XenoScreechComponent>.op_Implicit(xeno), ref xform))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoScreechComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		_closeMobs.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(xform.Coordinates, xeno.Comp.ParalyzeRange, _closeMobs, (LookupFlags)110);
		foreach (Entity<MobStateComponent> receiver in _closeMobs)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver)))
			{
				Stun(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.ParalyzeTime, stun: false);
				Deafen(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver), xeno.Comp.CloseDeafTime);
			}
		}
		_mobs.Clear();
		_entityLookup.GetEntitiesInRange<MobStateComponent>(xform.Coordinates, xeno.Comp.StunRange, _mobs, (LookupFlags)110);
		foreach (Entity<MobStateComponent> receiver2 in _mobs)
		{
			if (_xeno.CanAbilityAttackTarget(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver2)) && !_closeMobs.Contains(receiver2))
			{
				Stun(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver2), xeno.Comp.StunTime, stun: true);
				Deafen(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<MobStateComponent>.op_Implicit(receiver2), xeno.Comp.FarDeafTime);
			}
		}
		_parasites.Clear();
		_entityLookup.GetEntitiesInRange<XenoParasiteComponent>(xform.Coordinates, xeno.Comp.ParasiteStunRange, _parasites, (LookupFlags)110);
		foreach (Entity<XenoParasiteComponent> receiver3 in _parasites)
		{
			Stun(Entity<XenoScreechComponent>.op_Implicit(xeno), Entity<XenoParasiteComponent>.op_Implicit(receiver3), xeno.Comp.ParasiteStunTime, stun: true, occlusionCheck: false);
		}
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), xeno.Owner.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
	}

	private void Stun(EntityUid xeno, EntityUid receiver, TimeSpan time, bool stun, bool occlusionCheck = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsDead(receiver) && (!occlusionCheck || _examineSystem.InRangeUnOccluded(xeno, receiver)))
		{
			if (stun)
			{
				_stun.TryStun(receiver, time, refresh: false);
			}
			else
			{
				_stun.TryParalyze(receiver, time, refresh: false);
			}
		}
	}

	private void Deafen(EntityUid xeno, EntityUid receiver, TimeSpan time)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!_mobState.IsDead(receiver) && _examineSystem.InRangeUnOccluded(xeno, receiver))
		{
			_deaf.TryDeafen(receiver, time);
		}
	}
}
