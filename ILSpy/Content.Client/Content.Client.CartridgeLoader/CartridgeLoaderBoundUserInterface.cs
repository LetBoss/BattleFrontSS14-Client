using System;
using System.Collections.Generic;
using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.CartridgeLoader;

public abstract class CartridgeLoaderBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private EntityUid? _activeProgram;

	[ViewVariables]
	private UIFragment? _activeCartridgeUI;

	[ViewVariables]
	private Control? _activeUiFragment;

	private IEntityManager _entManager;

	protected CartridgeLoaderBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_entManager = IoCManager.Resolve<IEntityManager>();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is CartridgeLoaderUiState cartridgeLoaderUiState))
		{
			_activeCartridgeUI?.UpdateState(state);
			return;
		}
		List<(EntityUid, CartridgeComponent)> cartridgeComponents = GetCartridgeComponents(_entManager.GetEntityList(cartridgeLoaderUiState.Programs));
		UpdateAvailablePrograms(cartridgeComponents);
		EntityUid? cartridgeUid = (_activeProgram = _entManager.GetEntity(cartridgeLoaderUiState.ActiveUI));
		UIFragment uIFragment = RetrieveCartridgeUI(cartridgeUid);
		CartridgeComponent cartridgeComponent = RetrieveCartridgeComponent(cartridgeUid);
		Control val = uIFragment?.GetUIFragmentRoot();
		if (!(((object)_activeUiFragment)?.GetType() == ((object)val)?.GetType()))
		{
			if (_activeUiFragment != null)
			{
				DetachCartridgeUI(_activeUiFragment);
			}
			if (val != null && _activeProgram.HasValue)
			{
				AttachCartridgeUI(val, Loc.GetString(LocId.op_Implicit(cartridgeComponent?.ProgramName ?? LocId.op_Implicit("default-program-name"))));
				SendCartridgeUiReadyEvent(_activeProgram.Value);
			}
			_activeCartridgeUI = uIFragment;
			Control? activeUiFragment = _activeUiFragment;
			if (activeUiFragment != null)
			{
				activeUiFragment.Orphan();
			}
			_activeUiFragment = val;
		}
	}

	protected void ActivateCartridge(EntityUid cartridgeUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(_entManager.GetNetEntity(cartridgeUid, (MetaDataComponent)null), CartridgeUiMessageAction.Activate);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)cartridgeLoaderUiMessage);
	}

	protected void DeactivateActiveCartridge()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (_activeProgram.HasValue)
		{
			CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(_entManager.GetNetEntity(_activeProgram.Value, (MetaDataComponent)null), CartridgeUiMessageAction.Deactivate);
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)cartridgeLoaderUiMessage);
		}
	}

	protected void InstallCartridge(EntityUid cartridgeUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(_entManager.GetNetEntity(cartridgeUid, (MetaDataComponent)null), CartridgeUiMessageAction.Install);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)cartridgeLoaderUiMessage);
	}

	protected void UninstallCartridge(EntityUid cartridgeUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(_entManager.GetNetEntity(cartridgeUid, (MetaDataComponent)null), CartridgeUiMessageAction.Uninstall);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)cartridgeLoaderUiMessage);
	}

	private List<(EntityUid, CartridgeComponent)> GetCartridgeComponents(List<EntityUid> programs)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		List<(EntityUid, CartridgeComponent)> list = new List<(EntityUid, CartridgeComponent)>();
		foreach (EntityUid program in programs)
		{
			CartridgeComponent cartridgeComponent = RetrieveCartridgeComponent(program);
			if (cartridgeComponent != null)
			{
				list.Add((program, cartridgeComponent));
			}
		}
		return list;
	}

	protected abstract void AttachCartridgeUI(Control cartridgeUIFragment, string? title);

	protected abstract void DetachCartridgeUI(Control cartridgeUIFragment);

	protected abstract void UpdateAvailablePrograms(List<(EntityUid, CartridgeComponent)> programs);

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			Control? activeUiFragment = _activeUiFragment;
			if (activeUiFragment != null)
			{
				activeUiFragment.Orphan();
			}
		}
	}

	protected CartridgeComponent? RetrieveCartridgeComponent(EntityUid? cartridgeUid)
	{
		return EntityManagerExt.GetComponentOrNull<CartridgeComponent>(base.EntMan, cartridgeUid);
	}

	private void SendCartridgeUiReadyEvent(EntityUid cartridgeUid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CartridgeLoaderUiMessage cartridgeLoaderUiMessage = new CartridgeLoaderUiMessage(_entManager.GetNetEntity(cartridgeUid, (MetaDataComponent)null), CartridgeUiMessageAction.UIReady);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)cartridgeLoaderUiMessage);
	}

	private UIFragment? RetrieveCartridgeUI(EntityUid? cartridgeUid)
	{
		UIFragmentComponent componentOrNull = EntityManagerExt.GetComponentOrNull<UIFragmentComponent>(base.EntMan, cartridgeUid);
		componentOrNull?.Ui?.Setup((BoundUserInterface)(object)this, cartridgeUid);
		return componentOrNull?.Ui;
	}
}
