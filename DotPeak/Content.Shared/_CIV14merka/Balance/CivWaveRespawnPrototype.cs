// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Balance.CivWaveRespawnPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared._CIV14merka.Balance;

[Prototype(null, 1)]
public sealed class CivWaveRespawnPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public int BaseIntervalSeconds = 300;
  [DataField(null, false, 1, false, false, null)]
  public int MinIntervalSeconds = 180;
  [DataField(null, false, 1, false, false, null)]
  public int MaxIntervalSeconds = 420;
  [DataField(null, false, 1, false, false, null)]
  public int MinGhostsToTrigger = 2;
  [DataField(null, false, 1, false, false, null)]
  public float ScoreRatioForMax = 2f;
  [DataField(null, false, 1, false, false, null)]
  public int ConfirmWindowBaseSeconds = 20;
  [DataField(null, false, 1, false, false, null)]
  public int ConfirmWindowMinSeconds = 10;
  [DataField(null, false, 1, false, false, null)]
  public int ConfirmWindowMaxSeconds = 30;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
