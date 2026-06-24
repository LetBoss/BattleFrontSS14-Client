// Decompiled with JetBrains decompiler
// Type: Content.Client.UserInterface.Controls.TableContainer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Analyzers;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client.UserInterface.Controls;

[Virtual]
public class TableContainer : Container
{
  private int _columns = 1;
  private TableContainer.ColumnData[] _columnDataCache = Array.Empty<TableContainer.ColumnData>();
  private TableContainer.RowData[] _rowDataCache = Array.Empty<TableContainer.RowData>();

  public float MinForcedColumnWidth { get; set; } = 50f;

  public int Columns
  {
    get => this._columns;
    set
    {
      ArgumentOutOfRangeException.ThrowIfLessThan<int>(value, 1, nameof (value));
      this._columns = value;
    }
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize)
  {
    this.ResetCachedArrays();
    int index1 = 0;
    foreach (Control child in ((Control) this).Children)
    {
      ref TableContainer.ColumnData local = ref this._columnDataCache[index1];
      child.Measure(new Vector2(float.PositiveInfinity, float.PositiveInfinity));
      local.MaxWidth = Math.Max(local.MaxWidth, child.DesiredSize.X);
      ++index1;
      if (index1 == this._columns)
        index1 = 0;
    }
    float num1 = 0.0f;
    float val2 = 0.0f;
    float num2 = 0.0f;
    for (int index2 = 0; index2 < this._columns; ++index2)
    {
      ref TableContainer.ColumnData local = ref this._columnDataCache[index2];
      local.MinWidth = Math.Min(local.MaxWidth, this.MinForcedColumnWidth);
      local.Slack = local.MaxWidth - local.MinWidth;
      num1 += local.MinWidth;
      val2 += local.MaxWidth;
      num2 += local.Slack;
    }
    if ((double) val2 <= (double) availableSize.X)
    {
      for (int index3 = 0; index3 < this._columns; ++index3)
      {
        ref TableContainer.ColumnData local = ref this._columnDataCache[index3];
        local.AssignedWidth = local.MaxWidth;
      }
    }
    else
    {
      float num3 = Math.Max(0.0f, availableSize.X - num1);
      for (int index4 = 0; index4 < this._columns; ++index4)
      {
        ref TableContainer.ColumnData local = ref this._columnDataCache[index4];
        local.AssignedWidth = local.MinWidth + local.Slack / num2 * num3;
      }
    }
    int index5 = 0;
    int index6 = 0;
    foreach (Control child in ((Control) this).Children)
    {
      ref TableContainer.ColumnData local1 = ref this._columnDataCache[index5];
      ref TableContainer.RowData local2 = ref this._rowDataCache[index6];
      child.Measure(new Vector2(local1.AssignedWidth, float.PositiveInfinity));
      local2.MeasuredHeight = Math.Max(local2.MeasuredHeight, child.DesiredSize.Y);
      ++index5;
      if (index5 == this._columns)
      {
        index5 = 0;
        ++index6;
      }
    }
    float y = 0.0f;
    for (int index7 = 0; index7 < this._rowDataCache.Length; ++index7)
    {
      ref TableContainer.RowData local = ref this._rowDataCache[index7];
      y += local.MeasuredHeight;
    }
    return new Vector2(Math.Min(availableSize.X, val2), y);
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int index = 0; index < this._columns; ++index)
    {
      ref TableContainer.ColumnData local = ref this._columnDataCache[index];
      num1 += local.MinWidth;
      num2 += local.Slack;
    }
    float num3 = Math.Max(0.0f, finalSize.X - num1);
    float num4 = 0.0f;
    for (int index = 0; index < this._columns; ++index)
    {
      ref TableContainer.ColumnData local = ref this._columnDataCache[index];
      float num5 = local.Slack / num2;
      local.ArrangedWidth = local.MinWidth + num5 * num3;
      local.ArrangedX = num4;
      num4 += local.ArrangedWidth;
    }
    float num6 = 0.0f;
    for (int index1 = 0; index1 < this._rowDataCache.Length; ++index1)
    {
      ref TableContainer.RowData local1 = ref this._rowDataCache[index1];
      for (int index2 = 0; index2 < this._columns; ++index2)
      {
        ref TableContainer.ColumnData local2 = ref this._columnDataCache[index2];
        if (index2 + index1 * this._columns < ((Control) this).ChildCount)
          ((Control) this).GetChild(index2 + index1 * this._columns).Arrange(UIBox2.FromDimensions(local2.ArrangedX, num6, local2.ArrangedWidth, local1.MeasuredHeight));
        else
          break;
      }
      num6 += local1.MeasuredHeight;
    }
    return finalSize with { Y = num6 };
  }

  private void ResetCachedArrays()
  {
    if (this._columnDataCache.Length != this._columns)
      this._columnDataCache = new TableContainer.ColumnData[this._columns];
    Array.Clear((Array) this._columnDataCache, 0, this._columnDataCache.Length);
    int length = ((Control) this).ChildCount / this._columns;
    if (((Control) this).ChildCount % this._columns != 0)
      ++length;
    if (length != this._rowDataCache.Length)
      this._rowDataCache = new TableContainer.RowData[length];
    Array.Clear((Array) this._rowDataCache, 0, this._rowDataCache.Length);
  }

  private struct ColumnData
  {
    public float MaxWidth;
    public float MinWidth;
    public float Slack;
    public float AssignedWidth;
    public float ArrangedWidth;
    public float ArrangedX;
  }

  private struct RowData
  {
    public float MeasuredHeight;
  }
}
