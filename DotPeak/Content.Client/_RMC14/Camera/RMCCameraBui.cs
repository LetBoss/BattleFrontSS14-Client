// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Camera.RMCCameraBui
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.UserInterface;
using Content.Client.Eye;
using Content.Client.Message;
using Content.Client.UserInterface.ControlExtensions;
using Content.Shared._RMC14.Camera;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Localization;
using System;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Client._RMC14.Camera;

public sealed class RMCCameraBui : RMCPopOutBui<RMCCameraWindow>
{
  private EntityUid? _currentCamera;
  private Button? _currentCameraButton;
  private readonly EyeLerpingSystem _eyeLerping;
  private readonly RMCCameraSystem _system;

  protected override RMCCameraWindow? Window { get; set; }

  public RMCCameraBui(EntityUid owner, Enum uiKey)
    : base(owner, uiKey)
  {
    this._eyeLerping = this.EntMan.System<EyeLerpingSystem>();
    this._system = this.EntMan.System<RMCCameraSystem>();
  }

  protected virtual void Open()
  {
    base.Open();
    this.Window = this.CreatePopOutableWindow<RMCCameraWindow>();
    this.Window.SearchBar.OnTextChanged += (Action<LineEdit.LineEditEventArgs>) (_ => this.RefreshSearch());
    this.Window.PreviousCameraButton.Text = "<";
    this.Window.NextCameraButton.Text = ">";
    ((BaseButton) this.Window.PreviousCameraButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCCameraPreviousBuiMsg()));
    ((BaseButton) this.Window.NextCameraButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCCameraNextBuiMsg()));
    this.Refresh();
  }

  public void Refresh()
  {
    RMCCameraComputerComponent computerComponent;
    if (this.Window == null || !this.EntMan.TryGetComponent<RMCCameraComputerComponent>(this.Owner, ref computerComponent))
      return;
    LocId? title = computerComponent.Title;
    if (title.HasValue)
      this.Window.Title = Loc.GetString(LocId.op_Implicit(title.GetValueOrDefault()));
    NetEntity? netEntity1 = this.EntMan.GetNetEntity(computerComponent.CurrentCamera, (MetaDataComponent) null);
    Span<NetEntity> span1 = CollectionsMarshal.AsSpan<NetEntity>(computerComponent.CameraIds);
    Span<string> span2 = CollectionsMarshal.AsSpan<string>(computerComponent.CameraNames);
    for (int index = 0; index < span1.Length; ++index)
    {
      if (index < span2.Length)
      {
        RMCCameraButton button;
        if (index < ((Control) this.Window.CamerasContainer).ChildCount)
        {
          if (((Control) this.Window.CamerasContainer).GetChild(index) is RMCCameraButton child)
            button = child;
          else
            continue;
        }
        else
        {
          button = new RMCCameraButton();
          ((Control) this.Window.CamerasContainer).AddChild((Control) button);
        }
        NetEntity id = span1[index];
        string str = span2[index];
        button.TextLabel.SetMarkupPermissive($"[font size=11][color=white]{str}[/color][/font]");
        RMCCameraButton rmcCameraButton = button;
        NetEntity netEntity2 = id;
        NetEntity? nullable = netEntity1;
        int num = nullable.HasValue ? (NetEntity.op_Equality(netEntity2, nullable.GetValueOrDefault()) ? 1 : 0) : 0;
        ((BaseButton) rmcCameraButton).Pressed = num != 0;
        ((BaseButton) button).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
        {
          if (this._currentCameraButton != null)
            ((BaseButton) this._currentCameraButton).Pressed = false;
          this._currentCameraButton = (Button) button;
          this.SendPredictedMessage((BoundUserInterfaceMessage) new RMCCameraWatchBuiMsg(id));
        });
      }
    }
    for (int index = ((Control) this.Window.CamerasContainer).ChildCount - 1; index >= span1.Length; --index)
      ((Control) this.Window.CamerasContainer).RemoveChild(index);
    this.RefreshSearch();
    this.RefreshCamera();
  }

  private void RefreshSearch()
  {
    if (this.Window == null)
      return;
    foreach (Control child in ((Control) this.Window.CamerasContainer).Children)
    {
      if (child is Button parent)
        ((Control) parent).Visible = ((Control) parent).ChildrenContainText(this.Window.SearchBar.Text);
    }
  }

  private void RefreshCamera()
  {
    RMCCameraComputerComponent computerComponent;
    if (this.Window == null || !this.EntMan.TryGetComponent<RMCCameraComputerComponent>(this.Owner, ref computerComponent))
      return;
    EntityUid? currentCamera = this._currentCamera;
    if (currentCamera.HasValue)
      this._eyeLerping.RemoveEye(currentCamera.GetValueOrDefault());
    currentCamera = computerComponent.CurrentCamera;
    if (!currentCamera.HasValue)
      return;
    EntityUid valueOrDefault = currentCamera.GetValueOrDefault();
    this._eyeLerping.AddEye(valueOrDefault);
    this._currentCamera = new EntityUid?(valueOrDefault);
    EyeComponent eyeComponent;
    if (this.EntMan.TryGetComponent<EyeComponent>(valueOrDefault, ref eyeComponent))
      this.Window.Viewport.Eye = (IEye) eyeComponent.Eye;
    string name;
    if (!this._system.GetComputerCameraName(Entity<RMCCameraComputerComponent>.op_Implicit((this.Owner, computerComponent)), valueOrDefault, out name))
      return;
    this.Window.CameraName.Text = name;
  }
}
