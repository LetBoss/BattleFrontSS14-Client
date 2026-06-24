// Decompiled with JetBrains decompiler
// Type: Content.Client.PDA.PdaBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.CartridgeLoader;
using Content.Shared.CartridgeLoader;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.PDA;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.PDA;

public sealed class PdaBoundUserInterface : CartridgeLoaderBoundUserInterface
{
  private readonly PdaSystem _pdaSystem;
  [Robust.Shared.ViewVariables.ViewVariables]
  private PdaMenu? _menu;

  public PdaBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._pdaSystem = this.EntMan.System<PdaSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    if (this._menu != null)
      return;
    this.CreateMenu();
  }

  private void CreateMenu()
  {
    this._menu = BoundUserInterfaceExt.CreateWindowCenteredLeft<PdaMenu>((BoundUserInterface) this);
    ((BaseButton) this._menu.FlashLightToggleButton).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new PdaToggleFlashlightMessage()));
    ((BaseButton) this._menu.EjectIdButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("PDA-id")));
    ((BaseButton) this._menu.EjectPenButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("PDA-pen")));
    ((BaseButton) this._menu.EjectPaiButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new ItemSlotButtonPressedEvent("PDA-pai")));
    ((BaseButton) this._menu.ActivateMusicButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new PdaShowMusicMessage()));
    ((BaseButton) this._menu.AccessRingtoneButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new PdaShowRingtoneMessage()));
    ((BaseButton) this._menu.ShowUplinkButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new PdaShowUplinkMessage()));
    ((BaseButton) this._menu.LockUplinkButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendMessage((BoundUserInterfaceMessage) new PdaLockUplinkMessage()));
    this._menu.OnProgramItemPressed += new Action<EntityUid>(((CartridgeLoaderBoundUserInterface) this).ActivateCartridge);
    this._menu.OnInstallButtonPressed += new Action<EntityUid>(((CartridgeLoaderBoundUserInterface) this).InstallCartridge);
    this._menu.OnUninstallButtonPressed += new Action<EntityUid>(((CartridgeLoaderBoundUserInterface) this).UninstallCartridge);
    ((BaseButton) this._menu.ProgramCloseButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.DeactivateActiveCartridge());
    PdaBorderColorComponent borderColorComponent = this.GetBorderColorComponent();
    if (borderColorComponent == null)
      return;
    this._menu.BorderColor = borderColorComponent.BorderColor;
    this._menu.AccentHColor = borderColorComponent.AccentHColor;
    this._menu.AccentVColor = borderColorComponent.AccentVColor;
  }

  protected override void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is PdaUpdateState state1))
      return;
    if (this._menu == null)
      this._pdaSystem.Log.Error("PDA state received before menu was created.");
    else
      this._menu.UpdateState(state1);
  }

  protected override void AttachCartridgeUI(Control cartridgeUIFragment, string? title)
  {
    ((Control) this._menu?.ProgramView).AddChild(cartridgeUIFragment);
    this._menu?.ToProgramView(title ?? Loc.GetString("comp-pda-io-program-fallback-title"));
  }

  protected override void DetachCartridgeUI(Control cartridgeUIFragment)
  {
    if (this._menu == null)
      return;
    this._menu.ToHomeScreen();
    this._menu.HideProgramHeader();
    ((Control) this._menu.ProgramView).RemoveChild(cartridgeUIFragment);
  }

  protected override void UpdateAvailablePrograms(List<(EntityUid, CartridgeComponent)> programs)
  {
    this._menu?.UpdateAvailablePrograms(programs);
  }

  private PdaBorderColorComponent? GetBorderColorComponent()
  {
    return EntityManagerExt.GetComponentOrNull<PdaBorderColorComponent>(this.EntMan, this.Owner);
  }
}
