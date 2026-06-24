// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.SharedItemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Hands;
using Content.Shared._RMC14.Item;
using Content.Shared.Examine;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Analyzers;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Item;

public abstract class SharedItemSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  protected SharedContainerSystem Container;
  private Robust.Shared.GameObjects.EntityQuery<FixedItemSizeStorageComponent> _fixedItemSizeStorageQuery;

  public override void Initialize()
  {
    base.Initialize();
    this._fixedItemSizeStorageQuery = this.GetEntityQuery<FixedItemSizeStorageComponent>();
    this.SubscribeLocalEvent<ItemComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ItemComponent, GetVerbsEvent<InteractionVerb>>(this.AddPickupVerb));
    this.SubscribeLocalEvent<ItemComponent, InteractHandEvent>(new ComponentEventHandler<ItemComponent, InteractHandEvent>(this.OnHandInteract));
    this.SubscribeLocalEvent<ItemComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<ItemComponent, AfterAutoHandleStateEvent>(this.OnItemAutoState));
    this.SubscribeLocalEvent<ItemComponent, ExaminedEvent>(new ComponentEventHandler<ItemComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<ItemToggleSizeComponent, ItemToggledEvent>(new ComponentEventHandler<ItemToggleSizeComponent, ItemToggledEvent>(this.OnItemToggle));
  }

  private void OnItemAutoState(
    EntityUid uid,
    ItemComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.SetHeldPrefix(uid, component.HeldPrefix, true, component);
  }

  public void SetSize(EntityUid uid, ProtoId<ItemSizePrototype> size, ItemComponent? component = null)
  {
    if (!this.Resolve<ItemComponent>(uid, ref component, false) || component.Size == size)
      return;
    component.Size = size;
    this.Dirty(uid, (IComponent) component);
    ItemSizeChangedEvent args = new ItemSizeChangedEvent(uid);
    this.RaiseLocalEvent<ItemSizeChangedEvent>(uid, ref args, true);
  }

  public void SetShape(EntityUid uid, List<Box2i>? shape, ItemComponent? component = null)
  {
    if (!this.Resolve<ItemComponent>(uid, ref component, false) || component.Shape == shape)
      return;
    component.Shape = shape;
    this.Dirty(uid, (IComponent) component);
    ItemSizeChangedEvent args = new ItemSizeChangedEvent(uid);
    this.RaiseLocalEvent<ItemSizeChangedEvent>(uid, ref args, true);
  }

  public void SetStoredOffset(EntityUid uid, Vector2i newOffset, ItemComponent? component = null)
  {
    if (!this.Resolve<ItemComponent>(uid, ref component, false))
      return;
    component.StoredOffset = newOffset;
    this.Dirty(uid, (IComponent) component);
  }

  public void SetHeldPrefix(EntityUid uid, string? heldPrefix, bool force = false, ItemComponent? component = null)
  {
    if (!this.Resolve<ItemComponent>(uid, ref component, false) || !force && component.HeldPrefix == heldPrefix)
      return;
    component.HeldPrefix = heldPrefix;
    this.Dirty(uid, (IComponent) component);
    this.VisualsChanged(uid);
  }

  public void CopyVisuals(EntityUid uid, ItemComponent otherItem, ItemComponent? item = null)
  {
    if (!this.Resolve<ItemComponent>(uid, ref item))
      return;
    item.RsiPath = otherItem.RsiPath;
    item.InhandVisuals = otherItem.InhandVisuals;
    item.HeldPrefix = otherItem.HeldPrefix;
    this.Dirty(uid, (IComponent) item);
    this.VisualsChanged(uid);
  }

  private void OnHandInteract(EntityUid uid, ItemComponent component, InteractHandEvent args)
  {
    if (args.Handled || !this._handsSystem.TryPickup(args.User, uid))
      return;
    args.Handled = true;
    ItemPickedUpEvent args1 = new ItemPickedUpEvent(args.User, uid);
    this.RaiseLocalEvent<ItemPickedUpEvent>(uid, ref args1, true);
  }

  private void AddPickupVerb(
    EntityUid uid,
    ItemComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (args.Hands == null || args.Using.HasValue || !args.CanAccess || !args.CanInteract || !this._handsSystem.CanPickupAnyHand(args.User, args.Target, handsComp: args.Hands, item: component))
      return;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Act = (Action) (() => this._handsSystem.TryPickupAnyHand(args.User, args.Target, false, handsComp: args.Hands, item: component));
    interactionVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png"));
    BaseContainer container1;
    this.Container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (args.User, (TransformComponent) null, (MetaDataComponent) null), out container1);
    BaseContainer container2;
    if (this.Container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (args.Target, (TransformComponent) null, (MetaDataComponent) null), out container2) && container2 != container1)
      interactionVerb.Text = this.Loc.GetString("pick-up-verb-get-data-text-inventory");
    else
      interactionVerb.Text = this.Loc.GetString("pick-up-verb-get-data-text");
    args.Verbs.Add(interactionVerb);
  }

  private void OnExamine(EntityUid uid, ItemComponent component, ExaminedEvent args)
  {
    if (component.Size == (ProtoId<ItemSizePrototype>) "Invalid")
      return;
    args.PushMarkup(this.Loc.GetString("item-component-on-examine-size", ("size", (object) this.GetItemSizeLocale(component.Size))), -2);
  }

  public ItemSizePrototype GetSizePrototype(ProtoId<ItemSizePrototype> id)
  {
    return this._prototype.Index<ItemSizePrototype>(id);
  }

  public virtual void VisualsChanged(EntityUid owner)
  {
  }

  public string GetItemSizeLocale(ProtoId<ItemSizePrototype> size)
  {
    return this.Loc.GetString((string) this.GetSizePrototype(size).Name);
  }

  public int GetItemSizeWeight(ProtoId<ItemSizePrototype> size)
  {
    return this.GetSizePrototype(size).Weight;
  }

  public IReadOnlyList<Box2i> GetItemShape(
    Entity<StorageComponent?> storage,
    Entity<ItemComponent?> uid)
  {
    if (!this.Resolve<ItemComponent>((EntityUid) uid, ref uid.Comp))
      return (IReadOnlyList<Box2i>) new Box2i[0];
    FixedItemSizeStorageComponent component;
    if (!this._fixedItemSizeStorageQuery.TryComp((EntityUid) storage, out component))
      return (IReadOnlyList<Box2i>) uid.Comp.Shape ?? this.GetSizePrototype(uid.Comp.Size).DefaultShape;
    FixedItemSizeStorageComponent storageComponent = component;
    if (storageComponent.CachedSize == null)
      storageComponent.CachedSize = new Box2i[1]
      {
        Box2i.FromDimensions(Vector2i.Zero, Vector2i.op_Subtraction(component.Size, Vector2i.One))
      };
    return (IReadOnlyList<Box2i>) component.CachedSize;
  }

  public IReadOnlyList<Box2i> GetItemShape(ItemComponent component)
  {
    return (IReadOnlyList<Box2i>) component.Shape ?? this.GetSizePrototype(component.Size).DefaultShape;
  }

  public IReadOnlyList<Box2i> GetAdjustedItemShape(
    Entity<StorageComponent?> storage,
    Entity<ItemComponent?> entity,
    ItemStorageLocation location)
  {
    return this.GetAdjustedItemShape(storage, entity, location.Rotation, location.Position);
  }

  public IReadOnlyList<Box2i> GetAdjustedItemShape(
    Entity<StorageComponent?> storage,
    Entity<ItemComponent?> entity,
    Angle rotation,
    Vector2i position)
  {
    if (!this.Resolve<ItemComponent>((EntityUid) entity, ref entity.Comp))
      return (IReadOnlyList<Box2i>) new Box2i[0];
    IReadOnlyList<Box2i> itemShape = this.GetItemShape(storage, entity);
    Box2i boundingBox = itemShape.GetBoundingBox();
    Box2 box2_1 = Box2i.op_Implicit(boundingBox);
    Vector2 center = ((Box2) ref box2_1).Center;
    Matrix3x2 transform = Matrix3Helpers.CreateTransform(ref center, ref rotation);
    Vector2 vector2_1 = Vector2i.op_Implicit(boundingBox.BottomLeft);
    Matrix3x2 matrix3x2_1 = transform;
    box2_1 = Box2i.op_Implicit(boundingBox);
    ref Box2 local1 = ref box2_1;
    Vector2 bottomLeft = Matrix3Helpers.TransformBox(matrix3x2_1, ref local1).BottomLeft;
    Vector2 vector2_2 = vector2_1 - bottomLeft;
    List<Box2i> adjustedItemShape = new List<Box2i>();
    foreach (Box2i box2i1 in (IEnumerable<Box2i>) itemShape)
    {
      Matrix3x2 matrix3x2_2 = transform;
      box2_1 = Box2i.op_Implicit(box2i1);
      ref Box2 local2 = ref box2_1;
      Box2 box2_2 = Matrix3Helpers.TransformBox(matrix3x2_2, ref local2);
      Box2 box2_3 = ((Box2) ref box2_2).Translated(vector2_2);
      Box2i box2i2;
      // ISSUE: explicit constructor call
      ((Box2i) ref box2i2).\u002Ector(Vector2Helpers.Floored(box2_3.BottomLeft), Vector2Helpers.Floored(box2_3.TopRight));
      Box2i box2i3 = ((Box2i) ref box2i2).Translated(position);
      adjustedItemShape.Add(box2i3);
    }
    return (IReadOnlyList<Box2i>) adjustedItemShape;
  }

  private void OnItemToggle(
    EntityUid uid,
    ItemToggleSizeComponent itemToggleSize,
    ItemToggledEvent args)
  {
    ItemComponent comp;
    if (!this.TryComp<ItemComponent>(uid, out comp))
      return;
    if (args.Activated)
    {
      if (itemToggleSize.ActivatedShape != null)
      {
        ItemToggleSizeComponent toggleSizeComponent = itemToggleSize;
        if (toggleSizeComponent.DeactivatedShape == null)
          toggleSizeComponent.DeactivatedShape = new List<Box2i>((IEnumerable<Box2i>) this.GetItemShape(comp));
        this.Dirty(uid, (IComponent) itemToggleSize);
        this.SetShape(uid, itemToggleSize.ActivatedShape, comp);
      }
      if (!itemToggleSize.ActivatedSize.HasValue)
        return;
      ItemToggleSizeComponent toggleSizeComponent1 = itemToggleSize;
      toggleSizeComponent1.DeactivatedSize.GetValueOrDefault();
      if (!toggleSizeComponent1.DeactivatedSize.HasValue)
      {
        ProtoId<ItemSizePrototype> size = comp.Size;
        toggleSizeComponent1.DeactivatedSize = new ProtoId<ItemSizePrototype>?(size);
      }
      this.Dirty(uid, (IComponent) itemToggleSize);
      this.SetSize(uid, itemToggleSize.ActivatedSize.Value, comp);
    }
    else
    {
      if (itemToggleSize.DeactivatedShape != null)
        this.SetShape(uid, itemToggleSize.DeactivatedShape, comp);
      if (!itemToggleSize.DeactivatedSize.HasValue)
        return;
      this.SetSize(uid, itemToggleSize.DeactivatedSize.Value, comp);
    }
  }
}
