// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Factions.CivFactionPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.PurchaseRequest;
using Content.Shared._CIV14merka.Teams;
using Content.Shared.Roles;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Factions;

[Prototype(null, 1)]
public sealed class CivFactionPrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public string Name = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string ShortTag = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public Color Color = Color.White;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon;
  [DataField(null, false, 1, true, false, null)]
  public Dictionary<CivTdmClass, ProtoId<StartingGearPrototype>> Loadouts = new Dictionary<CivTdmClass, ProtoId<StartingGearPrototype>>();
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<StartingGearPrototype>? BotGear;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CivPurchaseCatalogPrototype>? PurchaseCatalog;
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<CivSupplyContentPrototype>? SupplyContent;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, EntProtoId> SpawnEntities = new Dictionary<string, EntProtoId>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
