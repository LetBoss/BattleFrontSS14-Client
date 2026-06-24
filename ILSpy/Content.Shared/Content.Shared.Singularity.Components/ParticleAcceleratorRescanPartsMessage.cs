using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Singularity.Components;

[Serializable]
[NetSerializable]
public sealed class ParticleAcceleratorRescanPartsMessage : BoundUserInterfaceMessage
{
}
