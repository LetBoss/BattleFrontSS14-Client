using System.Runtime.InteropServices;
using Robust.Shared.GameObjects;

namespace Content.Shared.Xenoarchaeology.Artifact;

[StructLayout(LayoutKind.Sequential, Size = 1)]
[ByRefEvent]
public record struct ArtifactUnlockingStartedEvent;
