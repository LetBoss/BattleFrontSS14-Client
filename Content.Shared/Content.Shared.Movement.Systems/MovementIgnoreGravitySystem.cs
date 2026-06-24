using System;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Systems;

public sealed class MovementIgnoreGravitySystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MovementIgnoreGravityComponent, ComponentGetState>((ComponentEventRefHandler<MovementIgnoreGravityComponent, ComponentGetState>)GetState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovementIgnoreGravityComponent, ComponentHandleState>((ComponentEventRefHandler<MovementIgnoreGravityComponent, ComponentHandleState>)HandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovementAlwaysTouchingComponent, CanWeightlessMoveEvent>((ComponentEventRefHandler<MovementAlwaysTouchingComponent, CanWeightlessMoveEvent>)OnWeightless, (Type[])null, (Type[])null);
	}

	private void OnWeightless(EntityUid uid, MovementAlwaysTouchingComponent component, ref CanWeightlessMoveEvent args)
	{
		args.CanMove = true;
	}

	private void HandleState(EntityUid uid, MovementIgnoreGravityComponent component, ref ComponentHandleState args)
	{
		if (((ComponentHandleState)(ref args)).Next != null)
		{
			component.Weightless = ((MovementIgnoreGravityComponentState)(object)((ComponentHandleState)(ref args)).Next).Weightless;
		}
	}

	private void GetState(EntityUid uid, MovementIgnoreGravityComponent component, ref ComponentGetState args)
	{
		((ComponentGetState)(ref args)).State = (IComponentState)(object)new MovementIgnoreGravityComponentState(component);
	}
}
