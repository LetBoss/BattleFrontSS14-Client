// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.MeleeSlow.XenoMeleeSlowSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Slow;
using Content.Shared.Standing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.MeleeSlow;

public sealed class XenoMeleeSlowSystem : EntitySystem
{
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCSlowSystem _slow;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoMeleeSlowComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoMeleeSlowComponent, MeleeHitEvent>(this.OnHit));
  }

  private void OnHit(Entity<XenoMeleeSlowComponent> xeno, ref MeleeHitEvent args)
  {
    if (!args.IsHit || args.HitEntities.Count == 0)
      return;
    using (IEnumerator<EntityUid> enumerator = args.HitEntities.GetEnumerator())
    {
      if (!enumerator.MoveNext())
        return;
      EntityUid current = enumerator.Current;
      if (!this._xeno.CanAbilityAttackTarget((EntityUid) xeno, current) || xeno.Comp.RequiresKnockDown && !this._standing.IsDown(current))
        return;
      TimeSpan duration = xeno.Comp.HigherOnXenos ? this._xeno.TryApplyXenoDebuffMultiplier(current, xeno.Comp.SlowTime) : xeno.Comp.SlowTime;
      this._slow.TrySlowdown(current, duration, ignoreDurationModifier: true);
    }
  }
}
