using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class QuickDialogResponseEvent : EntityEventArgs
{
	public int DialogId;

	public Dictionary<string, string> Responses;

	public QuickDialogButtonFlag ButtonPressed;

	public QuickDialogResponseEvent(int dialogId, Dictionary<string, string> responses, QuickDialogButtonFlag buttonPressed)
	{
		DialogId = dialogId;
		Responses = responses;
		ButtonPressed = buttonPressed;
	}
}
