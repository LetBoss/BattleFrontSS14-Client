// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Dodge.XenoDodgeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Dodge;

public sealed class XenoDodgeSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private MovementSpeedModifierSystem _speed;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private XenoSystem _xeno;
  private readonly HashSet<Entity<MobStateComponent>> _crowd = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoDodgeComponent, XenoDodgeActionEvent>(new EntityEventRefHandler<XenoDodgeComponent, XenoDodgeActionEvent>(this.OnXenoActionDodge));
    this.SubscribeLocalEvent<XenoActiveDodgeComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoActiveDodgeComponent, RefreshMovementSpeedModifiersEvent>(this.OnActiveDodgeRefresh));
    this.SubscribeLocalEvent<XenoActiveDodgeComponent, ComponentRemove>(new EntityEventRefHandler<XenoActiveDodgeComponent, ComponentRemove>(this.OnActiveDodgeRemove));
    this.SubscribeLocalEvent<XenoActiveDodgeComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<XenoActiveDodgeComponent, AttemptMobCollideEvent>(this.OnActiveDodgeAttemptMobCollide));
    this.SubscribeLocalEvent<XenoActiveDodgeComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<XenoActiveDodgeComponent, AttemptMobTargetCollideEvent>(this.OnActiveDodgeAttemptMobTargetCollide));
  }

  private void OnXenoActionDodge(Entity<XenoDodgeComponent> xeno, ref XenoDodgeActionEvent args)
  {
    if (args.Handled || !this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, (FixedPoint2) xeno.Comp.PlasmaCost))
      return;
    args.Handled = true;
    if (this._net.IsClient)
      return;
    this.EnsureComp<XenoActiveDodgeComponent>((EntityUid) xeno).ExpiresAt = this._timing.CurTime + xeno.Comp.Duration;
    this._speed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-dodge-self"), (EntityUid) xeno, (EntityUid) xeno, PopupType.Medium);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoDodgeActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), true);
  }

  private void OnActiveDodgeRefresh(
    Entity<XenoActiveDodgeComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    float num = ((FixedPoint2) 1.0 + xeno.Comp.SpeedMult + (xeno.Comp.InCrowd ? xeno.Comp.CrowdSpeedAddMult : (FixedPoint2) 0)).Float();
    args.ModifySpeed(num, num);
  }

  private void OnActiveDodgeRemove(Entity<XenoActiveDodgeComponent> xeno, ref ComponentRemove args)
  {
    if (this.TerminatingOrDeleted((EntityUid) xeno))
      return;
    this._speed.RefreshMovementSpeedModifiers((EntityUid) xeno);
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoDodgeActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), false);
  }

  private void OnActiveDodgeAttemptMobCollide(
    Entity<XenoActiveDodgeComponent> ent,
    ref AttemptMobCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnActiveDodgeAttemptMobTargetCollide(
    Entity<XenoActiveDodgeComponent> ent,
    ref AttemptMobTargetCollideEvent args)
  {
    args.Cancelled = true;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoActiveDodgeComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoActiveDodgeComponent>();
    EntityUid uid;
    XenoActiveDodgeComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.ExpiresAt < curTime)
      {
        this.RemCompDeferred<XenoActiveDodgeComponent>(uid);
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-dodge-end"), uid, uid, PopupType.MediumCaution);
      }
      else
      {
        this._crowd.Clear();
        this._lookup.GetEntitiesInRange<MobStateComponent>(this.Transform(uid).Coordinates, comp1.CrowdRange, this._crowd);
        bool flag = false;
        foreach (Entity<MobStateComponent> entity in this._crowd)
        {
          if (this._xeno.CanAbilityAttackTarget(uid, (EntityUid) entity) && !this._standing.IsDown((EntityUid) entity))
          {
            flag = true;
            break;
          }
        }
        if (flag != comp1.InCrowd)
        {
          comp1.InCrowd = flag;
          this._speed.RefreshMovementSpeedModifiers(uid);
        }
      }
    }
  }
}
