// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.SharedXenoConstructReinforceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared.Damage;
using Content.Shared.Explosion;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction;

public sealed class SharedXenoConstructReinforceSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoConstructReinforceComponent, DamageModifyEvent>(new EntityEventRefHandler<XenoConstructReinforceComponent, DamageModifyEvent>(this.OnReinforceDamageModify), after: new Type[1]
    {
      typeof (SharedRMCMeleeWeaponSystem)
    });
    this.SubscribeLocalEvent<XenoConstructReinforceComponent, BeforeExplodeEvent>(new EntityEventRefHandler<XenoConstructReinforceComponent, BeforeExplodeEvent>(this.OnReinforceBeforeExplode));
  }

  public void Reinforce(EntityUid uid, FixedPoint2 amount, TimeSpan duration)
  {
    XenoConstructReinforceComponent reinforceComponent = this.EnsureComp<XenoConstructReinforceComponent>(uid);
    reinforceComponent.ReinforceAmount = amount;
    reinforceComponent.Duration = duration;
  }

  private void ReduceDamage(Entity<XenoConstructReinforceComponent> ent, ref DamageSpecifier damage)
  {
    if (!damage.AnyPositive())
      return;
    damage = new DamageSpecifier(damage);
    foreach (KeyValuePair<string, FixedPoint2> keyValuePair in damage.DamageDict)
    {
      if (!(damage.DamageDict[keyValuePair.Key] <= 0))
      {
        FixedPoint2 fixedPoint2 = FixedPoint2.New(Math.Min(ent.Comp.ReinforceAmount.Double(), damage.DamageDict[keyValuePair.Key].Double()));
        damage.DamageDict[keyValuePair.Key] -= fixedPoint2;
        ent.Comp.ReinforceAmount -= fixedPoint2;
        if (ent.Comp.ReinforceAmount <= 0)
        {
          this.ReinforceRemoved(ent);
          break;
        }
      }
    }
  }

  private void OnReinforceBeforeExplode(
    Entity<XenoConstructReinforceComponent> ent,
    ref BeforeExplodeEvent args)
  {
    this.ReduceDamage(ent, ref args.Damage);
  }

  private void OnReinforceDamageModify(
    Entity<XenoConstructReinforceComponent> ent,
    ref DamageModifyEvent args)
  {
    this.ReduceDamage(ent, ref args.Damage);
  }

  private void ReinforceRemoved(Entity<XenoConstructReinforceComponent> ent)
  {
    this.RemCompDeferred<XenoConstructReinforceComponent>(ent.Owner);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoConstructReinforceComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoConstructReinforceComponent>();
    EntityUid uid;
    XenoConstructReinforceComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      XenoConstructReinforceComponent reinforceComponent = comp1;
      reinforceComponent.EndAt.GetValueOrDefault();
      if (!reinforceComponent.EndAt.HasValue)
      {
        TimeSpan timeSpan = curTime + comp1.Duration;
        reinforceComponent.EndAt = new TimeSpan?(timeSpan);
      }
      TimeSpan timeSpan1 = curTime;
      TimeSpan? endAt = comp1.EndAt;
      if ((endAt.HasValue ? (timeSpan1 < endAt.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        this.ReinforceRemoved((Entity<XenoConstructReinforceComponent>) (uid, comp1));
    }
  }
}
