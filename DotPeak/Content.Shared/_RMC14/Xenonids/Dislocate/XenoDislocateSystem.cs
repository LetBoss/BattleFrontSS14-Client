// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Dislocate.XenoDislocateSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Abduct;
using Content.Shared._RMC14.Xenonids.Tail_Lash;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Standing;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Dislocate;

public sealed class XenoDislocateSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private RMCPullingSystem _rmcPulling;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCSizeStunSystem _sizeStun;
  [Dependency]
  private RMCObstacleSlammingSystem _obstacleSlamming;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoDislocateComponent, XenoDislocateActionEvent>(new EntityEventRefHandler<XenoDislocateComponent, XenoDislocateActionEvent>(this.OnDislocateAction));
  }

  private void OnDislocateAction(
    Entity<XenoDislocateComponent> xeno,
    ref XenoDislocateActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((EntityTargetActionEvent) args))
      return;
    args.Handled = true;
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    EntityUid target = args.Target;
    this._rmcPulling.TryStopAllPullsFromAndOn(target);
    bool ignoreResistances = this.HasComp<RMCSlowdownComponent>(target) || this.HasComp<RMCSuperSlowdownComponent>(target) || this.HasComp<RMCRootedComponent>(target) || this.HasComp<StunnedComponent>(target) || this._standing.IsDown(target);
    FixedPoint2? total = this._damageable.TryChangeDamage(new EntityUid?(target), xeno.Comp.Damage, ignoreResistances, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
    FixedPoint2 zero = FixedPoint2.Zero;
    if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
    {
      Filter filter1 = Filter.Pvs(target, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
      SharedColorFlashEffectSystem colorFlash = this._colorFlash;
      Color red = Color.Red;
      List<EntityUid> entities = new List<EntityUid>();
      entities.Add(target);
      Filter filter2 = filter1;
      colorFlash.RaiseEffect(red, entities, filter2);
    }
    if (ignoreResistances)
      this._slow.TryRoot(target, this._xeno.TryApplyXenoDebuffMultiplier(target, xeno.Comp.RootTime));
    else if (this._net.IsServer)
    {
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
      this._rmcMelee.DoLunge((EntityUid) xeno, target);
      this._obstacleSlamming.MakeImmune(target);
      this._sizeStun.KnockBack(target, new MapCoordinates?(mapCoordinates), xeno.Comp.FlingRange, xeno.Comp.FlingRange, 10f);
      this.SpawnAttachedTo((string) xeno.Comp.Effect, target.ToCoordinates(), rotation: new Angle());
    }
    this.RefreshCooldowns(xeno);
  }

  private void RefreshCooldowns(Entity<XenoDislocateComponent> xeno)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) xeno))
    {
      BaseActionEvent baseActionEvent = this._actions.GetEvent((EntityUid) action);
      if ((baseActionEvent is XenoAbductActionEvent || baseActionEvent is XenoTailLashActionEvent) && action.Comp.Cooldown.HasValue)
      {
        TimeSpan end = action.Comp.Cooldown.Value.End - xeno.Comp.CooldownReductionTime;
        if (end < action.Comp.Cooldown.Value.Start)
          this._actions.ClearCooldown(new Entity<ActionComponent>?(action.AsNullable()));
        else
          this._actions.SetCooldown(new Entity<ActionComponent>?(action.AsNullable()), action.Comp.Cooldown.Value.Start, end);
      }
    }
  }
}
