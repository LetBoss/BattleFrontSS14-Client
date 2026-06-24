using Robust.Shared.GameObjects;

namespace Content.Shared.Tools.Components;

public sealed class ToolUseAttemptEvent(EntityUid user, float fuel) : CancellableEntityEventArgs
{
	public float Fuel = fuel;

	public EntityUid User { get; } = user;
}
