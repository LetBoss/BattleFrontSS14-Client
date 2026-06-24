// Decompiled with JetBrains decompiler
// Type: Content.Client.Bql.ToolshedVisualizeEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.Bql;
using Content.Shared.Eui;
using Robust.Client.Console;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Client.Bql;

public sealed class ToolshedVisualizeEui : BaseEui
{
  private readonly ToolshedVisualizeWindow _window;

  public ToolshedVisualizeEui()
  {
    this._window = new ToolshedVisualizeWindow(IoCManager.Resolve<IClientConsoleHost>(), IoCManager.Resolve<ILocalizationManager>());
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
  }

  public override void HandleState(EuiStateBase state)
  {
    if (!(state is ToolshedVisualizeEuiState visualizeEuiState))
      return;
    this._window.Update(visualizeEuiState.Entities);
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window).Close();
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this._window).OpenCentered();
  }
}
