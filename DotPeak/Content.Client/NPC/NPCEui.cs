// Decompiled with JetBrains decompiler
// Type: Content.Client.NPC.NPCEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Eui;
using System;

#nullable enable
namespace Content.Client.NPC;

public sealed class NPCEui : BaseEui
{
  private NPCWindow? _window = new NPCWindow();

  public override void Opened()
  {
    base.Opened();
    this._window = new NPCWindow();
    this._window.OpenCentered();
    this._window.OnClose += new Action(this.OnClosed);
  }

  private void OnClosed() => this.SendMessage((EuiMessageBase) new CloseEuiMessage());

  public override void Closed()
  {
    base.Closed();
    this._window?.Close();
    this._window = (NPCWindow) null;
  }
}
