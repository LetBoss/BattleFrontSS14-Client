using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact;

[ByRefEvent]
public record struct ArtifactUnlockingFinishedEvent(EntityUid? UnlockedNode);
