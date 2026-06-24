using System;
using Content.Client._CIV14merka.Ghost;
using Content.Client._CIV14merka.Lobby;
using Content.Client._CIV14merka.Lobby.UI;
using Content.Client._PUBG.Ghost;
using Content.Client.Ghost;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Client.UserInterface.Systems.Ghost.Controls;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Shared._CIV14merka;
using Content.Shared._CIV14merka.Ghost;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Ghost;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client.UserInterface.Systems.Ghost;

public sealed class GhostUIController : UIController, IOnSystemChanged<GhostSystem>, IOnSystemLoaded<GhostSystem>, IOnSystemUnloaded<GhostSystem>, IOnSystemChanged<CivRosterSystem>, IOnSystemLoaded<CivRosterSystem>, IOnSystemUnloaded<CivRosterSystem>
{
	[Dependency]
	private IEntityNetworkManager _net;

	[Dependency]
	private IPlayerManager _player;

	[UISystemDependency]
	private readonly GhostSystem? _system;

	private CivLeaveRoundWarningWindow? _civLeaveRoundWarningWindow;

	private CivGhostChangeClassWindow? _civChangeClassWindow;

	private GhostGui? Gui => base.UIManager.GetActiveUIWidgetOrNull<GhostGui>();

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, new Action(OnScreenLoad));
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
	}

	private void OnScreenLoad()
	{
		LoadGui();
	}

	private void OnScreenUnload()
	{
		UnloadGui();
	}

	public void OnSystemLoaded(GhostSystem system)
	{
		system.PlayerRemoved += OnPlayerRemoved;
		system.PlayerUpdated += OnPlayerUpdated;
		system.PlayerAttached += OnPlayerAttached;
		system.PlayerDetached += OnPlayerDetached;
		system.GhostWarpsResponse += OnWarpsResponse;
		system.GhostRoleCountUpdated += OnRoleCountUpdated;
	}

	public void OnSystemUnloaded(GhostSystem system)
	{
		system.PlayerRemoved -= OnPlayerRemoved;
		system.PlayerUpdated -= OnPlayerUpdated;
		system.PlayerAttached -= OnPlayerAttached;
		system.PlayerDetached -= OnPlayerDetached;
		system.GhostWarpsResponse -= OnWarpsResponse;
		system.GhostRoleCountUpdated -= OnRoleCountUpdated;
	}

	public void OnSystemLoaded(CivRosterSystem system)
	{
		system.StateUpdated += OnCivRosterStateUpdated;
	}

	public void OnSystemUnloaded(CivRosterSystem system)
	{
		system.StateUpdated -= OnCivRosterStateUpdated;
	}

	private void OnCivRosterStateUpdated(CivRosterStateEvent state)
	{
		UpdateGui();
	}

	public void UpdateGui()
	{
		if (Gui != null)
		{
			if (IsCommanderGhost())
			{
				Gui.Hide();
				return;
			}
			((Control)Gui).Visible = _system?.IsGhost ?? false;
			Gui.Update(_system?.AvailableGhostRoleCount, _system?.Player?.CanReturnToBody);
			CivRosterStateEvent state = base.EntityManager.System<CivRosterSystem>().GetState();
			Gui.SetTdmGhostButtonsVisible(state.RoundMode != Civ14RoundMode.PointCapture);
			Gui.SetCivChangeClassVisible(state.RoundInProgress || state.LateJoinActive);
		}
	}

	private void OnPlayerRemoved(GhostComponent component)
	{
		Gui?.Hide();
	}

	private void OnPlayerUpdated(GhostComponent component)
	{
		UpdateGui();
	}

	private void OnPlayerAttached(GhostComponent component)
	{
		if (Gui != null)
		{
			if (IsCommanderGhost())
			{
				Gui.Hide();
				return;
			}
			((Control)Gui).Visible = true;
			UpdateGui();
			base.EntityManager.System<PubgGhostSpectateClientSystem>().RefreshFollowButton();
		}
	}

	private void OnPlayerDetached()
	{
		Gui?.Hide();
		base.EntityManager.System<PubgGhostSpectateClientSystem>().RefreshFollowButton();
	}

	private void OnWarpsResponse(GhostWarpsResponseEvent msg)
	{
		GhostTargetWindow ghostTargetWindow = Gui?.TargetWindow;
		if (ghostTargetWindow != null)
		{
			ghostTargetWindow.UpdateWarps(msg.Warps);
			ghostTargetWindow.Populate();
		}
	}

	private void OnRoleCountUpdated(GhostUpdateGhostRoleCountEvent msg)
	{
		UpdateGui();
	}

	private void OnWarpClicked(NetEntity player)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		GhostWarpToTargetRequestEvent ghostWarpToTargetRequestEvent = new GhostWarpToTargetRequestEvent(player);
		_net.SendSystemNetworkMessage((EntityEventArgs)(object)ghostWarpToTargetRequestEvent, true);
	}

	private void OnGhostnadoClicked()
	{
		GhostnadoRequestEvent ghostnadoRequestEvent = new GhostnadoRequestEvent();
		_net.SendSystemNetworkMessage((EntityEventArgs)(object)ghostnadoRequestEvent, true);
	}

	public void LoadGui()
	{
		if (Gui != null)
		{
			Gui.RequestWarpsPressed += RequestWarps;
			Gui.ReturnToBodyPressed += ReturnToBody;
			Gui.GhostRolesPressed += GhostRolesPressed;
			Gui.GhostBarPressed += GhostBarPressed;
			Gui.GhostLobbyPressed += GhostLobbyPressed;
			Gui.GhostFollowAlliesPressed += GhostFollowAlliesPressed;
			Gui.CivChangeClassPressed += OpenCivChangeClassWindow;
			Gui.TargetWindow.WarpClicked += OnWarpClicked;
			Gui.TargetWindow.OnGhostnadoClicked += OnGhostnadoClicked;
			base.EntityManager.System<PubgGhostSpectateClientSystem>().RefreshFollowButton();
			UpdateGui();
		}
	}

	public void UnloadGui()
	{
		if (Gui == null)
		{
			if (_civLeaveRoundWarningWindow != null)
			{
				_civLeaveRoundWarningWindow.ConfirmPressed -= ConfirmCivLeaveRound;
				((BaseWindow)_civLeaveRoundWarningWindow).OnClose -= OnCivLeaveRoundWarningClosed;
				((BaseWindow)_civLeaveRoundWarningWindow).Close();
				_civLeaveRoundWarningWindow = null;
			}
			return;
		}
		Gui.RequestWarpsPressed -= RequestWarps;
		Gui.ReturnToBodyPressed -= ReturnToBody;
		Gui.GhostRolesPressed -= GhostRolesPressed;
		Gui.GhostBarPressed -= GhostBarPressed;
		Gui.GhostLobbyPressed -= GhostLobbyPressed;
		Gui.GhostFollowAlliesPressed -= GhostFollowAlliesPressed;
		Gui.CivChangeClassPressed -= OpenCivChangeClassWindow;
		Gui.TargetWindow.WarpClicked -= OnWarpClicked;
		Gui.Hide();
		if (_civLeaveRoundWarningWindow != null)
		{
			_civLeaveRoundWarningWindow.ConfirmPressed -= ConfirmCivLeaveRound;
			((BaseWindow)_civLeaveRoundWarningWindow).OnClose -= OnCivLeaveRoundWarningClosed;
			((BaseWindow)_civLeaveRoundWarningWindow).Close();
			_civLeaveRoundWarningWindow = null;
		}
	}

	private void ReturnToBody()
	{
		_system?.ReturnToBody();
	}

	private void RequestWarps()
	{
		_system?.RequestWarps();
		Gui?.TargetWindow.Populate();
		GhostGui? gui = Gui;
		if (gui != null)
		{
			((BaseWindow)gui.TargetWindow).OpenCentered();
		}
	}

	private void GhostRolesPressed()
	{
		_system?.OpenGhostRoles();
	}

	private void GhostBarPressed()
	{
		_system?.RequestGhostBar();
	}

	private void GhostLobbyPressed()
	{
		if (ShouldWarnOnCivLobbyReturn())
		{
			OpenCivLeaveRoundWarning();
		}
		else
		{
			_system?.RequestLobbyRespawn();
		}
	}

	private void GhostFollowAlliesPressed()
	{
		base.EntityManager.System<PubgGhostSpectateClientSystem>().RequestFollowAllies();
	}

	private bool IsCommanderGhost()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
			if (base.EntityManager.TryGetComponent<CivTeamMemberComponent>(valueOrDefault, ref civTeamMemberComponent))
			{
				return civTeamMemberComponent.IsCommander;
			}
		}
		return false;
	}

	private bool ShouldWarnOnCivLobbyReturn()
	{
		CivRosterStateEvent state = base.EntityManager.System<CivRosterSystem>().GetState();
		if (state.LateJoinActive && state.HasParticipatedInCurrentRound)
		{
			return !state.RejoinBlockedForCurrentRound;
		}
		return false;
	}

	private void OpenCivLeaveRoundWarning()
	{
		if (_civLeaveRoundWarningWindow == null)
		{
			_civLeaveRoundWarningWindow = new CivLeaveRoundWarningWindow();
			_civLeaveRoundWarningWindow.ConfirmPressed += ConfirmCivLeaveRound;
			((BaseWindow)_civLeaveRoundWarningWindow).OnClose += OnCivLeaveRoundWarningClosed;
		}
		((BaseWindow)_civLeaveRoundWarningWindow).OpenCentered();
	}

	private void ConfirmCivLeaveRound()
	{
		_system?.RequestLobbyRespawn();
	}

	private void OnCivLeaveRoundWarningClosed()
	{
		if (_civLeaveRoundWarningWindow != null)
		{
			_civLeaveRoundWarningWindow.ConfirmPressed -= ConfirmCivLeaveRound;
			((BaseWindow)_civLeaveRoundWarningWindow).OnClose -= OnCivLeaveRoundWarningClosed;
			_civLeaveRoundWarningWindow = null;
		}
	}

	private void OpenCivChangeClassWindow()
	{
		CivRosterStateEvent state = base.EntityManager.System<CivRosterSystem>().GetState();
		if (_civChangeClassWindow == null || ((Control)_civChangeClassWindow).Disposed)
		{
			_civChangeClassWindow = new CivGhostChangeClassWindow();
			_civChangeClassWindow.ClassSelected -= SendCivChangeClass;
			_civChangeClassWindow.ClassSelected += SendCivChangeClass;
			_civChangeClassWindow.RefreshRequested += RefreshCivChangeClassWindow;
		}
		_civChangeClassWindow.Populate(state);
		((BaseWindow)_civChangeClassWindow).OpenCentered();
	}

	private void RefreshCivChangeClassWindow()
	{
		if (_civChangeClassWindow != null && !((Control)_civChangeClassWindow).Disposed)
		{
			_civChangeClassWindow.Populate(base.EntityManager.System<CivRosterSystem>().GetState());
		}
	}

	private void SendCivChangeClass(CivTdmClass cls)
	{
		_net.SendSystemNetworkMessage((EntityEventArgs)(object)new CivGhostChangeClassRequestEvent(cls), true);
	}
}
