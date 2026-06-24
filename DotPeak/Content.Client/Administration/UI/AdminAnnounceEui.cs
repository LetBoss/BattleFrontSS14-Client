// Decompiled with JetBrains decompiler
// Type: Content.Client.Administration.UI.AdminAnnounceEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client.Administration.UI;

public sealed class AdminAnnounceEui : BaseEui
{
  private readonly AdminAnnounceWindow _window;

  public AdminAnnounceEui()
  {
    this._window = new AdminAnnounceWindow();
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
    ((BaseButton) this._window.AnnounceButton).OnPressed += new Action<BaseButton.ButtonEventArgs>(this.AnnounceButtonOnOnPressed);
  }

  private void AnnounceButtonOnOnPressed(BaseButton.ButtonEventArgs obj)
  {
    this.SendMessage((EuiMessageBase) new AdminAnnounceEuiMsg.DoAnnounce()
    {
      Announcement = Rope.Collapse(this._window.Announcement.TextRope),
      Announcer = this._window.Announcer.Text,
      AnnounceType = (AdminAnnounceType) (this._window.AnnounceMethod.SelectedMetadata ?? (object) AdminAnnounceType.Station),
      CloseAfter = !((BaseButton) this._window.KeepWindowOpen).Pressed
    });
  }

  public override void Opened() => ((BaseWindow) this._window).OpenCentered();

  public override void Closed() => ((BaseWindow) this._window).Close();
}
