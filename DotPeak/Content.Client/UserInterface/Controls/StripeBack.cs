// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.StripeBack
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

public sealed class StripeBack : Container
{
  private const float PadSize = 4f;
  private const float EdgeSize = 2f;
  private static readonly Color EdgeColor = Color.FromHex((ReadOnlySpan<char>) "#525252ff", new Color?());
  private bool _hasTopEdge = true;
  private bool _hasBottomEdge = true;
  private bool _hasMargins = true;
  public const string StylePropertyBackground = "background";

  public bool HasTopEdge
  {
    get => this._hasTopEdge;
    set
    {
      this._hasTopEdge = value;
      ((Control) this).InvalidateMeasure();
    }
  }

  public bool HasBottomEdge
  {
    get => this._hasBottomEdge;
    set
    {
      this._hasBottomEdge = value;
      ((Control) this).InvalidateMeasure();
    }
  }

  public bool HasMargins
  {
    get => this._hasMargins;
    set
    {
      this._hasMargins = value;
      ((Control) this).InvalidateMeasure();
    }
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize)
  {
    float num = this.HasMargins ? 4f : 0.0f;
    float y = 0.0f;
    if (this.HasBottomEdge)
      y += num + 2f;
    if (this.HasTopEdge)
      y += num + 2f;
    Vector2 vector2 = Vector2.Zero;
    availableSize.Y -= y;
    foreach (Control child in ((Control) this).Children)
    {
      child.Measure(availableSize);
      vector2 = Vector2.Max(vector2, child.DesiredSize);
    }
    return vector2 + new Vector2(0.0f, y);
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    UIBox2 uiBox2;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2).\u002Ector(Vector2.Zero, finalSize);
    float num = this.HasMargins ? 4f : 0.0f;
    if (this.HasTopEdge)
      uiBox2 = UIBox2.op_Addition(uiBox2, (0.0f, num + 2f, 0.0f, 0.0f));
    if (this.HasBottomEdge)
      uiBox2 = UIBox2.op_Addition(uiBox2, (0.0f, 0.0f, 0.0f, (float) -((double) num + 2.0)));
    foreach (Control child in ((Control) this).Children)
      child.Arrange(uiBox2);
    return finalSize;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2 = UIBox2i.op_Implicit(((Control) this).PixelSizeBox);
    float num = this.HasMargins ? 4f : 0.0f;
    if (this.HasTopEdge)
    {
      uiBox2 = UIBox2.op_Addition(uiBox2, (0.0f, (num + 2f) * ((Control) this).UIScale, 0.0f, 0.0f));
      handle.DrawRect(new UIBox2(0.0f, num * ((Control) this).UIScale, (float) ((Control) this).PixelWidth, uiBox2.Top), StripeBack.EdgeColor, true);
    }
    if (this.HasBottomEdge)
    {
      uiBox2 = UIBox2.op_Addition(uiBox2, (0.0f, 0.0f, 0.0f, (float) -(((double) num + 2.0) * (double) ((Control) this).UIScale)));
      handle.DrawRect(new UIBox2(0.0f, uiBox2.Bottom, (float) ((Control) this).PixelWidth, (float) ((Control) this).PixelHeight - num * ((Control) this).UIScale), StripeBack.EdgeColor, true);
    }
    this.GetActualStyleBox()?.Draw(handle, uiBox2, ((Control) this).UIScale);
  }

  private StyleBox? GetActualStyleBox()
  {
    StyleBox styleBox;
    return !((Control) this).TryGetStyleProperty<StyleBox>("background", ref styleBox) ? (StyleBox) null : styleBox;
  }
}
