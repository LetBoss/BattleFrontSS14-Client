// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.GlobalMap.CivMapGrid
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._CIV14merka.GlobalMap;

public static class CivMapGrid
{
  public const int Divisions = 8;

  public static (int Col, int Row) GetCell(Vector2 boundsMin, Vector2 boundsMax, Vector2 world)
  {
    Vector2 vector2 = boundsMax - boundsMin;
    int num1 = (double) vector2.X > 0.0 ? (int) (((double) world.X - (double) boundsMin.X) / (double) vector2.X * 8.0) : 0;
    int num2 = (double) vector2.Y > 0.0 ? (int) (((double) boundsMax.Y - (double) world.Y) / (double) vector2.Y * 8.0) : 0;
    return (Math.Clamp(num1, 0, 7), Math.Clamp(num2, 0, 7));
  }

  public static string GetCellLabel(Vector2 boundsMin, Vector2 boundsMax, Vector2 world)
  {
    (int num, int Row) = CivMapGrid.GetCell(boundsMin, boundsMax, world);
    return CivMapGrid.GetColumnLabel(num) + (Row + 1).ToString();
  }

  public static string GetColumnLabel(int col) => ((char) (65 + Math.Clamp(col, 0, 25))).ToString();

  public static float LineX(Vector2 boundsMin, Vector2 boundsMax, int i)
  {
    return boundsMin.X + (float) (((double) boundsMax.X - (double) boundsMin.X) * (double) i / 8.0);
  }

  public static float LineY(Vector2 boundsMin, Vector2 boundsMax, int i)
  {
    return boundsMax.Y - (float) (((double) boundsMax.Y - (double) boundsMin.Y) * (double) i / 8.0);
  }

  public static float ColumnCenterX(Vector2 boundsMin, Vector2 boundsMax, int col)
  {
    return boundsMin.X + (float) (((double) boundsMax.X - (double) boundsMin.X) * ((double) col + 0.5) / 8.0);
  }

  public static float RowCenterY(Vector2 boundsMin, Vector2 boundsMax, int row)
  {
    return boundsMax.Y - (float) (((double) boundsMax.Y - (double) boundsMin.Y) * ((double) row + 0.5) / 8.0);
  }
}
