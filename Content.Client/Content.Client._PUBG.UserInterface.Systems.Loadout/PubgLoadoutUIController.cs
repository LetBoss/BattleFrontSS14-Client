using System;
using Content.Client.Gameplay;
using Content.Shared._PUBG.Input;
using Content.Shared._PUBG.Loadout;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Player;

namespace Content.Client._PUBG.UserInterface.Systems.Loadout;

public sealed class PubgLoadoutUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
	private sealed class ToggleLoadoutInputHandler : InputCmdHandler
	{
		private readonly Func<bool> _onPressed;

		public ToggleLoadoutInputHandler(Func<bool> onPressed)
		{
			_onPressed = onPressed;
		}

		public override bool HandleCmdMessage(IEntityManager entManager, ICommonSession? session, IFullInputCmdMessage message)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			if ((int)message.State != 1)
			{
				return false;
			}
			return _onPressed();
		}
	}

	[Dependency]
	private IPlayerManager _player;

	[Dependency]
	private IEntityManager _entMan;

	private PubgWeaponModulesWindow? _window;

	private bool _systemSubscribed;

	public void OnStateEntered(GameplayState state)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		EnsureSystemSubscribed();
		CommandBinds.Builder.Bind(PubgKeyFunctions.PubgInventoryMenu, (InputCmdHandler)(object)new ToggleLoadoutInputHandler(TryToggleWindow)).Register<PubgLoadoutUIController>();
	}

	public void OnStateExited(GameplayState state)
	{
		CommandBinds.Unregister<PubgLoadoutUIController>();
		SetPolling(enabled: false);
		UnsubscribeSystem();
		CloseWindow();
	}

	private void EnsureSystemSubscribed()
	{
		if (!_systemSubscribed)
		{
			_entMan.System<PubgLoadoutSystem>().OnStateReceived += OnStateReceived;
			_systemSubscribed = true;
		}
	}

	private void UnsubscribeSystem()
	{
		if (_systemSubscribed)
		{
			PubgLoadoutSystem pubgLoadoutSystem = _entMan.SystemOrNull<PubgLoadoutSystem>();
			if (pubgLoadoutSystem != null)
			{
				pubgLoadoutSystem.OnStateReceived -= OnStateReceived;
			}
			_systemSubscribed = false;
		}
	}

	private void EnsureWindow()
	{
		PubgWeaponModulesWindow window = _window;
		if (window == null || ((Control)window).Disposed)
		{
			_window = base.UIManager.CreateWindow<PubgWeaponModulesWindow>();
			_window.ModuleRemoveRequested += OnModuleRemoveRequested;
			((BaseWindow)_window).OnClose += OnWindowClosed;
		}
	}

	public void ToggleForTest()
	{
		TryToggleWindow();
	}

	private bool TryToggleWindow()
	{
		PubgLoadoutSystem pubgLoadoutSystem = _entMan.SystemOrNull<PubgLoadoutSystem>();
		if (pubgLoadoutSystem == null)
		{
			return false;
		}
		EnsureWindow();
		if (_window == null)
		{
			return false;
		}
		if (((BaseWindow)_window).IsOpen)
		{
			SetPolling(enabled: false);
			CloseWindow();
			return true;
		}
		((BaseWindow)_window).OpenCentered();
		pubgLoadoutSystem.SetPolling(enabled: true);
		pubgLoadoutSystem.RequestState(force: true);
		return true;
	}

	private void OnWindowClosed()
	{
		SetPolling(enabled: false);
		ReleaseWindow();
	}

	private void OnModuleRemoveRequested(EntityUid weapon, string slotId)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		_entMan.SystemOrNull<PubgLoadoutSystem>()?.RequestAction(PubgLoadoutActionType.RemoveModule, default(EntityUid), PubgLoadoutSection.Other, weapon, slotId);
	}

	private void OnStateReceived(PubgLoadoutStateMessage msg)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null && ((BaseWindow)_window).IsOpen)
		{
			(EntityUid, PubgLoadoutWeaponState)? tuple = ResolveActiveWeapon(msg);
			if (tuple.HasValue)
			{
				(EntityUid, PubgLoadoutWeaponState) valueOrDefault = tuple.GetValueOrDefault();
				_window.UpdateData(valueOrDefault.Item1, valueOrDefault.Item2, _entMan);
			}
			else
			{
				_window.ShowEmpty();
			}
		}
	}

	private (EntityUid Uid, PubgLoadoutWeaponState State)? ResolveActiveWeapon(PubgLoadoutStateMessage msg)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (msg.Weapons.Count == 0)
		{
			return null;
		}
		EntityUid? localEntity = ((ISharedPlayerManager)_player).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			HandsComponent item = default(HandsComponent);
			if (!_entMan.TryGetComponent<HandsComponent>(valueOrDefault, ref item))
			{
				return ResolveFirstValid(msg);
			}
			if (!_entMan.System<SharedHandsSystem>().TryGetActiveItem(Entity<HandsComponent>.op_Implicit((valueOrDefault, item)), out var item2) || !item2.HasValue)
			{
				return null;
			}
			foreach (PubgLoadoutWeaponState weapon in msg.Weapons)
			{
				EntityUid entity = _entMan.GetEntity(weapon.Entity);
				if (entity == item2.Value)
				{
					return (entity, weapon);
				}
			}
			return null;
		}
		return ResolveFirstValid(msg);
	}

	private (EntityUid Uid, PubgLoadoutWeaponState State)? ResolveFirstValid(PubgLoadoutStateMessage msg)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		foreach (PubgLoadoutWeaponState weapon in msg.Weapons)
		{
			EntityUid entity = _entMan.GetEntity(weapon.Entity);
			if (entity != EntityUid.Invalid && _entMan.EntityExists(entity))
			{
				return (entity, weapon);
			}
		}
		return null;
	}

	private void SetPolling(bool enabled)
	{
		_entMan.SystemOrNull<PubgLoadoutSystem>()?.SetPolling(enabled);
	}

	private void CloseWindow()
	{
		if (_window != null)
		{
			PubgWeaponModulesWindow? window = _window;
			ReleaseWindow();
			((BaseWindow)window).Close();
		}
	}

	private void ReleaseWindow()
	{
		if (_window != null)
		{
			PubgWeaponModulesWindow? window = _window;
			_window = null;
			window.ModuleRemoveRequested -= OnModuleRemoveRequested;
			((BaseWindow)window).OnClose -= OnWindowClosed;
		}
	}
}
