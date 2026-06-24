using Robust.Shared.GameObjects;

namespace Content.Shared.StepTrigger.Systems;

[ByRefEvent]
public struct StepTriggerAttemptEvent
{
	public EntityUid Source;

	public EntityUid Tripper;

	public bool Continue;

	public bool Cancelled;
}
