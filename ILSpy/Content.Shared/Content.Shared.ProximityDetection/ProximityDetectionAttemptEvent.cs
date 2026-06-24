using Content.Shared.ProximityDetection.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.ProximityDetection;

[ByRefEvent]
public struct ProximityDetectionAttemptEvent(float distance, Entity<ProximityDetectorComponent> detector, EntityUid target)
{
	public bool Cancelled = false;

	public readonly float Distance = distance;

	public readonly Entity<ProximityDetectorComponent> Detector = detector;

	public readonly EntityUid Target = target;
}
