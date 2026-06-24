using System;
using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Fax;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client.Fax.AdminUI;

public sealed class AdminFaxEui : BaseEui
{
	private readonly AdminFaxWindow _window;

	public AdminFaxEui()
	{
		_window = new AdminFaxWindow();
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new AdminFaxEuiMsg.Close());
		};
		AdminFaxWindow window = _window;
		window.OnFollowFax = (Action<NetEntity>)Delegate.Combine(window.OnFollowFax, (Action<NetEntity>)delegate(NetEntity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			SendMessage(new AdminFaxEuiMsg.Follow(entity));
		});
		AdminFaxWindow window2 = _window;
		window2.OnMessageSend = (Action<(NetEntity, string, string, string, string, Color, bool)>)Delegate.Combine(window2.OnMessageSend, (Action<(NetEntity, string, string, string, string, Color, bool)>)delegate((NetEntity entity, string title, string stampedBy, string message, string stampSprite, Color stampColor, bool locked) args)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			SendMessage(new AdminFaxEuiMsg.Send(args.entity, args.title, args.stampedBy, args.message, args.stampSprite, args.stampColor, args.locked));
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

	public override void HandleState(EuiStateBase state)
	{
		if (state is AdminFaxEuiState adminFaxEuiState)
		{
			_window.PopulateFaxes(adminFaxEuiState.Entries);
		}
	}
}
