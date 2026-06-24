using System;
using Content.Shared._RMC14.Vehicle.Viewport;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Vehicle.Viewport;

public sealed class VehicleViewportSystem : EntitySystem
{
	[Dependency]
	private readonly SharedEyeSystem _eye;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<VehicleViewportUserComponent, MoveInputEvent>((EntityEventRefHandler<VehicleViewportUserComponent, MoveInputEvent>)OnUserMove, (Type[])null, (Type[])null);
	}

	private void OnUserMove(Entity<VehicleViewportUserComponent> ent, ref MoveInputEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		EyeComponent val = default(EyeComponent);
		if (args.HasDirectionalMovement && ((EntitySystem)this).TryComp<EyeComponent>(Entity<VehicleViewportUserComponent>.op_Implicit(ent), ref val))
		{
			_eye.SetTarget(ent.Owner, ent.Comp.PreviousTarget, val);
		}
	}
}
