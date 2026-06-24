// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Marines.Roles.Ranks.RankPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

#nullable enable
namespace Content.Shared._RMC14.Marines.Roles.Ranks;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class RankPrototype : IPrototype, IInheritingPrototype
{
  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<RankPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [AlwaysPushInheritance]
  [DataField(null, false, 1, true, false, null)]
  public string Name { get; set; }

  [AlwaysPushInheritance]
  [DataField(null, false, 1, true, false, null)]
  public string Prefix { get; set; }

  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public string? MalePrefix { get; set; }

  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public string? FemalePrefix { get; set; }

  [AlwaysPushInheritance]
  [DataField(null, false, 1, false, false, null)]
  public string? Paygrade { get; set; }
}
