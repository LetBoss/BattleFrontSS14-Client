using System;
using Content.Shared._RMC14.Xenonids.Word;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Xenonids.Word;

public sealed class XenoWordQueenBui : BoundUserInterface
{
	[ViewVariables]
	private XenoWordQueenWindow? _window;

	public XenoWordQueenBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<XenoWordQueenWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.SendButton).OnPressed += Send;
	}

	private void Send(ButtonEventArgs args)
	{
		if (_window != null)
		{
			string text = Rope.Collapse(_window.Text.TextRope);
			if (!string.IsNullOrWhiteSpace(text))
			{
				XenoWordQueenBuiMsg xenoWordQueenBuiMsg = new XenoWordQueenBuiMsg(text);
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)xenoWordQueenBuiMsg);
				((BaseWindow)_window).Close();
			}
		}
	}
}
