// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Projectile.Bone.XenoBoneChipsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Movement.Systems;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Projectile.Bone;

public sealed class XenoBoneChipsSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoProjectileSystem _xenoProjectile;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoBoneChipsComponent, XenoBoneChipsActionEvent>(new EntityEventRefHandler<XenoBoneChipsComponent, XenoBoneChipsActionEvent>(this.OnXenoBoneSpursAction));
    this.SubscribeLocalEvent<SlowedByBoneChipsComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<SlowedByBoneChipsComponent, RefreshMovementSpeedModifiersEvent>(this.OnSlowedBySpitRefreshMovement));
  }

  private void OnXenoBoneSpursAction(
    Entity<XenoBoneChipsComponent> xeno,
    ref XenoBoneChipsActionEvent args)
  {
    if (args.Handled)
      return;
    XenoBoneChipsActionEvent chipsActionEvent = args;
    XenoProjectileSystem xenoProjectile = this._xenoProjectile;
    EntityUid xeno1 = (EntityUid) xeno;
    EntityCoordinates target1 = args.Target;
    FixedPoint2 zero1 = FixedPoint2.Zero;
    EntProtoId projectileId = xeno.Comp.ProjectileId;
    Angle zero2 = Angle.Zero;
    double speed = (double) xeno.Comp.Speed;
    EntityUid? entity = args.Entity;
    float? stopAtDistance = new float?();
    EntityUid? target2 = entity;
    int? projectileHitLimit = new int?();
    int num = xenoProjectile.TryShoot(xeno1, target1, zero1, projectileId, (SoundSpecifier) null, 1, zero2, (float) speed, stopAtDistance, target2, projectileHitLimit: projectileHitLimit) ? 1 : 0;
    chipsActionEvent.Handled = num != 0;
  }

  private void OnSlowedBySpitRefreshMovement(
    Entity<SlowedByBoneChipsComponent> slowed,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!(slowed.Comp.ExpiresAt > this._timing.CurTime))
      return;
    args.ModifySpeed(slowed.Comp.Multiplier, slowed.Comp.Multiplier);
  }
}
