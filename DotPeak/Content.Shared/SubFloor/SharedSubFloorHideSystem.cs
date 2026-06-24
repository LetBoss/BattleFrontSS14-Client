// Decompiled with JetBrains decompiler
// Type: Content.Shared.SubFloor.SharedSubFloorHideSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Audio;
using Content.Shared.Construction.Components;
using Content.Shared.Explosion;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.SubFloor;

public abstract class SharedSubFloorHideSystem : EntitySystem
{
  [Dependency]
  private ITileDefinitionManager _tileDefinitionManager;
  [Dependency]
  private SharedAmbientSoundSystem _ambientSoundSystem;
  [Dependency]
  protected SharedMapSystem Map;
  [Dependency]
  protected SharedAppearanceSystem Appearance;
  [Dependency]
  private SharedVisibilitySystem _visibility;
  [Dependency]
  protected SharedPopupSystem _popup;
  private Robust.Shared.GameObjects.EntityQuery<SubFloorHideComponent> _hideQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._hideQuery = this.GetEntityQuery<SubFloorHideComponent>();
    this.SubscribeLocalEvent<TileChangedEvent>(new EntityEventRefHandler<TileChangedEvent>(this.OnTileChanged));
    this.SubscribeLocalEvent<SubFloorHideComponent, ComponentStartup>(new ComponentEventHandler<SubFloorHideComponent, ComponentStartup>(this.OnSubFloorStarted));
    this.SubscribeLocalEvent<SubFloorHideComponent, ComponentShutdown>(new ComponentEventHandler<SubFloorHideComponent, ComponentShutdown>(this.OnSubFloorTerminating));
    this.SubscribeLocalEvent<SubFloorHideComponent, AnchorStateChangedEvent>(new ComponentEventRefHandler<SubFloorHideComponent, AnchorStateChangedEvent>(this.HandleAnchorChanged));
    this.SubscribeLocalEvent<SubFloorHideComponent, GettingInteractedWithAttemptEvent>(new ComponentEventRefHandler<SubFloorHideComponent, GettingInteractedWithAttemptEvent>(this.OnInteractionAttempt));
    this.SubscribeLocalEvent<SubFloorHideComponent, GettingAttackedAttemptEvent>(new ComponentEventRefHandler<SubFloorHideComponent, GettingAttackedAttemptEvent>(this.OnAttackAttempt));
    this.SubscribeLocalEvent<SubFloorHideComponent, GetExplosionResistanceEvent>(new ComponentEventRefHandler<SubFloorHideComponent, GetExplosionResistanceEvent>(this.OnGetExplosionResistance));
    this.SubscribeLocalEvent<SubFloorHideComponent, AnchorAttemptEvent>(new ComponentEventHandler<SubFloorHideComponent, AnchorAttemptEvent>(this.OnAnchorAttempt));
    this.SubscribeLocalEvent<SubFloorHideComponent, UnanchorAttemptEvent>(new ComponentEventHandler<SubFloorHideComponent, UnanchorAttemptEvent>(this.OnUnanchorAttempt));
  }

  private void OnAnchorAttempt(
    EntityUid uid,
    SubFloorHideComponent component,
    AnchorAttemptEvent args)
  {
    TransformComponent transformComponent = this.Transform(uid);
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(transformComponent.GridUid, out comp) || !this.HasFloorCover(transformComponent.GridUid.Value, comp, this.Map.TileIndicesFor(transformComponent.GridUid.Value, comp, transformComponent.Coordinates)))
      return;
    this._popup.PopupClient(this.Loc.GetString("subfloor-anchor-failure", ("entity", (object) uid)), new EntityUid?(args.User));
    args.Cancel();
  }

  private void OnUnanchorAttempt(
    EntityUid uid,
    SubFloorHideComponent component,
    UnanchorAttemptEvent args)
  {
    if (!component.IsUnderCover)
      return;
    this._popup.PopupClient(this.Loc.GetString("subfloor-unanchor-failure", ("entity", (object) uid)), new EntityUid?(args.User));
    args.Cancel();
  }

  private void OnGetExplosionResistance(
    EntityUid uid,
    SubFloorHideComponent component,
    ref GetExplosionResistanceEvent args)
  {
    if (!component.BlockInteractions || !component.IsUnderCover)
      return;
    args.DamageCoefficient = 0.0f;
  }

  private void OnAttackAttempt(
    EntityUid uid,
    SubFloorHideComponent component,
    ref GettingAttackedAttemptEvent args)
  {
    if (!component.BlockInteractions || !component.IsUnderCover)
      return;
    args.Cancelled = true;
  }

  private void OnInteractionAttempt(
    EntityUid uid,
    SubFloorHideComponent component,
    ref GettingInteractedWithAttemptEvent args)
  {
    if (!component.BlockInteractions || !component.IsUnderCover)
      return;
    args.Cancelled = true;
  }

  private void OnSubFloorStarted(
    EntityUid uid,
    SubFloorHideComponent component,
    ComponentStartup _)
  {
    this.UpdateFloorCover(uid, component);
    this.UpdateAppearance(uid, component);
    this.EnsureComp<CollideOnAnchorComponent>(uid);
  }

  private void OnSubFloorTerminating(
    EntityUid uid,
    SubFloorHideComponent component,
    ComponentShutdown _)
  {
    if (this.Comp<MetaDataComponent>(uid).EntityLifeStage >= EntityLifeStage.Terminating)
      return;
    this.SetUnderCover((Entity<SubFloorHideComponent>) (uid, component), false);
    this.UpdateAppearance(uid, component);
  }

  private void HandleAnchorChanged(
    EntityUid uid,
    SubFloorHideComponent component,
    ref AnchorStateChangedEvent args)
  {
    if (args.Anchored)
    {
      TransformComponent xform = this.Transform(uid);
      this.UpdateFloorCover(uid, component, xform);
    }
    else
    {
      if (!component.IsUnderCover)
        return;
      this.SetUnderCover((Entity<SubFloorHideComponent>) (uid, component), false);
      this.UpdateAppearance(uid, component);
    }
  }

  private void OnTileChanged(ref TileChangedEvent args)
  {
    foreach (TileChangedEntry change in args.Changes)
    {
      if (!change.OldTile.IsEmpty && !change.NewTile.IsEmpty)
        this.UpdateTile((EntityUid) args.Entity, args.Entity.Comp, change.GridIndices);
    }
  }

  private void UpdateFloorCover(
    EntityUid uid,
    SubFloorHideComponent? component = null,
    TransformComponent? xform = null)
  {
    if (!this.Resolve<SubFloorHideComponent, TransformComponent>(uid, ref component, ref xform))
      return;
    MapGridComponent comp;
    if (xform.Anchored && this.TryComp<MapGridComponent>(xform.GridUid, out comp))
      this.SetUnderCover((Entity<SubFloorHideComponent>) (uid, component), this.HasFloorCover(xform.GridUid.Value, comp, this.Map.TileIndicesFor(xform.GridUid.Value, comp, xform.Coordinates)));
    else
      this.SetUnderCover((Entity<SubFloorHideComponent>) (uid, component), false);
    this.UpdateAppearance(uid, component);
  }

  private void SetUnderCover(Entity<SubFloorHideComponent> entity, bool value)
  {
    this._visibility.SetLayer((Entity<VisibilityComponent>) entity.Owner, !value || entity.Comp.VisibleLayers.Count != 0 ? (ushort) 1 : (ushort) 4);
    if (entity.Comp.IsUnderCover == value)
      return;
    entity.Comp.IsUnderCover = value;
  }

  public bool HasFloorCover(EntityUid gridUid, MapGridComponent grid, Vector2i position)
  {
    return !((ContentTileDefinition) this._tileDefinitionManager[this.Map.GetTileRef(gridUid, grid, position).Tile.TypeId]).IsSubFloor;
  }

  private void UpdateTile(EntityUid gridUid, MapGridComponent grid, Vector2i position)
  {
    bool flag = this.HasFloorCover(gridUid, grid, position);
    foreach (EntityUid anchoredEntity in this.Map.GetAnchoredEntities(gridUid, grid, position))
    {
      SubFloorHideComponent component;
      if (this._hideQuery.TryComp(anchoredEntity, out component) && component.IsUnderCover != flag)
      {
        this.SetUnderCover((Entity<SubFloorHideComponent>) (anchoredEntity, component), flag);
        this.UpdateAppearance(anchoredEntity, component);
      }
    }
  }

  public void UpdateAppearance(
    EntityUid uid,
    SubFloorHideComponent? hideComp = null,
    AppearanceComponent? appearance = null)
  {
    if (!this.Resolve<SubFloorHideComponent>(uid, ref hideComp, false))
      return;
    if (hideComp.BlockAmbience && hideComp.IsUnderCover)
      this._ambientSoundSystem.SetAmbience(uid, false);
    else if (hideComp.BlockAmbience && !hideComp.IsUnderCover)
      this._ambientSoundSystem.SetAmbience(uid, true);
    if (!this.Resolve<AppearanceComponent>(uid, ref appearance, false))
      return;
    this.Appearance.SetData(uid, (Enum) SubFloorVisuals.Covered, (object) hideComp.IsUnderCover, appearance);
  }

  [NetSerializable]
  [Serializable]
  protected sealed class ShowSubfloorRequestEvent : EntityEventArgs
  {
    public bool Value;
  }
}
