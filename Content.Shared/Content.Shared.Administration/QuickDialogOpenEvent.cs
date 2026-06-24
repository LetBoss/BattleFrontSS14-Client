using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class QuickDialogOpenEvent : EntityEventArgs
{
	public string Title;

	public int DialogId;

	public List<QuickDialogEntry> Prompts;

	public QuickDialogButtonFlag Buttons = QuickDialogButtonFlag.OkButton | QuickDialogButtonFlag.CancelButton;

	public QuickDialogOpenEvent(string title, List<QuickDialogEntry> prompts, int dialogId, QuickDialogButtonFlag buttons)
	{
		Title = title;
		Prompts = prompts;
		Buttons = buttons;
		DialogId = dialogId;
	}
}
