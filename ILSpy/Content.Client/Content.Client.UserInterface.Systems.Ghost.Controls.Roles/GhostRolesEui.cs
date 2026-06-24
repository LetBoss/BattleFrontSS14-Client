using System.Collections.Generic;
using System.Linq;
using Content.Client.Eui;
using Content.Client.Players.PlayTimeTracking;
using Content.Shared.Eui;
using Content.Shared.Ghost.Roles;
using Content.Shared.Roles;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Content.Client.UserInterface.Systems.Ghost.Controls.Roles;

public sealed class GhostRolesEui : BaseEui
{
	private readonly GhostRolesWindow _window;

	private GhostRoleRulesWindow? _windowRules;

	private uint _windowRulesId;

	public GhostRolesEui()
	{
		_window = new GhostRolesWindow();
		_window.OnRoleRequestButtonClicked += delegate(GhostRoleInfo info)
		{
			GhostRoleRulesWindow? windowRules = _windowRules;
			if (windowRules != null)
			{
				((BaseWindow)windowRules).Close();
			}
			if (info.Kind == GhostRoleKind.RaffleJoined)
			{
				SendMessage(new LeaveGhostRoleRaffleMessage(info.Identifier));
			}
			else
			{
				_windowRules = new GhostRoleRulesWindow(info.Rules, delegate
				{
					SendMessage(new RequestGhostRoleMessage(info.Identifier));
					if (info.Kind != GhostRoleKind.FirstComeFirstServe)
					{
						GhostRoleRulesWindow? windowRules2 = _windowRules;
						if (windowRules2 != null)
						{
							((BaseWindow)windowRules2).Close();
						}
					}
				});
				_windowRulesId = info.Identifier;
				((BaseWindow)_windowRules).OnClose += delegate
				{
					_windowRules = null;
				};
				((BaseWindow)_windowRules).OpenCentered();
			}
		};
		_window.OnRoleFollow += delegate(GhostRoleInfo info)
		{
			SendMessage(new FollowGhostRoleMessage(info.Identifier));
		};
		((BaseWindow)_window).OnClose += delegate
		{
			SendMessage(new CloseEuiMessage());
		};
	}

	public override void Opened()
	{
		base.Opened();
		((BaseWindow)_window).OpenCentered();
	}

	public override void Closed()
	{
		base.Closed();
		((BaseWindow)_window).Close();
		GhostRoleRulesWindow? windowRules = _windowRules;
		if (windowRules != null)
		{
			((BaseWindow)windowRules).Close();
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		base.HandleState(state);
		if (!(state is GhostRolesEuiState ghostRolesEuiState))
		{
			return;
		}
		_window.SaveCollapsibleBoxesStates();
		_window.ClearEntries();
		SpriteSystem entitySystem = IoCManager.Resolve<IEntityManager>().EntitySysManager.GetEntitySystem<SpriteSystem>();
		JobRequirementsManager jobRequirementsManager = IoCManager.Resolve<JobRequirementsManager>();
		foreach (IGrouping<(string, string, HashSet<JobRequirement>), GhostRoleInfo> item3 in from role in ghostRolesEuiState.GhostRoles
			group role by (Name: role.Name, Description: role.Description, Requirements: role.Requirements))
		{
			string item = item3.Key.Item1;
			string item2 = item3.Key.Item2;
			FormattedMessage reason;
			bool hasAccess = jobRequirementsManager.CheckRoleRequirements(item3.Key.Item3, null, out reason);
			_window.AddEntry(item, item2, hasAccess, reason, item3, entitySystem);
		}
		_window.RestoreCollapsibleBoxesStates();
		if (ghostRolesEuiState.GhostRoles.All((GhostRoleInfo role) => role.Identifier != _windowRulesId))
		{
			GhostRoleRulesWindow? windowRules = _windowRules;
			if (windowRules != null)
			{
				((BaseWindow)windowRules).Close();
			}
		}
	}
}
