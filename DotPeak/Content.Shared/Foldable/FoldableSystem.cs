// Decompiled with JetBrains decompiler
// Type: Content.Shared.Foldable.FoldableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Buckle;
using Content.Shared.Buckle.Components;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Foldable;

public sealed class FoldableSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedBuckleSystem _buckle;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private AnchorableSystem _anchorable;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FoldableComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<FoldableComponent, GetVerbsEvent<AlternativeVerb>>(this.AddFoldVerb));
    this.SubscribeLocalEvent<FoldableComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<FoldableComponent, AfterAutoHandleStateEvent>(this.OnHandleState));
    this.SubscribeLocalEvent<FoldableComponent, ComponentInit>(new ComponentEventHandler<FoldableComponent, ComponentInit>(this.OnFoldableInit));
    this.SubscribeLocalEvent<FoldableComponent, ContainerGettingInsertedAttemptEvent>(new ComponentEventHandler<FoldableComponent, ContainerGettingInsertedAttemptEvent>(this.OnInsertEvent));
    this.SubscribeLocalEvent<FoldableComponent, StorageOpenAttemptEvent>(new ComponentEventRefHandler<FoldableComponent, StorageOpenAttemptEvent>(this.OnFoldableOpenAttempt));
    this.SubscribeLocalEvent<FoldableComponent, EntityStorageInsertedIntoAttemptEvent>(new EntityEventRefHandler<FoldableComponent, EntityStorageInsertedIntoAttemptEvent>(this.OnEntityStorageAttemptInsert));
    this.SubscribeLocalEvent<FoldableComponent, StrapAttemptEvent>(new ComponentEventRefHandler<FoldableComponent, StrapAttemptEvent>(this.OnStrapAttempt));
  }

  private void OnHandleState(
    EntityUid uid,
    FoldableComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.SetFolded(uid, component, component.IsFolded);
  }

  private void OnFoldableInit(EntityUid uid, FoldableComponent component, ComponentInit args)
  {
    this.SetFolded(uid, component, component.IsFolded);
  }

  private void OnFoldableOpenAttempt(
    EntityUid uid,
    FoldableComponent component,
    ref StorageOpenAttemptEvent args)
  {
    if (!component.IsFolded)
      return;
    args.Cancelled = true;
  }

  public void OnStrapAttempt(EntityUid uid, FoldableComponent comp, ref StrapAttemptEvent args)
  {
    if (!comp.IsFolded || comp.FitIntoEntityStorage)
      return;
    args.Cancelled = true;
  }

  private void OnEntityStorageAttemptInsert(
    Entity<FoldableComponent> entity,
    ref EntityStorageInsertedIntoAttemptEvent args)
  {
    if (!entity.Comp.IsFolded)
      return;
    args.Cancelled = true;
  }

  public bool IsFolded(EntityUid uid, FoldableComponent? component = null)
  {
    return this.Resolve<FoldableComponent>(uid, ref component) && component.IsFolded;
  }

  public void SetFolded(EntityUid uid, FoldableComponent component, bool folded)
  {
    component.IsFolded = folded;
    this.Dirty(uid, (IComponent) component);
    this._appearance.SetData(uid, (Enum) FoldableSystem.FoldedVisuals.State, (object) folded);
    if (component.EnableStrapOnUnfold)
      this._buckle.StrapSetEnabled(uid, !component.IsFolded);
    FoldedEvent args = new FoldedEvent(folded);
    this.RaiseLocalEvent<FoldedEvent>(uid, ref args);
  }

  private void OnInsertEvent(
    EntityUid uid,
    FoldableComponent component,
    ContainerGettingInsertedAttemptEvent args)
  {
    if (component.IsFolded || component.CanFoldInsideContainer)
      return;
    args.Cancel();
  }

  public bool TryToggleFold(EntityUid uid, FoldableComponent comp, EntityUid? folder = null)
  {
    int num = this.TrySetFolded(uid, comp, !comp.IsFolded) ? 1 : 0;
    if (num != 0)
      return num != 0;
    if (!folder.HasValue)
      return num != 0;
    if (comp.IsFolded)
    {
      this._popup.PopupPredicted(this.Loc.GetString("foldable-unfold-fail", ("object", (object) uid)), uid, new EntityUid?(folder.Value));
      return num != 0;
    }
    this._popup.PopupPredicted(this.Loc.GetString("foldable-fold-fail", ("object", (object) uid)), uid, new EntityUid?(folder.Value));
    return num != 0;
  }

  public bool CanToggleFold(EntityUid uid, FoldableComponent? fold = null)
  {
    PhysicsComponent comp;
    if (!this.Resolve<FoldableComponent>(uid, ref fold) || this._container.IsEntityInContainer(uid) && !fold.CanFoldInsideContainer || !this.TryComp<PhysicsComponent>(uid, out comp) || !this._anchorable.TileFree(this.Transform(uid).Coordinates, comp, new EntityUid?(uid)) || fold.IsLocked)
      return false;
    FoldAttemptEvent args = new FoldAttemptEvent(fold);
    this.RaiseLocalEvent<FoldAttemptEvent>(uid, ref args);
    return !args.Cancelled;
  }

  public bool TrySetFolded(EntityUid uid, FoldableComponent comp, bool state)
  {
    if (state == comp.IsFolded || !this.CanToggleFold(uid, comp))
      return false;
    this.SetFolded(uid, comp, state);
    return true;
  }

  private void AddFoldVerb(
    EntityUid uid,
    FoldableComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || component.IsLocked)
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryToggleFold(uid, component, new EntityUid?(args.User)));
    alternativeVerb1.Text = component.IsFolded ? this.Loc.GetString((string) component.UnfoldVerbText) : this.Loc.GetString((string) component.FoldVerbText);
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png"));
    alternativeVerb1.Priority = component.IsFolded ? 0 : 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  [NetSerializable]
  [Serializable]
  public enum FoldedVisuals : byte
  {
    State,
  }
}
