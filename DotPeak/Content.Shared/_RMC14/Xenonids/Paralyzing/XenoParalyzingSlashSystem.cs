// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Paralyzing.XenoParalyzingSlashSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Synth;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Jittering;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Paralyzing;

public sealed class XenoParalyzingSlashSystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private RMCDazedSystem _daze;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoParalyzingSlashComponent, XenoParalyzingSlashActionEvent>(new EntityEventRefHandler<XenoParalyzingSlashComponent, XenoParalyzingSlashActionEvent>(this.OnXenoParalyzingSlashAction));
    this.SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoActiveParalyzingSlashComponent, MeleeHitEvent>(this.OnXenoParalyzingSlashHit));
    this.SubscribeLocalEvent<XenoActiveParalyzingSlashComponent, ComponentShutdown>(new EntityEventRefHandler<XenoActiveParalyzingSlashComponent, ComponentShutdown>(this.OnXenoParalyzingSlashRemoved));
  }

  private void OnXenoParalyzingSlashAction(
    Entity<XenoParalyzingSlashComponent> xeno,
    ref XenoParalyzingSlashActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((InstantActionEvent) args))
      return;
    args.Handled = true;
    XenoActiveParalyzingSlashComponent paralyzingSlashComponent = this.EnsureComp<XenoActiveParalyzingSlashComponent>((EntityUid) xeno);
    paralyzingSlashComponent.ExpireAt = this._timing.CurTime + xeno.Comp.ActiveDuration;
    paralyzingSlashComponent.ParalyzeDelay = xeno.Comp.StunDelay;
    paralyzingSlashComponent.ParalyzeDuration = xeno.Comp.StunDuration;
    paralyzingSlashComponent.DazeTime = xeno.Comp.DazeTime;
    this.Dirty((EntityUid) xeno, (IComponent) paralyzingSlashComponent);
    this._popup.PopupClient(this.Loc.GetString("cm-xeno-paralyzing-slash-activate"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoParalyzingSlashActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), true);
  }

  private void OnXenoParalyzingSlashRemoved(
    Entity<XenoActiveParalyzingSlashComponent> xeno,
    ref ComponentShutdown args)
  {
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoParalyzingSlashActionEvent>((EntityUid) xeno))
      this._actions.SetToggled(new Entity<ActionComponent>?(entity.AsNullable()), false);
  }

  private void OnXenoParalyzingSlashHit(
    Entity<XenoActiveParalyzingSlashComponent> xeno,
    ref MeleeHitEvent args)
  {
    if (!args.IsHit || args.HitEntities.Count == 0)
      return;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity) && !this.HasComp<VictimBeingParalyzedComponent>(hitEntity) && !this.HasComp<XenoComponent>(hitEntity))
      {
        if (this.HasComp<SynthComponent>(hitEntity))
        {
          this._popup.PopupEntity(this.Loc.GetString("cm-xeno-paralyzing-slash-immune", ("target", (object) hitEntity)), hitEntity, hitEntity, PopupType.SmallCaution);
        }
        else
        {
          this._daze.TryDaze(hitEntity, xeno.Comp.DazeTime, true, stutter: true);
          this._jitter.DoJitter(hitEntity, xeno.Comp.ParalyzeDelay, true);
          if (!this.HasComp<XenoComponent>(hitEntity))
          {
            VictimBeingParalyzedComponent paralyzedComponent = this.EnsureComp<VictimBeingParalyzedComponent>(hitEntity);
            paralyzedComponent.ParalyzeAt = this._timing.CurTime + xeno.Comp.ParalyzeDelay;
            paralyzedComponent.ParalyzeDuration = xeno.Comp.ParalyzeDuration;
            this.Dirty(hitEntity, (IComponent) paralyzedComponent);
          }
          string message = this.Loc.GetString("cm-xeno-paralyzing-slash-hit", ("target", (object) hitEntity));
          if (this._net.IsServer)
            this._popup.PopupEntity(message, hitEntity, (EntityUid) xeno);
          this.RemCompDeferred<XenoActiveParalyzingSlashComponent>((EntityUid) xeno);
          break;
        }
      }
    }
  }

  public override void Update(float frameTime)
  {
    TimeSpan curTime = this._timing.CurTime;
    if (this._net.IsServer)
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<XenoActiveParalyzingSlashComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoActiveParalyzingSlashComponent>();
      EntityUid uid;
      XenoActiveParalyzingSlashComponent comp1;
      while (entityQueryEnumerator.MoveNext(out uid, out comp1))
      {
        if (!(comp1.ExpireAt > curTime))
        {
          this.RemCompDeferred<XenoActiveParalyzingSlashComponent>(uid);
          this._popup.PopupEntity(this.Loc.GetString("cm-xeno-paralyzing-slash-expire"), uid, uid, PopupType.SmallCaution);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<VictimBeingParalyzedComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<VictimBeingParalyzedComponent>();
    EntityUid uid1;
    VictimBeingParalyzedComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(comp1_1.ParalyzeAt > curTime))
      {
        this.RemCompDeferred<VictimBeingParalyzedComponent>(uid1);
        this._stun.TryParalyze(uid1, comp1_1.ParalyzeDuration, true);
      }
    }
  }
}
