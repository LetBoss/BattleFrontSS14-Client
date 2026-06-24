using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components;

public sealed class EnsnareEvent : EntityEventArgs
{
	public readonly float WalkSpeed;

	public readonly float SprintSpeed;

	public EnsnareEvent(float walkSpeed, float sprintSpeed)
	{
		WalkSpeed = walkSpeed;
		SprintSpeed = sprintSpeed;
	}
}
