// Decompiled with JetBrains decompiler
// Type: Content.Shared.Foldable.DeployFoldableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.EntitySystems;
using Content.Shared.DragDrop;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;

#nullable enable
namespace Content.Shared.Foldable;

public sealed class DeployFoldableSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private FoldableSystem _foldable;
  [Dependency]
  private AnchorableSystem _anchorable;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private TagSystem _tag;
  private static readonly ProtoId<TagPrototype> CatwalkTag = (ProtoId<TagPrototype>) "Catwalk";
  private static readonly ProtoId<TagPrototype> StairsTag = (ProtoId<TagPrototype>) "RMCStairs";
  private static readonly ProtoId<TagPrototype> CarpetTag = (ProtoId<TagPrototype>) "Carpet";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DeployFoldableComponent, AfterInteractEvent>(new EntityEventRefHandler<DeployFoldableComponent, AfterInteractEvent>(this.OnAfterInteract));
    this.SubscribeLocalEvent<DeployFoldableComponent, CanDragEvent>(new EntityEventRefHandler<DeployFoldableComponent, CanDragEvent>(this.OnCanDrag));
    this.SubscribeLocalEvent<DeployFoldableComponent, DragDropDraggedEvent>(new EntityEventRefHandler<DeployFoldableComponent, DragDropDraggedEvent>(this.OnDragDropDragged));
    this.SubscribeLocalEvent<DeployFoldableComponent, CanDropDraggedEvent>(new EntityEventRefHandler<DeployFoldableComponent, CanDropDraggedEvent>(this.OnCanDropDragged));
  }

  private void OnCanDropDragged(Entity<DeployFoldableComponent> ent, ref CanDropDraggedEvent args)
  {
    if (args.User != args.Target)
      return;
    args.Handled = true;
    args.CanDrop = true;
  }

  private void OnDragDropDragged(Entity<DeployFoldableComponent> ent, ref DragDropDraggedEvent args)
  {
    FoldableComponent comp;
    if (args.User != args.Target || !this.TryComp<FoldableComponent>((EntityUid) ent, out comp) || !this._foldable.TrySetFolded((EntityUid) ent, comp, true))
      return;
    this._hands.PickupOrDrop(new EntityUid?(args.User), ent.Owner);
    args.Handled = true;
  }

  private void OnCanDrag(Entity<DeployFoldableComponent> ent, ref CanDragEvent args)
  {
    FoldableComponent comp;
    if (!this.TryComp<FoldableComponent>((EntityUid) ent, out comp) || comp.IsFolded)
      return;
    args.Handled = true;
  }

  private void OnAfterInteract(Entity<DeployFoldableComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled || !args.CanReach)
      return;
    EntityUid? nullable = args.Target;
    if (nullable.HasValue)
    {
      TagSystem tag = this._tag;
      nullable = args.Target;
      EntityUid entityUid = nullable.Value;
      ProtoId<TagPrototype>[] protoIdArray = new ProtoId<TagPrototype>[3]
      {
        DeployFoldableSystem.CatwalkTag,
        DeployFoldableSystem.StairsTag,
        DeployFoldableSystem.CarpetTag
      };
      if (!tag.HasAnyTag(entityUid, protoIdArray))
        return;
    }
    FoldableComponent comp1;
    if (!this.TryComp<FoldableComponent>((EntityUid) ent, out comp1))
      return;
    PhysicsComponent comp2;
    if (this.TryComp<PhysicsComponent>(ent.Owner, out comp2))
    {
      AnchorableSystem anchorable = this._anchorable;
      EntityCoordinates clickLocation = args.ClickLocation;
      PhysicsComponent anchorBody = comp2;
      nullable = new EntityUid?();
      EntityUid? anchoringEntity = nullable;
      if (anchorable.TileFree(clickLocation, anchorBody, anchoringEntity))
      {
        HandsComponent comp3;
        if (!this.TryComp<HandsComponent>(args.User, out comp3) || !this._hands.TryDrop((Entity<HandsComponent>) (args.User, comp3), args.Used, new EntityCoordinates?(args.ClickLocation)))
          return;
        if (!this._foldable.TrySetFolded((EntityUid) ent, comp1, false))
        {
          this._hands.TryPickup(args.User, args.Used, handsComp: comp3);
          return;
        }
        args.Handled = true;
        return;
      }
    }
    this._popup.PopupPredicted(this.Loc.GetString("foldable-deploy-fail", ("object", (object) ent)), (EntityUid) ent, new EntityUid?(args.User));
  }
}
