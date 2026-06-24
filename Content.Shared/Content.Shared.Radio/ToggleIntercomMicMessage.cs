using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio;

[Serializable]
[NetSerializable]
public sealed class ToggleIntercomMicMessage : BoundUserInterfaceMessage
{
	public bool Enabled;

	public ToggleIntercomMicMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
