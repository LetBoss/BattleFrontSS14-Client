// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.Mutiny.MutineerInviteEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared._RMC14.Marines.Mutiny;
using Content.Shared.Eui;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines.Mutiny;

public sealed class MutineerInviteEui : BaseEui
{
  private readonly MutineerInviteWindow _window;

  public MutineerInviteEui()
  {
    this._window = new MutineerInviteWindow();
    ((BaseButton) this._window.DenyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendMessage((EuiMessageBase) new MutineerInviteChoiceMessage(MutineerInviteUiButton.Deny));
      ((BaseWindow) this._window).Close();
    });
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new MutineerInviteChoiceMessage(MutineerInviteUiButton.Deny)));
    ((BaseButton) this._window.AcceptButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
    {
      this.SendMessage((EuiMessageBase) new MutineerInviteChoiceMessage(MutineerInviteUiButton.Accept));
      ((BaseWindow) this._window).Close();
    });
  }

  public override void Opened()
  {
    IoCManager.Resolve<IClyde>().RequestWindowAttention();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed() => ((BaseWindow) this._window).Close();
}
