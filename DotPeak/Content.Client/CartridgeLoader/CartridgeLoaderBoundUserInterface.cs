// Decompiled with JetBrains decompiler
// Type: Content.Client.CartridgeLoader.CartridgeLoaderBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.CartridgeLoader;

public abstract class CartridgeLoaderBoundUserInterface : BoundUserInterface
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private EntityUid? _activeProgram;
  [Robust.Shared.ViewVariables.ViewVariables]
  private UIFragment? _activeCartridgeUI;
  [Robust.Shared.ViewVariables.ViewVariables]
  private Control? _activeUiFragment;
  private IEntityManager _entManager;

  protected CartridgeLoaderBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._entManager = IoCManager.Resolve<IEntityManager>();
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is CartridgeLoaderUiState cartridgeLoaderUiState))
    {
      this._activeCartridgeUI?.UpdateState(state);
    }
    else
    {
      this.UpdateAvailablePrograms(this.GetCartridgeComponents(this._entManager.GetEntityList(cartridgeLoaderUiState.Programs)));
      EntityUid? entity = this._entManager.GetEntity(cartridgeLoaderUiState.ActiveUI);
      this._activeProgram = entity;
      UIFragment uiFragment = this.RetrieveCartridgeUI(entity);
      CartridgeComponent cartridgeComponent = this.RetrieveCartridgeComponent(entity);
      Control uiFragmentRoot = uiFragment?.GetUIFragmentRoot();
      if (this._activeUiFragment?.GetType() == uiFragmentRoot?.GetType())
        return;
      if (this._activeUiFragment != null)
        this.DetachCartridgeUI(this._activeUiFragment);
      if (uiFragmentRoot != null && this._activeProgram.HasValue)
      {
        this.AttachCartridgeUI(uiFragmentRoot, Loc.GetString(LocId.op_Implicit(cartridgeComponent != null ? cartridgeComponent.ProgramName : LocId.op_Implicit("default-program-name"))));
        this.SendCartridgeUiReadyEvent(this._activeProgram.Value);
      }
      this._activeCartridgeUI = uiFragment;
      this._activeUiFragment?.Orphan();
      this._activeUiFragment = uiFragmentRoot;
    }
  }

  protected void ActivateCartridge(EntityUid cartridgeUid)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CartridgeLoaderUiMessage(this._entManager.GetNetEntity(cartridgeUid, (MetaDataComponent) null), CartridgeUiMessageAction.Activate));
  }

  protected void DeactivateActiveCartridge()
  {
    if (!this._activeProgram.HasValue)
      return;
    this.SendMessage((BoundUserInterfaceMessage) new CartridgeLoaderUiMessage(this._entManager.GetNetEntity(this._activeProgram.Value, (MetaDataComponent) null), CartridgeUiMessageAction.Deactivate));
  }

  protected void InstallCartridge(EntityUid cartridgeUid)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CartridgeLoaderUiMessage(this._entManager.GetNetEntity(cartridgeUid, (MetaDataComponent) null), CartridgeUiMessageAction.Install));
  }

  protected void UninstallCartridge(EntityUid cartridgeUid)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CartridgeLoaderUiMessage(this._entManager.GetNetEntity(cartridgeUid, (MetaDataComponent) null), CartridgeUiMessageAction.Uninstall));
  }

  private List<(EntityUid, CartridgeComponent)> GetCartridgeComponents(List<EntityUid> programs)
  {
    List<(EntityUid, CartridgeComponent)> cartridgeComponents = new List<(EntityUid, CartridgeComponent)>();
    foreach (EntityUid program in programs)
    {
      CartridgeComponent cartridgeComponent = this.RetrieveCartridgeComponent(new EntityUid?(program));
      if (cartridgeComponent != null)
        cartridgeComponents.Add((program, cartridgeComponent));
    }
    return cartridgeComponents;
  }

  protected abstract void AttachCartridgeUI(Control cartridgeUIFragment, string? title);

  protected abstract void DetachCartridgeUI(Control cartridgeUIFragment);

  protected abstract void UpdateAvailablePrograms(List<(EntityUid, CartridgeComponent)> programs);

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    this._activeUiFragment?.Orphan();
  }

  protected CartridgeComponent? RetrieveCartridgeComponent(EntityUid? cartridgeUid)
  {
    return EntityManagerExt.GetComponentOrNull<CartridgeComponent>(this.EntMan, cartridgeUid);
  }

  private void SendCartridgeUiReadyEvent(EntityUid cartridgeUid)
  {
    this.SendMessage((BoundUserInterfaceMessage) new CartridgeLoaderUiMessage(this._entManager.GetNetEntity(cartridgeUid, (MetaDataComponent) null), CartridgeUiMessageAction.UIReady));
  }

  private UIFragment? RetrieveCartridgeUI(EntityUid? cartridgeUid)
  {
    UIFragmentComponent componentOrNull = EntityManagerExt.GetComponentOrNull<UIFragmentComponent>(this.EntMan, cartridgeUid);
    componentOrNull?.Ui?.Setup((BoundUserInterface) this, cartridgeUid);
    return componentOrNull?.Ui;
  }
}
