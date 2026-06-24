// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.VanguardShieldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared._RMC14.Shields;

public sealed class VanguardShieldSystem : EntitySystem
{
  [Dependency]
  private XenoShieldSystem _shield;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedPopupSystem _popup;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<VanguardShieldComponent, MapInitEvent>(new EntityEventRefHandler<VanguardShieldComponent, MapInitEvent>(this.OnVanguardShieldInit));
    this.SubscribeLocalEvent<VanguardShieldComponent, DamageModifyAfterResistEvent>(new EntityEventRefHandler<VanguardShieldComponent, DamageModifyAfterResistEvent>(this.OnVanguardShieldHit), new Type[1]
    {
      typeof (XenoShieldSystem)
    });
    this.SubscribeLocalEvent<VanguardShieldComponent, GetExplosionResistanceEvent>(new EntityEventRefHandler<VanguardShieldComponent, GetExplosionResistanceEvent>(this.OnVanguardShieldGetResistance));
    this.SubscribeLocalEvent<VanguardShieldComponent, RemovedShieldEvent>(new EntityEventRefHandler<VanguardShieldComponent, RemovedShieldEvent>(this.OnVanguardShieldRemoved));
  }

  private void OnVanguardShieldInit(Entity<VanguardShieldComponent> xeno, ref MapInitEvent args)
  {
    this.RegenShield((EntityUid) xeno);
  }

  private void OnVanguardShieldHit(
    Entity<VanguardShieldComponent> xeno,
    ref DamageModifyAfterResistEvent args)
  {
    XenoShieldComponent comp;
    if (args.Damage.GetTotal() <= 0 || !this.TryComp<XenoShieldComponent>((EntityUid) xeno, out comp) || comp.Shield != XenoShieldSystem.ShieldType.Vanguard)
      return;
    if (this._net.IsServer)
      xeno.Comp.LastTimeHit = this._timing.CurTime;
    if (xeno.Comp.WasHit || !(args.Damage.GetTotal() > (FixedPoint2) xeno.Comp.DecayThreshold))
      return;
    xeno.Comp.WasHit = true;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-shield-vanguard-hit"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
    args.Damage.ClampMax((FixedPoint2) 0);
  }

  private void OnVanguardShieldGetResistance(
    Entity<VanguardShieldComponent> xeno,
    ref GetExplosionResistanceEvent args)
  {
    XenoShieldComponent comp;
    if (!this.TryComp<XenoShieldComponent>((EntityUid) xeno, out comp) || comp.Shield != XenoShieldSystem.ShieldType.Vanguard || comp.ShieldAmount <= 0)
      return;
    float num = (float) Math.Pow(1.1, (double) xeno.Comp.ExplosionResistance / 5.0);
    args.DamageCoefficient /= num;
  }

  private void OnVanguardShieldRemoved(
    Entity<VanguardShieldComponent> xeno,
    ref RemovedShieldEvent args)
  {
    if (args.Type != XenoShieldSystem.ShieldType.Vanguard)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-shield-vanguard-break"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
    xeno.Comp.NextDecay = this._timing.CurTime;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<VanguardShieldComponent, XenoShieldComponent> entityQueryEnumerator = this.EntityQueryEnumerator<VanguardShieldComponent, XenoShieldComponent>();
    EntityUid uid;
    VanguardShieldComponent comp1;
    XenoShieldComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.LastRecharge <= comp1.LastTimeHit && comp1.LastTimeHit + comp1.RechargeTime <= curTime)
        this.RegenShield(uid);
      if (comp2.Active && comp1.WasHit && !(comp1.NextDecay > curTime))
      {
        comp1.NextDecay = curTime + comp1.DecayEvery;
        comp2.ShieldAmount = (FixedPoint2) Math.Max(0.0, (comp2.ShieldAmount * comp1.DecayMult - (FixedPoint2) comp1.DecaySub).Double());
        if (comp2.ShieldAmount <= 0)
          this._shield.RemoveShield(uid, XenoShieldSystem.ShieldType.Vanguard);
        this.Dirty(uid, (IComponent) comp2);
      }
    }
  }

  public bool ShieldBuff(EntityUid ent)
  {
    XenoShieldComponent comp1;
    VanguardShieldComponent comp2;
    return this.TryComp<XenoShieldComponent>(ent, out comp1) && (comp1.Shield == XenoShieldSystem.ShieldType.Vanguard && comp1.Active || this.TryComp<VanguardShieldComponent>(ent, out comp2) && comp2.NextDecay + comp2.BuffExtraTime > this._timing.CurTime);
  }

  public void RegenShield(EntityUid ent)
  {
    VanguardShieldComponent comp1;
    XenoShieldComponent comp2;
    if (!this.TryComp<VanguardShieldComponent>(ent, out comp1) || !this.TryComp<XenoShieldComponent>(ent, out comp2))
      return;
    comp1.LastRecharge = this._timing.CurTime;
    comp1.WasHit = false;
    if (!comp2.Active && this._net.IsServer)
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-shield-vanguard-regen"), ent, ent, PopupType.Medium);
    XenoShieldSystem shield = this._shield;
    EntityUid uid = ent;
    FixedPoint2 regenAmount = comp1.RegenAmount;
    double num = comp1.RegenAmount.Double();
    TimeSpan? duration = new TimeSpan?();
    double maxShield = num;
    shield.ApplyShield(uid, XenoShieldSystem.ShieldType.Vanguard, regenAmount, duration, maxShield: maxShield);
  }
}
