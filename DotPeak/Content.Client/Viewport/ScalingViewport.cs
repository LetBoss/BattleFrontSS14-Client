// Decompiled with JetBrains decompiler
// Type: Content.Client.Viewport.ScalingViewport
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Graphics;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Viewport;

public sealed class ScalingViewport : Control, IViewportControl
{
  [Dependency]
  private IClyde _clyde;
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IInputManager _inputManager;
  private IClydeViewport? _viewport;
  private IEye? _eye;
  private Vector2i _viewportSize;
  private int _curRenderScale;
  private ScalingViewportStretchMode _stretchMode;
  private ScalingViewportRenderScaleMode _renderScaleMode;
  private ScalingViewportIgnoreDimension _ignoreDimension;
  private int _fixedRenderScale = 1;
  private readonly List<CopyPixelsDelegate<Rgba32>> _queuedScreenshots = new List<CopyPixelsDelegate<Rgba32>>();

  public int CurrentRenderScale => this._curRenderScale;

  public IEye? Eye
  {
    get => this._eye;
    set
    {
      this._eye = value;
      if (this._viewport == null)
        return;
      this._viewport.Eye = value;
    }
  }

  public Vector2i ViewportSize
  {
    get => this._viewportSize;
    set
    {
      this._viewportSize = value;
      this.InvalidateViewport();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public Vector2i? FixedStretchSize { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public ScalingViewportStretchMode StretchMode
  {
    get => this._stretchMode;
    set
    {
      this._stretchMode = value;
      this.InvalidateViewport();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public ScalingViewportRenderScaleMode RenderScaleMode
  {
    get => this._renderScaleMode;
    set
    {
      this._renderScaleMode = value;
      this.InvalidateViewport();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public int FixedRenderScale
  {
    get => this._fixedRenderScale;
    set
    {
      this._fixedRenderScale = value;
      this.InvalidateViewport();
    }
  }

  [Robust.Shared.ViewVariables.ViewVariables]
  public ScalingViewportIgnoreDimension IgnoreDimension
  {
    get => this._ignoreDimension;
    set
    {
      this._ignoreDimension = value;
      this.InvalidateViewport();
    }
  }

  public ScalingViewport()
  {
    IoCManager.InjectDependencies<ScalingViewport>(this);
    this.RectClipContent = true;
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    if (((BoundKeyEventArgs) args).Handled)
      return;
    this._inputManager.ViewportKeyEvent((Control) this, (BoundKeyEventArgs) args);
  }

  protected virtual void KeyBindUp(GUIBoundKeyEventArgs args)
  {
    base.KeyBindUp(args);
    if (((BoundKeyEventArgs) args).Handled)
      return;
    this._inputManager.ViewportKeyEvent((Control) this, (BoundKeyEventArgs) args);
  }

  protected virtual void Draw(IRenderHandle handle)
  {
    this.EnsureViewportCreated();
    this._viewport.Render();
    if (this._queuedScreenshots.Count != 0)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: method pointer
      ((IRenderTarget) this._viewport.RenderTarget).CopyPixelsToMemory<Rgba32>(new CopyPixelsDelegate<Rgba32>((object) new ScalingViewport.\u003C\u003Ec__DisplayClass39_0()
      {
        callbacks = this._queuedScreenshots.ToArray()
      }, __methodptr(\u003CDraw\u003Eb__0)), new UIBox2i?());
      this._queuedScreenshots.Clear();
    }
    UIBox2i drawBox = this.GetDrawBox();
    UIBox2i uiBox2i = ((UIBox2i) ref drawBox).Translated(this.GlobalPixelPosition);
    this._viewport.RenderScreenOverlaysBelow(handle, (IViewportControl) this, ref uiBox2i);
    handle.DrawingHandleScreen.DrawTextureRect(this._viewport.RenderTarget.Texture, UIBox2i.op_Implicit(drawBox), new Color?());
    this._viewport.RenderScreenOverlaysAbove(handle, (IViewportControl) this, ref uiBox2i);
  }

  public void Screenshot(CopyPixelsDelegate<Rgba32> callback)
  {
    this._queuedScreenshots.Add(callback);
  }

  private UIBox2i GetDrawBox()
  {
    Vector2i size = this._viewport.Size;
    Vector2 vector2_1 = Vector2i.op_Implicit(this.PixelSize);
    if (this.FixedStretchSize.HasValue)
      return UIBox2i.op_Explicit(UIBox2.FromDimensions((vector2_1 - Vector2i.op_Implicit(this.FixedStretchSize.Value)) / 2f, Vector2i.op_Implicit(this.FixedStretchSize.Value)));
    float num1;
    float num2;
    Vector2Helpers.Deconstruct(vector2_1 / Vector2i.op_Implicit(size), ref num1, ref num2);
    float val1 = num1;
    float val2 = num2;
    float num3 = 1f;
    switch (this._ignoreDimension)
    {
      case ScalingViewportIgnoreDimension.None:
        num3 = Math.Min(val1, val2);
        break;
      case ScalingViewportIgnoreDimension.Horizontal:
        num3 = val2;
        break;
      case ScalingViewportIgnoreDimension.Vertical:
        num3 = val1;
        break;
    }
    Vector2 vector2_2 = Vector2i.op_Multiply(size, num3);
    return UIBox2i.op_Explicit(UIBox2.FromDimensions((vector2_1 - vector2_2) / 2f, vector2_2));
  }

  private void RegenerateViewport()
  {
    Vector2i viewportSize = this.ViewportSize;
    float val1;
    float val2_1;
    Vector2Helpers.Deconstruct(Vector2i.op_Implicit(this.PixelSize) / Vector2i.op_Implicit(viewportSize), ref val1, ref val2_1);
    float num1 = Math.Min(val1, val2_1);
    int val2_2 = 1;
    switch (this._renderScaleMode)
    {
      case ScalingViewportRenderScaleMode.Fixed:
        val2_2 = this._fixedRenderScale;
        break;
      case ScalingViewportRenderScaleMode.FloorInt:
        val2_2 = (int) Math.Floor((double) num1);
        break;
      case ScalingViewportRenderScaleMode.CeilInt:
        val2_2 = (int) Math.Ceiling((double) num1);
        break;
    }
    int num2 = Math.Max(1, val2_2);
    this._curRenderScale = num2;
    IClyde clyde = this._clyde;
    Vector2i vector2i = Vector2i.op_Multiply(this.ViewportSize, num2);
    TextureSampleParameters sampleParameters = new TextureSampleParameters();
    ((TextureSampleParameters) ref sampleParameters).Filter = this.StretchMode == ScalingViewportStretchMode.Bilinear;
    TextureSampleParameters? nullable = new TextureSampleParameters?(sampleParameters);
    this._viewport = clyde.CreateViewport(vector2i, nullable, (string) null);
    this._viewport.RenderScale = new Vector2((float) num2, (float) num2);
    this._viewport.Eye = this._eye;
  }

  protected virtual void Resized()
  {
    base.Resized();
    this.InvalidateViewport();
  }

  private void InvalidateViewport()
  {
    ((IDisposable) this._viewport)?.Dispose();
    this._viewport = (IClydeViewport) null;
  }

  public MapCoordinates ScreenToMap(Vector2 coords)
  {
    if (this._eye == null)
      return new MapCoordinates();
    this.EnsureViewportCreated();
    Matrix3x2 result;
    Matrix3x2.Invert(this.GetLocalToScreenMatrix(), out result);
    coords = Vector2.Transform(coords, result);
    return this._viewport.LocalToWorld(coords);
  }

  public MapCoordinates PixelToMap(Vector2 coords)
  {
    if (this._eye == null)
      return new MapCoordinates();
    this.EnsureViewportCreated();
    Matrix3x2 result;
    Matrix3x2.Invert(this.GetLocalToScreenMatrix(), out result);
    coords = Vector2.Transform(coords, result);
    PixelToMapEvent pixelToMapEvent;
    // ISSUE: explicit constructor call
    ((PixelToMapEvent) ref pixelToMapEvent).\u002Ector(coords, (IViewportControl) this, this._viewport);
    ((IBroadcastEventBus) this._entityManager.EventBus).RaiseEvent<PixelToMapEvent>((EventSource) 1, ref pixelToMapEvent);
    return this._viewport.LocalToWorld(pixelToMapEvent.VisiblePosition);
  }

  public Vector2 WorldToScreen(Vector2 map)
  {
    if (this._eye == null)
      return new Vector2();
    this.EnsureViewportCreated();
    return Vector2.Transform(this._viewport.WorldToLocal(map), this.GetLocalToScreenMatrix());
  }

  public Matrix3x2 GetWorldToScreenMatrix()
  {
    this.EnsureViewportCreated();
    return this._viewport.GetWorldToLocalMatrix() * this.GetLocalToScreenMatrix();
  }

  public Matrix3x2 GetLocalToScreenMatrix()
  {
    this.EnsureViewportCreated();
    UIBox2i drawBox = this.GetDrawBox();
    Vector2 vector2_1 = Vector2i.op_Implicit(((UIBox2i) ref drawBox).Size) / Vector2i.op_Implicit(this._viewport.Size);
    if ((double) vector2_1.X == 0.0 || (double) vector2_1.Y == 0.0)
      return Matrix3x2.Identity;
    Vector2 vector2_2 = Vector2i.op_Implicit(Vector2i.op_Addition(this.GlobalPixelPosition, drawBox.TopLeft));
    ref Vector2 local1 = ref vector2_2;
    Angle angle = Angle.op_Implicit(0.0f);
    ref Angle local2 = ref angle;
    ref Vector2 local3 = ref vector2_1;
    return Matrix3Helpers.CreateTransform(ref local1, ref local2, ref local3);
  }

  private void EnsureViewportCreated()
  {
    if (this._viewport != null)
      return;
    this.RegenerateViewport();
  }
}
