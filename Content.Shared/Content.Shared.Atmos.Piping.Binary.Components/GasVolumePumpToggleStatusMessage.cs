using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Binary.Components;

[Serializable]
[NetSerializable]
public sealed class GasVolumePumpToggleStatusMessage : BoundUserInterfaceMessage
{
	public bool Enabled { get; }

	public GasVolumePumpToggleStatusMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
