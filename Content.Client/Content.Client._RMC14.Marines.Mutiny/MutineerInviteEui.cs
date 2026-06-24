using Content.Client.Eui;
using Content.Shared._RMC14.Marines.Mutiny;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Marines.Mutiny;

public sealed class MutineerInviteEui : BaseEui
{
	private readonly MutineerInviteWindow _window;

	public MutineerInviteEui()
	{
		_window = new MutineerInviteWindow();
		((BaseButton)_window.DenyButton).OnPressed += delegate
		{
			SendMessage(new MutineerInviteChoiceMessage(MutineerInviteUiButton.Deny));
			((BaseWindow)_window).Close();
		};
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new MutineerInviteChoiceMessage(MutineerInviteUiButton.Deny));
		};
		((BaseButton)_window.AcceptButton).OnPressed += delegate
		{
			SendMessage(new MutineerInviteChoiceMessage(MutineerInviteUiButton.Accept));
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
