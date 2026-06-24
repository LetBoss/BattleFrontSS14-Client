// Decompiled with JetBrains decompiler
// Type: Content.Shared.Flash.DamagedByFlashingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Flash.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Flash;

public sealed class DamagedByFlashingSystem : EntitySystem
{
  [Dependency]
  private DamageableSystem _damageable;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<DamagedByFlashingComponent, FlashAttemptEvent>(new EntityEventRefHandler<DamagedByFlashingComponent, FlashAttemptEvent>(this.OnFlashAttempt));
  }

  private void OnFlashAttempt(Entity<DamagedByFlashingComponent> ent, ref FlashAttemptEvent args)
  {
    this._damageable.TryChangeDamage(new EntityUid?((EntityUid) ent), ent.Comp.FlashDamage);
  }
}
