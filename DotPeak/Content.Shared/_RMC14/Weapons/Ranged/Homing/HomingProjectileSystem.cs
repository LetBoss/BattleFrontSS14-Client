// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Homing.HomingProjectileSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Homing;

public sealed class HomingProjectileSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedPhysicsSystem _physics;
  private readonly List<EntityUid> _toRemove = new List<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<HomingProjectileComponent, StartCollideEvent>(new EntityEventRefHandler<HomingProjectileComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<HomingProjectileComponent, PreventCollideEvent>(new EntityEventRefHandler<HomingProjectileComponent, PreventCollideEvent>(this.OnPreventCollide));
  }

  private void OnStartCollide(Entity<HomingProjectileComponent> ent, ref StartCollideEvent args)
  {
    if (args.OtherEntity != ent.Comp.Target)
      return;
    this.RemComp<HomingProjectileComponent>((EntityUid) ent);
  }

  private void OnPreventCollide(Entity<HomingProjectileComponent> ent, ref PreventCollideEvent args)
  {
    if (args.OtherEntity != ent.Comp.Target)
      return;
    this.RemComp<HomingProjectileComponent>((EntityUid) ent);
  }

  public override void Update(float frameTime)
  {
    this._toRemove.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<HomingProjectileComponent> entityQueryEnumerator = this.EntityQueryEnumerator<HomingProjectileComponent>();
    EntityUid uid1;
    HomingProjectileComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
    {
      PhysicsComponent comp2;
      if (this.TryComp<PhysicsComponent>(uid1, out comp2))
      {
        EntityUid target = comp1.Target;
        MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates(target, this.Transform(target));
        MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(uid1, this.Transform(uid1));
        if (mapCoordinates1.MapId != mapCoordinates2.MapId)
        {
          this._toRemove.Add(uid1);
        }
        else
        {
          Vector2 vector2_1 = mapCoordinates1.Position - mapCoordinates2.Position;
          if (this._transform.InRange(this.Transform(uid1).Coordinates, this.Transform(target).Coordinates, 1f))
          {
            this._toRemove.Add(uid1);
          }
          else
          {
            Vector2 vector2_2 = Vector2.Zero + Vector2Helpers.Normalized(vector2_1) * (float) comp1.ProjectileSpeed;
            Vector2 mapLinearVelocity = this._physics.GetMapLinearVelocity(uid1, comp2);
            Vector2 velocity = comp2.LinearVelocity + vector2_2 - mapLinearVelocity;
            this._physics.SetLinearVelocity(uid1, velocity, body: comp2);
            ProjectileComponent comp3;
            if (this.TryComp<ProjectileComponent>(uid1, out comp3))
              this._transform.SetWorldRotationNoLerp((Entity<TransformComponent>) uid1, Angle.op_Addition(DirectionExtensions.ToWorldAngle(vector2_1), comp3.Angle));
          }
        }
      }
    }
    try
    {
      foreach (EntityUid uid2 in this._toRemove)
        this.RemComp<HomingProjectileComponent>(uid2);
    }
    finally
    {
      this._toRemove.Clear();
    }
  }
}
