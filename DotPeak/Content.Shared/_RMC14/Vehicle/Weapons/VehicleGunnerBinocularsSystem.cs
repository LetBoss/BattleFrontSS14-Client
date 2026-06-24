// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleGunnerBinocularsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Scoping;
using Content.Shared.Buckle.Components;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleGunnerBinocularsSystem : EntitySystem
{
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedScopeSystem _scope;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<VehicleGunnerBinocularsGiverComponent, StrappedEvent>(new EntityEventRefHandler<VehicleGunnerBinocularsGiverComponent, StrappedEvent>(this.OnStrapped));
    this.SubscribeLocalEvent<VehicleGunnerBinocularsGiverComponent, UnstrappedEvent>(new EntityEventRefHandler<VehicleGunnerBinocularsGiverComponent, UnstrappedEvent>(this.OnUnstrapped));
  }

  private void OnStrapped(
    Entity<VehicleGunnerBinocularsGiverComponent> seat,
    ref StrappedEvent args)
  {
    EntityUid owner = args.Buckle.Owner;
    EntityUid entityUid = this.Spawn((string) seat.Comp.BinocularsPrototype, this.Transform(owner).Coordinates);
    VehicleGunnerBinocularsComponent binocularsComponent = this.EnsureComp<VehicleGunnerBinocularsComponent>(entityUid);
    binocularsComponent.Gunner = new EntityUid?(owner);
    this.Dirty(entityUid, (IComponent) binocularsComponent);
    if (this._hands.TryPickupAnyHand(owner, entityUid))
    {
      ScopeComponent comp;
      if (!this.TryComp<ScopeComponent>(entityUid, out comp))
        return;
      this._scope.StartScoping((Entity<ScopeComponent>) (entityUid, comp), owner);
    }
    else
      this.QueueDel(new EntityUid?(entityUid));
  }

  private void OnUnstrapped(
    Entity<VehicleGunnerBinocularsGiverComponent> seat,
    ref UnstrappedEvent args)
  {
    EntityUid owner = args.Buckle.Owner;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VehicleGunnerBinocularsComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VehicleGunnerBinocularsComponent>();
    EntityUid uid;
    VehicleGunnerBinocularsComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      EntityUid? gunner = comp1.Gunner;
      EntityUid entityUid = owner;
      if ((gunner.HasValue ? (gunner.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0)
        this.QueueDel(new EntityUid?(uid));
    }
  }
}
