// Decompiled with JetBrains decompiler
// Type: Content.Client.SurveillanceCamera.UI.SurveillanceCameraSetupBoundUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.SurveillanceCamera;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.SurveillanceCamera.UI;

public sealed class SurveillanceCameraSetupBoundUi : BoundUserInterface
{
  [Robust.Shared.ViewVariables.ViewVariables]
  private readonly SurveillanceCameraSetupUiKey _type;
  [Robust.Shared.ViewVariables.ViewVariables]
  private SurveillanceCameraSetupWindow? _window;

  public SurveillanceCameraSetupBoundUi(EntityUid component, Enum uiKey)
    : base(component, uiKey)
  {
    if (!(uiKey is SurveillanceCameraSetupUiKey cameraSetupUiKey))
      return;
    this._type = cameraSetupUiKey;
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = new SurveillanceCameraSetupWindow();
    if (this._type == SurveillanceCameraSetupUiKey.Router)
      this._window.HideNameSelector();
    ((BaseWindow) this._window).OpenCentered();
    this._window.OnNameConfirm += new Action<string>(this.SendDeviceName);
    this._window.OnNetworkConfirm += new Action<int>(this.SendSelectedNetwork);
    ((BaseWindow) this._window).OnClose += new Action(((BoundUserInterface) this).Close);
  }

  private void SendSelectedNetwork(int idx)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraSetupSetNetwork(idx));
  }

  private void SendDeviceName(string name)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraSetupSetName(name));
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    base.UpdateState(state);
    if (this._window == null || !(state is SurveillanceCameraSetupBoundUiState setupBoundUiState))
      return;
    this._window.UpdateState(setupBoundUiState.Name, setupBoundUiState.NameDisabled, setupBoundUiState.NetworkDisabled);
    this._window.LoadAvailableNetworks(setupBoundUiState.Network, setupBoundUiState.Networks);
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
    this._window = (SurveillanceCameraSetupWindow) null;
  }
}
