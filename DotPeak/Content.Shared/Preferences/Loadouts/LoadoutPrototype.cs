// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.LoadoutPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Preferences.Loadouts.Effects;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Preferences.Loadouts;

[Prototype(null, 1)]
public sealed class LoadoutPrototype : IPrototype, IEquipmentLoadout
{
  [DataField(null, false, 1, false, false, null)]
  public string? GroupBy;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? DummyEntity;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StartingGearPrototype>? StartingGear;
  [DataField(null, false, 1, false, false, null)]
  public int? Cost;
  [DataField(null, false, 1, false, false, null)]
  public List<LoadoutEffect> Effects = new List<LoadoutEffect>();

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, EntProtoId> Equipment { get; set; } = new Dictionary<string, EntProtoId>();

  [DataField(null, false, 1, false, false, null)]
  public List<EntProtoId> Inhand { get; set; } = new List<EntProtoId>();

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, List<EntProtoId>> Storage { get; set; } = new Dictionary<string, List<EntProtoId>>();
}
