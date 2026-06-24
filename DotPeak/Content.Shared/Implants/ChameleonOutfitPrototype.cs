// Decompiled with JetBrains decompiler
// Type: Content.Shared.Implants.ChameleonOutfitPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Roles;
using Content.Shared.StatusIcon;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Implants;

[Prototype(null, 1)]
public sealed class ChameleonOutfitPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobPrototype>? Job;
  [DataField(null, false, 1, false, false, null)]
  public LocId? Name;
  [DataField(null, false, 1, false, false, null)]
  public LocId? LoadoutName;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StartingGearPrototype>? StartingGear;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<JobIconPrototype>? Icon;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<DepartmentPrototype>>? Departments;
  [DataField(null, false, 1, false, false, null)]
  public bool HasMindShield;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, EntProtoId> Equipment { get; set; } = new Dictionary<string, EntProtoId>();
}
