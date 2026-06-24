using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio;

[Serializable]
[NetSerializable]
public sealed class ToggleIntercomSpeakerMessage : BoundUserInterfaceMessage
{
	public bool Enabled;

	public ToggleIntercomSpeakerMessage(bool enabled)
	{
		Enabled = enabled;
	}
}
