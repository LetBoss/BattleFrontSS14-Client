using System;
using Content.Shared.Camera;
using Content.Shared.Movement.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleGunnerViewSystem : EntitySystem
{
	[Dependency]
	private readonly SharedContentEyeSystem _eye;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerViewUserComponent, GetEyePvsScaleEvent>((EntityEventRefHandler<VehicleGunnerViewUserComponent, GetEyePvsScaleEvent>)OnGetEyePvsScale, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerViewUserComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<VehicleGunnerViewUserComponent, AfterAutoHandleStateEvent>)OnHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerViewUserComponent, ComponentStartup>((EntityEventRefHandler<VehicleGunnerViewUserComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleGunnerViewUserComponent, ComponentShutdown>((EntityEventRefHandler<VehicleGunnerViewUserComponent, ComponentShutdown>)OnShutdown, (Type[])null, (Type[])null);
	}

	private void OnGetEyePvsScale(Entity<VehicleGunnerViewUserComponent> ent, ref GetEyePvsScaleEvent args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		args.Scale += ent.Comp.PvsScale + ent.Comp.CursorPvsIncrease;
	}

	private void OnHandleState(Entity<VehicleGunnerViewUserComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_eye.UpdatePvsScale(ent.Owner);
	}

	private void OnStartup(Entity<VehicleGunnerViewUserComponent> ent, ref ComponentStartup args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_eye.UpdatePvsScale(ent.Owner);
	}

	private void OnShutdown(Entity<VehicleGunnerViewUserComponent> ent, ref ComponentShutdown args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_eye.UpdatePvsScale(ent.Owner);
	}
}
