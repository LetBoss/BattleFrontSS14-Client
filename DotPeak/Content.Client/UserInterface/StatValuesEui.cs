// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.StatValuesEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Eui;
using Content.Shared.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using System;

#nullable enable
namespace Content.Client.UserInterface;

public sealed class StatValuesEui : BaseEui
{
  private readonly StatsWindow _window;

  public StatValuesEui()
  {
    this._window = new StatsWindow();
    this._window.Title = "Melee stats";
    ((BaseWindow) this._window).OpenCentered();
    ((BaseWindow) this._window).OnClose += new Action(((BaseEui) this).Closed);
  }

  public override void HandleMessage(EuiMessageBase msg)
  {
    base.HandleMessage(msg);
    if (!(msg is StatValuesEuiMessage valuesEuiMessage))
      return;
    this._window.Title = valuesEuiMessage.Title;
    this._window.UpdateValues(valuesEuiMessage.Headers, valuesEuiMessage.Values);
  }
}
