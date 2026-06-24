using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Utility;

namespace Content.Client.Administration.UI;

public sealed class AdminAnnounceEui : BaseEui
{
	private readonly AdminAnnounceWindow _window;

	public AdminAnnounceEui()
	{
		_window = new AdminAnnounceWindow();
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
		((BaseButton)_window.AnnounceButton).OnPressed += AnnounceButtonOnOnPressed;
	}

	private void AnnounceButtonOnOnPressed(ButtonEventArgs obj)
	{
		SendMessage(new AdminAnnounceEuiMsg.DoAnnounce
		{
			Announcement = Rope.Collapse(_window.Announcement.TextRope),
			Announcer = _window.Announcer.Text,
			AnnounceType = (AdminAnnounceType)(_window.AnnounceMethod.SelectedMetadata ?? ((object)AdminAnnounceType.Station)),
			CloseAfter = !((BaseButton)_window.KeepWindowOpen).Pressed
		});
	}

	public override void Opened()
	{
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		((BaseWindow)_window).Close();
	}
}
