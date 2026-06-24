// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Commander.CivAirstrikeConfigPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared._CIV14merka.Commander;

[Prototype(null, 1)]
public sealed class CivAirstrikeConfigPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public int Columns = 2;
  [DataField(null, false, 1, false, false, null)]
  public int Rows = 10;
  [DataField(null, false, 1, false, false, null)]
  public float Spacing = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float DelaySeconds = 2f;
  [DataField(null, false, 1, false, false, null)]
  public int CooldownSeconds = 180;
  [DataField(null, false, 1, false, false, null)]
  public float ExplosionIntensity = 850f;
  [DataField(null, false, 1, false, false, null)]
  public float ExplosionSlope = 8f;
  [DataField(null, false, 1, false, false, null)]
  public float ExplosionMaxTileIntensity = 40f;
  [DataField(null, false, 1, false, false, null)]
  public string ExplosionType = "RMC";
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? WarningSound;
  [DataField(null, false, 1, false, false, null)]
  public string WarningPopup = "INCOMING!";
  [DataField(null, false, 1, false, false, null)]
  public int FlybyCount = 2;
  [DataField(null, false, 1, false, false, null)]
  public float FlybySpacing = 4.5f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybySpeed = 28f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyAlpha = 0.7f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyScaleMin = 0.45f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyScaleMax = 1.4f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyApproachDistance = 60f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyApproachMin = 24f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyApproachFactor = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyTurnMin = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyTurnMax = 28f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyTurnFactor = 0.18f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyExitBias = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyTurnBoost = 1.4f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyTurnLead = 1.75f;
  [DataField(null, false, 1, false, false, null)]
  public float FlybyRunOutFactor = 0.75f;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
