using Robust.Shared.GameObjects;

namespace Content.Shared.Ensnaring.Components;

public sealed class EnsnareRemoveEvent : CancellableEntityEventArgs
{
	public readonly float WalkSpeed;

	public readonly float SprintSpeed;

	public EnsnareRemoveEvent(float walkSpeed, float sprintSpeed)
	{
		WalkSpeed = walkSpeed;
		SprintSpeed = sprintSpeed;
	}
}
