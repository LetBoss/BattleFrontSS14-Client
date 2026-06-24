using System;
using System.Numerics;
using Content.Client._CIV14merka.GlobalMap;
using Content.Client._PUBG.Airdrop;
using Content.Client._PUBG.Party;
using Content.Client._PUBG.RespawnTowers;
using Content.Client.Gameplay;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Screens;
using Content.Client.UserInterface.Systems.Gameplay;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.CCVar;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public sealed class PubgMinimapUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private const float DefaultMinimapSize = 200f;

	private const int MinimapAnchorMargin = 15;

	private const float DefaultLayoutBottomMargin = -220f;

	private const float DefaultLayoutTopMargin = 200f;

	private const float DefaultLayoutRightMargin = -15f;

	private const int MinWidgetSize = 120;

	private const int MaxWidgetSize = 500;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private IConfigurationManager _cfg;

	private bool _minimapVisible;

	private PubgMinimapControl? _minimapControl;

	private PanelContainer? _minimapPanel;

	private PubgMinimapControl? _mapZoneControl;

	private bool _partyMarkersEnabled = true;

	private float _partyMarkersOpacity = 1f;

	private TimeSpan _nextUpdate;

	private const float UpdateInterval = 0.1f;

	public override void Initialize()
	{
		((UIController)this).Initialize();
		GameplayStateLoadController uIController = base.UIManager.GetUIController<GameplayStateLoadController>();
		uIController.OnScreenLoad = (Action)Delegate.Combine(uIController.OnScreenLoad, (Action)delegate
		{
			if (_minimapVisible)
			{
				EnsureUI();
			}
		});
		uIController.OnScreenUnload = (Action)Delegate.Combine(uIController.OnScreenUnload, new Action(OnScreenUnload));
		_cfg.OnValueChanged<float>(CCVars.PubgMinimapOpacity, (Action<float>)OnOpacityChanged, false);
		_cfg.OnValueChanged<int>(CCVars.PubgMinimapWidgetSize, (Action<int>)OnWidgetSizeChanged, false);
		_cfg.OnValueChanged<int>(CCVars.PubgMinimapOffsetX, (Action<int>)OnPositionChanged, false);
		_cfg.OnValueChanged<int>(CCVars.PubgMinimapOffsetY, (Action<int>)OnPositionChanged, false);
		_cfg.OnValueChanged<string>(CCVars.UILayout, (Action<string>)OnScreenLayoutChanged, false);
		_cfg.OnValueChanged<bool>(CCVars.PubgPartyMarkersEnabled, (Action<bool>)OnPartyMarkersEnabledChanged, true);
		_cfg.OnValueChanged<float>(CCVars.PubgPartyMarkersOpacity, (Action<float>)OnPartyMarkersOpacityChanged, true);
	}

	public void OnStateEntered(GameplayState state)
	{
		if (_minimapVisible)
		{
			EnsureUI();
		}
	}

	public void OnStateExited(GameplayState state)
	{
		HideMinimap();
		ReleaseMapZoneMinimap();
	}

	private void OnOpacityChanged(float _)
	{
		UpdateMinimapOpacity();
	}

	private void OnWidgetSizeChanged(int _)
	{
		UpdateMinimapSize();
	}

	private void OnPositionChanged(int _)
	{
		UpdateMinimapLayout();
	}

	private void OnScreenLayoutChanged(string _)
	{
		UpdateMinimapLayout();
	}

	private void OnPartyMarkersEnabledChanged(bool enabled)
	{
		_partyMarkersEnabled = enabled;
	}

	private void OnPartyMarkersOpacityChanged(float opacity)
	{
		_partyMarkersOpacity = Math.Clamp(opacity, 0f, 1f);
	}

	private void OnScreenUnload()
	{
		ReleaseMapZoneMinimap();
		if (_minimapPanel != null)
		{
			if (_minimapControl != null)
			{
				_minimapControl.OnMapClick = null;
			}
			if (!((Control)_minimapPanel).Disposed)
			{
				((Control)_minimapPanel).Orphan();
			}
			_minimapPanel = null;
			_minimapControl = null;
		}
	}

	private ScreenType GetScreenType()
	{
		if (!Enum.TryParse<ScreenType>(_cfg.GetCVar<string>(CCVars.UILayout), out var result))
		{
			return ScreenType.Default;
		}
		return result;
	}

	private void UpdateMinimapOpacity()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (_minimapPanel != null)
		{
			float cVar = _cfg.GetCVar<float>(CCVars.PubgMinimapOpacity);
			PanelContainer? minimapPanel = _minimapPanel;
			Color white = Color.White;
			((Control)minimapPanel).Modulate = ((Color)(ref white)).WithAlpha(cVar);
		}
	}

	private void UpdateMinimapSize()
	{
		if (_minimapPanel != null && _minimapControl != null)
		{
			int num = Math.Clamp(_cfg.GetCVar<int>(CCVars.PubgMinimapWidgetSize), 120, 500);
			Vector2 vector = new Vector2(num, num);
			((Control)_minimapControl).SetSize = vector;
			((Control)_minimapControl).MinSize = vector;
			((Control)_minimapControl).MaxSize = vector;
			((Control)_minimapPanel).SetSize = vector;
			((Control)_minimapPanel).MinSize = vector;
			((Control)_minimapPanel).MaxSize = vector;
		}
	}

	private void UpdateMinimapLayout()
	{
		if (_minimapPanel != null)
		{
			ScreenType screenType = GetScreenType();
			int cVar = _cfg.GetCVar<int>(CCVars.PubgMinimapOffsetX);
			int cVar2 = _cfg.GetCVar<int>(CCVars.PubgMinimapOffsetY);
			if (screenType == ScreenType.Default)
			{
				LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_minimapPanel, (LayoutPreset)3, (LayoutPresetMode)0, 15);
				LayoutContainer.SetMarginBottom((Control)(object)_minimapPanel, -220f + (float)cVar2);
				LayoutContainer.SetMarginRight((Control)(object)_minimapPanel, -15f + (float)cVar);
			}
			else
			{
				LayoutContainer.SetAnchorAndMarginPreset((Control)(object)_minimapPanel, (LayoutPreset)1, (LayoutPresetMode)0, 15);
				LayoutContainer.SetMarginTop((Control)(object)_minimapPanel, 200f + (float)cVar2);
				LayoutContainer.SetMarginRight((Control)(object)_minimapPanel, -15f + (float)cVar);
			}
		}
	}

	public override void FrameUpdate(FrameEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		((UIController)this).FrameUpdate(args);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			if (_minimapVisible)
			{
				HideMinimap();
			}
			ReleaseMapZoneMinimap();
			return;
		}
		if (base.UIManager.ActiveScreen is BattlefrontGameScreen screen)
		{
			if (_minimapVisible)
			{
				HideMinimap();
			}
			if (base.EntityManager.System<CivGlobalMapSystem>().GlobalMapActive)
			{
				ReleaseMapZoneMinimap();
				return;
			}
			PubgMinimapControl pubgMinimapControl = EnsureMapZoneMinimap(screen);
			pubgMinimapControl.UpdatePlayerPosition(localEntity.Value);
			if (!(_timing.CurTime < _nextUpdate))
			{
				_nextUpdate = _timing.CurTime + TimeSpan.FromSeconds(0.10000000149011612);
				FeedMinimap(pubgMinimapControl, localEntity.Value);
			}
			return;
		}
		ReleaseMapZoneMinimap();
		if (!_minimapVisible)
		{
			ShowMinimap();
		}
		if (_minimapVisible && _minimapControl != null)
		{
			_minimapControl.UpdatePlayerPosition(localEntity.Value);
			if (!(_timing.CurTime < _nextUpdate))
			{
				_nextUpdate = _timing.CurTime + TimeSpan.FromSeconds(0.10000000149011612);
				FeedMinimap(_minimapControl, localEntity.Value);
			}
		}
	}

	private void FeedMinimap(PubgMinimapControl control, EntityUid player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		control.UpdatePlayerPosition(player);
		PubgMinimapStateSystem pubgMinimapStateSystem = base.EntityManager.System<PubgMinimapStateSystem>();
		control.ZoneCurrentCenter = pubgMinimapStateSystem.ZoneCurrentCenter;
		control.ZoneCurrentRadius = pubgMinimapStateSystem.ZoneCurrentRadius;
		control.ZoneNextCenter = pubgMinimapStateSystem.ZoneNextCenter;
		control.ZoneNextRadius = pubgMinimapStateSystem.ZoneNextRadius;
		control.ZoneActive = pubgMinimapStateSystem.ZoneActive;
		control.ZoneVisible = pubgMinimapStateSystem.ZoneVisible;
		control.ZoneMapId = pubgMinimapStateSystem.ZoneMapId;
		control.RedZoneActive = pubgMinimapStateSystem.RedZoneActive;
		control.RedZoneCenter = pubgMinimapStateSystem.RedZoneCenter;
		control.RedZoneRadius = pubgMinimapStateSystem.RedZoneRadius;
		PubgPartyClientSystem pubgPartyClientSystem = base.EntityManager.System<PubgPartyClientSystem>();
		control.PartyMembers = (_partyMarkersEnabled ? pubgPartyClientSystem.Members : null);
		control.PartyMarkersOpacity = _partyMarkersOpacity;
		PubgPartyPingClientSystem pubgPartyPingClientSystem = base.EntityManager.System<PubgPartyPingClientSystem>();
		control.ActivePings = pubgPartyPingClientSystem.ActivePings;
		PubgAirdropSystem pubgAirdropSystem = base.EntityManager.System<PubgAirdropSystem>();
		control.AirdropActive = pubgAirdropSystem.Active;
		if (!pubgAirdropSystem.Active)
		{
			control.AirdropCenter = null;
			control.AirdropRemainingSeconds = 0;
		}
		else
		{
			control.AirdropCenter = pubgAirdropSystem.Position;
			control.AirdropRemainingSeconds = pubgAirdropSystem.RemainingSeconds;
			control.AirdropMapId = pubgAirdropSystem.MapId;
		}
		PubgRespawnTowerSystem pubgRespawnTowerSystem = base.EntityManager.System<PubgRespawnTowerSystem>();
		control.RespawnTowerMapId = pubgRespawnTowerSystem.MapId;
		control.RespawnTowerPositions = pubgRespawnTowerSystem.TowerPositions;
		control.ActiveRespawnTowerPositions = pubgRespawnTowerSystem.ActiveTowerPositions;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (base.EntityManager.TryGetComponent<CivTeamMemberComponent>(player, ref civTeamMemberComponent))
		{
			CivGlobalMapSystem civGlobalMapSystem = base.EntityManager.System<CivGlobalMapSystem>();
			control.CivGlobalMapMarkers = civGlobalMapSystem.VisibleMarkers;
			control.CivGlobalMapPlayers = civGlobalMapSystem.VisiblePlayers;
			control.CivGlobalMapPoints = civGlobalMapSystem.VisiblePoints;
			control.CivGlobalMapOrders = civGlobalMapSystem.VisibleOrders;
			control.CivGlobalMapDeaths = civGlobalMapSystem.VisibleDeaths;
			control.CivViewerTeamId = civTeamMemberComponent.TeamId;
			control.CivViewerSquadId = civTeamMemberComponent.SquadId;
			control.CivHasBounds = civGlobalMapSystem.HasBounds;
			control.CivBoundsMin = civGlobalMapSystem.BoundsMin;
			control.CivBoundsMax = civGlobalMapSystem.BoundsMax;
		}
		else if (base.EntityManager.HasComponent<GhostComponent>(player))
		{
			CivGlobalMapSystem civGlobalMapSystem2 = base.EntityManager.System<CivGlobalMapSystem>();
			control.CivGlobalMapMarkers = civGlobalMapSystem2.VisibleMarkers;
			control.CivGlobalMapPlayers = civGlobalMapSystem2.VisiblePlayers;
			control.CivGlobalMapPoints = civGlobalMapSystem2.VisiblePoints;
			control.CivGlobalMapOrders = civGlobalMapSystem2.VisibleOrders;
			control.CivGlobalMapDeaths = civGlobalMapSystem2.VisibleDeaths;
			control.CivViewerTeamId = civGlobalMapSystem2.ViewerTeamId;
			control.CivViewerSquadId = civGlobalMapSystem2.ViewerSquadId;
			control.CivHasBounds = civGlobalMapSystem2.HasBounds;
			control.CivBoundsMin = civGlobalMapSystem2.BoundsMin;
			control.CivBoundsMax = civGlobalMapSystem2.BoundsMax;
		}
		else
		{
			control.CivGlobalMapMarkers = null;
			control.CivGlobalMapPlayers = null;
			control.CivGlobalMapPoints = null;
			control.CivGlobalMapOrders = null;
			control.CivGlobalMapDeaths = null;
			control.CivViewerTeamId = 0;
			control.CivViewerSquadId = 0;
			control.CivHasBounds = false;
		}
	}

	private PubgMinimapControl EnsureMapZoneMinimap(BattlefrontGameScreen screen)
	{
		PubgMinimapControl mapZoneControl = _mapZoneControl;
		if (mapZoneControl != null && !((Control)mapZoneControl).Disposed)
		{
			if (((Control)_mapZoneControl).Parent == null)
			{
				screen.SetMapZoneContent((Control)(object)_mapZoneControl);
			}
			return _mapZoneControl;
		}
		PubgMinimapControl pubgMinimapControl = new PubgMinimapControl();
		((Control)pubgMinimapControl).HorizontalExpand = true;
		((Control)pubgMinimapControl).VerticalExpand = true;
		((Control)pubgMinimapControl).MinSize = Vector2.Zero;
		((Control)pubgMinimapControl).SetSize = new Vector2(float.NaN, float.NaN);
		_mapZoneControl = pubgMinimapControl;
		_mapZoneControl.OnMapClick = HandleMinimapClick;
		screen.SetMapZoneContent((Control)(object)_mapZoneControl);
		return _mapZoneControl;
	}

	private void ReleaseMapZoneMinimap()
	{
		if (_mapZoneControl == null)
		{
			return;
		}
		if (!((Control)_mapZoneControl).Disposed && ((Control)_mapZoneControl).Parent != null)
		{
			if (base.UIManager.ActiveScreen is BattlefrontGameScreen battlefrontGameScreen)
			{
				battlefrontGameScreen.ClearMapZoneContent((Control)(object)_mapZoneControl);
			}
			else
			{
				((Control)_mapZoneControl).Orphan();
			}
		}
		_mapZoneControl.OnMapClick = null;
		_mapZoneControl = null;
	}

	private void EnsureUI()
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f0: Expected O, but got Unknown
		UIScreen activeScreen = base.UIManager.ActiveScreen;
		if (activeScreen != null && !(activeScreen is BattlefrontGameScreen) && activeScreen.GetWidget<MainViewport>() != null)
		{
			LayoutContainer val;
			try
			{
				val = ((Control)activeScreen).FindControl<LayoutContainer>("ViewportContainer");
			}
			catch (ArgumentException)
			{
				return;
			}
			if (_minimapPanel == null)
			{
				PubgMinimapControl pubgMinimapControl = new PubgMinimapControl();
				((Control)pubgMinimapControl).MinSize = new Vector2(200f, 200f);
				((Control)pubgMinimapControl).MaxSize = new Vector2(200f, 200f);
				_minimapControl = pubgMinimapControl;
				_minimapControl.OnMapClick = HandleMinimapClick;
				_minimapPanel = new PanelContainer
				{
					PanelOverride = (StyleBox)new StyleBoxFlat
					{
						BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#000000DD", (Color?)null),
						BorderColor = Color.FromHex((ReadOnlySpan<char>)"#ffa500", (Color?)null),
						BorderThickness = new Thickness(2f)
					}
				};
				((Control)_minimapPanel).AddChild((Control)(object)_minimapControl);
				((Control)val).AddChild((Control)(object)_minimapPanel);
			}
			else if ((object)((Control)_minimapPanel).Parent != val)
			{
				((Control)_minimapPanel).Orphan();
				((Control)val).AddChild((Control)(object)_minimapPanel);
			}
			UpdateMinimapSize();
			UpdateMinimapLayout();
			UpdateMinimapOpacity();
		}
	}

	private void HandleMinimapClick(MapCoordinates coords)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		CivTeamMemberComponent civTeamMemberComponent = default(CivTeamMemberComponent);
		if (localEntity.HasValue && base.EntityManager.TryGetComponent<CivTeamMemberComponent>(localEntity.Value, ref civTeamMemberComponent) && civTeamMemberComponent.IsCommander)
		{
			base.EntityManager.System<CivGlobalMapSystem>().RequestCommanderMoveToPosition(coords.MapId, coords.Position);
		}
		else
		{
			base.EntityManager.System<PubgPartyPingClientSystem>().QueueMapClick(coords);
		}
	}

	public void ShowMinimap()
	{
		_minimapVisible = true;
		EnsureUI();
	}

	public void HideMinimap()
	{
		_minimapVisible = false;
		_nextUpdate = TimeSpan.Zero;
		PanelContainer minimapPanel = _minimapPanel;
		if (minimapPanel != null && !((Control)minimapPanel).Disposed)
		{
			((Control)minimapPanel).Orphan();
		}
		_minimapPanel = null;
		_minimapControl = null;
	}
}
