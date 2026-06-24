// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Systems.Chat.Widgets.ResizableChatBox
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Systems.Chat.Widgets;

public sealed class ResizableChatBox : ChatBox
{
  [Dependency]
  private IClyde _clyde;
  private const int DragMarginSize = 7;
  private const int MinDistanceFromBottom = 255 /*0xFF*/;
  private const int MinLeft = 500;
  private ResizableChatBox.DragMode _currentDrag;
  private Vector2 _dragOffsetTopLeft;
  private Vector2 _dragOffsetBottomRight;
  private byte _clampIn;
  public Action<Vector2>? OnChatResizeFinish;

  public ResizableChatBox() => IoCManager.InjectDependencies<ResizableChatBox>(this);

  protected virtual void EnteredTree()
  {
    ((Control) this).EnteredTree();
    this._clyde.OnWindowResized += new Action<WindowResizedEventArgs>(this.ClydeOnOnWindowResized);
  }

  protected virtual void ExitedTree()
  {
    ((Control) this).ExitedTree();
    this._clyde.OnWindowResized -= new Action<WindowResizedEventArgs>(this.ClydeOnOnWindowResized);
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      this._currentDrag = this.GetDragModeFor(args.RelativePosition);
      if (this._currentDrag != ResizableChatBox.DragMode.None)
      {
        this._dragOffsetTopLeft = ((BoundKeyEventArgs) args).PointerLocation.Position / ((Control) this).UIScale - ((Control) this).Position;
        this._dragOffsetBottomRight = ((Control) this).Position + ((Control) this).Size - ((BoundKeyEventArgs) args).PointerLocation.Position / ((Control) this).UIScale;
      }
    }
    ((Control) this).KeyBindDown(args);
  }

  protected virtual void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
      return;
    if (this._currentDrag != ResizableChatBox.DragMode.None)
    {
      this._dragOffsetTopLeft = this._dragOffsetBottomRight = Vector2.Zero;
      this._currentDrag = ResizableChatBox.DragMode.None;
      ((Control) this).UserInterfaceManager.KeyboardFocused?.ReleaseKeyboardFocus();
      Action<Vector2> chatResizeFinish = this.OnChatResizeFinish;
      if (chatResizeFinish != null)
        chatResizeFinish(((Control) this).Size);
    }
    ((Control) this).KeyBindUp(args);
  }

  private ResizableChatBox.DragMode GetDragModeFor(Vector2 relativeMousePos)
  {
    ResizableChatBox.DragMode dragModeFor = ResizableChatBox.DragMode.None;
    if ((double) relativeMousePos.Y > (double) ((Control) this).Size.Y - 7.0)
      dragModeFor = ResizableChatBox.DragMode.Bottom;
    if ((double) relativeMousePos.X < 7.0)
      dragModeFor |= ResizableChatBox.DragMode.Left;
    return dragModeFor;
  }

  protected virtual void MouseMove(GUIMouseMoveEventArgs args)
  {
    ((Control) this).MouseMove(args);
    if (((Control) this).Parent == null)
      return;
    if (this._currentDrag == ResizableChatBox.DragMode.None)
    {
      Control.CursorShape cursorShape = (Control.CursorShape) 0;
      switch (this.GetDragModeFor(((GUIMouseEventArgs) args).RelativePosition))
      {
        case ResizableChatBox.DragMode.Bottom:
          cursorShape = (Control.CursorShape) 5;
          break;
        case ResizableChatBox.DragMode.Left:
          cursorShape = (Control.CursorShape) 4;
          break;
        case ResizableChatBox.DragMode.Bottom | ResizableChatBox.DragMode.Left:
          cursorShape = (Control.CursorShape) 2;
          break;
      }
      ((Control) this).DefaultCursorShape = cursorShape;
    }
    else
    {
      float top = ((Control) this).Rect.Top;
      float num1 = ((Control) this).Rect.Bottom;
      float num2 = ((Control) this).Rect.Left;
      float right = ((Control) this).Rect.Right;
      float num3;
      float num4;
      Vector2Helpers.Deconstruct(((Control) this).MinSize, ref num3, ref num4);
      float num5 = num3;
      float num6 = num4;
      if ((this._currentDrag & ResizableChatBox.DragMode.Bottom) == ResizableChatBox.DragMode.Bottom)
        num1 = Math.Max(((GUIMouseEventArgs) args).GlobalPosition.Y + this._dragOffsetBottomRight.Y, top + num6);
      if ((this._currentDrag & ResizableChatBox.DragMode.Left) == ResizableChatBox.DragMode.Left)
      {
        float val2 = right - num5;
        num2 = Math.Min(((GUIMouseEventArgs) args).GlobalPosition.X - this._dragOffsetTopLeft.X, val2);
      }
      this.ClampSize(new float?(num2), new float?(num1));
    }
  }

  protected virtual void UIScaleChanged()
  {
    ((Control) this).UIScaleChanged();
    this.ClampAfterDelay();
  }

  private void ClydeOnOnWindowResized(WindowResizedEventArgs obj) => this.ClampAfterDelay();

  private void ClampAfterDelay() => this._clampIn = (byte) 2;

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    ((Control) this).FrameUpdate(args);
    if (this._clampIn <= (byte) 0)
      return;
    --this._clampIn;
    if (this._clampIn != (byte) 0)
      return;
    this.ClampSize();
  }

  private void ClampSize(float? desiredLeft = null, float? desiredBottom = null)
  {
    if (((Control) this).Parent == null)
      return;
    float right = ((Control) this).Rect.Right;
    float? nullable = desiredLeft;
    float num1 = (float) ((double) nullable ?? (double) ((Control) this).Rect.Left);
    nullable = desiredBottom;
    float num2 = (float) ((double) nullable ?? (double) ((Control) this).Rect.Bottom);
    float max1 = ((Control) this).Parent.Size.Y - (float) byte.MaxValue;
    float num3 = (double) max1 > (double) ((Control) this).MinHeight ? Math.Clamp(num2, ((Control) this).MinHeight, max1) : ((Control) this).MinHeight;
    float max2 = ((Control) this).Parent.Size.X - ((Control) this).MinWidth;
    float num4 = (double) max2 > 500.0 ? Math.Clamp(num1, 500f, max2) : max2;
    LayoutContainer.SetMarginLeft((Control) this, (float) -((double) right + 10.0 - (double) num4));
    LayoutContainer.SetMarginBottom((Control) this, num3);
  }

  protected virtual void MouseExited()
  {
    ((Control) this).MouseExited();
    if (this._currentDrag != ResizableChatBox.DragMode.None)
      return;
    ((Control) this).DefaultCursorShape = (Control.CursorShape) 0;
  }

  [Flags]
  private enum DragMode : byte
  {
    None = 0,
    Bottom = 2,
    Left = 4,
  }
}
