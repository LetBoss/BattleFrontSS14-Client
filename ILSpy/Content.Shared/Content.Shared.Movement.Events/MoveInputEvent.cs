using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;

namespace Content.Shared.Movement.Events;

[ByRefEvent]
public readonly struct MoveInputEvent
{
	public readonly Entity<InputMoverComponent> Entity;

	public readonly MoveButtons OldMovement;

	public bool HasDirectionalMovement => (Entity.Comp.HeldMoveButtons & MoveButtons.AnyDirection) != 0;

	public MoveInputEvent(Entity<InputMoverComponent> entity, MoveButtons oldMovement)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		OldMovement = oldMovement;
	}
}
