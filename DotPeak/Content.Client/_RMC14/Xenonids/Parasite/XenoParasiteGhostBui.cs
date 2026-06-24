// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Parasite.XenoParasiteGhostBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Egg;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Parasite;

public sealed class XenoParasiteGhostBui(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private XenoParasiteGhostWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<XenoParasiteGhostWindow>((BoundUserInterface) this);
    ((BaseButton) this._window.DenyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => ((BaseWindow) this._window).Close());
    ((BaseButton) this._window.ConfirmButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new XenoParasiteGhostBuiMsg()));
  }
}
