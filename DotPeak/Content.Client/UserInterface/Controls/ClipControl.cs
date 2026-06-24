// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.ClipControl
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using System.Numerics;

#nullable disable
namespace Content.Client.UserInterface.Controls;

public sealed class ClipControl : Control
{
  private bool _clipHorizontal = true;
  private bool _clipVertical = true;

  public bool ClipHorizontal
  {
    get => this._clipHorizontal;
    set
    {
      this._clipHorizontal = value;
      this.InvalidateMeasure();
    }
  }

  public bool ClipVertical
  {
    get => this._clipVertical;
    set
    {
      this._clipVertical = value;
      this.InvalidateMeasure();
    }
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize)
  {
    Vector2 vector2;
    if (this.ClipHorizontal)
    {
      vector2 = availableSize with
      {
        X = float.PositiveInfinity
      };
      availableSize = vector2;
    }
    if (this.ClipVertical)
    {
      vector2 = availableSize with
      {
        Y = float.PositiveInfinity
      };
      availableSize = vector2;
    }
    return base.MeasureOverride(availableSize);
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    foreach (Control child in this.Children)
      child.Arrange(UIBox2.FromDimensions(Vector2.Zero, child.DesiredSize));
    return finalSize;
  }
}
