// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Stats.CivPlayerRoundStats
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Stats;

[NetSerializable]
[Serializable]
public sealed class CivPlayerRoundStats
{
  public NetUserId UserId { get; set; }

  public int Kills { get; set; }

  public int Deaths { get; set; }

  public int TeamKills { get; set; }

  public int DamageDealt { get; set; }

  public int DamageTaken { get; set; }

  public int TeamDamage { get; set; }

  public Dictionary<string, int> WeaponDamage { get; set; } = new Dictionary<string, int>();

  public Dictionary<string, int> WeaponKills { get; set; } = new Dictionary<string, int>();

  public List<CivKillDetail> KillDetails { get; set; } = new List<CivKillDetail>();

  public Dictionary<string, int> ShotsFired { get; set; } = new Dictionary<string, int>();

  public Dictionary<string, int> ShotsHit { get; set; } = new Dictionary<string, int>();

  public Dictionary<string, int> VendorItems { get; set; } = new Dictionary<string, int>();

  public int MinesPlaced { get; set; }

  public int MineKillsEnemies { get; set; }

  public int MineKillsTeammates { get; set; }

  public int MineSelfDetonations { get; set; }

  public int MineKillsConfirmed { get; set; }

  public int MortarShellsFired { get; set; }

  public int MortarHits { get; set; }

  public int MortarKills { get; set; }

  public int ArtilleryKills { get; set; }

  public int AirstrikeKills { get; set; }

  public int HealsApplied { get; set; }

  public int HealingDone { get; set; }

  public int RevivesApplied { get; set; }

  public int PointsCaptured { get; set; }

  public int PointsRecaptured { get; set; }

  public int PointHoldSeconds { get; set; }

  public int PointsDefendedContests { get; set; }

  public int VendorItemsTaken { get; set; }

  public int CommanderPurchasesApproved { get; set; }

  public int CommanderPointsSpent { get; set; }

  public int CommanderShopPurchases { get; set; }

  public int CommanderShopSpent { get; set; }

  public int VehicleTimeSeconds { get; set; }

  public int VehicleKills { get; set; }

  public int VehiclesDestroyed { get; set; }

  public int Roadkills { get; set; }

  public int BestKillstreak { get; set; }

  public int BestMultikill { get; set; }

  public int StructuresBuilt { get; set; }

  public int DistanceWalked { get; set; }

  public int DistanceDriven { get; set; }

  public TimeSpan SurvivalTime { get; set; }

  public int Score { get; set; }

  public List<string> Awards { get; set; } = new List<string>();
}
