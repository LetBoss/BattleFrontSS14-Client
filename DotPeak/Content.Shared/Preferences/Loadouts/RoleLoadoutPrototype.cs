// Decompiled with JetBrains decompiler
// Type: Content.Shared.Preferences.Loadouts.RoleLoadoutPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Dataset;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Preferences.Loadouts;

[Prototype(null, 1)]
public sealed class RoleLoadoutPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public bool CanCustomizeName;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<LocalizedDatasetPrototype>? NameDataset;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<LoadoutGroupPrototype>> Groups = new List<ProtoId<LoadoutGroupPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public int? Points;

  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;
}
