using Content.Client.Eui;
using Robust.Client.UserInterface.CustomControls;

namespace Content.Client.Revolutionary.UI;

public sealed class DeconvertedEui : BaseEui
{
	private readonly DeconvertedMenu _menu;

	public DeconvertedEui()
	{
		_menu = new DeconvertedMenu();
	}

	public override void Opened()
	{
		((BaseWindow)_menu).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_menu).Close();
	}
}
