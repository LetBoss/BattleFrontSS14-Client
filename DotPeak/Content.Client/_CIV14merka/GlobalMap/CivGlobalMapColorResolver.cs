// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.GlobalMap.CivGlobalMapColorResolver
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.GlobalMap;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._CIV14merka.GlobalMap;

public static class CivGlobalMapColorResolver
{
  public static readonly Color SquadColor = Color.FromHex((ReadOnlySpan<char>) "#6de685", new Color?());
  public static readonly Color TeamColor = Color.FromHex((ReadOnlySpan<char>) "#4da6ff", new Color?());
  public static readonly Color EnemyTeamColor = Color.FromHex((ReadOnlySpan<char>) "#d85d5d", new Color?());

  public static Color GetColor(CivGlobalMapMarkerType type)
  {
    Color color;
    switch (type)
    {
      case CivGlobalMapMarkerType.Attack:
        color = Color.FromHex((ReadOnlySpan<char>) "#ff5449", new Color?());
        break;
      case CivGlobalMapMarkerType.Defense:
        color = Color.FromHex((ReadOnlySpan<char>) "#5ca8ff", new Color?());
        break;
      case CivGlobalMapMarkerType.Enemy:
        color = Color.FromHex((ReadOnlySpan<char>) "#ff6d3f", new Color?());
        break;
      case CivGlobalMapMarkerType.Help:
        color = Color.FromHex((ReadOnlySpan<char>) "#ffd85a", new Color?());
        break;
      case CivGlobalMapMarkerType.Allies:
        color = Color.FromHex((ReadOnlySpan<char>) "#6de685", new Color?());
        break;
      default:
        color = Color.White;
        break;
    }
    return color;
  }

  public static Color GetPlayerColor(
    int viewerTeamId,
    int viewerSquadId,
    int playerTeamId,
    int playerSquadId)
  {
    if (viewerTeamId == 0)
      return playerTeamId != 1 ? CivGlobalMapColorResolver.EnemyTeamColor : CivGlobalMapColorResolver.TeamColor;
    if (playerTeamId != viewerTeamId)
      return CivGlobalMapColorResolver.EnemyTeamColor;
    return viewerSquadId != 0 && playerSquadId == viewerSquadId ? CivGlobalMapColorResolver.SquadColor : CivGlobalMapColorResolver.TeamColor;
  }

  public static string GetShortText(CivGlobalMapMarkerType type)
  {
    string shortText;
    switch (type)
    {
      case CivGlobalMapMarkerType.Attack:
        shortText = "A";
        break;
      case CivGlobalMapMarkerType.Defense:
        shortText = "D";
        break;
      case CivGlobalMapMarkerType.Enemy:
        shortText = "E";
        break;
      case CivGlobalMapMarkerType.Help:
        shortText = "H";
        break;
      case CivGlobalMapMarkerType.Allies:
        shortText = "F";
        break;
      default:
        shortText = "?";
        break;
    }
    return shortText;
  }
}
