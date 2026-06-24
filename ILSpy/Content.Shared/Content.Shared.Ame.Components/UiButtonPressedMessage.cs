using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ame.Components;

[Serializable]
[NetSerializable]
public sealed class UiButtonPressedMessage : BoundUserInterfaceMessage
{
	public readonly UiButton Button;

	public UiButtonPressedMessage(UiButton button)
	{
		Button = button;
	}
}
