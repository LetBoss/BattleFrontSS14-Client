using System;
using Content.Client._PUBG.Leaderboard;
using Content.Client.Changelog;
using Content.Client.UserInterface.Systems.EscapeMenu;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Collections;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Client.Info;

public sealed class LinkBanner : BoxContainer
{
	private readonly IConfigurationManager _cfg;

	private readonly IUriOpener _uriOpener;

	private LeaderboardWindow? _leaderboardWindow;

	private ValueList<(CVarDef<string> cVar, Button button)> _infoLinks;

	public LinkBanner()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Expected O, but got Unknown
		BoxContainer buttons = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		((Control)this).AddChild((Control)(object)buttons);
		_uriOpener = IoCManager.Resolve<IUriOpener>();
		_cfg = IoCManager.Resolve<IConfigurationManager>();
		Button val = new Button
		{
			Text = Loc.GetString("pubg-leaderboard-button"),
			StyleClasses = { "Caution" }
		};
		((BaseButton)val).OnPressed += delegate
		{
			ToggleLeaderboard();
		};
		((Control)buttons).AddChild((Control)(object)val);
		Button val2 = new Button
		{
			Text = Loc.GetString("pubg-discord-button"),
			StyleClasses = { "OpenRight" }
		};
		((BaseButton)val2).OnPressed += delegate
		{
			_uriOpener.OpenUri("https://discord.gg/xdQ4vSKRB8");
		};
		((Control)buttons).AddChild((Control)(object)val2);
		AddInfoButton("server-info-discord-button", CCVars.InfoLinksDiscord);
		AddInfoButton("server-info-website-button", CCVars.InfoLinksWebsite);
		AddInfoButton("server-info-wiki-button", CCVars.InfoLinksWiki);
		AddInfoButton("server-info-forum-button", CCVars.InfoLinksForum);
		AddInfoButton("server-info-telegram-button", CCVars.InfoLinksTelegram);
		ChangelogButton changelogButton = new ChangelogButton();
		((BaseButton)changelogButton).OnPressed += delegate
		{
			((Control)this).UserInterfaceManager.GetUIController<ChangelogUIController>().ToggleWindow();
		};
		((Control)buttons).AddChild((Control)(object)changelogButton);
		AddInfoButton("rmc-ui-patreon", CCVars.InfoLinksPatreon);
		void AddInfoButton(string loc, CVarDef<string> cVar)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Expected O, but got Unknown
			Button val3 = new Button
			{
				Text = Loc.GetString(loc)
			};
			((BaseButton)val3).OnPressed += delegate
			{
				_uriOpener.OpenUri(_cfg.GetCVar<string>(cVar));
			};
			((Control)buttons).AddChild((Control)(object)val3);
			_infoLinks.Add((cVar, val3));
		}
	}

	private void ToggleLeaderboard()
	{
		if (_leaderboardWindow == null)
		{
			_leaderboardWindow = new LeaderboardWindow();
			((BaseWindow)_leaderboardWindow).OnClose += delegate
			{
				_leaderboardWindow = null;
			};
		}
		if (((BaseWindow)_leaderboardWindow).IsOpen)
		{
			((BaseWindow)_leaderboardWindow).Close();
		}
		else
		{
			((BaseWindow)_leaderboardWindow).OpenCentered();
		}
	}

	protected override void EnteredTree()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).EnteredTree();
		Enumerator<(CVarDef<string>, Button)> enumerator = _infoLinks.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				(CVarDef<string>, Button) current = enumerator.Current;
				var (val, _) = current;
				((Control)current.Item2).Visible = _cfg.GetCVar<string>(val) != "";
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
	}
}
