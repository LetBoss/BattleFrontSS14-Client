// Decompiled with JetBrains decompiler
// Type: Content.Client.SurveillanceCamera.UI.SurveillanceCameraMonitorBoundUserInterface
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Eye;
using Content.Shared.SurveillanceCamera;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using System;

#nullable enable
namespace Content.Client.SurveillanceCamera.UI;

public sealed class SurveillanceCameraMonitorBoundUserInterface : BoundUserInterface
{
  private readonly EyeLerpingSystem _eyeLerpingSystem;
  private readonly SurveillanceCameraMonitorSystem _surveillanceCameraMonitorSystem;
  [Robust.Shared.ViewVariables.ViewVariables]
  private SurveillanceCameraMonitorWindow? _window;
  [Robust.Shared.ViewVariables.ViewVariables]
  private EntityUid? _currentCamera;

  public SurveillanceCameraMonitorBoundUserInterface(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._eyeLerpingSystem = this.EntMan.System<EyeLerpingSystem>();
    this._surveillanceCameraMonitorSystem = this.EntMan.System<SurveillanceCameraMonitorSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this._window = BoundUserInterfaceExt.CreateWindow<SurveillanceCameraMonitorWindow>((BoundUserInterface) this);
    this._window.CameraSelected += new Action<string>(this.OnCameraSelected);
    this._window.SubnetOpened += new Action<string>(this.OnSubnetRequest);
    this._window.CameraRefresh += new Action(this.OnCameraRefresh);
    this._window.SubnetRefresh += new Action(this.OnSubnetRefresh);
    this._window.CameraSwitchTimer += new Action(this.OnCameraSwitchTimer);
    this._window.CameraDisconnect += new Action(this.OnCameraDisconnect);
  }

  private void OnCameraSelected(string address)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraMonitorSwitchMessage(address));
  }

  private void OnSubnetRequest(string subnet)
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraMonitorSubnetRequestMessage(subnet));
  }

  private void OnCameraSwitchTimer()
  {
    this._surveillanceCameraMonitorSystem.AddTimer(this.Owner, new Action(this._window.OnSwitchTimerComplete));
  }

  private void OnCameraRefresh()
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraRefreshCamerasMessage());
  }

  private void OnSubnetRefresh()
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraRefreshSubnetsMessage());
  }

  private void OnCameraDisconnect()
  {
    this.SendMessage((BoundUserInterfaceMessage) new SurveillanceCameraDisconnectMessage());
  }

  protected virtual void UpdateState(BoundUserInterfaceState state)
  {
    if (this._window == null || !(state is SurveillanceCameraMonitorUiState cameraMonitorUiState))
      return;
    EntityUid? entity = this.EntMan.GetEntity(cameraMonitorUiState.ActiveCamera);
    if (!entity.HasValue)
    {
      this._window.UpdateState((IEye) null, cameraMonitorUiState.Subnets, cameraMonitorUiState.ActiveAddress, cameraMonitorUiState.ActiveSubnet, cameraMonitorUiState.Cameras);
      if (!this._currentCamera.HasValue)
        return;
      this._surveillanceCameraMonitorSystem.RemoveTimer(this.Owner);
      this._eyeLerpingSystem.RemoveEye(this._currentCamera.Value);
      this._currentCamera = new EntityUid?();
    }
    else
    {
      if (!this._currentCamera.HasValue)
      {
        this._eyeLerpingSystem.AddEye(entity.Value);
        this._currentCamera = entity;
      }
      else
      {
        EntityUid? currentCamera = this._currentCamera;
        EntityUid? nullable = entity;
        if ((currentCamera.HasValue == nullable.HasValue ? (currentCamera.HasValue ? (EntityUid.op_Inequality(currentCamera.GetValueOrDefault(), nullable.GetValueOrDefault()) ? 1 : 0) : 0) : 1) != 0)
        {
          this._eyeLerpingSystem.RemoveEye(this._currentCamera.Value);
          this._eyeLerpingSystem.AddEye(entity.Value);
          this._currentCamera = entity;
        }
      }
      EyeComponent eyeComponent;
      if (!this.EntMan.TryGetComponent<EyeComponent>(entity, ref eyeComponent))
        return;
      this._window.UpdateState((IEye) eyeComponent.Eye, cameraMonitorUiState.Subnets, cameraMonitorUiState.ActiveAddress, cameraMonitorUiState.ActiveSubnet, cameraMonitorUiState.Cameras);
    }
  }

  protected virtual void Dispose(bool disposing)
  {
    base.Dispose(disposing);
    if (this._currentCamera.HasValue)
    {
      this._eyeLerpingSystem.RemoveEye(this._currentCamera.Value);
      this._currentCamera = new EntityUid?();
    }
    if (!disposing)
      return;
    ((Control) this._window)?.Orphan();
  }
}
