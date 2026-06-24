using Content.Client.Credits;
using Content.Client.Lobby;
using Content.Client.UserInterface.Systems.Info;
using Content.Shared.CCVar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;

namespace Content.Client._RMC14.Roadmap;

public sealed class RoadmapUIController : UIController, IOnStateEntered<LobbyState>
{
	[Dependency]
	private IConfigurationManager _config;

	[Dependency]
	private InfoUIController _infoUIController;

	[Dependency]
	private IUriOpener _uriOpener;

	private RoadmapWindow? _window;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		_infoUIController.Accepted += OnAccepted;
	}

	public void OnStateEntered(LobbyState state)
	{
	}

	private void OnAccepted()
	{
	}

	public void ToggleRoadmap()
	{
		if (_window != null)
		{
			((BaseWindow)_window).Close();
			_window = null;
			return;
		}
		_window = new RoadmapWindow();
		((BaseWindow)_window).OnClose += delegate
		{
			_window = null;
		};
		string discordLink = _config.GetCVar<string>(CCVars.InfoLinksDiscord);
		if (discordLink != null && discordLink.Length > 0)
		{
			((Control)_window.DiscordButton).StyleClasses.Add("Caution");
			((Control)_window.DiscordButton).Visible = true;
			((BaseButton)_window.DiscordButton).OnPressed += delegate
			{
				_uriOpener.OpenUri(discordLink);
			};
		}
		string patreonLink = _config.GetCVar<string>(CCVars.InfoLinksPatreon);
		if (patreonLink != null && patreonLink.Length > 0)
		{
			((Control)_window.PatreonButton).StyleClasses.Add("Caution");
			((Control)_window.PatreonButton).Visible = true;
			((BaseButton)_window.PatreonButton).OnPressed += delegate
			{
				_uriOpener.OpenUri(patreonLink);
			};
		}
		((Control)_window.CreditsButton).StyleClasses.Add("Caution");
		((BaseButton)_window.CreditsButton).OnPressed += delegate
		{
			((BaseWindow)new CreditsWindow()).OpenCentered();
		};
		((BaseWindow)_window).OpenCentered();
	}
}
