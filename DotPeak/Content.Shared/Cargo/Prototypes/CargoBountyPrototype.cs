// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.Prototypes.CargoBountyPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Cargo.Prototypes;

[Prototype(null, 1)]
public sealed class CargoBountyPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public int Reward;
  [DataField(null, false, 1, false, false, null)]
  public LocId Description = LocId.op_Implicit(string.Empty);
  [DataField(null, false, 1, true, false, null)]
  public List<CargoBountyItemEntry> Entries = new List<CargoBountyItemEntry>();
  [DataField(null, false, 1, false, false, null)]
  public string IdPrefix = "NT";
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CargoBountyGroupPrototype> Group = ProtoId<CargoBountyGroupPrototype>.op_Implicit("StationBounty");
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Sprite;

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
