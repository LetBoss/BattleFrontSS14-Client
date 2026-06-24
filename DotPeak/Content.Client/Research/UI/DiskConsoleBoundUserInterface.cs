// Decompiled with JetBrains decompiler
// Type: Content.Client.Research.UI.DiskConsoleBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Research;
using Content.Shared.Research.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Research.UI;

public sealed class DiskConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private DiskConsoleMenu? _menu;

  protected virtual void Open()
  {
    base.Open();
    this._menu = BoundUserInterfaceExt.CreateWindow<DiskConsoleMenu>((BoundUserInterface) this);
    this._menu.OnServerButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new ConsoleServerSelectionMessage()));
    this._menu.OnPrintButtonPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new DiskConsolePrintDiskMessage()));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is DiskConsoleBoundUserInterfaceState state1))
      return;
    this._menu?.Update(state1);
  }
}
