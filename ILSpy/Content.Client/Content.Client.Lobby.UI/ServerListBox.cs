using System.Collections.Generic;
using Robust.Client;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Utility;

namespace Content.Client.Lobby.UI;

public sealed class ServerListBox : BoxContainer
{
	private readonly IGameController _gameController;

	private readonly List<Button> _connectButtons = new List<Button>();

	private readonly IUriOpener _uriOpener;

	public ServerListBox()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		_gameController = IoCManager.Resolve<IGameController>();
		_uriOpener = IoCManager.Resolve<IUriOpener>();
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		ScrollContainer val = new ScrollContainer
		{
			HScrollEnabled = false,
			VScrollEnabled = true,
			MinHeight = 330f,
			MaxHeight = 330f,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
		AddServers(val2);
	}

	private void AddServers(BoxContainer container)
	{
		AddServerInfo(container, Loc.GetString("pubg-lobby-server-titan-name"), Loc.GetString("pubg-lobby-server-titan-desc"), "ss14://f2.deadspace14.net:1213", null);
		AddServerInfo(container, Loc.GetString("pubg-lobby-server-deimos-name"), Loc.GetString("pubg-lobby-server-deimos-desc"), "ss14://f3.deadspace14.net:1216", null);
		AddServerInfo(container, Loc.GetString("pubg-lobby-server-soyuz-name"), Loc.GetString("pubg-lobby-server-soyuz-desc"), "ss14://s1.deadspace14.net:1215", null);
		AddServerInfo(container, Loc.GetString("pubg-lobby-server-frontier-name"), Loc.GetString("pubg-lobby-server-frontier-desc"), "ss14://ff.deadspace14.net:1214", null);
	}

	private void AddServerInfo(BoxContainer container, string serverName, string description, string serverUrl, string? discord)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			MinHeight = 50f,
			Margin = new Thickness(0f, 0f, 0f, 5f)
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		Label val3 = new Label
		{
			Text = serverName,
			MinWidth = 200f
		};
		RichTextLabel val4 = new RichTextLabel
		{
			MaxWidth = 500f
		};
		val4.SetMessage(FormattedMessage.FromMarkupOrThrow(description), (Color?)null);
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)3
		};
		Button val6 = new Button
		{
			Text = Loc.GetString("pubg-lobby-server-connect")
		};
		if (discord != null)
		{
			Button val7 = new Button
			{
				Text = Loc.GetString("server-info-discord-button")
			};
			((BaseButton)val7).OnPressed += delegate
			{
				_uriOpener.OpenUri(discord);
			};
			((Control)val5).AddChild((Control)(object)val7);
		}
		_connectButtons.Add(val6);
		((BaseButton)val6).OnPressed += delegate
		{
			_gameController.Redial(serverUrl, Loc.GetString("pubg-lobby-server-connecting"));
			foreach (Button connectButton in _connectButtons)
			{
				((BaseButton)connectButton).Disabled = true;
			}
		};
		((Control)val5).AddChild((Control)(object)val6);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val5);
		((Control)container).AddChild((Control)(object)val);
	}
}
