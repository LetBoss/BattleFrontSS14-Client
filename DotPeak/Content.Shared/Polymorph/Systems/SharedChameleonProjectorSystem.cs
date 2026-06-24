// Decompiled with JetBrains decompiler
// Type: Content.Shared.Polymorph.Systems.SharedChameleonProjectorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Item;
using Content.Shared.Polymorph.Components;
using Content.Shared.Popups;
using Content.Shared.Storage.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Polymorph.Systems;

public abstract class SharedChameleonProjectorSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private ISerializationManager _serMan;
  [Dependency]
  private MetaDataSystem _meta;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _xform;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ChameleonDisguiseComponent, InteractHandEvent>(new EntityEventRefHandler<ChameleonDisguiseComponent, InteractHandEvent>(this.OnDisguiseInteractHand), new Type[1]
    {
      typeof (SharedItemSystem)
    });
    this.SubscribeLocalEvent<ChameleonDisguiseComponent, DamageChangedEvent>(new EntityEventRefHandler<ChameleonDisguiseComponent, DamageChangedEvent>(this.OnDisguiseDamaged));
    this.SubscribeLocalEvent<ChameleonDisguiseComponent, InsertIntoEntityStorageAttemptEvent>(new EntityEventRefHandler<ChameleonDisguiseComponent, InsertIntoEntityStorageAttemptEvent>(this.OnDisguiseInsertAttempt));
    this.SubscribeLocalEvent<ChameleonDisguiseComponent, ComponentShutdown>(new EntityEventRefHandler<ChameleonDisguiseComponent, ComponentShutdown>(this.OnDisguiseShutdown));
    this.SubscribeLocalEvent<ChameleonDisguisedComponent, EntGotInsertedIntoContainerMessage>(new EntityEventRefHandler<ChameleonDisguisedComponent, EntGotInsertedIntoContainerMessage>(this.OnDisguisedInserted));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, AfterInteractEvent>(new EntityEventRefHandler<ChameleonProjectorComponent, AfterInteractEvent>(this.OnInteract));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, GetVerbsEvent<UtilityVerb>>(new EntityEventRefHandler<ChameleonProjectorComponent, GetVerbsEvent<UtilityVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, DisguiseToggleNoRotEvent>(new EntityEventRefHandler<ChameleonProjectorComponent, DisguiseToggleNoRotEvent>(this.OnToggleNoRot));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, DisguiseToggleAnchoredEvent>(new EntityEventRefHandler<ChameleonProjectorComponent, DisguiseToggleAnchoredEvent>(this.OnToggleAnchored));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, HandDeselectedEvent>(new EntityEventRefHandler<ChameleonProjectorComponent, HandDeselectedEvent>(this.OnDeselected));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, GotUnequippedHandEvent>(new EntityEventRefHandler<ChameleonProjectorComponent, GotUnequippedHandEvent>(this.OnUnequipped));
    this.SubscribeLocalEvent<ChameleonProjectorComponent, ComponentShutdown>(new EntityEventRefHandler<ChameleonProjectorComponent, ComponentShutdown>(this.OnProjectorShutdown));
  }

  private void OnDisguiseInteractHand(
    Entity<ChameleonDisguiseComponent> ent,
    ref InteractHandEvent args)
  {
    this.TryReveal((Entity<ChameleonDisguisedComponent>) ent.Comp.User);
    args.Handled = true;
  }

  private void OnDisguiseDamaged(
    Entity<ChameleonDisguiseComponent> ent,
    ref DamageChangedEvent args)
  {
    DamageSpecifier damageDelta = args.DamageDelta;
    if (damageDelta == null)
      return;
    this._damageable.TryChangeDamage(new EntityUid?(ent.Comp.User), damageDelta);
  }

  private void OnDisguiseInsertAttempt(
    Entity<ChameleonDisguiseComponent> ent,
    ref InsertIntoEntityStorageAttemptEvent args)
  {
    args.Cancelled = true;
    this.TryReveal((Entity<ChameleonDisguisedComponent>) ent.Comp.User);
  }

  private void OnDisguiseShutdown(
    Entity<ChameleonDisguiseComponent> ent,
    ref ComponentShutdown args)
  {
    this._actions.RemoveProvidedActions(ent.Comp.User, ent.Comp.Projector);
  }

  private void OnDisguisedInserted(
    Entity<ChameleonDisguisedComponent> ent,
    ref EntGotInsertedIntoContainerMessage args)
  {
    this.TryReveal((Entity<ChameleonDisguisedComponent>) ((EntityUid) ent, (ChameleonDisguisedComponent) ent));
  }

  private void OnInteract(Entity<ChameleonProjectorComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = true;
    this.TryDisguise(ent, args.User, valueOrDefault);
  }

  private void OnGetVerbs(
    Entity<ChameleonProjectorComponent> ent,
    ref GetVerbsEvent<UtilityVerb> args)
  {
    if (!args.CanAccess)
      return;
    EntityUid user = args.User;
    EntityUid target = args.Target;
    SortedSet<UtilityVerb> verbs = args.Verbs;
    UtilityVerb utilityVerb = new UtilityVerb();
    utilityVerb.Act = (Action) (() => this.TryDisguise(ent, user, target));
    utilityVerb.Text = this.Loc.GetString("chameleon-projector-set-disguise");
    verbs.Add(utilityVerb);
  }

  public bool TryDisguise(
    Entity<ChameleonProjectorComponent> ent,
    EntityUid user,
    EntityUid target)
  {
    if (this._container.IsEntityInContainer(target) || this._container.IsEntityInContainer(user))
    {
      this._popup.PopupClient(this.Loc.GetString("chameleon-projector-inside-container"), target, new EntityUid?(user));
      return false;
    }
    if (this.IsInvalid(ent.Comp, target))
    {
      this._popup.PopupClient(this.Loc.GetString("chameleon-projector-invalid"), target, new EntityUid?(user));
      return false;
    }
    this._popup.PopupClient(this.Loc.GetString("chameleon-projector-success"), target, new EntityUid?(user));
    this.Disguise(ent, user, target);
    return true;
  }

  private void OnToggleNoRot(
    Entity<ChameleonProjectorComponent> ent,
    ref DisguiseToggleNoRotEvent args)
  {
    EntityUid? disguised = ent.Comp.Disguised;
    if (!disguised.HasValue)
      return;
    EntityUid valueOrDefault = disguised.GetValueOrDefault();
    TransformComponent xform = this.Transform(valueOrDefault);
    this._xform.SetLocalRotationNoLerp(valueOrDefault, Angle.op_Implicit(0.0f), xform);
    xform.NoLocalRotation = !xform.NoLocalRotation;
    args.Handled = true;
  }

  private void OnToggleAnchored(
    Entity<ChameleonProjectorComponent> ent,
    ref DisguiseToggleAnchoredEvent args)
  {
    EntityUid? disguised = ent.Comp.Disguised;
    if (!disguised.HasValue)
      return;
    EntityUid valueOrDefault = disguised.GetValueOrDefault();
    TransformComponent xform = this.Transform(valueOrDefault);
    if (xform.Anchored)
      this._xform.Unanchor(valueOrDefault, xform);
    else
      this._xform.AnchorEntity((Entity<TransformComponent>) (valueOrDefault, xform));
    args.Handled = true;
  }

  private void OnDeselected(Entity<ChameleonProjectorComponent> ent, ref HandDeselectedEvent args)
  {
    this.RevealProjector(ent);
  }

  private void OnUnequipped(
    Entity<ChameleonProjectorComponent> ent,
    ref GotUnequippedHandEvent args)
  {
    this.RevealProjector(ent);
  }

  private void OnProjectorShutdown(
    Entity<ChameleonProjectorComponent> ent,
    ref ComponentShutdown args)
  {
    this.RevealProjector(ent);
  }

  public bool IsInvalid(ChameleonProjectorComponent comp, EntityUid target)
  {
    return this._whitelist.IsWhitelistFail(comp.Whitelist, target) || this._whitelist.IsBlacklistPass(comp.Blacklist, target);
  }

  public void Disguise(Entity<ChameleonProjectorComponent> ent, EntityUid user, EntityUid entity)
  {
    ChameleonProjectorComponent comp = ent.Comp;
    if (this._net.IsClient)
      return;
    this.TryReveal((Entity<ChameleonDisguisedComponent>) user);
    this._actions.AddAction(user, ref comp.NoRotActionEntity, (string) comp.NoRotAction, (EntityUid) ent);
    this._actions.AddAction(user, ref comp.AnchorActionEntity, (string) comp.AnchorAction, (EntityUid) ent);
    comp.Disguised = new EntityUid?(user);
    EntityUid entityUid = this.SpawnAttachedTo((string) comp.DisguiseProto, user.ToCoordinates(), rotation: new Angle());
    ChameleonDisguisedComponent disguisedComponent = this.AddComp<ChameleonDisguisedComponent>(user);
    disguisedComponent.Disguise = entityUid;
    this.Dirty(user, (IComponent) disguisedComponent);
    MetaDataComponent metaDataComponent = this.MetaData(entity);
    this._meta.SetEntityName(entityUid, metaDataComponent.EntityName);
    this._meta.SetEntityDescription(entityUid, metaDataComponent.EntityDescription);
    ChameleonDisguiseComponent disguiseComponent = this.EnsureComp<ChameleonDisguiseComponent>(entityUid);
    disguiseComponent.User = user;
    disguiseComponent.Projector = (EntityUid) ent;
    disguiseComponent.SourceEntity = entity;
    disguiseComponent.SourceProto = (EntProtoId?) this.Prototype(entity)?.ID;
    this.Dirty(entityUid, (IComponent) disguiseComponent);
    this.CopyComp<ItemComponent>((Entity<ChameleonDisguiseComponent>) (entityUid, disguiseComponent));
    this._appearance.CopyData((Entity<AppearanceComponent>) entity, (Entity<AppearanceComponent>) entityUid);
  }

  public bool TryReveal(Entity<ChameleonDisguisedComponent?> ent)
  {
    if (!this.Resolve<ChameleonDisguisedComponent>((EntityUid) ent, ref ent.Comp, false))
      return false;
    ChameleonDisguiseComponent comp1;
    ChameleonProjectorComponent comp2;
    if (this.TryComp<ChameleonDisguiseComponent>(ent.Comp.Disguise, out comp1) && this.TryComp<ChameleonProjectorComponent>(comp1.Projector, out comp2))
      comp2.Disguised = new EntityUid?();
    TransformComponent xform = this.Transform((EntityUid) ent);
    xform.NoLocalRotation = false;
    this._xform.Unanchor((EntityUid) ent, xform);
    this.Del(new EntityUid?(ent.Comp.Disguise));
    this.RemComp<ChameleonDisguisedComponent>((EntityUid) ent);
    return true;
  }

  public void RevealProjector(Entity<ChameleonProjectorComponent> ent)
  {
    EntityUid? disguised = ent.Comp.Disguised;
    if (!disguised.HasValue)
      return;
    this.TryReveal((Entity<ChameleonDisguisedComponent>) disguised.GetValueOrDefault());
  }

  protected bool CopyComp<T>(Entity<ChameleonDisguiseComponent> ent) where T : Component, new()
  {
    T src;
    if (!this.GetSrcComp<T>(ent.Comp, out src))
      return true;
    this.RemComp<T>((EntityUid) ent);
    T target = this.AddComp<T>((EntityUid) ent);
    this._serMan.CopyTo<T>(src, ref target, notNullableOverride: true);
    this.Dirty((EntityUid) ent, (IComponent) target);
    return false;
  }

  private bool GetSrcComp<T>(ChameleonDisguiseComponent comp, [NotNullWhen(true)] out T? src) where T : Component, new()
  {
    if (this.TryComp<T>(comp.SourceEntity, out src))
      return true;
    EntProtoId? sourceProto = comp.SourceProto;
    EntityPrototype prototype;
    return sourceProto.HasValue && this._proto.TryIndex<EntityPrototype>((string) sourceProto.GetValueOrDefault(), out prototype) && prototype.TryGetComponent<T>(out src, this.EntityManager.ComponentFactory);
  }
}
