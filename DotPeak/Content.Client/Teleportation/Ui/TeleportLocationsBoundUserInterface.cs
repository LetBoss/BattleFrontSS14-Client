// Decompiled with JetBrains decompiler
// Type: Content.Client.Teleportation.Ui.TeleportLocationsBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Teleportation;
using Content.Shared.Teleportation.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Teleportation.Ui;

public sealed class TeleportLocationsBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private TeleportMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<TeleportMenu>((BoundUserInterface) this);
    TeleportLocationsComponent locationsComponent;
    if (!this.EntMan.TryGetComponent<TeleportLocationsComponent>(this.Owner, ref locationsComponent))
      return;
    this._menu.Title = Loc.GetString(LocId.op_Implicit(locationsComponent.Name));
    this._menu.Warps = locationsComponent.AvailableWarps;
    this._menu.AddTeleportButtons();
    this._menu.TeleportClicked += (Action<NetEntity, string>) ((netEnt, pointName) => this.SendPredictedMessage((BoundUserInterfaceMessage) new TeleportLocationDestinationMessage(netEnt, pointName)));
  }
}
