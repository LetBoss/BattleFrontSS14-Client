using System;
using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Finesse;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared.Coordinates;
using Content.Shared.Stunnable;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Xenonids.TailTrip;

public sealed class XenoTailTripSystem : EntitySystem
{
	[Dependency]
	private INetManager _net;

	[Dependency]
	private SharedStunSystem _stun;

	[Dependency]
	private RMCDazedSystem _daze;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private RMCSlowSystem _slow;

	[Dependency]
	private SharedRMCActionsSystem _rmcActions;

	[Dependency]
	private RMCSizeStunSystem _size;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<XenoTailTripComponent, XenoTailTripActionEvent>((EntityEventRefHandler<XenoTailTripComponent, XenoTailTripActionEvent>)OnXenoTailTripAction, (Type[])null, (Type[])null);
	}

	private void OnXenoTailTripAction(Entity<XenoTailTripComponent> xeno, ref XenoTailTripActionEvent args)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || !_rmcActions.TryUseAction(args))
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		if (_net.IsServer)
		{
			((EntitySystem)this).SpawnAttachedTo(EntProtoId.op_Implicit(xeno.Comp.TailEffect), args.Target.ToCoordinates(), (ComponentRegistry)null, default(Angle));
		}
		((EntitySystem)this).EnsureComp<XenoSweepingComponent>(Entity<XenoTailTripComponent>.op_Implicit(xeno));
		_audio.PlayPredicted(xeno.Comp.Sound, Entity<XenoTailTripComponent>.op_Implicit(xeno), (EntityUid?)Entity<XenoTailTripComponent>.op_Implicit(xeno), (AudioParams?)null);
		if (((EntitySystem)this).HasComp<XenoMarkedComponent>(args.Target))
		{
			if (!_size.TryGetSize(args.Target, out var size) || (int)size < 5)
			{
				_stun.TryParalyze(args.Target, xeno.Comp.MarkedStunTime, refresh: true);
			}
			_daze.TryDaze(args.Target, xeno.Comp.MarkedDazeTime, refresh: true, null, stutter: true);
			((EntitySystem)this).RemCompDeferred<XenoMarkedComponent>(args.Target);
		}
		else
		{
			if (!_size.TryGetSize(args.Target, out var size2) || (int)size2 < 5)
			{
				_stun.TryParalyze(args.Target, xeno.Comp.StunTime, refresh: true);
			}
			_slow.TrySlowdown(args.Target, xeno.Comp.SlowTime, refresh: true, ignoreDurationModifier: true);
		}
	}
}
