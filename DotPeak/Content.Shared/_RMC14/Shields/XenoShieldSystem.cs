// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Shields.XenoShieldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.FixedPoint;
using Content.Shared.Projectiles;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Shields;

public sealed class XenoShieldSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  private static readonly ProtoId<DamageTypePrototype> ShieldSoundDamageType = (ProtoId<DamageTypePrototype>) "Piercing";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoShieldComponent, DamageModifyAfterResistEvent>(new EntityEventRefHandler<XenoShieldComponent, DamageModifyAfterResistEvent>(this.OnDamage));
  }

  private void OnDamage(Entity<XenoShieldComponent> ent, ref DamageModifyAfterResistEvent args)
  {
    if (!ent.Comp.Active || !args.Damage.AnyPositive())
      return;
    ent.Comp.ShieldAmount -= args.Damage.GetTotal();
    if (ent.Comp.ShieldAmount <= 0)
    {
      FixedPoint2 fixedPoint2 = ent.Comp.ShieldAmount + args.Damage.GetTotal();
      ent.Comp.ShieldAmount = (FixedPoint2) 0;
      foreach (KeyValuePair<string, FixedPoint2> keyValuePair in args.Damage.DamageDict)
      {
        if (!(fixedPoint2 == 0))
        {
          if (keyValuePair.Value > 0)
          {
            double num = Math.Min(keyValuePair.Value.Double(), fixedPoint2.Double());
            args.Damage.DamageDict[keyValuePair.Key] -= (FixedPoint2) num;
            fixedPoint2 -= (FixedPoint2) num;
          }
        }
        else
          break;
      }
      this._audio.PlayPredicted(ent.Comp.ShieldBreak, (EntityUid) ent, new EntityUid?());
      this.RemoveShield((EntityUid) ent, ent.Comp.Shield);
    }
    else
    {
      if (this.HasComp<ProjectileComponent>(args.Tool) && args.Damage.DamageDict.ContainsKey((string) XenoShieldSystem.ShieldSoundDamageType))
        this._audio.PlayPredicted(ent.Comp.ShieldImpact, (EntityUid) ent, new EntityUid?());
      args.Damage.ClampMax((FixedPoint2) 0);
      this._appearance.SetData((EntityUid) ent, (Enum) RMCShieldVisuals.Current, (object) ent.Comp.ShieldAmount);
    }
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public bool ApplyShield(
    EntityUid uid,
    XenoShieldSystem.ShieldType type,
    FixedPoint2 amount,
    TimeSpan? duration = null,
    double decay = 0.0,
    bool addShield = false,
    double maxShield = 200.0,
    string? visualState = null)
  {
    if (this.HasComp<UnshieldableComponent>(uid))
      return false;
    XenoShieldComponent xenoShieldComponent = this.EnsureComp<XenoShieldComponent>(uid);
    if (xenoShieldComponent.Active && xenoShieldComponent.Shield == type)
    {
      xenoShieldComponent.ShieldAmount = !addShield ? (FixedPoint2) Math.Max(xenoShieldComponent.ShieldAmount.Double(), amount.Double()) : (FixedPoint2) Math.Min((xenoShieldComponent.ShieldAmount + amount).Double(), maxShield);
      this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Current, (object) xenoShieldComponent.ShieldAmount);
      return true;
    }
    this.RemoveShield(uid, xenoShieldComponent.Shield);
    xenoShieldComponent.Shield = type;
    xenoShieldComponent.ShieldAmount = amount;
    xenoShieldComponent.Duration = duration;
    xenoShieldComponent.DecayPerSecond = decay;
    if (duration.HasValue)
      xenoShieldComponent.ShieldDecayAt = this._timing.CurTime + duration.Value;
    xenoShieldComponent.Active = true;
    if (visualState != null)
    {
      this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Prefix, (object) visualState);
      this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Active, (object) true);
      this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Current, (object) xenoShieldComponent.ShieldAmount);
      this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Max, (object) maxShield);
    }
    this.Dirty(uid, (IComponent) xenoShieldComponent);
    return true;
  }

  public void RemoveShield(EntityUid uid, XenoShieldSystem.ShieldType shieldType)
  {
    XenoShieldComponent comp;
    if (!this.TryComp<XenoShieldComponent>(uid, out comp) || !comp.Active || comp.Shield != shieldType)
      return;
    comp.Active = false;
    this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Active, (object) false);
    comp.ShieldAmount = (FixedPoint2) 0;
    this.Dirty(uid, (IComponent) comp);
    RemovedShieldEvent args = new RemovedShieldEvent(shieldType);
    this.RaiseLocalEvent<RemovedShieldEvent>(uid, ref args);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoShieldComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoShieldComponent>();
    EntityUid uid;
    XenoShieldComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Duration.HasValue && !(curTime < comp1.ShieldDecayAt))
      {
        comp1.ShieldAmount -= (FixedPoint2) (comp1.DecayPerSecond * (double) frameTime);
        this._appearance.SetData(uid, (Enum) RMCShieldVisuals.Current, (object) comp1.ShieldAmount);
        if (comp1.ShieldAmount <= 0)
          this.RemoveShield(uid, comp1.Shield);
        else
          this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  public enum ShieldType
  {
    Generic,
    Ravager,
    Hedgehog,
    Vanguard,
    Praetorian,
    Crusher,
    Warden,
    Gardener,
    ShieldPillar,
    King,
    CumulativeGeneric,
  }
}
