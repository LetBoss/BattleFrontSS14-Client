// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.Systems.Minimap.PubgPartyPingColorResolver
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.Systems.Minimap;

public static class PubgPartyPingColorResolver
{
  private static readonly Color[] SourcePalette = new Color[6]
  {
    Color.FromHex((ReadOnlySpan<char>) "#4FC3F7", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#66BB6A", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#FFB74D", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#F06292", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#26A69A", new Color?()),
    Color.FromHex((ReadOnlySpan<char>) "#9575CD", new Color?())
  };

  public static Color GetColor(NetEntity source)
  {
    int num = source.Id;
    switch (num)
    {
      case int.MinValue:
        num = int.MaxValue;
        break;
      case 0:
        return PubgPartyPingColorResolver.SourcePalette[0];
    }
    int index = Math.Abs(num) % PubgPartyPingColorResolver.SourcePalette.Length;
    return PubgPartyPingColorResolver.SourcePalette[index];
  }
}
