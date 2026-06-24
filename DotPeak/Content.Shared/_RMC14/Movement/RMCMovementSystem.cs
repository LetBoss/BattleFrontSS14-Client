// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Movement.RMCMovementSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Climbing.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Collision.Shapes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Movement;

public sealed class RMCMovementSystem : EntitySystem
{
  [Dependency]
  private FixtureSystem _fixture;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedPopupSystem _popup;
  private HashSet<EntityUid> _intersectedEntities = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCMobCollisionComponent, MapInitEvent>(new EntityEventRefHandler<RMCMobCollisionComponent, MapInitEvent>(this.OnMobCollisionMapInit));
    this.SubscribeLocalEvent<RMCMobCollisionComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCMobCollisionComponent, MobStateChangedEvent>(this.OnMobCollisionMobStateChanged));
  }

  private void OnMobCollisionMapInit(Entity<RMCMobCollisionComponent> ent, ref MapInitEvent args)
  {
    MobCollisionComponent comp1;
    if (this.TryComp<MobCollisionComponent>((EntityUid) ent, out comp1))
    {
      comp1.FixtureId = ent.Comp.FixtureId;
      this.DirtyField<MobCollisionComponent>((EntityUid) ent, comp1, "FixtureId");
    }
    PhysicsComponent comp2;
    if (this._mobState.IsDead((EntityUid) ent) || !this.TryComp<PhysicsComponent>((EntityUid) ent, out comp2))
      return;
    this.CreateMobCollisionFixture((Entity<RMCMobCollisionComponent, PhysicsComponent>) ((EntityUid) ent, (RMCMobCollisionComponent) ent, comp2));
  }

  private void CreateMobCollisionFixture(
    Entity<RMCMobCollisionComponent, PhysicsComponent?> ent)
  {
    if (!this.Resolve<PhysicsComponent>((EntityUid) ent, ref ent.Comp2, false))
      return;
    this._fixture.TryCreateFixture((EntityUid) ent, ent.Comp1.FixtureShape, ent.Comp1.FixtureId, hard: false, collisionLayer: (int) ent.Comp1.FixtureLayer, collisionMask: (int) ent.Comp1.FixtureLayer, body: (PhysicsComponent) ent);
  }

  private void OnMobCollisionMobStateChanged(
    Entity<RMCMobCollisionComponent> ent,
    ref MobStateChangedEvent args)
  {
    switch (args.NewMobState)
    {
      case MobState.Alive:
        this.CreateMobCollisionFixture((Entity<RMCMobCollisionComponent, PhysicsComponent>) ent);
        break;
      case MobState.Dead:
        this._fixture.DestroyFixture((EntityUid) ent, ent.Comp.FixtureId);
        break;
    }
  }

  public bool CanClimbOver(
    EntityUid? user,
    EntityUid movingEntity,
    EntityUid target,
    bool includeTarget = true,
    bool popup = true)
  {
    if (!user.HasValue)
      user = new EntityUid?(movingEntity);
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(movingEntity);
    MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(target);
    Robust.Shared.Physics.Transform transform = new Robust.Shared.Physics.Transform(0.0f);
    EdgeShape shape = new EdgeShape(mapCoordinates1.Position, mapCoordinates2.Position);
    this._intersectedEntities.Clear();
    this._lookup.GetEntitiesIntersecting<EdgeShape>(this._transform.GetMapId((Entity<TransformComponent>) movingEntity), shape, transform, this._intersectedEntities);
    if (includeTarget)
      this._intersectedEntities.Add(target);
    else
      this._intersectedEntities.Remove(target);
    foreach (EntityUid intersectedEntity in this._intersectedEntities)
    {
      AttemptClimbEvent args = new AttemptClimbEvent(user.Value, movingEntity, intersectedEntity);
      this.RaiseLocalEvent<AttemptClimbEvent>(intersectedEntity, ref args);
      if (args.Cancelled)
      {
        if (popup)
          this._popup.PopupClient(this.Loc.GetString("rmc-climb-prevented-by-obstacles"), user, PopupType.MediumCaution);
        return false;
      }
    }
    return true;
  }
}
