using System;
using Content.Shared.Atmos.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasAnalyzerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private GasAnalyzerWindow? _window;

	public GasAnalyzerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindowCenteredLeft<GasAnalyzerWindow>((BoundUserInterface)(object)this);
		((BaseWindow)_window).OnClose += base.Close;
	}

	protected override void ReceiveMessage(BoundUserInterfaceMessage message)
	{
		if (_window != null && message is GasAnalyzerComponent.GasAnalyzerUserMessage msg)
		{
			_window.Populate(msg);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			GasAnalyzerWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
