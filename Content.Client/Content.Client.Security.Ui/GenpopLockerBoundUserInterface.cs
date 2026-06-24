using System;
using Content.Shared.Security.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client.Security.Ui;

public sealed class GenpopLockerBoundUserInterface : BoundUserInterface
{
	private GenpopLockerMenu? _menu;

	public GenpopLockerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_menu = new GenpopLockerMenu(((BoundUserInterface)this).Owner, base.EntMan);
		_menu.OnConfigurationComplete += delegate(string name, float time, string crime)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GenpopLockerIdConfiguredMessage(name, time, crime));
			((BoundUserInterface)this).Close();
		};
		((BaseWindow)_menu).OnClose += base.Close;
		((BaseWindow)_menu).OpenCentered();
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			GenpopLockerMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
			_menu = null;
		}
	}
}
