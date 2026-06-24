// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedFloorOcclusionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Water;
using Content.Shared.Movement.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;

#nullable enable
namespace Content.Shared.Movement.Systems;

public abstract class SharedFloorOcclusionSystem : EntitySystem
{
  [Dependency]
  private RMCWaterSystem _rmcWater;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<FloorOccluderComponent, StartCollideEvent>(new EntityEventRefHandler<FloorOccluderComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<FloorOccluderComponent, EndCollideEvent>(new EntityEventRefHandler<FloorOccluderComponent, EndCollideEvent>(this.OnEndCollide));
  }

  private void OnStartCollide(Entity<FloorOccluderComponent> entity, ref StartCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    FloorOcclusionComponent comp;
    if (!this.TryComp<FloorOcclusionComponent>(otherEntity, out comp) || comp.Colliding.Contains(entity.Owner) || !this._rmcWater.CanCollide((Entity<RMCWaterComponent>) entity.Owner, otherEntity))
      return;
    comp.Colliding.Add(entity.Owner);
    this.Dirty(otherEntity, (IComponent) comp);
    this.SetEnabled((Entity<FloorOcclusionComponent>) (otherEntity, comp));
  }

  private void OnEndCollide(Entity<FloorOccluderComponent> entity, ref EndCollideEvent args)
  {
    EntityUid otherEntity = args.OtherEntity;
    FloorOcclusionComponent comp;
    if (!this.TryComp<FloorOcclusionComponent>(otherEntity, out comp) || !comp.Colliding.Remove(entity.Owner))
      return;
    this.Dirty(otherEntity, (IComponent) comp);
    this.SetEnabled((Entity<FloorOcclusionComponent>) (otherEntity, comp));
  }

  protected virtual void SetEnabled(Entity<FloorOcclusionComponent> entity)
  {
  }
}
