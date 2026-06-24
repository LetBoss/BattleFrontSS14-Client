using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Atmos.Piping.Trinary.Components;

[Serializable]
[NetSerializable]
public sealed class GasMixerToggleStatusMessage : BoundUserInterfaceMessage
{
	public bool Enabled { get; }

	public GasMixerToggleStatusMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
