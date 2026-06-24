using Content.Shared.Xenoarchaeology.Artifact.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Shared.Xenoarchaeology.Artifact;

[ByRefEvent]
public readonly record struct XenoArtifactNodeActivatedEvent(Entity<XenoArtifactComponent> Artifact, Entity<XenoArtifactNodeComponent> Node, EntityUid? User, EntityUid? Target, EntityCoordinates Coordinates);
