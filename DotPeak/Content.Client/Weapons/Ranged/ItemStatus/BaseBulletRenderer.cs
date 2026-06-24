// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.ItemStatus.BaseBulletRenderer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Ranged.ItemStatus;

public abstract class BaseBulletRenderer : Control
{
  private int _capacity;
  private BaseBulletRenderer.LayoutParameters _params;

  public int Rows { get; set; } = 2;

  public int Count { get; set; }

  public int Capacity
  {
    get => this._capacity;
    set
    {
      if (this._capacity == value)
        return;
      this._capacity = value;
      this.InvalidateMeasure();
    }
  }

  protected BaseBulletRenderer.LayoutParameters Parameters
  {
    get => this._params;
    set
    {
      this._params = value;
      this.InvalidateMeasure();
    }
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize)
  {
    int count = Math.Min(this.Capacity, this.CountPerRow(availableSize.X));
    int num = Math.Min((int) MathF.Ceiling((float) this.Capacity / (float) count), this.Rows);
    int y = this._params.ItemHeight * num + (this._params.VerticalSeparation * num - 1);
    return new Vector2((float) this.RowWidth(count), (float) y);
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    Matrix3x2 transform = ((DrawingHandleBase) handle).GetTransform();
    DrawingHandleScreen drawingHandleScreen = handle;
    Vector2 vector2_1 = new Vector2(this.UIScale);
    Matrix3x2 matrix3x2 = Matrix3Helpers.CreateScale(ref vector2_1) * transform;
    ref Matrix3x2 local = ref matrix3x2;
    ((DrawingHandleBase) drawingHandleScreen).SetTransform(ref local);
    int val1 = this.CountPerRow(this.Size.X);
    Vector2 vector2_2 = new Vector2();
    int num1 = this.Capacity - this.Count;
    int num2 = 0;
    for (int index1 = 0; index1 < this.Rows; ++index1)
    {
      bool altColor = false;
      int count = Math.Min(val1, this.Capacity - num2);
      if (count <= 0)
        break;
      int num3 = this.Capacity - num2 - count;
      if (num3 < this._params.MinCountPerRow && index1 != this.Rows - 1 && this._params.MinCountPerRow < val1 && num3 > 0)
        count -= this._params.MinCountPerRow - num3;
      int num4 = this.RowWidth(count);
      vector2_2.X += this.Size.X - (float) num4;
      for (int index2 = 0; index2 < count; ++index2)
      {
        int num5 = this.Capacity - num2 - count + index2;
        Vector2 renderPos = vector2_2;
        renderPos.Y = this.Size.Y - renderPos.Y - (float) this._params.ItemHeight;
        this.DrawItem(handle, renderPos, num5 < num1, altColor);
        vector2_2.X += (float) this._params.ItemSeparation;
        altColor = !altColor;
      }
      num2 += count;
      vector2_2.X = 0.0f;
      vector2_2.Y += (float) (this._params.ItemHeight + this._params.VerticalSeparation);
    }
  }

  protected abstract void DrawItem(
    DrawingHandleScreen handle,
    Vector2 renderPos,
    bool spent,
    bool altColor);

  private int CountPerRow(float width)
  {
    return (int) (((double) width - (double) this._params.ItemWidth + (double) this._params.ItemSeparation) / (double) this._params.ItemSeparation);
  }

  private int RowWidth(int count)
  {
    return (count - 1) * this._params.ItemSeparation + this._params.ItemWidth;
  }

  protected struct LayoutParameters
  {
    public int ItemHeight;
    public int ItemSeparation;
    public int ItemWidth;
    public int VerticalSeparation;
    public int MinCountPerRow;
  }
}
