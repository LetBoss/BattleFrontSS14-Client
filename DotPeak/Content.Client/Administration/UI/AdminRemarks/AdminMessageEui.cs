// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.AdminRemarks.AdminMessageEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration.Notes;
using Content.Shared.Eui;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using System;

#nullable enable
namespace Content.Client.Administration.UI.AdminRemarks;

public sealed class AdminMessageEui : BaseEui
{
  private readonly AdminMessagePopupWindow _popup;

  public AdminMessageEui()
  {
    this._popup = new AdminMessagePopupWindow();
    this._popup.OnAcceptPressed += (Action) (() => this.SendMessage((EuiMessageBase) new AdminMessageEuiMsg.Dismiss(true)));
    this._popup.OnDismissPressed += (Action) (() => this.SendMessage((EuiMessageBase) new AdminMessageEuiMsg.Dismiss(false)));
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is AdminMessageEuiState state1))
      return;
    this._popup.SetState(state1);
  }

  public override void Opened()
  {
    ((Control) this._popup.UserInterfaceManager.WindowRoot).AddChild((Control) this._popup);
    LayoutContainer.SetAnchorPreset((Control) this._popup, (LayoutContainer.LayoutPreset) 15, false);
  }

  public override void Closed() => this._popup.Orphan();
}
