// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Stats.CivStatsScoringPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared._CIV14merka.Stats;

[Prototype(null, 1)]
public sealed class CivStatsScoringPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public int Kill = 100;
  [DataField(null, false, 1, false, false, null)]
  public int TeamKill = -500;
  [DataField(null, false, 1, false, false, null)]
  public int Death = -25;
  [DataField(null, false, 1, false, false, null)]
  public int DamageDealtPer100 = 20;
  [DataField(null, false, 1, false, false, null)]
  public int MineKillEnemy = 150;
  [DataField(null, false, 1, false, false, null)]
  public int MineKillTeammate = -400;
  [DataField(null, false, 1, false, false, null)]
  public int MineSelfDetonation = -200;
  [DataField(null, false, 1, false, false, null)]
  public int MortarHit = 120;
  [DataField(null, false, 1, false, false, null)]
  public int HealApplied = 10;
  [DataField(null, false, 1, false, false, null)]
  public int HealPer10Hp = 8;
  [DataField(null, false, 1, false, false, null)]
  public int PointCaptured = 250;
  [DataField(null, false, 1, false, false, null)]
  public int PointRecaptured = 100;
  [DataField(null, false, 1, false, false, null)]
  public int PointHoldPer30Sec = 50;
  [DataField(null, false, 1, false, false, null)]
  public int PointDefendedContest = 75;
  [DataField(null, false, 1, false, false, null)]
  public int VendorItem;
  [DataField(null, false, 1, false, false, null)]
  public int CommanderPurchaseApproved = 30;
  [DataField(null, false, 1, false, false, null)]
  public int SurvivalPerMinute = 10;
  [DataField(null, false, 1, false, false, null)]
  public int BestKillerMinKills = 1;
  [DataField(null, false, 1, false, false, null)]
  public int MostDeathsMinDeaths = 1;
  [DataField(null, false, 1, false, false, null)]
  public int BestMinerMinEnemyKills = 2;
  [DataField(null, false, 1, false, false, null)]
  public int BestMedicMinHeals = 5;
  [DataField(null, false, 1, false, false, null)]
  public int BestMortarMinHits = 3;
  [DataField(null, false, 1, false, false, null)]
  public int BestCapperMinPoints = 1;
  [DataField(null, false, 1, false, false, null)]
  public int BestDefenderMinHoldSeconds = 60;
  [DataField(null, false, 1, false, false, null)]
  public int BestBuilderMinStructures = 3;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
