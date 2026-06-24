// Decompiled with JetBrains decompiler
// Type: Content.Client.CrewManifest.CrewManifestEui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eui;
using Content.Shared.CrewManifest;
using Content.Shared.Eui;
using Robust.Client.UserInterface.CustomControls;
using System;

#nullable enable
namespace Content.Client.CrewManifest;

public sealed class CrewManifestEui : BaseEui
{
  private readonly CrewManifestUi _window;

  public CrewManifestEui()
  {
    this._window = new CrewManifestUi();
    ((BaseWindow) this._window).OnClose += (Action) (() => this.SendMessage((EuiMessageBase) new CloseEuiMessage()));
  }

  public override void Opened()
  {
    base.Opened();
    ((BaseWindow) this._window).OpenCentered();
  }

  public override void Closed()
  {
    base.Closed();
    ((BaseWindow) this._window).Close();
  }

  public override void HandleState(EuiStateBase state)
  {
    base.HandleState(state);
    if (!(state is CrewManifestEuiState manifestEuiState))
      return;
    this._window.Populate(manifestEuiState.StationName, manifestEuiState.Entries);
  }
}
