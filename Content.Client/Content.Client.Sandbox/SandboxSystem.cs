using System;
using Content.Client.Administration.Managers;
using Content.Client.Movement.Systems;
using Content.Shared.Sandbox;
using Robust.Client.Console;
using Robust.Client.Placement;
using Robust.Shared.Console;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Player;

namespace Content.Client.Sandbox;

public sealed class SandboxSystem : SharedSandboxSystem
{
	[Dependency]
	private IClientAdminManager _adminManager;

	[Dependency]
	private IClientConsoleHost _consoleHost;

	[Dependency]
	private IMapManager _map;

	[Dependency]
	private IPlacementManager _placement;

	[Dependency]
	private ContentEyeSystem _contentEye;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private SharedMapSystem _mapSystem;

	private bool _sandboxEnabled;

	public bool SandboxAllowed { get; private set; }

	public event Action? SandboxEnabled;

	public event Action? SandboxDisabled;

	public override void Initialize()
	{
		_adminManager.AdminStatusUpdated += CheckStatus;
		((EntitySystem)this).SubscribeNetworkEvent<MsgSandboxStatus>((EntityEventHandler<MsgSandboxStatus>)OnSandboxStatus, (Type[])null, (Type[])null);
	}

	private void CheckStatus()
	{
		bool flag = _sandboxEnabled || _adminManager.IsActive();
		if (flag != SandboxAllowed)
		{
			SandboxAllowed = flag;
			if (SandboxAllowed)
			{
				this.SandboxEnabled?.Invoke();
			}
			else
			{
				this.SandboxDisabled?.Invoke();
			}
		}
	}

	public override void Shutdown()
	{
		_adminManager.AdminStatusUpdated -= CheckStatus;
		((EntitySystem)this).Shutdown();
	}

	private void OnSandboxStatus(MsgSandboxStatus ev)
	{
		SetAllowed(ev.SandboxAllowed);
	}

	private void SetAllowed(bool sandboxEnabled)
	{
		_sandboxEnabled = sandboxEnabled;
		CheckStatus();
	}

	public void Respawn()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MsgSandboxRespawn());
	}

	public void GiveAdminAccess()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MsgSandboxGiveAccess());
	}

	public void GiveAGhost()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MsgSandboxGiveAghost());
	}

	public void Suicide()
	{
		((EntitySystem)this).RaiseNetworkEvent((EntityEventArgs)(object)new MsgSandboxSuicide());
	}

	public bool Copy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		if (!SandboxAllowed)
		{
			return false;
		}
		MetaDataComponent val = default(MetaDataComponent);
		if (((EntityUid)(ref uid)).IsValid() && ((EntitySystem)this).TryComp(uid, ref val) && !val.EntityDeleted)
		{
			if (val.EntityPrototype == null || val.EntityPrototype.HideSpawnMenu || val.EntityPrototype.Abstract)
			{
				return false;
			}
			if (_placement.Eraser)
			{
				_placement.ToggleEraser();
			}
			_placement.BeginPlacing(new PlacementInformation
			{
				EntityType = val.EntityPrototype.ID,
				IsTile = false,
				TileType = 0,
				PlacementOption = val.EntityPrototype.PlacementMode
			}, (PlacementHijack)null);
			return true;
		}
		EntityUid val2 = default(EntityUid);
		MapGridComponent val3 = default(MapGridComponent);
		TileRef val4 = default(TileRef);
		if (!_map.TryFindGridAt(_transform.ToMapCoordinates(coords, true), ref val2, ref val3) || !_mapSystem.TryGetTileRef(val2, val3, coords, ref val4))
		{
			return false;
		}
		if (_placement.Eraser)
		{
			_placement.ToggleEraser();
		}
		_placement.BeginPlacing(new PlacementInformation
		{
			EntityType = null,
			IsTile = true,
			TileType = val4.Tile.TypeId,
			PlacementOption = "AlignTileAny"
		}, (PlacementHijack)null);
		return true;
	}

	public void ToggleLight()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("togglelight");
	}

	public void ToggleFov()
	{
		_contentEye.RequestToggleFov();
	}

	public void ToggleShadows()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("toggleshadows");
	}

	public void ToggleSubFloor()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("showsubfloor");
	}

	public void ShowMarkers()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("showmarkers");
	}

	public void ShowBb()
	{
		((IConsoleHost)_consoleHost).ExecuteCommand("physics shapes");
	}
}
