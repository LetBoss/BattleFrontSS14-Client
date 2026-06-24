// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.ShootUseDelaySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Timing;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

public sealed class ShootUseDelaySystem : EntitySystem
{
  [Dependency]
  private UseDelaySystem _useDelay;
  private const string ShootUseDelayId = "CMShootUseDelay";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ShootUseDelayComponent, GunShotEvent>(new EntityEventRefHandler<ShootUseDelayComponent, GunShotEvent>(this.OnGunShot));
  }

  private void OnGunShot(Entity<ShootUseDelayComponent> ent, ref GunShotEvent args)
  {
    UseDelayComponent comp1;
    GunComponent comp2;
    if (!this.TryComp<UseDelayComponent>((EntityUid) ent, out comp1) || !this.TryComp<GunComponent>((EntityUid) ent, out comp2))
      return;
    this._useDelay.SetLength((Entity<UseDelayComponent>) (ent.Owner, comp1), TimeSpan.FromSeconds(1.0 / (double) comp2.FireRateModified), "CMShootUseDelay");
    this._useDelay.TryResetDelay((Entity<UseDelayComponent>) (ent.Owner, comp1), true, "CMShootUseDelay");
  }
}
