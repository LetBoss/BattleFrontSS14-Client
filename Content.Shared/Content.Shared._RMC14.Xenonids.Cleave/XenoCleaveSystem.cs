using System;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Coordinates;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.Cleave;

public sealed class XenoCleaveSystem : EntitySystem
{
	[Dependency]
	private XenoSystem _xeno;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private VanguardShieldSystem _vanguard;

	[Dependency]
	private RMCPullingSystem _rmcPulling;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedRMCMeleeWeaponSystem _rmcMelee;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private RMCSizeStunSystem _sizeStun;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoCleaveComponent, XenoCleaveActionEvent>((EntityEventRefHandler<XenoCleaveComponent, XenoCleaveActionEvent>)OnCleaveAction, (Type[])null, (Type[])null);
	}

	private void OnCleaveAction(Entity<XenoCleaveComponent> xeno, ref XenoCleaveActionEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (!_xeno.CanAbilityAttackTarget(Entity<XenoCleaveComponent>.op_Implicit(xeno), args.Target) || ((HandledEntityEventArgs)args).Handled)
		{
			return;
		}
		if (_net.IsServer)
		{
			_audio.PlayPvs(xeno.Comp.Sound, Entity<XenoCleaveComponent>.op_Implicit(xeno), (AudioParams?)null);
		}
		bool buffed = _vanguard.ShieldBuff(Entity<XenoCleaveComponent>.op_Implicit(xeno));
		((HandledEntityEventArgs)args).Handled = true;
		_rmcMelee.DoLunge(Entity<XenoCleaveComponent>.op_Implicit(xeno), args.Target);
		if (args.Flings)
		{
			float flingRange = (buffed ? xeno.Comp.FlingDistanceBuffed : xeno.Comp.FlingDistance);
			if (_sizeStun.TryGetSize(args.Target, out var size) && (int)size >= 5)
			{
				flingRange *= 0.1f;
			}
			_rmcPulling.TryStopAllPullsFromAndOn(args.Target);
			MapCoordinates origin = _transform.GetMapCoordinates(Entity<XenoCleaveComponent>.op_Implicit(xeno), (TransformComponent)null);
			_sizeStun.KnockBack(args.Target, origin, flingRange, flingRange, 10f, ignoreSize: true);
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.FlingEffect), args.Target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
		else
		{
			TimeSpan rootTime = (buffed ? xeno.Comp.RootTimeBuffed : xeno.Comp.RootTime);
			_slow.TryRoot(args.Target, _xeno.TryApplyXenoDebuffMultiplier(args.Target, rootTime));
			if (_net.IsServer)
			{
				((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.RootEffect), args.Target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
			}
		}
	}
}
