// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Projectiles.StoppingPower.RMCStoppingPowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Projectiles.Aimed;
using Content.Shared._RMC14.Stamina;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;
using Content.Shared._RMC14.Xenonids.Fortify;
using Content.Shared.Camera;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Projectiles.StoppingPower;

public sealed class RMCStoppingPowerSystem : EntitySystem
{
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private RMCStaminaSystem _stamina;
  [Dependency]
  private SharedCameraRecoilSystem _cameraRecoil;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private MobStateSystem _mobState;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCStoppingPowerComponent, MapInitEvent>(new EntityEventRefHandler<RMCStoppingPowerComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<RMCStoppingPowerComponent, ProjectileHitEvent>(new EntityEventRefHandler<RMCStoppingPowerComponent, ProjectileHitEvent>(this.OnStoppingPowerHit));
    this.SubscribeLocalEvent<RMCStoppingPowerComponent, ShotByAimedShotEvent>(new EntityEventRefHandler<RMCStoppingPowerComponent, ShotByAimedShotEvent>(this.OnShotByAimedShot));
  }

  private void OnMapInit(Entity<RMCStoppingPowerComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.ShotFrom = new MapCoordinates?(this._transform.GetMapCoordinates((EntityUid) ent));
    this.Dirty<RMCStoppingPowerComponent>(ent);
  }

  private void OnShotByAimedShot(
    Entity<RMCStoppingPowerComponent> ent,
    ref ShotByAimedShotEvent args)
  {
    RMCFocusedShootingComponent comp;
    if (!this.TryComp<RMCFocusedShootingComponent>(args.Gun, out comp))
      return;
    ent.Comp.FocusedCounter = comp.FocusCounter;
  }

  private void OnStoppingPowerHit(
    Entity<RMCStoppingPowerComponent> ent,
    ref ProjectileHitEvent args)
  {
    ent.Comp.CurrentStoppingPower = 0.0f;
    this.Dirty<RMCStoppingPowerComponent>(ent);
    if (ent.Comp.RequiresAimedShot && !this.TryComp<AimedProjectileComponent>((EntityUid) ent, out AimedProjectileComponent _))
      return;
    if (ent.Comp.FocusedCounterThreshold.HasValue)
    {
      int focusedCounter = ent.Comp.FocusedCounter;
      int? counterThreshold = ent.Comp.FocusedCounterThreshold;
      int valueOrDefault = counterThreshold.GetValueOrDefault();
      if (focusedCounter < valueOrDefault & counterThreshold.HasValue)
        return;
    }
    float num = (float) Math.Min(Math.Ceiling((double) args.Damage.GetTotal().Float() / ent.Comp.StoppingPowerDivider), (double) ent.Comp.MaxStoppingPower);
    if ((double) num <= (double) ent.Comp.StoppingThreshold)
      return;
    EntityUid target = args.Target;
    RMCSizes size;
    this._sizeStun.TryGetSize(target, out size);
    ent.Comp.CurrentStoppingPower = num;
    this.Dirty<RMCStoppingPowerComponent>(ent);
    if (size >= RMCSizes.Big)
    {
      if ((double) num >= (double) ent.Comp.BigXenoScreenShakeThreshold)
        this._cameraRecoil.KickCamera(target, new Vector2(num - 3f, num - 2f));
      if ((double) num >= (double) ent.Comp.BigXenoInterruptThreshold)
      {
        AimedProjectileComponent comp;
        if (size >= RMCSizes.Immobile && (!this.TryComp<AimedProjectileComponent>((EntityUid) ent, out comp) || comp.Target != target))
          return;
        if (this.HasComp<XenoFortifyComponent>(target))
          this._transform.SetWorldRotation(target, Angle.op_Addition(this._transform.GetWorldRotation(target), Angle.FromDegrees(45.0)));
        this._stun.TryParalyze(target, ent.Comp.BigXenoStunTime, true);
        this.SendMessage(target, this.Loc.GetString("rmc-xeno-stun-interrupt-shaken"), PopupType.SmallCaution);
      }
      else
        this.SendMessage(target, this.Loc.GetString("rmc-xeno-shaken"));
    }
    else
    {
      this._cameraRecoil.KickCamera(target, new Vector2(num - 2f, num - 1f));
      if (!this.HasComp<KnockedDownComponent>(target) && !this._mobState.IsDead(target) && ent.Comp.ShotFrom.HasValue)
      {
        this._sizeStun.KnockBack(target, new MapCoordinates?(ent.Comp.ShotFrom.Value));
        this.SendMessage(target, this.Loc.GetString("rmc-xeno-knocked-back"), PopupType.SmallCaution);
      }
      else
        this.SendMessage(target, this.Loc.GetString("rmc-xeno-shaken"));
      TimeSpan time = TimeSpan.FromSeconds(((double) num - (double) ent.Comp.StoppingThreshold) * (double) ent.Comp.XenoStunMultiplier);
      if (size >= RMCSizes.VerySmallXeno)
        this._stun.TryParalyze(target, time, true);
      else if (this.HasComp<RMCStaminaComponent>(target))
        this._stamina.DoStaminaDamage((Entity<RMCStaminaComponent>) target, (double) args.Damage.GetTotal().Float());
      else
        this._stun.TryParalyze(target, time, true);
    }
  }

  private void SendMessage(EntityUid target, string message, PopupType popupType = PopupType.Small)
  {
    this._popup.PopupEntity(message, target, target, popupType);
  }
}
