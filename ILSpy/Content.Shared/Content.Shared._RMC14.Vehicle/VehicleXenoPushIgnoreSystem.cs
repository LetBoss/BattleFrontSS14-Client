using System;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleXenoPushIgnoreSystem : EntitySystem
{
	[Dependency]
	private readonly INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleXenoPushIgnoreComponent, PreventCollideEvent>((EntityEventRefHandler<VehicleXenoPushIgnoreComponent, PreventCollideEvent>)OnPreventCollide, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleXenoPushIgnoreComponent, LandEvent>((EntityEventRefHandler<VehicleXenoPushIgnoreComponent, LandEvent>)OnLand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleXenoPushIgnoreComponent, StopThrowEvent>((EntityEventRefHandler<VehicleXenoPushIgnoreComponent, StopThrowEvent>)OnStopThrow, (Type[])null, (Type[])null);
	}

	private void OnPreventCollide(Entity<VehicleXenoPushIgnoreComponent> ent, ref PreventCollideEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ThrownItemComponent>(Entity<VehicleXenoPushIgnoreComponent>.op_Implicit(ent)))
		{
			args.Cancelled = true;
		}
	}

	private void OnLand(Entity<VehicleXenoPushIgnoreComponent> ent, ref LandEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).RemCompDeferred<VehicleXenoPushIgnoreComponent>(Entity<VehicleXenoPushIgnoreComponent>.op_Implicit(ent));
		}
	}

	private void OnStopThrow(Entity<VehicleXenoPushIgnoreComponent> ent, ref StopThrowEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient)
		{
			((EntitySystem)this).RemCompDeferred<VehicleXenoPushIgnoreComponent>(Entity<VehicleXenoPushIgnoreComponent>.op_Implicit(ent));
		}
	}
}
