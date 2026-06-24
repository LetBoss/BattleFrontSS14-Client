// Decompiled with JetBrains decompiler
// Type: Content.Shared.Arcade.BlockGameBlock
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable disable
namespace Content.Shared.Arcade;

[NetSerializable]
[Serializable]
public struct BlockGameBlock(Vector2i position, BlockGameBlock.BlockGameBlockColor gameBlockColor)
{
  public Vector2i Position = position;
  public readonly BlockGameBlock.BlockGameBlockColor GameBlockColor = gameBlockColor;

  public static BlockGameBlock.BlockGameBlockColor ToGhostBlockColor(
    BlockGameBlock.BlockGameBlockColor inColor)
  {
    BlockGameBlock.BlockGameBlockColor ghostBlockColor;
    switch (inColor)
    {
      case BlockGameBlock.BlockGameBlockColor.Red:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostRed;
        break;
      case BlockGameBlock.BlockGameBlockColor.Orange:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostOrange;
        break;
      case BlockGameBlock.BlockGameBlockColor.Yellow:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostYellow;
        break;
      case BlockGameBlock.BlockGameBlockColor.Green:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostGreen;
        break;
      case BlockGameBlock.BlockGameBlockColor.Blue:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostBlue;
        break;
      case BlockGameBlock.BlockGameBlockColor.LightBlue:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostLightBlue;
        break;
      case BlockGameBlock.BlockGameBlockColor.Purple:
        ghostBlockColor = BlockGameBlock.BlockGameBlockColor.GhostPurple;
        break;
      default:
        ghostBlockColor = inColor;
        break;
    }
    return ghostBlockColor;
  }

  public static Color ToColor(BlockGameBlock.BlockGameBlockColor inColor)
  {
    Color color;
    switch (inColor)
    {
      case BlockGameBlock.BlockGameBlockColor.Red:
        color = Color.Red;
        break;
      case BlockGameBlock.BlockGameBlockColor.Orange:
        color = Color.Orange;
        break;
      case BlockGameBlock.BlockGameBlockColor.Yellow:
        color = Color.Yellow;
        break;
      case BlockGameBlock.BlockGameBlockColor.Green:
        color = Color.Lime;
        break;
      case BlockGameBlock.BlockGameBlockColor.Blue:
        color = Color.Blue;
        break;
      case BlockGameBlock.BlockGameBlockColor.LightBlue:
        color = Color.Cyan;
        break;
      case BlockGameBlock.BlockGameBlockColor.Purple:
        color = Color.DarkOrchid;
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostRed:
        Color red = Color.Red;
        color = ((Color) ref red).WithAlpha(0.33f);
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostOrange:
        Color orange = Color.Orange;
        color = ((Color) ref orange).WithAlpha(0.33f);
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostYellow:
        Color yellow = Color.Yellow;
        color = ((Color) ref yellow).WithAlpha(0.33f);
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostGreen:
        Color lime = Color.Lime;
        color = ((Color) ref lime).WithAlpha(0.33f);
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostBlue:
        Color blue = Color.Blue;
        color = ((Color) ref blue).WithAlpha(0.33f);
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostLightBlue:
        Color cyan = Color.Cyan;
        color = ((Color) ref cyan).WithAlpha(0.33f);
        break;
      case BlockGameBlock.BlockGameBlockColor.GhostPurple:
        Color darkOrchid = Color.DarkOrchid;
        color = ((Color) ref darkOrchid).WithAlpha(0.33f);
        break;
      default:
        color = Color.Olive;
        break;
    }
    return color;
  }

  [NetSerializable]
  [Serializable]
  public enum BlockGameBlockColor
  {
    Red,
    Orange,
    Yellow,
    Green,
    Blue,
    LightBlue,
    Purple,
    GhostRed,
    GhostOrange,
    GhostYellow,
    GhostGreen,
    GhostBlue,
    GhostLightBlue,
    GhostPurple,
  }
}
