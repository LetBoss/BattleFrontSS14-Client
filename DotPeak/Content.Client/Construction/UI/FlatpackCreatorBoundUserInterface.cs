// Decompiled with JetBrains decompiler
// Type: Content.Client.Construction.UI.FlatpackCreatorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Construction.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Construction.UI;

public sealed class FlatpackCreatorBoundUserInterface(EntityUid owner, Enum uiKey) : 
  BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private FlatpackCreatorMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<FlatpackCreatorMenu>((BoundUserInterface) this);
    this._menu.SetEntity(this.Owner);
    this._menu.PackButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new FlatpackCreatorStartPackBuiMessage()));
    this._menu.OpenCentered();
  }
}
