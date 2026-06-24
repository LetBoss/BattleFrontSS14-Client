// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Weapons.CivTargetedShotSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.Physics.Events;

#nullable enable
namespace Content.Shared._CIV14merka.Weapons;

public sealed class CivTargetedShotSystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<CivPhantomShotComponent, PreventCollideEvent>(new EntityEventRefHandler<CivPhantomShotComponent, PreventCollideEvent>(this.OnPhantomPreventCollide));
  }

  private void OnPhantomPreventCollide(
    Entity<CivPhantomShotComponent> ent,
    ref PreventCollideEvent args)
  {
    if (!(args.OtherEntity != ent.Comp.Target))
      return;
    args.Cancelled = true;
  }

  public void SetPhantomTarget(EntityUid projectile, EntityUid target)
  {
    TargetedProjectileComponent projectileComponent = this.EnsureComp<TargetedProjectileComponent>(projectile);
    projectileComponent.Target = target;
    this.Dirty(projectile, (IComponent) projectileComponent);
    CivPhantomShotComponent phantomShotComponent = this.EnsureComp<CivPhantomShotComponent>(projectile);
    phantomShotComponent.Target = target;
    this.Dirty(projectile, (IComponent) phantomShotComponent);
  }
}
