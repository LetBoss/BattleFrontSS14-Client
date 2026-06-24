// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Prototypes.LatheRecipePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Content.Shared.Lathe.Prototypes;
using Content.Shared.Materials;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Research.Prototypes;

[Robust.Shared.Prototypes.Prototype(null, 1)]
public sealed class LatheRecipePrototype : IPrototype, IInheritingPrototype
{
  [DataField(null, false, 1, false, false, null)]
  public LocId? Name;
  [DataField(null, false, 1, false, false, null)]
  public LocId? Description;
  [DataField(null, false, 1, false, false, null)]
  public EntProtoId? Result;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<ReagentPrototype>, FixedPoint2>? ResultReagents;
  [DataField(null, false, 1, false, false, null)]
  public SpriteSpecifier? Icon;
  [DataField("completetime", false, 1, false, false, null)]
  public TimeSpan CompleteTime = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<MaterialPrototype>, int> Materials = new Dictionary<ProtoId<MaterialPrototype>, int>();
  [DataField(null, false, 1, false, false, null)]
  public bool ApplyMaterialDiscount = true;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<LatheCategoryPrototype>> Categories = new List<ProtoId<LatheCategoryPrototype>>();

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [ParentDataField(typeof (AbstractPrototypeIdArraySerializer<LatheRecipePrototype>), 1)]
  public string[]? Parents { get; private set; }

  [NeverPushInheritance]
  [AbstractDataField(1)]
  public bool Abstract { get; private set; }
}
