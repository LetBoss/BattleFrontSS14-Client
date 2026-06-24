// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Cruelty.XenoCrueltySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Leap;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Cruelty;

public sealed class XenoCrueltySystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private XenoSystem _xeno;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoCrueltyComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoCrueltyComponent, MeleeHitEvent>(this.OnCrueltyHit));
  }

  private void OnCrueltyHit(Entity<XenoCrueltyComponent> xeno, ref MeleeHitEvent args)
  {
    bool flag = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    foreach ((EntityUid owner, ActionComponent comp) in this._rmcActions.GetActionsWithEvent<XenoLeapActionEvent>((EntityUid) xeno))
    {
      if (comp.Cooldown.HasValue)
      {
        TimeSpan end = comp.Cooldown.Value.End - xeno.Comp.CooldownReduction;
        if (end < comp.Cooldown.Value.Start)
          this._actions.ClearCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) owner));
        else
          this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) owner), comp.Cooldown.Value.Start, end);
      }
    }
  }
}
