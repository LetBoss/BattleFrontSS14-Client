using Content.Client.Eui;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client.Ghost.UI;

public sealed class ReturnToBodyEui : BaseEui
{
	private readonly ReturnToBodyMenu _menu;

	public ReturnToBodyEui()
	{
		_menu = new ReturnToBodyMenu();
		((BaseButton)_menu.DenyButton).OnPressed += delegate
		{
			SendMessage(new ReturnToBodyMessage(accepted: false));
			((BaseWindow)_menu).Close();
		};
		((BaseButton)_menu.AcceptButton).OnPressed += delegate
		{
			SendMessage(new ReturnToBodyMessage(accepted: true));
			((BaseWindow)_menu).Close();
		};
	}

	public override void Opened()
	{
		IoCManager.Resolve<IClyde>().RequestWindowAttention();
		((BaseWindow)_menu).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		SendMessage(new ReturnToBodyMessage(accepted: false));
		((BaseWindow)_menu).Close();
	}
}
