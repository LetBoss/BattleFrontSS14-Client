// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.CivTdmClassHelper
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Teams;
using Robust.Shared.Localization;

#nullable enable
namespace Content.Shared._CIV14merka;

public static class CivTdmClassHelper
{
  public static string GetDisplayName(CivTdmClass classType)
  {
    string displayName;
    switch (classType)
    {
      case CivTdmClass.Rifleman:
        displayName = Loc.GetString("civ-eq-class-rifleman");
        break;
      case CivTdmClass.MachineGunner:
        displayName = Loc.GetString("civ-eq-class-machine-gunner");
        break;
      case CivTdmClass.Specialist:
        displayName = Loc.GetString("civ-eq-class-specialist");
        break;
      case CivTdmClass.Medic:
        displayName = Loc.GetString("civ-eq-class-medic");
        break;
      case CivTdmClass.SquadLeader:
        displayName = Loc.GetString("civ-eq-class-squad-leader");
        break;
      case CivTdmClass.Engineer:
        displayName = Loc.GetString("civ-eq-class-engineer");
        break;
      case CivTdmClass.EngineerLeader:
        displayName = Loc.GetString("civ-eq-class-engineer-leader");
        break;
      case CivTdmClass.MedicLeader:
        displayName = Loc.GetString("civ-eq-class-medic-leader");
        break;
      case CivTdmClass.Logist:
        displayName = Loc.GetString("civ-eq-class-logist");
        break;
      case CivTdmClass.Scout:
        displayName = Loc.GetString("civ-eq-class-scout");
        break;
      default:
        displayName = Loc.GetString("civ-eq-class-unknown");
        break;
    }
    return displayName;
  }

  public static string GetShortTag(CivTdmClass classType)
  {
    string shortTag;
    switch (classType)
    {
      case CivTdmClass.Rifleman:
        shortTag = "R";
        break;
      case CivTdmClass.MachineGunner:
        shortTag = "MG";
        break;
      case CivTdmClass.Specialist:
        shortTag = "SP";
        break;
      case CivTdmClass.Medic:
        shortTag = "M";
        break;
      case CivTdmClass.SquadLeader:
        shortTag = "SL";
        break;
      case CivTdmClass.Engineer:
        shortTag = "ENG";
        break;
      case CivTdmClass.EngineerLeader:
        shortTag = "ENL";
        break;
      case CivTdmClass.MedicLeader:
        shortTag = "ML";
        break;
      case CivTdmClass.Logist:
        shortTag = "LOG";
        break;
      case CivTdmClass.Scout:
        shortTag = "SC";
        break;
      default:
        shortTag = "?";
        break;
    }
    return shortTag;
  }
}
