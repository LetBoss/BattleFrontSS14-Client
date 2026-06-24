// Decompiled with JetBrains decompiler
// Type: Content.Client.Interaction.DragDropHelper`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.CCVar;
using Robust.Client.Input;
using Robust.Shared.Configuration;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using System;

#nullable enable
namespace Content.Client.Interaction;

public sealed class DragDropHelper<T>
{
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IConfigurationManager _cfg;
  private readonly OnBeginDrag _onBeginDrag;
  private readonly OnEndDrag _onEndDrag;
  private readonly OnContinueDrag _onContinueDrag;
  private float _deadzone;
  private ScreenCoordinates _mouseDownScreenPos;
  private DragDropHelper<
  #nullable disable
  T>.DragState _state;

  public ScreenCoordinates MouseScreenPosition => this._inputManager.MouseScreenPosition;

  public bool IsDragging => this._state == DragDropHelper<T>.DragState.Dragging;

  public 
  #nullable enable
  T? Dragged { get; private set; }

  public DragDropHelper(
    OnBeginDrag onBeginDrag,
    OnContinueDrag onContinueDrag,
    OnEndDrag onEndDrag)
  {
    IoCManager.InjectDependencies<DragDropHelper<T>>(this);
    this._onBeginDrag = onBeginDrag;
    this._onEndDrag = onEndDrag;
    this._onContinueDrag = onContinueDrag;
    this._cfg.OnValueChanged<float>(CCVars.DragDropDeadZone, new Action<float>(this.SetDeadZone), true);
  }

  public void MouseDown(T target)
  {
    if (this._state != DragDropHelper<T>.DragState.NotDragging)
      this.EndDrag();
    this.Dragged = target;
    this._state = DragDropHelper<T>.DragState.MouseDown;
    this._mouseDownScreenPos = this._inputManager.MouseScreenPosition;
  }

  public void EndDrag()
  {
    this.Dragged = default (T);
    this._state = DragDropHelper<T>.DragState.NotDragging;
    this._onEndDrag();
  }

  private void StartDragging()
  {
    if (this._onBeginDrag())
      this._state = DragDropHelper<T>.DragState.Dragging;
    else
      this.EndDrag();
  }

  public void Update(float frameTime)
  {
    switch (this._state)
    {
      case DragDropHelper<T>.DragState.MouseDown:
        if ((double) (this._mouseDownScreenPos.Position - this._inputManager.MouseScreenPosition.Position).Length() <= (double) this._deadzone)
          break;
        this.StartDragging();
        break;
      case DragDropHelper<T>.DragState.Dragging:
        if (this._onContinueDrag(frameTime))
          break;
        this.EndDrag();
        break;
    }
  }

  private void SetDeadZone(float value) => this._deadzone = value;

  private enum DragState : byte
  {
    NotDragging,
    MouseDown,
    Dragging,
  }
}
