// Decompiled with JetBrains decompiler
// Type: Content.Client.Silicons.Borgs.BorgBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Silicons.Borgs;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Silicons.Borgs;

public sealed class BorgBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private BorgMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<BorgMenu>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.BrainButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new BorgEjectBrainBuiMessage()));
    this._menu.EjectBatteryButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new BorgEjectBatteryBuiMessage()));
    this._menu.NameChanged += (Action<string>) (name => this.SendMessage((BoundUserInterfaceMessage) new BorgSetNameBuiMessage(name)));
    this._menu.RemoveModuleButtonPressed += (Action<EntityUid>) (module => this.SendMessage((BoundUserInterfaceMessage) new BorgRemoveModuleBuiMessage(this.EntMan.GetNetEntity(module, (MetaDataComponent) null))));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is BorgBuiState state1))
      return;
    this._menu?.UpdateState(state1);
  }
}
