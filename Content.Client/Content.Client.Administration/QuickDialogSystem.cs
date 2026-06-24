using System;
using System.Collections.Generic;
using Content.Client.UserInterface.Controls;
using Content.Shared.Administration;
using Robust.Shared.GameObjects;

namespace Content.Client.Administration;

public sealed class QuickDialogSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).SubscribeNetworkEvent<QuickDialogOpenEvent>((EntityEventHandler<QuickDialogOpenEvent>)OpenDialog, (Type[])null, (Type[])null);
	}

	private void OpenDialog(QuickDialogOpenEvent ev)
	{
		bool ok = (ev.Buttons & QuickDialogButtonFlag.OkButton) != 0;
		bool cancel = (ev.Buttons & QuickDialogButtonFlag.CancelButton) != 0;
		DialogWindow dialogWindow = new DialogWindow(ev.Title, ev.Prompts, ok, cancel);
		dialogWindow.OnConfirmed = (Action<Dictionary<string, string>>)Delegate.Combine(dialogWindow.OnConfirmed, (Action<Dictionary<string, string>>)delegate(Dictionary<string, string> responses)
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new QuickDialogResponseEvent(ev.DialogId, responses, QuickDialogButtonFlag.OkButton));
		});
		dialogWindow.OnCancelled = (Action)Delegate.Combine(dialogWindow.OnCancelled, (Action)delegate
		{
			((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new QuickDialogResponseEvent(ev.DialogId, new Dictionary<string, string>(), QuickDialogButtonFlag.CancelButton));
		});
	}
}
