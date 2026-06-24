// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.Boombox.UI.PubgPhoneBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Boombox;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._PUBG.Boombox.UI;

public sealed class PubgPhoneBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private PubgPhoneWindow? _window;

  protected virtual void Open()
  {
    base.Open();
    this._window = new PubgPhoneWindow(this.EntMan, this.Owner);
    ((BaseWindow) this._window).OpenCentered();
    ((BaseWindow) this._window).OnClose += new Action(((BoundUserInterface) this).Close);
    this._window.OnSyncPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new PubgBoomboxSyncMessage()));
    this._window.OnPlayPressed += (Action<string>) (trackId => this.SendMessage((BoundUserInterfaceMessage) new PubgBoomboxPlayMessage(trackId)));
    this._window.OnStopPressed += (Action) (() => this.SendMessage((BoundUserInterfaceMessage) new PubgBoomboxStopMessage()));
    this._window.OnSeekReleased += (Action<float>) (seconds => this.SendMessage((BoundUserInterfaceMessage) new PubgBoomboxSeekMessage(seconds)));
    this._window.OnVolumeReleased += (Action<float>) (volume => this.SendMessage((BoundUserInterfaceMessage) new PubgBoomboxVolumeMessage(volume)));
    this._window.OnRangeReleased += (Action<float>) (range => this.SendMessage((BoundUserInterfaceMessage) new PubgBoomboxRangeMessage(range)));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (!(state is PubgBoomboxUiState state1))
      return;
    this._window?.UpdateLibrary(state1);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Dispose();
    this._window = (PubgPhoneWindow) null;
  }
}
