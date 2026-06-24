// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.TechnologyDisk.Systems.TechnologyDiskSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Lathe;
using Content.Shared.NameModifier.Components;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Content.Shared.Research.Systems;
using Content.Shared.Research.TechnologyDisk.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Research.TechnologyDisk.Systems;

public sealed class TechnologyDiskSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _protoMan;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedResearchSystem _research;
  [Dependency]
  private SharedLatheSystem _lathe;
  [Dependency]
  private NameModifierSystem _nameModifier;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<TechnologyDiskComponent, MapInitEvent>(new EntityEventRefHandler<TechnologyDiskComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<TechnologyDiskComponent, AfterInteractEvent>(new EntityEventRefHandler<TechnologyDiskComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<TechnologyDiskComponent, ExaminedEvent>(new EntityEventRefHandler<TechnologyDiskComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<TechnologyDiskComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<TechnologyDiskComponent, RefreshNameModifiersEvent>(this.OnRefreshNameModifiers));
  }

  private void OnMapInit(Entity<TechnologyDiskComponent> ent, ref MapInitEvent args)
  {
    if (ent.Comp.Recipes != null)
      return;
    int num = int.Parse(this._protoMan.Index<WeightedRandomPrototype>(ent.Comp.TierWeightPrototype).Pick(this._random));
    HashSet<ProtoId<LatheRecipePrototype>> collection = new HashSet<ProtoId<LatheRecipePrototype>>();
    foreach (TechnologyPrototype enumeratePrototype in this._protoMan.EnumeratePrototypes<TechnologyPrototype>())
    {
      if (enumeratePrototype.Tier == num)
        collection.UnionWith((IEnumerable<ProtoId<LatheRecipePrototype>>) enumeratePrototype.RecipeUnlocks);
    }
    if (collection.Count == 0)
      return;
    ent.Comp.Recipes = new List<ProtoId<LatheRecipePrototype>>();
    ent.Comp.Recipes.Add(this._random.Pick<ProtoId<LatheRecipePrototype>>((IReadOnlyCollection<ProtoId<LatheRecipePrototype>>) collection));
    this.Dirty<TechnologyDiskComponent>(ent);
    this._nameModifier.RefreshNameModifiers((Entity<NameModifierComponent>) ent.Owner);
  }

  private void OnAfterInteract(Entity<TechnologyDiskComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    TechnologyDatabaseComponent comp;
    if (!this.HasComp<ResearchServerComponent>(valueOrDefault) || !this.TryComp<TechnologyDatabaseComponent>(valueOrDefault, out comp))
      return;
    if (ent.Comp.Recipes != null)
    {
      foreach (ProtoId<LatheRecipePrototype> recipe in ent.Comp.Recipes)
        this._research.AddLatheRecipe(valueOrDefault, (string) recipe, comp);
    }
    this._popup.PopupClient(this.Loc.GetString("tech-disk-inserted"), valueOrDefault, new EntityUid?(args.User));
    this.PredictedQueueDel(ent.Owner);
    args.Handled = true;
  }

  private void OnExamine(Entity<TechnologyDiskComponent> ent, ref ExaminedEvent args)
  {
    string markup = this.Loc.GetString("tech-disk-examine-none");
    if (ent.Comp.Recipes != null && ent.Comp.Recipes.Count > 0)
    {
      markup = this.Loc.GetString("tech-disk-examine", ("result", (object) this._lathe.GetRecipeName(this._protoMan.Index<LatheRecipePrototype>(ent.Comp.Recipes[0]))));
      if (ent.Comp.Recipes.Count > 1)
        markup = $"{markup} {this.Loc.GetString("tech-disk-examine-more")}";
    }
    args.PushMarkup(markup);
  }

  private void OnRefreshNameModifiers(
    Entity<TechnologyDiskComponent> entity,
    ref RefreshNameModifiersEvent args)
  {
    if (entity.Comp.Recipes == null)
      return;
    foreach (ProtoId<LatheRecipePrototype> recipe in entity.Comp.Recipes)
    {
      LatheRecipePrototype proto = this._protoMan.Index<LatheRecipePrototype>(recipe);
      args.AddModifier((LocId) "tech-disk-name-format", 0, ("technology", (object) this._lathe.GetRecipeName(proto)));
    }
  }
}
