// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lathe.Prototypes.LatheRecipePackPrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Research.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Lathe.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class LatheRecipePackPrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, true, false, null)]
  [AlwaysPushInheritance]
  public HashSet<ProtoId<LatheRecipePrototype>> Recipes = new HashSet<ProtoId<LatheRecipePrototype>>();

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<LatheRecipePackPrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
