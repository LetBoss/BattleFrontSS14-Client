using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Administration.Events;
using Content.Shared.Roles;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

namespace Content.Client.Administration.Systems;

public sealed class AdminSystem : EntitySystem
{
	private Dictionary<NetUserId, PlayerInfo>? _playerList;

	[Dependency]
	private IOverlayManager _overlayManager;

	[Dependency]
	private IResourceCache _resourceCache;

	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IEyeManager _eyeManager;

	[Dependency]
	private EntityLookupSystem _entityLookup;

	[Dependency]
	private IUserInterfaceManager _userInterfaceManager;

	[Dependency]
	private IConfigurationManager _configurationManager;

	[Dependency]
	private SharedRoleSystem _roles;

	[Dependency]
	private IPrototypeManager _proto;

	private AdminNameOverlay _adminNameOverlay;

	public IReadOnlyList<PlayerInfo> PlayerList
	{
		get
		{
			if (_playerList != null)
			{
				return _playerList.Values.ToList();
			}
			return new List<PlayerInfo>();
		}
	}

	public event Action<List<PlayerInfo>>? PlayerListChanged;

	public event Action? OverlayEnabled;

	public event Action? OverlayDisabled;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeOverlay();
		((EntitySystem)this).SubscribeNetworkEvent<FullPlayerListEvent>((EntityEventHandler<FullPlayerListEvent>)OnPlayerListChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeNetworkEvent<PlayerInfoChangedEvent>((EntityEventHandler<PlayerInfoChangedEvent>)OnPlayerInfoChanged, (Type[])null, (Type[])null);
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ShutdownOverlay();
	}

	private void OnPlayerInfoChanged(PlayerInfoChangedEvent ev)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!(ev.PlayerInfo == null))
		{
			if (_playerList == null)
			{
				_playerList = new Dictionary<NetUserId, PlayerInfo>();
			}
			_playerList[ev.PlayerInfo.SessionId] = ev.PlayerInfo;
			this.PlayerListChanged?.Invoke(_playerList.Values.ToList());
		}
	}

	private void OnPlayerListChanged(FullPlayerListEvent msg)
	{
		_playerList = msg.PlayersInfo.ToDictionary((PlayerInfo x) => x.SessionId, (PlayerInfo x) => x);
		this.PlayerListChanged?.Invoke(msg.PlayersInfo);
	}

	private void InitializeOverlay()
	{
		_adminNameOverlay = new AdminNameOverlay(this, (IEntityManager)(object)base.EntityManager, _eyeManager, _resourceCache, _entityLookup, _userInterfaceManager, _configurationManager, _roles, _proto);
		_adminManager.AdminStatusUpdated += OnAdminStatusUpdated;
	}

	private void ShutdownOverlay()
	{
		_adminManager.AdminStatusUpdated -= OnAdminStatusUpdated;
	}

	private void OnAdminStatusUpdated()
	{
		AdminOverlayOff();
	}

	public void AdminOverlayOn()
	{
		if (!_overlayManager.HasOverlay<AdminNameOverlay>())
		{
			_overlayManager.AddOverlay((Overlay)(object)_adminNameOverlay);
			this.OverlayEnabled?.Invoke();
		}
	}

	public void AdminOverlayOff()
	{
		_overlayManager.RemoveOverlay<AdminNameOverlay>();
		this.OverlayDisabled?.Invoke();
	}
}
