// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingColoring
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

public static class MarkingColoring
{
  public static List<Color> GetMarkingLayerColors(
    MarkingPrototype prototype,
    Color? skinColor,
    Color? eyeColor,
    MarkingSet markingSet)
  {
    List<Color> markingLayerColors = new List<Color>();
    if (!prototype.RMCFollowSkinColor)
      return markingLayerColors;
    Color color1 = prototype.Coloring.Default.GetColor(skinColor, eyeColor, markingSet);
    if (prototype.Coloring.Layers == null)
    {
      for (int index = 0; index < prototype.Sprites.Count; ++index)
        markingLayerColors.Add(color1);
      return markingLayerColors;
    }
    for (int index = 0; index < prototype.Sprites.Count; ++index)
    {
      string str;
      switch (prototype.Sprites[index])
      {
        case SpriteSpecifier.Rsi rsi:
          str = rsi.RsiState;
          break;
        case SpriteSpecifier.Texture texture:
          str = texture.TexturePath.Filename;
          break;
        default:
          str = (string) null;
          break;
      }
      string key = str;
      if (key == null)
      {
        markingLayerColors.Add(color1);
      }
      else
      {
        LayerColoringDefinition coloringDefinition;
        if (prototype.Coloring.Layers.TryGetValue(key, out coloringDefinition))
        {
          Color color2 = coloringDefinition.GetColor(skinColor, eyeColor, markingSet);
          markingLayerColors.Add(color2);
        }
        else
          markingLayerColors.Add(color1);
      }
    }
    return markingLayerColors;
  }
}
