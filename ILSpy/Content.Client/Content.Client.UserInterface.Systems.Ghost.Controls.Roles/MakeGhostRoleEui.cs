using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Content.Shared.Ghost.Roles.Raffles;
using Robust.Client.Console;
using Robust.Client.Player;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles;

public sealed class MakeGhostRoleEui : BaseEui
{
	[Dependency]
	private IEntityManager _entManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IClientConsoleHost _consoleHost;

	private readonly MakeGhostRoleWindow _window;

	public MakeGhostRoleEui()
	{
		_window = new MakeGhostRoleWindow();
		((BaseWindow)_window).OnClose += OnClose;
		_window.OnMake += OnMake;
	}

	public override void HandleState(EuiStateBase state)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (state is MakeGhostRoleEuiState makeGhostRoleEuiState)
		{
			_window.SetEntity(_entManager, makeGhostRoleEuiState.Entity);
		}
	}

	public override void Opened()
	{
		base.Opened();
		((BaseWindow)_window).OpenCentered();
	}

	private unsafe void OnMake(NetEntity entity, string name, string description, string rules, bool makeSentient, GhostRoleRaffleSettings? raffleSettings)
	{
		ICommonSession localSession = ((ISharedPlayerManager)_playerManager).LocalSession;
		if (localSession != null)
		{
			string value = ((raffleSettings != null) ? "makeghostroleraffled" : "makeghostrole");
			string text = $"{value} \"{CommandParsing.Escape(((object)(*(NetEntity*)(&entity))/*cast due to constrained. prefix*/).ToString())}\" \"{CommandParsing.Escape(name)}\" \"{CommandParsing.Escape(description)}\" ";
			if (raffleSettings != null)
			{
				text += $"{raffleSettings.InitialDuration} {raffleSettings.JoinExtendsDurationBy} {raffleSettings.MaxDuration} ";
			}
			text = text + "\"" + CommandParsing.Escape(rules) + "\"";
			((IConsoleHost)_consoleHost).ExecuteCommand(localSession, text);
			if (makeSentient)
			{
				string text2 = "makesentient \"" + CommandParsing.Escape(((object)(*(NetEntity*)(&entity))/*cast due to constrained. prefix*/).ToString()) + "\"";
				((IConsoleHost)_consoleHost).ExecuteCommand(localSession, text2);
			}
			((BaseWindow)_window).Close();
		}
	}

	private void OnClose()
	{
		base.Closed();
		SendMessage(new CloseEuiMessage());
	}
}
