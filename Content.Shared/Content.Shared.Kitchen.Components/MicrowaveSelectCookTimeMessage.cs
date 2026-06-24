using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen.Components;

[Serializable]
[NetSerializable]
public sealed class MicrowaveSelectCookTimeMessage : BoundUserInterfaceMessage
{
	public int ButtonIndex;

	public uint NewCookTime;

	public MicrowaveSelectCookTimeMessage(int buttonIndex, uint inputTime)
	{
		ButtonIndex = buttonIndex;
		NewCookTime = inputTime;
	}
}
