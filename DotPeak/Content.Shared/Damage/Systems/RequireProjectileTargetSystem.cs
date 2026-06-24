// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.Components.RequireProjectileTargetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Projectiles;
using Content.Shared.Standing;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using System;

#nullable enable
namespace Content.Shared.Damage.Components;

public sealed class RequireProjectileTargetSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RequireProjectileTargetComponent, PreventCollideEvent>(new EntityEventRefHandler<RequireProjectileTargetComponent, PreventCollideEvent>((object) this, __methodptr(PreventCollide)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RequireProjectileTargetComponent, StoodEvent>(new EntityEventRefHandler<RequireProjectileTargetComponent, StoodEvent>((object) this, __methodptr(StandingBulletHit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RequireProjectileTargetComponent, DownedEvent>(new EntityEventRefHandler<RequireProjectileTargetComponent, DownedEvent>((object) this, __methodptr(LayingBulletPass)), (Type[]) null, (Type[]) null);
  }

  private void PreventCollide(
    Entity<RequireProjectileTargetComponent> ent,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled || !ent.Comp.Active)
      return;
    EntityUid otherEntity = args.OtherEntity;
    ProjectileComponent projectileComponent;
    if (!this.TryComp<ProjectileComponent>(otherEntity, ref projectileComponent))
      return;
    EntityUid? target = this.CompOrNull<TargetedProjectileComponent>(otherEntity)?.Target;
    EntityUid entityUid = Entity<RequireProjectileTargetComponent>.op_Implicit(ent);
    if ((target.HasValue ? (EntityUid.op_Inequality(target.GetValueOrDefault(), entityUid) ? 1 : 0) : 1) == 0)
      return;
    EntityUid? shooter = projectileComponent.Shooter;
    if (!shooter.HasValue || this.TerminatingOrDeleted(shooter.Value, (MetaDataComponent) null) || this._container.IsEntityOrParentInContainer(shooter.Value, (MetaDataComponent) null, (TransformComponent) null))
      return;
    args.Cancelled = true;
  }

  private void SetActive(Entity<RequireProjectileTargetComponent> ent, bool value)
  {
    if (ent.Comp.Active == value)
      return;
    ent.Comp.Active = value;
    this.Dirty<RequireProjectileTargetComponent>(ent, (MetaDataComponent) null);
  }

  private void StandingBulletHit(Entity<RequireProjectileTargetComponent> ent, ref StoodEvent args)
  {
    this.SetActive(ent, false);
  }

  private void LayingBulletPass(Entity<RequireProjectileTargetComponent> ent, ref DownedEvent args)
  {
    this.SetActive(ent, true);
  }
}
