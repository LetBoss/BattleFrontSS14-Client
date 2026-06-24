using Content.Shared.ProximityDetection.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.ProximityDetection;

[ByRefEvent]
public readonly record struct ProximityTargetUpdatedEvent(float Distance, Entity<ProximityDetectorComponent> Detector, EntityUid? Target = null);
