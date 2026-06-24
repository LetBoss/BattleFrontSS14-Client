// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.UseDelayOnShootSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class UseDelayOnShootSystem : EntitySystem
{
  [Dependency]
  private UseDelaySystem _delay;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<UseDelayOnShootComponent, GunShotEvent>(new ComponentEventRefHandler<UseDelayOnShootComponent, GunShotEvent>(this.OnUseShoot));
  }

  private void OnUseShoot(EntityUid uid, UseDelayOnShootComponent component, ref GunShotEvent args)
  {
    UseDelayComponent comp;
    if (!this.TryComp<UseDelayComponent>(uid, out comp))
      return;
    this._delay.TryResetDelay((Entity<UseDelayComponent>) (uid, comp));
  }
}
