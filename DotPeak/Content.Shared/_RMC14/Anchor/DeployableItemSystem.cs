// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Anchor.DeployableItemSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared.ActionBlocker;
using Content.Shared.Construction.EntitySystems;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DragDrop;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Foldable;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Anchor;

public sealed class DeployableItemSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private AnchorableSystem _anchorable;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private FoldableSystem _foldable;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedCMInventorySystem _cmInventory;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly HashSet<Entity<DeployableItemComponent>> _deployables = new HashSet<Entity<DeployableItemComponent>>();
  private static readonly ProtoId<TagPrototype> CatwalkTag = (ProtoId<TagPrototype>) "Catwalk";
  private static readonly ProtoId<TagPrototype> StairsTag = (ProtoId<TagPrototype>) "RMCStairs";
  private static readonly ProtoId<TagPrototype> CarpetTag = (ProtoId<TagPrototype>) "Carpet";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<DeployableItemComponent, AfterInteractEvent>(new EntityEventRefHandler<DeployableItemComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<DeployableItemComponent, InteractHandEvent>(new EntityEventRefHandler<DeployableItemComponent, InteractHandEvent>(this.OnInteractHand));
    this.SubscribeLocalEvent<DeployableItemComponent, UseInHandEvent>(new EntityEventRefHandler<DeployableItemComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<DeployableItemComponent, CanDragEvent>(new EntityEventRefHandler<DeployableItemComponent, CanDragEvent>(this.OnCanDrag));
    this.SubscribeLocalEvent<DeployableItemComponent, CanDropDraggedEvent>(new EntityEventRefHandler<DeployableItemComponent, CanDropDraggedEvent>(this.OnCanDropDragged));
    this.SubscribeLocalEvent<DeployableItemComponent, DragDropDraggedEvent>(new EntityEventRefHandler<DeployableItemComponent, DragDropDraggedEvent>(this.OnDragDropDragged));
    this.SubscribeLocalEvent<DeployableItemComponent, ExaminedEvent>(new EntityEventRefHandler<DeployableItemComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<DeployableItemComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<DeployableItemComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAlternativeVerbs));
    this.SubscribeLocalEvent<DeployFoldableComponent, UseInHandEvent>(new EntityEventRefHandler<DeployFoldableComponent, UseInHandEvent>(this.OnFoldableUseInHand));
    this.SubscribeLocalEvent<HandsComponent, CanDropTargetEvent>(new EntityEventRefHandler<HandsComponent, CanDropTargetEvent>(this.OnCanDropTarget));
  }

  private void OnCanDrag(Entity<DeployableItemComponent> ent, ref CanDragEvent args)
  {
    args.Handled = true;
  }

  private void OnCanDropDragged(Entity<DeployableItemComponent> ent, ref CanDropDraggedEvent args)
  {
    if (!this.Transform((EntityUid) ent).Anchored)
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnCanDropTarget(Entity<HandsComponent> ent, ref CanDropTargetEvent args)
  {
    if (ent.Owner != args.User || !this.CanPickup(args.Dragged, args.User))
      return;
    args.CanDrop = true;
    args.Handled = true;
  }

  private void OnDragDropDragged(Entity<DeployableItemComponent> ent, ref DragDropDraggedEvent args)
  {
    if (args.User != args.Target || !this.CanPickup((EntityUid) ent, args.User))
      return;
    args.Handled = true;
    this.Pickup(ent, args.User);
  }

  private void OnExamined(Entity<DeployableItemComponent> ent, ref ExaminedEvent args)
  {
    (int Filled, int Total) = this._cmInventory.GetItemSlotsFilled((Entity<ItemSlotsComponent>) ent.Owner);
    using (args.PushGroup("DeployableItemComponent"))
    {
      if (ent.Comp.Position != DeployableItemPosition.None)
      {
        args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-deployed-click"));
        args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-deployed-drag"));
        if (!ent.Comp.MagazineExamine)
          return;
        args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-magazines", ("filled", (object) Filled), ("total", (object) Total)));
      }
      else
      {
        args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-not-deployed"));
        if (!ent.Comp.MagazineExamine)
          return;
        if (Filled == 0)
          args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-empty"));
        else if ((FixedPoint2) Filled < (FixedPoint2) Total * ent.Comp.AlmostEmptyThreshold)
          args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-almost-empty"));
        else if ((FixedPoint2) Filled < (FixedPoint2) Total * ent.Comp.HalfFullThreshold)
          args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-half-full"));
        else
          args.PushMarkup(this.Loc.GetString("cm-magazine-box-examine-almost-full"));
      }
    }
  }

  private void OnGetAlternativeVerbs(
    Entity<DeployableItemComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || ent.Comp.Position == DeployableItemPosition.None)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("cm-magazine-box-pick-up");
    alternativeVerb.Act = (Action) (() => this.Pickup(ent, user));
    alternativeVerb.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png"));
    verbs.Add(alternativeVerb);
  }

  private void OnAfterInteract(Entity<DeployableItemComponent> ent, ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (target.HasValue)
    {
      TagSystem tag = this._tag;
      target = args.Target;
      EntityUid entityUid = target.Value;
      ProtoId<TagPrototype>[] protoIdArray = new ProtoId<TagPrototype>[3]
      {
        DeployableItemSystem.CatwalkTag,
        DeployableItemSystem.StairsTag,
        DeployableItemSystem.CarpetTag
      };
      if (!tag.HasAnyTag(entityUid, protoIdArray))
        return;
    }
    args.Handled = true;
    this.Deploy(ent, args.User, args.ClickLocation);
  }

  private void OnInteractHand(Entity<DeployableItemComponent> ent, ref InteractHandEvent args)
  {
    if (args.Handled || !ent.Comp.LeftClickPickup || ent.Comp.Position == DeployableItemPosition.None)
      return;
    args.Handled = true;
    this.Pickup(ent, args.User);
  }

  private void OnUseInHand(Entity<DeployableItemComponent> ent, ref UseInHandEvent args)
  {
    args.Handled = true;
    this.Deploy(ent, args.User, this._transform.GetMoverCoordinates((EntityUid) ent));
  }

  private void OnFoldableUseInHand(Entity<DeployFoldableComponent> ent, ref UseInHandEvent args)
  {
    FoldableComponent comp1;
    if (args.Handled || !this.TryComp<FoldableComponent>((EntityUid) ent, out comp1))
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(args.User);
    PhysicsComponent comp2;
    if (!this.TryComp<PhysicsComponent>(ent.Owner, out comp2) || !this._anchorable.TileFree(moverCoordinates, comp2))
    {
      this._popup.PopupPredicted(this.Loc.GetString("foldable-deploy-fail", ("object", (object) ent)), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      HandsComponent comp3;
      if (!this.TryComp<HandsComponent>(args.User, out comp3) || !this._hands.TryDrop((Entity<HandsComponent>) (args.User, comp3), (EntityUid) ent, new EntityCoordinates?(moverCoordinates)))
        return;
      if (!this._foldable.TrySetFolded((EntityUid) ent, comp1, false))
        this._hands.TryPickup(args.User, (EntityUid) ent, handsComp: comp3);
      else
        args.Handled = true;
    }
  }

  private void Deploy(
    Entity<DeployableItemComponent> ent,
    EntityUid user,
    EntityCoordinates location)
  {
    location = this._transform.GetMoverCoordinates(location).SnapToGrid();
    TransformComponent comp = this.Transform((EntityUid) ent);
    Entity<TransformComponent> entity = new Entity<TransformComponent>((EntityUid) ent, comp);
    if (!this._transform.GetGrid(entity).HasValue)
      return;
    MapId mapId = this._transform.GetMapId(entity);
    Vector2 position1 = this._transform.ToMapCoordinates(location).Position;
    this._deployables.Clear();
    this._entityLookup.GetEntitiesInRange<DeployableItemComponent>(mapId, position1, 0.3f, this._deployables);
    bool flag1 = false;
    bool flag2 = false;
    foreach (Entity<DeployableItemComponent> deployable in this._deployables)
    {
      if (!(deployable.Owner == ent.Owner))
      {
        switch (deployable.Comp.Position)
        {
          case DeployableItemPosition.Lower:
            flag1 = true;
            break;
          case DeployableItemPosition.Upper:
            flag2 = true;
            break;
        }
        if (flag1 & flag2)
        {
          this._popup.PopupClient(this.Loc.GetString("cm-magazine-box-no-space"), new EntityUid?(user), PopupType.SmallCaution);
          return;
        }
      }
    }
    DeployableItemPosition deployableItemPosition;
    Vector2 position2;
    if (!flag1)
    {
      deployableItemPosition = DeployableItemPosition.Lower;
      position2 = new Vector2(0.0f, -0.25f);
    }
    else
    {
      if (flag2)
        return;
      deployableItemPosition = DeployableItemPosition.Upper;
      position2 = new Vector2(0.0f, 0.25f);
    }
    location = location.Offset(position2);
    if (!this._hands.TryDrop((Entity<HandsComponent>) user, (EntityUid) ent, new EntityCoordinates?(location)))
      return;
    this._transform.SetCoordinates((EntityUid) ent, location);
    this._physics.SetBodyType((EntityUid) ent, BodyType.Static);
    ent.Comp.Position = deployableItemPosition;
    this._appearance.SetData((EntityUid) ent, (Enum) DeployableItemVisuals.Deployed, (object) true);
    this.Dirty<DeployableItemComponent>(ent);
  }

  private bool CanPickup(EntityUid deployable, EntityUid user)
  {
    DeployableItemComponent comp;
    return !this.TerminatingOrDeleted(deployable) && this._hands.TryGetEmptyHand((Entity<HandsComponent>) user, out string _) && this._actionBlocker.CanPickup(user, deployable) && this.TryComp<DeployableItemComponent>(deployable, out comp) && comp.Position != 0;
  }

  private void Pickup(Entity<DeployableItemComponent> ent, EntityUid user)
  {
    if (!this.CanPickup((EntityUid) ent, user))
      return;
    this._physics.SetBodyType((EntityUid) ent, BodyType.Dynamic);
    if (!this._hands.TryPickupAnyHand(user, (EntityUid) ent))
    {
      this._physics.SetBodyType((EntityUid) ent, BodyType.Static);
    }
    else
    {
      ent.Comp.Position = DeployableItemPosition.None;
      this._appearance.SetData((EntityUid) ent, (Enum) DeployableItemVisuals.Deployed, (object) false);
      this.Dirty<DeployableItemComponent>(ent);
    }
  }
}
