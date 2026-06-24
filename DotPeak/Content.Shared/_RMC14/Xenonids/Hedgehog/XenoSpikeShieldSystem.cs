// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Hedgehog.XenoSpikeShieldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Hedgehog;

public sealed class XenoSpikeShieldSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoProjectileSystem _xenoProjectile;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private XenoShieldSystem _shield;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoSpikeShieldComponent, DamageModifyAfterResistEvent>(new EntityEventRefHandler<XenoSpikeShieldComponent, DamageModifyAfterResistEvent>(this.OnHedgehogShieldDamage), new Type[1]
    {
      typeof (XenoShieldSystem)
    });
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoSpikeShieldComponent, XenoShieldComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoSpikeShieldComponent, XenoShieldComponent>();
    EntityUid uid;
    XenoSpikeShieldComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out XenoShieldComponent _))
    {
      if (comp1.ShieldExpireAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        TimeSpan? shieldExpireAt = comp1.ShieldExpireAt;
        if ((shieldExpireAt.HasValue ? (timeSpan >= shieldExpireAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          this._shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Hedgehog);
      }
    }
  }

  private void OnHedgehogShieldDamage(
    Entity<XenoSpikeShieldComponent> ent,
    ref DamageModifyAfterResistEvent args)
  {
    XenoShieldComponent comp;
    if (!this.TryComp<XenoShieldComponent>((EntityUid) ent, out comp) || !comp.Active || comp.Shield != XenoShieldSystem.ShieldType.Hedgehog)
      return;
    if (this.HasComp<ProjectileComponent>(args.Tool))
    {
      XenoProjectileSystem xenoProjectile = this._xenoProjectile;
      EntityUid owner = ent.Owner;
      EntityCoordinates targetCoords = new EntityCoordinates((EntityUid) ent, Vector2.UnitX * 2.5f);
      FixedPoint2 zero = FixedPoint2.Zero;
      EntProtoId projectile = ent.Comp.Projectile;
      int projectileCount = ent.Comp.ProjectileCount;
      Angle deviation = new Angle(2.0 * Math.PI);
      int? projectileHitLimit1 = ent.Comp.ProjectileHitLimit;
      float? stopAtDistance = new float?();
      EntityUid? target = new EntityUid?();
      int? projectileHitLimit2 = projectileHitLimit1;
      xenoProjectile.TryShoot(owner, targetCoords, zero, projectile, (SoundSpecifier) null, projectileCount, deviation, 15f, stopAtDistance, target, false, projectileHitLimit2);
      this._popup.PopupPredicted(this.Loc.GetString("rmc-spike-shield-hit", ("user", (object) ent)), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    }
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }
}
