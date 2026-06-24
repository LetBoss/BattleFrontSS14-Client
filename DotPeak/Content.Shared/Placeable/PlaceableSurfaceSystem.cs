// Decompiled with JetBrains decompiler
// Type: Content.Shared.Placeable.PlaceableSurfaceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Storage;
using Content.Shared.Storage.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Numerics;

#nullable enable
namespace Content.Shared.Placeable;

public sealed class PlaceableSurfaceSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _handsSystem;
  [Dependency]
  private SharedTransformSystem _transformSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PlaceableSurfaceComponent, AfterInteractUsingEvent>(new ComponentEventHandler<PlaceableSurfaceComponent, AfterInteractUsingEvent>(this.OnAfterInteractUsing));
    this.SubscribeLocalEvent<PlaceableSurfaceComponent, StorageInteractUsingAttemptEvent>(new EntityEventRefHandler<PlaceableSurfaceComponent, StorageInteractUsingAttemptEvent>(this.OnStorageInteractUsingAttempt));
    this.SubscribeLocalEvent<PlaceableSurfaceComponent, StorageAfterOpenEvent>(new EntityEventRefHandler<PlaceableSurfaceComponent, StorageAfterOpenEvent>(this.OnStorageAfterOpen));
    this.SubscribeLocalEvent<PlaceableSurfaceComponent, StorageAfterCloseEvent>(new EntityEventRefHandler<PlaceableSurfaceComponent, StorageAfterCloseEvent>(this.OnStorageAfterClose));
  }

  public void SetPlaceable(EntityUid uid, bool isPlaceable, PlaceableSurfaceComponent? surface = null)
  {
    if (!this.Resolve<PlaceableSurfaceComponent>(uid, ref surface, false) || surface.IsPlaceable == isPlaceable)
      return;
    surface.IsPlaceable = isPlaceable;
    this.Dirty(uid, (IComponent) surface);
  }

  public void SetPlaceCentered(
    EntityUid uid,
    bool placeCentered,
    PlaceableSurfaceComponent? surface = null)
  {
    if (!this.Resolve<PlaceableSurfaceComponent>(uid, ref surface))
      return;
    surface.PlaceCentered = placeCentered;
    this.Dirty(uid, (IComponent) surface);
  }

  public void SetPositionOffset(EntityUid uid, Vector2 offset, PlaceableSurfaceComponent? surface = null)
  {
    if (!this.Resolve<PlaceableSurfaceComponent>(uid, ref surface))
      return;
    surface.PositionOffset = offset;
    this.Dirty(uid, (IComponent) surface);
  }

  private void OnAfterInteractUsing(
    EntityUid uid,
    PlaceableSurfaceComponent surface,
    AfterInteractUsingEvent args)
  {
    if (args.Handled || !args.CanReach || !surface.IsPlaceable || this.HasComp<DumpableComponent>(args.Used) || !this._handsSystem.TryDrop((Entity<HandsComponent>) args.User, args.Used))
      return;
    this._transformSystem.SetCoordinates(args.Used, surface.PlaceCentered ? this.Transform(uid).Coordinates.Offset(surface.PositionOffset) : args.ClickLocation);
    args.Handled = true;
  }

  private void OnStorageInteractUsingAttempt(
    Entity<PlaceableSurfaceComponent> ent,
    ref StorageInteractUsingAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnStorageAfterOpen(
    Entity<PlaceableSurfaceComponent> ent,
    ref StorageAfterOpenEvent args)
  {
    this.SetPlaceable(ent.Owner, true, ent.Comp);
  }

  private void OnStorageAfterClose(
    Entity<PlaceableSurfaceComponent> ent,
    ref StorageAfterCloseEvent args)
  {
    this.SetPlaceable(ent.Owner, false, ent.Comp);
  }
}
