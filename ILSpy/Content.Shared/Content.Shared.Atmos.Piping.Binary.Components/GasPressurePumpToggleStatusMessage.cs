using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasPressurePumpToggleStatusMessage(bool enabled) : BoundUserInterfaceMessage
{
	public bool Enabled { get; } = enabled;
}
