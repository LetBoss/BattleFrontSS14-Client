// Decompiled with JetBrains decompiler
// Type: Content.Shared.Placeable.ItemPlacerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

#nullable enable
namespace Content.Shared.Placeable;

public sealed class ItemPlacerSystem : EntitySystem
{
  [Dependency]
  private CollisionWakeSystem _wake;
  [Dependency]
  private PlaceableSurfaceSystem _placeableSurface;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ItemPlacerComponent, StartCollideEvent>(new ComponentEventRefHandler<ItemPlacerComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<ItemPlacerComponent, EndCollideEvent>(new ComponentEventRefHandler<ItemPlacerComponent, EndCollideEvent>(this.OnEndCollide));
  }

  private void OnStartCollide(EntityUid uid, ItemPlacerComponent comp, ref StartCollideEvent args)
  {
    if (this._whitelistSystem.IsWhitelistFail(comp.Whitelist, args.OtherEntity))
      return;
    CollisionWakeComponent comp1;
    if (this.TryComp<CollisionWakeComponent>(args.OtherEntity, out comp1))
      this._wake.SetEnabled(args.OtherEntity, false, comp1);
    int count = comp.PlacedEntities.Count;
    if (comp.MaxEntities == 0U || (long) count < (long) comp.MaxEntities)
    {
      comp.PlacedEntities.Add(args.OtherEntity);
      ItemPlacedEvent args1 = new ItemPlacedEvent(args.OtherEntity);
      this.RaiseLocalEvent<ItemPlacedEvent>(uid, ref args1);
    }
    if (comp.MaxEntities <= 0U || (long) count < (long) (comp.MaxEntities - 1U))
      return;
    this._placeableSurface.SetPlaceable(uid, false);
  }

  private void OnEndCollide(EntityUid uid, ItemPlacerComponent comp, ref EndCollideEvent args)
  {
    CollisionWakeComponent comp1;
    if (this.TryComp<CollisionWakeComponent>(args.OtherEntity, out comp1))
      this._wake.SetEnabled(args.OtherEntity, true, comp1);
    comp.PlacedEntities.Remove(args.OtherEntity);
    ItemRemovedEvent args1 = new ItemRemovedEvent(args.OtherEntity);
    this.RaiseLocalEvent<ItemRemovedEvent>(uid, ref args1);
    this._placeableSurface.SetPlaceable(uid, true);
  }
}
