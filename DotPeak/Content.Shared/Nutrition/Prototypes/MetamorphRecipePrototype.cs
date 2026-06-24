// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.Prototypes.MetamorphRecipePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Nutrition.FoodMetamorphRules;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Nutrition.Prototypes;

[Prototype(null, 1)]
public sealed class MetamorphRecipePrototype : IPrototype
{
  [DataField(null, false, 1, true, false, null)]
  public ProtoId<TagPrototype> Key = (ProtoId<TagPrototype>) string.Empty;
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Result;
  [DataField(null, false, 1, false, false, null)]
  public List<FoodMetamorphRule> Rules = new List<FoodMetamorphRule>();

  [IdDataField(1, null)]
  public string ID { get; private set; }
}
