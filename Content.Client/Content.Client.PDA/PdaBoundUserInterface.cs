using System;
using System.Collections.Generic;
using Content.Client.CartridgeLoader;
using Content.Shared.CartridgeLoader;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.PDA;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client.PDA;

public sealed class PdaBoundUserInterface : CartridgeLoaderBoundUserInterface
{
	private readonly PdaSystem _pdaSystem;

	[ViewVariables]
	private PdaMenu? _menu;

	public PdaBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_pdaSystem = ((BoundUserInterface)this).EntMan.System<PdaSystem>();
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		if (_menu == null)
		{
			CreateMenu();
		}
	}

	private void CreateMenu()
	{
		_menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<PdaMenu>((BoundUserInterface)(object)this);
		((BaseButton)_menu.FlashLightToggleButton).OnToggled += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PdaToggleFlashlightMessage());
		};
		((BaseButton)_menu.EjectIdButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("PDA-id"));
		};
		((BaseButton)_menu.EjectPenButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("PDA-pen"));
		};
		((BaseButton)_menu.EjectPaiButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new ItemSlotButtonPressedEvent("PDA-pai"));
		};
		((BaseButton)_menu.ActivateMusicButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PdaShowMusicMessage());
		};
		((BaseButton)_menu.AccessRingtoneButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PdaShowRingtoneMessage());
		};
		((BaseButton)_menu.ShowUplinkButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PdaShowUplinkMessage());
		};
		((BaseButton)_menu.LockUplinkButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PdaLockUplinkMessage());
		};
		_menu.OnProgramItemPressed += base.ActivateCartridge;
		_menu.OnInstallButtonPressed += base.InstallCartridge;
		_menu.OnUninstallButtonPressed += base.UninstallCartridge;
		((BaseButton)_menu.ProgramCloseButton).OnPressed += delegate
		{
			DeactivateActiveCartridge();
		};
		PdaBorderColorComponent borderColorComponent = GetBorderColorComponent();
		if (borderColorComponent != null)
		{
			_menu.BorderColor = borderColorComponent.BorderColor;
			_menu.AccentHColor = borderColorComponent.AccentHColor;
			_menu.AccentVColor = borderColorComponent.AccentVColor;
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		base.UpdateState(state);
		if (state is PdaUpdateState state2)
		{
			if (_menu == null)
			{
				((EntitySystem)_pdaSystem).Log.Error("PDA state received before menu was created.");
			}
			else
			{
				_menu.UpdateState(state2);
			}
		}
	}

	protected override void AttachCartridgeUI(Control cartridgeUIFragment, string? title)
	{
		PdaMenu? menu = _menu;
		if (menu != null)
		{
			((Control)menu.ProgramView).AddChild(cartridgeUIFragment);
		}
		_menu?.ToProgramView(title ?? Loc.GetString("comp-pda-io-program-fallback-title"));
	}

	protected override void DetachCartridgeUI(Control cartridgeUIFragment)
	{
		if (_menu != null)
		{
			_menu.ToHomeScreen();
			_menu.HideProgramHeader();
			((Control)_menu.ProgramView).RemoveChild(cartridgeUIFragment);
		}
	}

	protected override void UpdateAvailablePrograms(List<(EntityUid, CartridgeComponent)> programs)
	{
		_menu?.UpdateAvailablePrograms(programs);
	}

	private PdaBorderColorComponent? GetBorderColorComponent()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return EntityManagerExt.GetComponentOrNull<PdaBorderColorComponent>(((BoundUserInterface)this).EntMan, ((BoundUserInterface)this).Owner);
	}
}
