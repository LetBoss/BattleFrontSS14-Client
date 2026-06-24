using Content.Client.Eui;
using Content.Shared.Cloning;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client.Cloning.UI;

public sealed class AcceptCloningEui : BaseEui
{
	private readonly AcceptCloningWindow _window;

	public AcceptCloningEui()
	{
		_window = new AcceptCloningWindow();
		((BaseButton)_window.DenyButton).OnPressed += delegate
		{
			SendMessage(new AcceptCloningChoiceMessage(AcceptCloningUiButton.Deny));
			((BaseWindow)_window).Close();
		};
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new AcceptCloningChoiceMessage(AcceptCloningUiButton.Deny));
		};
		((BaseButton)_window.AcceptButton).OnPressed += delegate
		{
			SendMessage(new AcceptCloningChoiceMessage(AcceptCloningUiButton.Accept));
			((BaseWindow)_window).Close();
		};
	}

	public override void Opened()
	{
		IoCManager.Resolve<IClyde>().RequestWindowAttention();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		((BaseWindow)_window).Close();
	}
}
