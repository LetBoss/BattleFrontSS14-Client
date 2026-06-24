// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting.RMCFocusedShootingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Targeting;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.AimedShot.FocusedShooting;

public sealed class RMCFocusedShootingSystem : EntitySystem
{
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCFocusedShootingComponent, AimedShotEvent>(new EntityEventRefHandler<RMCFocusedShootingComponent, AimedShotEvent>(this.OnAimedShot));
    this.SubscribeLocalEvent<RMCFocusedShootingComponent, TargetingStartedEvent>(new EntityEventRefHandler<RMCFocusedShootingComponent, TargetingStartedEvent>(this.OnTargetingStarted));
  }

  private void OnTargetingStarted(
    Entity<RMCFocusedShootingComponent> ent,
    ref TargetingStartedEvent args)
  {
    int num = ent.Comp.FocusCounter;
    EntityUid target = args.Target;
    EntityUid? currentTarget = ent.Comp.CurrentTarget;
    if ((currentTarget.HasValue ? (target != currentTarget.GetValueOrDefault() ? 1 : 0) : 1) != 0 || ent.Comp.FocusCounter > 2)
      num = 0;
    TargetingLaserComponent comp;
    if (this.TryComp<TargetingLaserComponent>((EntityUid) ent, out comp))
    {
      comp.CurrentLaserColor = num != 2 ? comp.LaserColor : ent.Comp.LaserColor;
      this.Dirty((EntityUid) ent, (IComponent) comp);
    }
    if (num < 2 || args.TargetedEffect != TargetedEffects.Targeted)
      return;
    args.TargetedEffect = TargetedEffects.TargetedIntense;
    if (args.DirectionEffect != DirectionTargetedEffects.DirectionTargeted)
      return;
    args.DirectionEffect = DirectionTargetedEffects.DirectionTargetedIntense;
  }

  private void OnAimedShot(Entity<RMCFocusedShootingComponent> ent, ref AimedShotEvent args)
  {
    int num = ent.Comp.FocusCounter;
    EntityUid? currentTarget = ent.Comp.CurrentTarget;
    EntityUid parentUid = this._transform.GetParentUid((EntityUid) ent);
    RMCFocusingComponent focusingComponent = this.EnsureComp<RMCFocusingComponent>(parentUid);
    EntityUid? nullable = currentTarget;
    EntityUid target = args.Target;
    if ((nullable.HasValue ? (nullable.GetValueOrDefault() == target ? 1 : 0) : 0) != 0)
    {
      if (ent.Comp.FocusCounter > 2)
        num = 0;
    }
    else
    {
      if (ent.Comp.CurrentTarget.HasValue)
        focusingComponent.OldTarget = new EntityUid?(focusingComponent.FocusTarget);
      ent.Comp.CurrentTarget = new EntityUid?(args.Target);
      num = 0;
    }
    focusingComponent.FocusTarget = args.Target;
    this.Dirty(parentUid, (IComponent) focusingComponent);
    ent.Comp.FocusCounter = Math.Min(num + 1, 3);
    this.Dirty<RMCFocusedShootingComponent>(ent);
  }
}
