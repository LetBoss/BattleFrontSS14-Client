// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.AccessGroupPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Access;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class AccessGroupPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public HashSet<ProtoId<AccessLevelPrototype>> Tags;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId<IFFFactionComponent>? Faction;
  [DataField(null, false, 1, false, false, null)]
  public string AccessGroup = "";
  [DataField(null, false, 1, false, false, null)]
  public bool Hidden;

  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  public string? Name { get; set; }

  public string GetAccessGroupName()
  {
    string name = this.Name;
    return name != null ? Loc.GetString(name) : this.ID;
  }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<AccessGroupPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
