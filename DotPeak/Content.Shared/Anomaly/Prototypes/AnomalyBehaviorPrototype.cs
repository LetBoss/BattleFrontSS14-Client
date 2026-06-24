// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Prototypes.AnomalyBehaviorPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Anomaly.Prototypes;

[Prototype(null, 1)]
public sealed class AnomalyBehaviorPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public string Description = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public float EarnPointModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float PulseFrequencyModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float PulsePowerModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public float ParticleSensivity = 1f;
  [DataField(null, false, 1, false, true, null)]
  public ComponentRegistry Components = new ComponentRegistry();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
