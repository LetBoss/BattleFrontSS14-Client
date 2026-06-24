// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.EntitySystems.RMCSharedScatteringGrenadeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Explosion.Components;
using Content.Shared.Projectiles;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;

#nullable enable
namespace Content.Shared.Explosion.EntitySystems;

public sealed class RMCSharedScatteringGrenadeSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transformSystem;
  [Dependency]
  private IRobustRandom _random;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ScatteringGrenadeComponent, StartCollideEvent>(new EntityEventRefHandler<ScatteringGrenadeComponent, StartCollideEvent>(this.OnStartCollide));
    this.SubscribeLocalEvent<ScatteringGrenadeComponent, ProjectileHitEvent>(new EntityEventRefHandler<ScatteringGrenadeComponent, ProjectileHitEvent>(this.OnProjectileHit));
    this.SubscribeLocalEvent<ScatteringGrenadeComponent, ScatterGrenadeContentsEvent>(new EntityEventRefHandler<ScatteringGrenadeComponent, ScatterGrenadeContentsEvent>(this.OnScatterGrenadeContents));
  }

  private void OnScatterGrenadeContents(
    Entity<ScatteringGrenadeComponent> ent,
    ref ScatterGrenadeContentsEvent args)
  {
    // ISSUE: explicit reference operation
    double num1 = ((Angle) @this._transformSystem.GetMoverCoordinateRotation(ent.Owner, this.Transform(ent.Owner)).worldRot).Degrees + (double) ent.Comp.DirectionAngle;
    float num2 = ent.Comp.SpreadAngle / (float) args.TotalCount;
    double minValue = num1 - (double) ent.Comp.SpreadAngle / 2.0 + (double) num2 * (double) args.ThrownCount;
    double maxValue = num1 - (double) ent.Comp.SpreadAngle / 2.0 + (double) num2 * (double) (args.ThrownCount + 1);
    args.Angle = Angle.FromDegrees((double) this._random.Next((int) minValue, (int) maxValue));
    args.Handled = true;
  }

  private void OnStartCollide(Entity<ScatteringGrenadeComponent> ent, ref StartCollideEvent args)
  {
    if ((args.OtherFixture.CollisionLayer & 10) == 0 || !ent.Comp.TriggerOnWallCollide)
      return;
    ent.Comp.IsTriggered = true;
    this.Dirty<ScatteringGrenadeComponent>(ent);
  }

  private void OnProjectileHit(Entity<ScatteringGrenadeComponent> ent, ref ProjectileHitEvent args)
  {
    if (!ent.Comp.DirectHitTrigger)
      return;
    ent.Comp.DirectionAngle += ent.Comp.ReboundAngle;
    ent.Comp.IsTriggered = true;
    this.Dirty<ScatteringGrenadeComponent>(ent);
  }
}
