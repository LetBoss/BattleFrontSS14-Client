using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Punch;

public sealed class XenoPunchSystem : EntitySystem
{
	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedColorFlashEffectSystem _colorFlash;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private RMCObstacleSlammingSystem _obstacleSlamming;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private RMCSizeStunSystem _size;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoPunchComponent, XenoPunchActionEvent>((EntityEventRefHandler<XenoPunchComponent, XenoPunchActionEvent>)OnXenoPunchAction, (Type[])null, (Type[])null);
	}

	private void OnXenoPunchAction(Entity<XenoPunchComponent> xeno, ref XenoPunchActionEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		XenoPunchAttemptEvent attempt = default(XenoPunchAttemptEvent);
		((EntitySystem)this).RaiseLocalEvent<XenoPunchAttemptEvent>(Entity<XenoPunchComponent>.op_Implicit(xeno), ref attempt, false);
		if (attempt.Cancelled)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoPunchComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		EntityUid targetId = args.Target;
		_rmcPulling.TryStopAllPullsFromAndOn(targetId);
		if (_damageable.TryChangeDamage(targetId, _xeno.TryApplyXenoSlashDamageMultiplier(targetId, xeno.Comp.Damage), ignoreResistances: false, interruptsDoAfters: true, null, Entity<XenoPunchComponent>.op_Implicit(xeno), Entity<XenoPunchComponent>.op_Implicit(xeno))?.GetTotal() > FixedPoint2.Zero)
		{
			Filter filter = Filter.Pvs(targetId, 2f, (IEntityManager)(object)base.EntityManager, (ISharedPlayerManager)null, (IConfigurationManager)null).RemoveWhereAttachedEntity((Predicate<EntityUid>)((EntityUid o) => o == xeno.Owner));
			_colorFlash.RaiseEffect(Color.Red, new List<EntityUid> { targetId }, filter);
		}
		MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoPunchComponent>.op_Implicit(xeno), (TransformComponent)null);
		_rmcMelee.DoLunge(Entity<XenoPunchComponent>.op_Implicit(xeno), targetId);
		_obstacleSlamming.MakeImmune(targetId);
		_size.KnockBack(targetId, origin, xeno.Comp.Range, xeno.Comp.Range, xeno.Comp.ThrowSpeed);
		if (!((EntitySystem)this).HasComp<XenoComponent>(targetId))
		{
			_slow.TrySlowdown(targetId, xeno.Comp.SlowDuration);
		}
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.Effect), targetId.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
	}
}
