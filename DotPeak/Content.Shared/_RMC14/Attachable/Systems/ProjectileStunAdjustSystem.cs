// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Attachable.Systems.ProjectileStunAdjustSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Components;
using Content.Shared._RMC14.Attachable.Events;
using Content.Shared._RMC14.Stun;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

#nullable enable
namespace Content.Shared._RMC14.Attachable.Systems;

public sealed class ProjectileStunAdjustSystem : EntitySystem
{
  private const string ModifierExamineColour = "yellow";

  public override void Initialize()
  {
    this.SubscribeLocalEvent<ProjectileStunAdjustComponent, AmmoShotEvent>(new EntityEventRefHandler<ProjectileStunAdjustComponent, AmmoShotEvent>(this.OnAmmoShot));
    this.SubscribeLocalEvent<GrantProjectileStunAdjustComponent, AttachableAlteredEvent>(new EntityEventRefHandler<GrantProjectileStunAdjustComponent, AttachableAlteredEvent>(this.OnAttachableAltered));
    this.SubscribeLocalEvent<GrantProjectileStunAdjustComponent, AttachableGetExamineDataEvent>(new EntityEventRefHandler<GrantProjectileStunAdjustComponent, AttachableGetExamineDataEvent>(this.OnGrantProjectileStunAdjustmentGetExamineData));
  }

  private void OnAmmoShot(Entity<ProjectileStunAdjustComponent> ent, ref AmmoShotEvent args)
  {
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      RMCStunOnHitComponent comp;
      if (this.TryComp<RMCStunOnHitComponent>(firedProjectile, out comp))
      {
        Span<RMCStunOnHit> span = CollectionsMarshal.AsSpan<RMCStunOnHit>(comp.Stuns);
        for (int index = 0; index < span.Length; ++index)
        {
          ref RMCStunOnHit local = ref span[index];
          local.StunTime *= (double) ent.Comp.StunDurationAdjustment;
          local.DazeTime *= (double) ent.Comp.DazeDurationAdjustment;
          local.MaxRange *= ent.Comp.MaxRangeAdjustment;
          local.ForceKnockBack = ent.Comp.ForceKnockBackAdjustment;
          local.KnockBackPowerMin *= ent.Comp.KnockBackPowerMinAdjustment;
          local.KnockBackPowerMax *= ent.Comp.KnockBackPowerMaxAdjustment;
          local.LosesEffectWithRange = ent.Comp.LosesEffectWithRangeAdjustment;
          local.SlowsEffectBigXenos = ent.Comp.SlowsEffectBigXenosAdjustment;
          local.SuperSlowTime *= (double) ent.Comp.SuperSlowTimeAdjustment;
          local.SlowTime *= (double) ent.Comp.SlowTimeAdjustment;
          local.StunArea += ent.Comp.StunAreaAdjustment;
        }
        this.Dirty(firedProjectile, (IComponent) comp);
      }
    }
  }

  private void OnAttachableAltered(
    Entity<GrantProjectileStunAdjustComponent> ent,
    ref AttachableAlteredEvent args)
  {
    switch (args.Alteration)
    {
      case AttachableAlteredType.Attached:
        ProjectileStunAdjustComponent stunAdjustComponent = this.EnsureComp<ProjectileStunAdjustComponent>(args.Holder);
        stunAdjustComponent.StunDurationAdjustment = ent.Comp.StunDurationAdjustment;
        stunAdjustComponent.DazeDurationAdjustment = ent.Comp.DazeDurationAdjustment;
        stunAdjustComponent.MaxRangeAdjustment = ent.Comp.MaxRangeAdjustment;
        stunAdjustComponent.ForceKnockBackAdjustment = ent.Comp.ForceKnockBackAdjustment;
        stunAdjustComponent.KnockBackPowerMinAdjustment = ent.Comp.KnockBackPowerMinAdjustment;
        stunAdjustComponent.KnockBackPowerMaxAdjustment = ent.Comp.KnockBackPowerMaxAdjustment;
        stunAdjustComponent.LosesEffectWithRangeAdjustment = ent.Comp.LosesEffectWithRangeAdjustment;
        stunAdjustComponent.SlowsEffectBigXenosAdjustment = ent.Comp.SlowsEffectBigXenosAdjustment;
        stunAdjustComponent.SuperSlowTimeAdjustment = ent.Comp.SuperSlowTimeAdjustment;
        stunAdjustComponent.SlowTimeAdjustment = ent.Comp.SlowTimeAdjustment;
        stunAdjustComponent.StunAreaAdjustment = ent.Comp.StunAreaAdjustment;
        this.Dirty(args.Holder, (IComponent) stunAdjustComponent);
        break;
      case AttachableAlteredType.Detached:
        this.RemComp<ProjectileStunAdjustComponent>(args.Holder);
        break;
    }
  }

  private void OnGrantProjectileStunAdjustmentGetExamineData(
    Entity<GrantProjectileStunAdjustComponent> attachable,
    ref AttachableGetExamineDataEvent args)
  {
    List<string> collection = new List<string>();
    float durationAdjustment = attachable.Comp.StunDurationAdjustment;
    if ((double) durationAdjustment >= 1.0099999904632568 || (double) durationAdjustment <= 0.99000000953674316)
      collection.Add(this.Loc.GetString("rmc-attachable-examine-ranged-projectile-stun-duration", ("colour", (object) "yellow"), ("sign", (double) attachable.Comp.StunDurationAdjustment > 1.0 ? (object) '+' : (object) ""), ("stunDurationMult", (object) (float) ((double) attachable.Comp.StunDurationAdjustment - 1.0))));
    if (!args.Data.ContainsKey((byte) 0))
      args.Data[(byte) 0] = (new AttachableModifierConditions?(), collection);
    else
      args.Data[(byte) 0].effectStrings.AddRange((IEnumerable<string>) collection);
  }
}
