// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.TraitPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

#nullable enable
namespace Content.Shared.Traits;

[Prototype(null, 1)]
public sealed class TraitPrototype : IPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Blacklist;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? TraitGear;
  [DataField(null, false, 1, false, false, null)]
  public int Cost;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<TraitCategoryPrototype>? Category;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public LocId Name { get; private set; } = (LocId) string.Empty;

  [DataField(null, false, 1, false, false, null)]
  public LocId? Description { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry Components { get; private set; }
}
