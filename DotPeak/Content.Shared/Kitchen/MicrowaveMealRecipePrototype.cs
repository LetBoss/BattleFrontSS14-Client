// Decompiled with JetBrains decompiler
// Type: Content.Shared.Kitchen.FoodRecipePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Kitchen;

[Robust.Shared.Prototypes.Prototype("microwaveMealRecipe", 1)]
public sealed class FoodRecipePrototype : IPrototype
{
  [DataField("name", false, 1, false, false, null)]
  private string _name = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public string Group = "Other";
  [DataField("reagents", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, ReagentPrototype>))]
  private System.Collections.Generic.Dictionary<string, FixedPoint2> _ingsReagents = new System.Collections.Generic.Dictionary<string, FixedPoint2>();
  [DataField("solids", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<FixedPoint2, EntityPrototype>))]
  private System.Collections.Generic.Dictionary<string, FixedPoint2> _ingsSolids = new System.Collections.Generic.Dictionary<string, FixedPoint2>();
  [DataField(null, false, 1, false, false, null)]
  public bool SecretRecipe;

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField("result", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string Result { get; private set; } = string.Empty;

  [DataField("time", false, 1, false, false, null)]
  public uint CookTime { get; private set; } = 5;

  public string Name => Loc.GetString(this._name);

  public IReadOnlyDictionary<string, FixedPoint2> IngredientsReagents
  {
    get => (IReadOnlyDictionary<string, FixedPoint2>) this._ingsReagents;
  }

  public IReadOnlyDictionary<string, FixedPoint2> IngredientsSolids
  {
    get => (IReadOnlyDictionary<string, FixedPoint2>) this._ingsSolids;
  }

  public FixedPoint2 IngredientCount()
  {
    FixedPoint2 fixedPoint2_1 = (FixedPoint2) 0 + (FixedPoint2) this._ingsReagents.Count;
    foreach (FixedPoint2 fixedPoint2_2 in this._ingsSolids.Values)
      fixedPoint2_1 += fixedPoint2_2;
    return fixedPoint2_1;
  }
}
