// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Systems.BlueprintSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Lathe;
using Content.Shared.Popups;
using Content.Shared.Research.Components;
using Content.Shared.Research.Prototypes;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared.Research.Systems;

public sealed class BlueprintSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<BlueprintReceiverComponent, ComponentStartup>(new EntityEventRefHandler<BlueprintReceiverComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<BlueprintReceiverComponent, AfterInteractUsingEvent>(new EntityEventRefHandler<BlueprintReceiverComponent, AfterInteractUsingEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<BlueprintReceiverComponent, LatheGetRecipesEvent>(new EntityEventRefHandler<BlueprintReceiverComponent, LatheGetRecipesEvent>(this.OnGetRecipes));
  }

  private void OnStartup(Entity<BlueprintReceiverComponent> ent, ref ComponentStartup args)
  {
    this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.ContainerId);
  }

  private void OnAfterInteract(
    Entity<BlueprintReceiverComponent> ent,
    ref AfterInteractUsingEvent args)
  {
    BlueprintComponent comp;
    if (args.Handled || !args.CanReach || !this.TryComp<BlueprintComponent>(args.Used, out comp))
      return;
    args.Handled = this.TryInsertBlueprint(ent, (Entity<BlueprintComponent>) (args.Used, comp), new EntityUid?(args.User));
  }

  private void OnGetRecipes(Entity<BlueprintReceiverComponent> ent, ref LatheGetRecipesEvent args)
  {
    foreach (ProtoId<LatheRecipePrototype> blueprintRecipe in this.GetBlueprintRecipes(ent))
      args.Recipes.Add(blueprintRecipe);
  }

  public bool TryInsertBlueprint(
    Entity<BlueprintReceiverComponent> ent,
    Entity<BlueprintComponent> blueprint,
    EntityUid? user)
  {
    if (!this.CanInsertBlueprint(ent, blueprint, user))
      return false;
    if (user.HasValue)
      this._popup.PopupPredicted(this.Loc.GetString("blueprint-receiver-popup-insert", (nameof (user), (object) Identity.Entity(user.Value, (IEntityManager) this.EntityManager)), (nameof (blueprint), (object) Identity.Entity((EntityUid) blueprint, (IEntityManager) this.EntityManager)), ("receiver", (object) Identity.Entity((EntityUid) ent, (IEntityManager) this.EntityManager))), (EntityUid) ent, user);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) blueprint.Owner, this._container.GetContainer((EntityUid) ent, ent.Comp.ContainerId));
    TechnologyDatabaseModifiedEvent args = new TechnologyDatabaseModifiedEvent(blueprint.Comp.ProvidedRecipes.Select<ProtoId<LatheRecipePrototype>, string>((Func<ProtoId<LatheRecipePrototype>, string>) (it => it.Id)).ToList<string>());
    this.RaiseLocalEvent<TechnologyDatabaseModifiedEvent>((EntityUid) ent, ref args);
    return true;
  }

  public bool CanInsertBlueprint(
    Entity<BlueprintReceiverComponent> ent,
    Entity<BlueprintComponent> blueprint,
    EntityUid? user)
  {
    if (this._entityWhitelist.IsWhitelistFail(ent.Comp.Whitelist, (EntityUid) blueprint))
      return false;
    if (blueprint.Comp.ProvidedRecipes.Count == 0)
    {
      this.Log.Error($"Attempted to insert blueprint {this.ToPrettyString(new EntityUid?((EntityUid) blueprint))} with no recipes.");
      return false;
    }
    HashSet<ProtoId<LatheRecipePrototype>> blueprintRecipes = this.GetBlueprintRecipes(ent);
    if (blueprintRecipes.Count == 0 || !blueprintRecipes.IsSupersetOf((IEnumerable<ProtoId<LatheRecipePrototype>>) blueprint.Comp.ProvidedRecipes))
      return this._container.CanInsert((EntityUid) blueprint, this._container.GetContainer((EntityUid) ent, ent.Comp.ContainerId));
    this._popup.PopupPredicted(this.Loc.GetString("blueprint-receiver-popup-recipe-exists"), (EntityUid) ent, user);
    return false;
  }

  public HashSet<ProtoId<LatheRecipePrototype>> GetBlueprintRecipes(
    Entity<BlueprintReceiverComponent> ent)
  {
    BaseContainer container = this._container.GetContainer((EntityUid) ent, ent.Comp.ContainerId);
    HashSet<ProtoId<LatheRecipePrototype>> blueprintRecipes = new HashSet<ProtoId<LatheRecipePrototype>>();
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      BlueprintComponent comp;
      if (this.TryComp<BlueprintComponent>(containedEntity, out comp))
      {
        foreach (ProtoId<LatheRecipePrototype> providedRecipe in comp.ProvidedRecipes)
          blueprintRecipes.Add(providedRecipe);
      }
    }
    return blueprintRecipes;
  }
}
