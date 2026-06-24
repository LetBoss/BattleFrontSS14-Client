using System;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleRunoverSystem : EntitySystem
{
	[Dependency]
	private readonly EntityLookupSystem _lookup;

	[Dependency]
	private readonly SharedStunSystem _stun;

	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly INetManager _net;

	public static readonly TimeSpan StandUpGrace = TimeSpan.FromSeconds(0.6);

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleRunoverComponent, PreventCollideEvent>((EntityEventRefHandler<VehicleRunoverComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleRunoverComponent, StoodEvent>((EntityEventRefHandler<VehicleRunoverComponent, StoodEvent>)OnRunoverStood, (Type[])null, (Type[])null);
	}

	private void OnPreventCollide(Entity<VehicleRunoverComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.OtherEntity == ent.Comp.Vehicle)
		{
			args.Cancelled = true;
		}
	}

	private void OnRunoverStood(Entity<VehicleRunoverComponent> ent, ref StoodEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		if (((EntitySystem)this).TerminatingOrDeleted(ent.Comp.Vehicle, (MetaDataComponent)null))
		{
			((EntitySystem)this).RemCompDeferred<VehicleRunoverComponent>(Entity<VehicleRunoverComponent>.op_Implicit(ent));
			return;
		}
		TimeSpan graceUntil = _timing.CurTime + StandUpGrace;
		if (ent.Comp.ExpiresAt < graceUntil)
		{
			ent.Comp.ExpiresAt = graceUntil;
			((EntitySystem)this).Dirty<VehicleRunoverComponent>(ent, (MetaDataComponent)null);
		}
	}

	public override void Update(float frameTime)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient)
		{
			return;
		}
		TimeSpan time = _timing.CurTime;
		EntityQueryEnumerator<VehicleRunoverComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleRunoverComponent>();
		EntityUid uid = default(EntityUid);
		VehicleRunoverComponent comp = default(VehicleRunoverComponent);
		while (query.MoveNext(ref uid, ref comp))
		{
			if (((EntitySystem)this).TerminatingOrDeleted(comp.Vehicle, (MetaDataComponent)null))
			{
				((EntitySystem)this).RemCompDeferred<VehicleRunoverComponent>(uid);
			}
			else if (IsOverlapping(uid, comp.Vehicle))
			{
				if (comp.Duration == TimeSpan.Zero)
				{
					comp.Duration = TimeSpan.FromSeconds(1.5);
				}
				comp.ExpiresAt = time + comp.Duration + StandUpGrace;
				_stun.TryKnockdown(uid, comp.Duration, refresh: true);
			}
			else if (comp.ExpiresAt <= time)
			{
				((EntitySystem)this).RemCompDeferred<VehicleRunoverComponent>(uid);
			}
		}
	}

	private bool IsOverlapping(EntityUid mob, EntityUid vehicle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Box2 mobAabb = _lookup.GetWorldAABB(mob, (TransformComponent)null);
		Box2 vehicleAabb = _lookup.GetWorldAABB(vehicle, (TransformComponent)null);
		return ((Box2)(ref mobAabb)).Intersects(ref vehicleAabb);
	}
}
