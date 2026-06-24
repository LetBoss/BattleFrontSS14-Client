// Decompiled with JetBrains decompiler
// Type: Content.Shared.Lathe.SharedLatheSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Emag.Systems;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Lathe.Prototypes;
using Content.Shared.Localizations;
using Content.Shared.Materials;
using Content.Shared.Research.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared.Lathe;

public abstract class SharedLatheSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private SharedMaterialStorageSystem _materialStorage;
  [Dependency]
  private EmagSystem _emag;
  [Dependency]
  private RMCReagentSystem _reagents;
  public readonly Dictionary<string, List<LatheRecipePrototype>> InverseRecipes = new Dictionary<string, List<LatheRecipePrototype>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EmagLatheRecipesComponent, GotEmaggedEvent>(new ComponentEventRefHandler<EmagLatheRecipesComponent, GotEmaggedEvent>(this.OnEmagged));
    this.SubscribeLocalEvent<LatheComponent, ExaminedEvent>(new EntityEventRefHandler<LatheComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<PrototypesReloadedEventArgs>(new EntityEventHandler<PrototypesReloadedEventArgs>(this.OnPrototypesReloaded));
    this.BuildInverseRecipeDictionary();
  }

  public HashSet<ProtoId<LatheRecipePrototype>> GetAllPossibleRecipes(LatheComponent component)
  {
    HashSet<ProtoId<LatheRecipePrototype>> allPossibleRecipes = new HashSet<ProtoId<LatheRecipePrototype>>();
    foreach (ProtoId<LatheRecipePackPrototype> staticPack in component.StaticPacks)
      allPossibleRecipes.UnionWith((IEnumerable<ProtoId<LatheRecipePrototype>>) this._proto.Index<LatheRecipePackPrototype>(staticPack).Recipes);
    foreach (ProtoId<LatheRecipePackPrototype> dynamicPack in component.DynamicPacks)
      allPossibleRecipes.UnionWith((IEnumerable<ProtoId<LatheRecipePrototype>>) this._proto.Index<LatheRecipePackPrototype>(dynamicPack).Recipes);
    return allPossibleRecipes;
  }

  public void AddRecipesFromPacks(
    HashSet<ProtoId<LatheRecipePrototype>> recipes,
    IEnumerable<ProtoId<LatheRecipePackPrototype>> packs)
  {
    foreach (ProtoId<LatheRecipePackPrototype> pack in packs)
    {
      LatheRecipePackPrototype recipePackPrototype = this._proto.Index<LatheRecipePackPrototype>(pack);
      recipes.UnionWith((IEnumerable<ProtoId<LatheRecipePrototype>>) recipePackPrototype.Recipes);
    }
  }

  private void OnExamined(Entity<LatheComponent> ent, ref ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || ent.Comp.ReagentOutputSlotId == null)
      return;
    args.PushMarkup(this.Loc.GetString("lathe-menu-reagent-slot-examine"));
  }

  public bool CanProduce(EntityUid uid, string recipe, int amount = 1, LatheComponent? component = null)
  {
    LatheRecipePrototype prototype;
    return this._proto.TryIndex<LatheRecipePrototype>(recipe, out prototype) && this.CanProduce(uid, prototype, amount, component);
  }

  public bool CanProduce(
    EntityUid uid,
    LatheRecipePrototype recipe,
    int amount = 1,
    LatheComponent? component = null)
  {
    if (!this.Resolve<LatheComponent>(uid, ref component) || !this.HasRecipe(uid, recipe, component))
      return false;
    foreach ((ProtoId<MaterialPrototype> protoId, int original) in recipe.Materials)
    {
      int num = SharedLatheSystem.AdjustMaterial(original, recipe.ApplyMaterialDiscount, component.MaterialUseMultiplier);
      if (this._materialStorage.GetMaterialAmount(uid, (string) protoId) < num * amount)
        return false;
    }
    return true;
  }

  private void OnEmagged(
    EntityUid uid,
    EmagLatheRecipesComponent component,
    ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(uid, EmagType.Interaction))
      return;
    args.Handled = true;
  }

  public static int AdjustMaterial(int original, bool reduce, float multiplier)
  {
    return !reduce ? original : (int) MathF.Ceiling((float) original * multiplier);
  }

  protected abstract bool HasRecipe(
    EntityUid uid,
    LatheRecipePrototype recipe,
    LatheComponent component);

  private void OnPrototypesReloaded(PrototypesReloadedEventArgs obj)
  {
    if (!obj.WasModified<LatheRecipePrototype>())
      return;
    this.BuildInverseRecipeDictionary();
  }

  private void BuildInverseRecipeDictionary()
  {
    this.InverseRecipes.Clear();
    foreach (LatheRecipePrototype enumeratePrototype in this._proto.EnumeratePrototypes<LatheRecipePrototype>())
    {
      EntProtoId? result = enumeratePrototype.Result;
      if (result.HasValue)
        this.InverseRecipes.GetOrNew<string, List<LatheRecipePrototype>>((string) result.GetValueOrDefault()).Add(enumeratePrototype);
    }
  }

  public bool TryGetRecipesFromEntity(string prototype, [NotNullWhen(true)] out List<LatheRecipePrototype>? recipes)
  {
    recipes = new List<LatheRecipePrototype>();
    List<LatheRecipePrototype> collection;
    if (this.InverseRecipes.TryGetValue(prototype, out collection))
      recipes.AddRange((IEnumerable<LatheRecipePrototype>) collection);
    return recipes.Count != 0;
  }

  public string GetRecipeName(ProtoId<LatheRecipePrototype> proto)
  {
    return this.GetRecipeName(this._proto.Index<LatheRecipePrototype>(proto));
  }

  public string GetRecipeName(LatheRecipePrototype proto)
  {
    LocId? name1 = proto.Name;
    if (!string.IsNullOrWhiteSpace(name1.HasValue ? (string) name1.GetValueOrDefault() : (string) null))
    {
      ILocalizationManager loc = this.Loc;
      LocId? name2 = proto.Name;
      string valueOrDefault = name2.HasValue ? (string) name2.GetValueOrDefault() : (string) null;
      return loc.GetString(valueOrDefault);
    }
    EntProtoId? result = proto.Result;
    if (result.HasValue)
      return this._proto.Index(result.GetValueOrDefault()).Name;
    Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> resultReagents = proto.ResultReagents;
    return resultReagents != null ? ContentLocalizationManager.FormatList(resultReagents.Select<KeyValuePair<ProtoId<ReagentPrototype>, FixedPoint2>, string>((Func<KeyValuePair<ProtoId<ReagentPrototype>, FixedPoint2>, string>) (p => this.Loc.GetString("lathe-menu-result-reagent-display", ("reagent", (object) this._reagents.Index(p.Key).LocalizedName), ("amount", (object) p.Value)))).ToList<string>()) : string.Empty;
  }

  public string GetRecipeDescription(ProtoId<LatheRecipePrototype> proto)
  {
    return this.GetRecipeDescription(this._proto.Index<LatheRecipePrototype>(proto));
  }

  public string GetRecipeDescription(LatheRecipePrototype proto)
  {
    LocId? description1 = proto.Description;
    if (!string.IsNullOrWhiteSpace(description1.HasValue ? (string) description1.GetValueOrDefault() : (string) null))
    {
      ILocalizationManager loc = this.Loc;
      LocId? description2 = proto.Description;
      string valueOrDefault = description2.HasValue ? (string) description2.GetValueOrDefault() : (string) null;
      return loc.GetString(valueOrDefault);
    }
    EntProtoId? result = proto.Result;
    if (result.HasValue)
      return this._proto.Index(result.GetValueOrDefault()).Description;
    Dictionary<ProtoId<ReagentPrototype>, FixedPoint2> resultReagents = proto.ResultReagents;
    return resultReagents != null ? this._reagents.Index(resultReagents.First<KeyValuePair<ProtoId<ReagentPrototype>, FixedPoint2>>().Key).LocalizedDescription : string.Empty;
  }
}
