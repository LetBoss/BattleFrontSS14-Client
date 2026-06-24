// Decompiled with JetBrains decompiler
// Type: Content.Client.Fax.AdminUI.AdminFaxEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Fax;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client.Fax.AdminUI;

public sealed class AdminFaxEui : BaseEui
{
  private readonly AdminFaxWindow _window;

  public AdminFaxEui()
  {
    this._window = new AdminFaxWindow();
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new AdminFaxEuiMsg.Close()));
    this._window.OnFollowFax += (Action<NetEntity>) (entity => this.SendMessage((EuiMessageBase) new AdminFaxEuiMsg.Follow(entity)));
    this._window.OnMessageSend += (Action<(NetEntity, string, string, string, string, Color, bool)>) (args => this.SendMessage((EuiMessageBase) new AdminFaxEuiMsg.Send(args.entity, args.title, args.stampedBy, args.message, args.stampSprite, args.stampColor, args.locked)));
  }

  public override void Opened() => ((BaseWindow) this._window).OpenCentered();

  public override void Closed() => ((BaseWindow) this._window).Close();

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is AdminFaxEuiState adminFaxEuiState))
      return;
    this._window.PopulateFaxes(adminFaxEuiState.Entries);
  }
}
