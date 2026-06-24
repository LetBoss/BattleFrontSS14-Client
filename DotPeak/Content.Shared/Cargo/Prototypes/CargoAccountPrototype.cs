// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Prototypes.CargoAccountPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radio;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoAccountPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public LocId Name;
  [DataField(null, false, 1, false, false, null)]
  public LocId Code;
  [DataField(null, false, 1, false, false, null)]
  public Color Color;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<RadioChannelPrototype> RadioChannel;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId AcquisitionSlip;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
