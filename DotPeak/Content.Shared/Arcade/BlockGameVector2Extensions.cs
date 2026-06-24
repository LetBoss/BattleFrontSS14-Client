// Decompiled with JetBrains decompiler
// Type: Content.Shared.Arcade.BlockGameVector2Extensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;

#nullable disable
namespace Content.Shared.Arcade;

public static class BlockGameVector2Extensions
{
  public static BlockGameBlock ToBlockGameBlock(
    this Vector2i vector2,
    BlockGameBlock.BlockGameBlockColor gameBlockColor)
  {
    return new BlockGameBlock(vector2, gameBlockColor);
  }

  public static Vector2i AddToX(this Vector2i vector2, int amount)
  {
    return new Vector2i(vector2.X + amount, vector2.Y);
  }

  public static Vector2i AddToY(this Vector2i vector2, int amount)
  {
    return new Vector2i(vector2.X, vector2.Y + amount);
  }

  public static Vector2i Rotate90DegreesAsOffset(this Vector2i vector)
  {
    return new Vector2i(-vector.Y, vector.X);
  }
}
