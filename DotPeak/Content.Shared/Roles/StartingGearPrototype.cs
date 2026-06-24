// Decompiled with JetBrains decompiler
// Type: Content.Shared.Roles.StartingGearPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Roles;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class StartingGearPrototype : IPrototype, IInheritingPrototype, IEquipmentLoadout
{
  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; } = string.Empty;

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<StartingGearPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public Dictionary<string, EntProtoId> Equipment { get; set; } = new Dictionary<string, EntProtoId>();

  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public List<EntProtoId> Inhand { get; set; } = new List<EntProtoId>();

  [DataField(null, false, 1, false, false, null)]
  [AlwaysPushInheritance]
  public Dictionary<string, List<EntProtoId>> Storage { get; set; } = new Dictionary<string, List<EntProtoId>>();
}
