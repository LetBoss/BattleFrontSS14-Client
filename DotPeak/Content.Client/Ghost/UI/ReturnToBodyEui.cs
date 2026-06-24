// Decompiled with JetBrains decompiler
// Type: Content.Client.Ghost.UI.ReturnToBodyEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.Ghost;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Ghost.UI;

public sealed class ReturnToBodyEui : BaseEui
{
  private readonly ReturnToBodyMenu _menu;

  public ReturnToBodyEui()
  {
    this._menu = new ReturnToBodyMenu();
    ((BaseButton) this._menu.DenyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendMessage((EuiMessageBase) new ReturnToBodyMessage(false));
      ((BaseWindow) this._menu).Close();
    });
    ((BaseButton) this._menu.AcceptButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendMessage((EuiMessageBase) new ReturnToBodyMessage(true));
      ((BaseWindow) this._menu).Close();
    });
  }

  public override void Opened()
  {
    IoCManager.Resolve<IClyde>().RequestWindowAttention();
    ((BaseWindow) this._menu).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    this.SendMessage((EuiMessageBase) new ReturnToBodyMessage(false));
    ((BaseWindow) this._menu).Close();
  }
}
