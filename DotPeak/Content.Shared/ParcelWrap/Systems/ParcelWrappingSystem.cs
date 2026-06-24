// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParcelWrap.Systems.ParcelWrappingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Materials;
using Content.Shared.ParcelWrap.Components;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.ParcelWrap.Systems;

public sealed class ParcelWrappingSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedChargesSystem _charges;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedItemSystem _item;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeParcelWrap();
    this.InitializeWrappedParcel();
  }

  public bool IsWrappable(Entity<ParcelWrapComponent> wrapper, EntityUid target)
  {
    return wrapper.Owner != target && !this._charges.IsEmpty((Entity<LimitedChargesComponent>) wrapper.Owner) && this._whitelist.IsWhitelistPass(wrapper.Comp.Whitelist, target) && this._whitelist.IsBlacklistFail(wrapper.Comp.Blacklist, target);
  }

  private void InitializeParcelWrap()
  {
    this.SubscribeLocalEvent<ParcelWrapComponent, AfterInteractEvent>(new EntityEventRefHandler<ParcelWrapComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<ParcelWrapComponent, GetVerbsEvent<UtilityVerb>>(new EntityEventRefHandler<ParcelWrapComponent, GetVerbsEvent<UtilityVerb>>(this.OnGetVerbsForParcelWrap));
    this.SubscribeLocalEvent<ParcelWrapComponent, ParcelWrapItemDoAfterEvent>(new EntityEventRefHandler<ParcelWrapComponent, ParcelWrapItemDoAfterEvent>(this.OnWrapItemDoAfter));
  }

  private void OnAfterInteract(Entity<ParcelWrapComponent> entity, ref AfterInteractEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!args.CanReach || !this.IsWrappable(entity, valueOrDefault))
      return;
    args.Handled = this.TryStartWrapDoAfter(args.User, entity, valueOrDefault);
  }

  private void OnGetVerbsForParcelWrap(
    Entity<ParcelWrapComponent> entity,
    ref GetVerbsEvent<UtilityVerb> args)
  {
    if (!args.CanAccess || !this.IsWrappable(entity, args.Target))
      return;
    EntityUid user = args.User;
    EntityUid target = args.Target;
    SortedSet<UtilityVerb> verbs = args.Verbs;
    UtilityVerb utilityVerb = new UtilityVerb();
    utilityVerb.Text = this.Loc.GetString("parcel-wrap-verb-wrap");
    utilityVerb.Act = (Action) (() => this.TryStartWrapDoAfter(user, entity, target));
    verbs.Add(utilityVerb);
  }

  private void OnWrapItemDoAfter(
    Entity<ParcelWrapComponent> wrapper,
    ref ParcelWrapItemDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    this.WrapInternal(args.User, wrapper, valueOrDefault);
    args.Handled = true;
  }

  private bool TryStartWrapDoAfter(
    EntityUid user,
    Entity<ParcelWrapComponent> wrapper,
    EntityUid target)
  {
    return this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, wrapper.Comp.WrapDelay, (DoAfterEvent) new ParcelWrapItemDoAfterEvent(), new EntityUid?((EntityUid) wrapper), new EntityUid?(target), new EntityUid?((EntityUid) wrapper))
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnDamage = true
    });
  }

  private void WrapInternal(EntityUid user, Entity<ParcelWrapComponent> wrapper, EntityUid target)
  {
    if (this._net.IsServer)
    {
      EntityUid uid = this.Spawn((string) wrapper.Comp.ParcelPrototype, this.Transform(target).Coordinates);
      ItemComponent comp;
      this.TryComp<ItemComponent>(target, out comp);
      ProtoId<ItemSizePrototype> size = wrapper.Comp.FallbackItemSize;
      if (wrapper.Comp.WrappedItemsMaintainSize && comp != null)
        size = comp.Size;
      ItemComponent component = this.Comp<ItemComponent>(uid);
      this._item.SetSize(uid, size, component);
      this._appearance.SetData(uid, (Enum) WrappedParcelVisuals.Size, (object) size.Id);
      if (wrapper.Comp.WrappedItemsMaintainShape && comp != null)
      {
        List<Box2i> shape = comp.Shape;
        if (shape != null)
          this._item.SetShape(uid, shape, component);
      }
      BaseContainer container;
      if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (target, (TransformComponent) null, (MetaDataComponent) null), out container))
      {
        this._container.Remove((Entity<TransformComponent, MetaDataComponent>) target, container);
        this._container.InsertOrDrop((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) (uid, (TransformComponent) null, (MetaDataComponent) null), container);
      }
      WrappedParcelComponent wrappedParcelComponent = this.EnsureComp<WrappedParcelComponent>(uid);
      if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) target, (BaseContainer) wrappedParcelComponent.Contents))
        this.QueueDel(new EntityUid?(uid));
    }
    this._charges.TryUseCharges((Entity<LimitedChargesComponent>) wrapper.Owner, 1);
    if (this._net.IsServer && this._charges.IsEmpty((Entity<LimitedChargesComponent>) wrapper.Owner))
      this.QueueDel(new EntityUid?((EntityUid) wrapper));
    this._audio.PlayPredicted(wrapper.Comp.WrapSound, target, new EntityUid?(user));
  }

  private void InitializeWrappedParcel()
  {
    this.SubscribeLocalEvent<WrappedParcelComponent, ComponentInit>(new EntityEventRefHandler<WrappedParcelComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<WrappedParcelComponent, UseInHandEvent>(new EntityEventRefHandler<WrappedParcelComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<WrappedParcelComponent, GetVerbsEvent<InteractionVerb>>(new EntityEventRefHandler<WrappedParcelComponent, GetVerbsEvent<InteractionVerb>>(this.OnGetVerbsForWrappedParcel));
    this.SubscribeLocalEvent<WrappedParcelComponent, UnwrapWrappedParcelDoAfterEvent>(new EntityEventRefHandler<WrappedParcelComponent, UnwrapWrappedParcelDoAfterEvent>(this.OnUnwrapParcelDoAfter));
    this.SubscribeLocalEvent<WrappedParcelComponent, DestructionEventArgs>(new EntityEventRefHandler<WrappedParcelComponent, DestructionEventArgs>(this.OnDestroyed<DestructionEventArgs>));
    this.SubscribeLocalEvent<WrappedParcelComponent, GotReclaimedEvent>(new EntityEventRefHandler<WrappedParcelComponent, GotReclaimedEvent>(this.OnDestroyed<GotReclaimedEvent>));
  }

  private void OnComponentInit(Entity<WrappedParcelComponent> entity, ref ComponentInit args)
  {
    entity.Comp.Contents = this._container.EnsureContainer<ContainerSlot>((EntityUid) entity, entity.Comp.ContainerId);
  }

  private void OnUseInHand(Entity<WrappedParcelComponent> entity, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = this.TryStartUnwrapDoAfter(args.User, entity);
  }

  private void OnGetVerbsForWrappedParcel(
    Entity<WrappedParcelComponent> entity,
    ref GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess)
      return;
    EntityUid user = args.User;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = this.Loc.GetString("parcel-wrap-verb-unwrap");
    interactionVerb.Act = (Action) (() => this.TryStartUnwrapDoAfter(user, entity));
    verbs.Add(interactionVerb);
  }

  private void OnUnwrapParcelDoAfter(
    Entity<WrappedParcelComponent> entity,
    ref UnwrapWrappedParcelDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    WrappedParcelComponent comp;
    if (!this.TryComp<WrappedParcelComponent>(valueOrDefault, out comp))
      return;
    this.UnwrapInternal(new EntityUid?(args.User), (Entity<WrappedParcelComponent>) (valueOrDefault, comp));
    args.Handled = true;
  }

  private void OnDestroyed<T>(Entity<WrappedParcelComponent> parcel, ref T args)
  {
    EntityUid? nullable = this.UnwrapInternal(new EntityUid?(), parcel);
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    this._popup.PopupPredicted(this.Loc.GetString("parcel-wrap-popup-parcel-destroyed", ("contents", (object) valueOrDefault)), valueOrDefault, new EntityUid?(), PopupType.MediumCaution);
  }

  private bool TryStartUnwrapDoAfter(EntityUid user, Entity<WrappedParcelComponent> parcel)
  {
    return this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, parcel.Comp.UnwrapDelay, (DoAfterEvent) new UnwrapWrappedParcelDoAfterEvent(), new EntityUid?((EntityUid) parcel), new EntityUid?((EntityUid) parcel))
    {
      NeedHand = true
    });
  }

  private EntityUid? UnwrapInternal(EntityUid? user, Entity<WrappedParcelComponent> parcel)
  {
    EntityUid? containedEntity = parcel.Comp.Contents.ContainedEntity;
    this._audio.PlayPredicted(parcel.Comp.UnwrapSound, (EntityUid) parcel, user);
    if (!this._net.IsServer)
      return containedEntity;
    TransformComponent transformComponent = this.Transform((EntityUid) parcel);
    if (containedEntity.HasValue)
    {
      EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
      this._container.Remove((Entity<TransformComponent, MetaDataComponent>) valueOrDefault, (BaseContainer) parcel.Comp.Contents, force: true, destination: new EntityCoordinates?(transformComponent.Coordinates));
      BaseContainer container;
      if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) parcel, (TransformComponent) null, (MetaDataComponent) null), out container))
      {
        this._container.Remove((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) parcel, (TransformComponent) null, (MetaDataComponent) null), container, force: true);
        this._container.InsertOrDrop((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) (valueOrDefault, (TransformComponent) null, (MetaDataComponent) null), container);
      }
    }
    EntProtoId? unwrapTrash = parcel.Comp.UnwrapTrash;
    if (unwrapTrash.HasValue)
      this._transform.DropNextTo((Entity<TransformComponent>) (this.Spawn((string) unwrapTrash.GetValueOrDefault(), transformComponent.Coordinates), (TransformComponent) null), (Entity<TransformComponent>) ((EntityUid) parcel, transformComponent));
    this.QueueDel(new EntityUid?((EntityUid) parcel));
    return containedEntity;
  }
}
