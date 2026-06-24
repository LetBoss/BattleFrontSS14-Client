// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.Teams.CivTeamIconResolver
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Client._CIV14merka.Teams;

public static class CivTeamIconResolver
{
  private const int BlueTeamId = 1;
  private const int RedTeamId = 2;
  private const int MaxSquadBadgeId = 20;
  private static readonly ResPath MapBlipsRsi = new ResPath("/Textures/_RMC14/Interface/map_blips.rsi");
  private static readonly ResPath ClassIconsRsi = new ResPath("/Textures/_RMC14/Interface/cm_job_icons.rsi");
  private static readonly ResPath MarineHudRsi = new ResPath("/Textures/_RMC14/Interface/marine_hud.rsi");
  private static readonly SpriteSpecifier.Rsi ClassBadgeBackground = new SpriteSpecifier.Rsi(CivTeamIconResolver.ClassIconsRsi, "hudsquad");
  private static readonly SpriteSpecifier.Rsi GenericSquadBadge = new SpriteSpecifier.Rsi(CivTeamIconResolver.MarineHudRsi, "hudsquad");
  private static readonly ResPath FlagsRsi = new ResPath("/Textures/_CIV14merka/Interface/team_flags.rsi");
  private static readonly Color DeltaColor = Color.FromHex((ReadOnlySpan<char>) "#4148C8", new Color?());
  private static readonly Color AlphaColor = Color.FromHex((ReadOnlySpan<char>) "#E61919", new Color?());

  public static SpriteSpecifier.Rsi GetClassBadgeBackground()
  {
    return CivTeamIconResolver.ClassBadgeBackground;
  }

  public static SpriteSpecifier.Rsi GetClassBadgeBackground(int teamId)
  {
    return CivTeamIconResolver.ClassBadgeBackground;
  }

  public static Color GetClassBadgeColor(int teamId)
  {
    Color classBadgeColor;
    switch (teamId)
    {
      case 1:
        classBadgeColor = CivTeamIconResolver.DeltaColor;
        break;
      case 2:
        classBadgeColor = CivTeamIconResolver.AlphaColor;
        break;
      default:
        classBadgeColor = Color.White;
        break;
    }
    return classBadgeColor;
  }

  public static SpriteSpecifier.Rsi GetTeamBadge(int teamId)
  {
    string str1;
    switch (teamId)
    {
      case 1:
        str1 = "gm_shield";
        break;
      case 2:
        str1 = "gm_star";
        break;
      default:
        str1 = "gm_star";
        break;
    }
    string str2 = str1;
    return new SpriteSpecifier.Rsi(CivTeamIconResolver.MapBlipsRsi, str2);
  }

  public static SpriteSpecifier.Rsi GetGenericSquadBadge() => CivTeamIconResolver.GenericSquadBadge;

  public static SpriteSpecifier.Rsi GetClassIcon(CivTdmClass roleClass, int teamId)
  {
    string str1;
    switch (roleClass)
    {
      case CivTdmClass.MachineGunner:
        str1 = "hudsquad_gun";
        break;
      case CivTdmClass.Specialist:
        str1 = "hudsquad_spec_sniper";
        break;
      case CivTdmClass.Medic:
        str1 = "hudsquad_med";
        break;
      case CivTdmClass.SquadLeader:
        str1 = "hudsquad_leader";
        break;
      case CivTdmClass.Engineer:
        str1 = "hudsquad_engi";
        break;
      case CivTdmClass.EngineerLeader:
        str1 = "hudsquad_ce";
        break;
      case CivTdmClass.MedicLeader:
        str1 = "hudsquad_cmo";
        break;
      case CivTdmClass.Logist:
        str1 = "hudsquad_ct";
        break;
      case CivTdmClass.Scout:
        str1 = "cmb_obs";
        break;
      default:
        str1 = "hudsquad_rifleman";
        break;
    }
    string str2 = str1;
    return new SpriteSpecifier.Rsi(CivTeamIconResolver.ClassIconsRsi, str2);
  }

  public static SpriteSpecifier.Rsi? GetSquadBadge(int squadId)
  {
    if (squadId <= 0 || squadId > 20)
      return (SpriteSpecifier.Rsi) null;
    return new SpriteSpecifier.Rsi(CivTeamIconResolver.MarineHudRsi, $"hudsquad_ft{squadId}");
  }

  public static SpriteSpecifier.Rsi? GetTeamFlag(int teamId)
  {
    string str1;
    switch (teamId)
    {
      case 1:
        str1 = "flag_usa";
        break;
      case 2:
        str1 = "flag_russia";
        break;
      default:
        str1 = (string) null;
        break;
    }
    string str2 = str1;
    return str2 == null ? (SpriteSpecifier.Rsi) null : new SpriteSpecifier.Rsi(CivTeamIconResolver.FlagsRsi, str2);
  }
}
