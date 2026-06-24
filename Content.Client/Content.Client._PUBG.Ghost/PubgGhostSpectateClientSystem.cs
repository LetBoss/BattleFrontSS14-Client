using System;
using Content.Client._RMC14.NightVision;
using Content.Client.Movement.Systems;
using Content.Client.UserInterface.Systems.Ghost.Widgets;
using Content.Shared._PUBG;
using Content.Shared._PUBG.Ghost;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._PUBG.Ghost;

public sealed class PubgGhostSpectateClientSystem : EntitySystem
{
	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IUserInterfaceManager _ui;

	[Dependency]
	private ContentEyeSystem _contentEye;

	[Dependency]
	private IOverlayManager _overlay;

	private bool _pubgActive;

	private int _teamSize;

	private PubgGhostFollowSelectWindow? _followWindow;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeNetworkEvent<PubgModeStatusEvent>((EntitySessionEventHandler<PubgModeStatusEvent>)OnPubgModeStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgTeamModeStatusEvent>((EntitySessionEventHandler<PubgTeamModeStatusEvent>)OnPubgTeamStatus, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgGhostFollowTeammateOptionsEvent>((EntitySessionEventHandler<PubgGhostFollowTeammateOptionsEvent>)OnFollowOptions, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PubgGhostFollowStateEvent>((EntitySessionEventHandler<PubgGhostFollowStateEvent>)OnGhostFollowState, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		CloseFollowWindow();
	}

	public void RequestFollowAllies()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgGhostFollowTeammateRequestEvent());
	}

	public void RefreshFollowButton()
	{
		_ui.GetActiveUIWidgetOrNull<GhostGui>()?.SetFollowAlliesVisible(ShouldShowFollowButton());
	}

	private void OnPubgModeStatus(PubgModeStatusEvent ev, EntitySessionEventArgs args)
	{
		_pubgActive = ev.Enabled;
		if (!ev.Enabled)
		{
			_teamSize = 0;
			CloseFollowWindow();
		}
		RefreshFollowButton();
	}

	private void OnPubgTeamStatus(PubgTeamModeStatusEvent ev, EntitySessionEventArgs args)
	{
		_pubgActive = ev.Enabled;
		_teamSize = ev.TeamSize;
		if (!ShouldShowFollowButton())
		{
			CloseFollowWindow();
		}
		RefreshFollowButton();
	}

	private void OnFollowOptions(PubgGhostFollowTeammateOptionsEvent ev, EntitySessionEventArgs args)
	{
		if (!ShouldShowFollowButton())
		{
			CloseFollowWindow();
			return;
		}
		EnsureFollowWindow();
		_followWindow.SetOptions(ev.Options);
		if (!((BaseWindow)_followWindow).IsOpen)
		{
			((BaseWindow)_followWindow).OpenCentered();
		}
	}

	private void OnGhostFollowState(PubgGhostFollowStateEvent ev, EntitySessionEventArgs args)
	{
		if (ev.Enabled)
		{
			CloseFollowWindow();
			_contentEye.RequestEye(drawFov: true, drawLight: true);
			_overlay.RemoveOverlay<HalfNightVisionBrightnessOverlay>();
		}
	}

	private bool ShouldShowFollowButton()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!_pubgActive || _teamSize <= 1)
		{
			return false;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			return ((EntitySystem)this).HasComp<GhostComponent>(localEntity.Value);
		}
		return false;
	}

	private void EnsureFollowWindow()
	{
		if (_followWindow == null)
		{
			_followWindow = new PubgGhostFollowSelectWindow();
			_followWindow.FollowRequested += OnFollowRequested;
		}
	}

	private void CloseFollowWindow()
	{
		if (_followWindow != null)
		{
			_followWindow.FollowRequested -= OnFollowRequested;
			((BaseWindow)_followWindow).Close();
			_followWindow = null;
		}
	}

	private void OnFollowRequested(NetEntity teammate)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new PubgGhostFollowTeammateRequestEvent(teammate));
		CloseFollowWindow();
	}
}
