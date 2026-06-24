// Decompiled with JetBrains decompiler
// Type: Content.Client.Pinpointer.UI.NavMapBeaconBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Pinpointer;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Pinpointer.UI;

public sealed class NavMapBeaconBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private NavMapBeaconWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<NavMapBeaconWindow>((BoundUserInterface) this);
    NavMapBeaconComponent navMap;
    if (this.EntMan.TryGetComponent<NavMapBeaconComponent>(this.Owner, ref navMap))
      this._window.SetEntity(this.Owner, navMap);
    this._window.OnApplyButtonPressed += (Action<string, bool, Color>) ((label, enabled, color) => this.SendMessage((BoundUserInterfaceMessage) new NavMapBeaconConfigureBuiMessage(label, enabled, color)));
  }
}
