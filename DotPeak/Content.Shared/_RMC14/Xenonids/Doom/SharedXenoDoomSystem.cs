// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Doom.SharedXenoDoomSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.BlurredVision;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared.Actions;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Light.Components;
using Content.Shared.Mobs.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Doom;

public abstract class SharedXenoDoomSystem : EntitySystem
{
  [Dependency]
  private SharedPointLightSystem _pointLight;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedRMCActionsSystem _rmcAction;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  protected EntityLookupSystem _entityLookup;
  [Dependency]
  protected SharedTransformSystem _transform;
  [Dependency]
  private ExamineSystemShared _examine;
  [Dependency]
  private RMCDazedSystem _daze;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCCameraShakeSystem _cameraShake;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoDoomComponent, XenoDoomActionEvent>(new EntityEventRefHandler<XenoDoomComponent, XenoDoomActionEvent>(this.OnXenoDoomAction));
    this.SubscribeLocalEvent<LightDoomedComponent, ComponentStartup>(new EntityEventRefHandler<LightDoomedComponent, ComponentStartup>(this.OnDoomedLightAdded));
    this.SubscribeLocalEvent<LightDoomedComponent, ComponentShutdown>(new EntityEventRefHandler<LightDoomedComponent, ComponentShutdown>(this.OnDoomedLightRemoved));
    this.SubscribeLocalEvent<LightDoomedComponent, AttemptPointLightToggleEvent>(new EntityEventRefHandler<LightDoomedComponent, AttemptPointLightToggleEvent>(this.OnDoomedLightAttemptToggle));
    this.SubscribeLocalEvent<LightDoomedComponent, PointLightToggleEvent>(new EntityEventRefHandler<LightDoomedComponent, PointLightToggleEvent>(this.OnDoomedLightToggle));
    this.SubscribeLocalEvent<LightDoomedComponent, ItemToggleActivateAttemptEvent>(new EntityEventRefHandler<LightDoomedComponent, ItemToggleActivateAttemptEvent>(this.OnDoomedLightItemToggle));
    this.SubscribeLocalEvent<MobDoomedComponent, ComponentStartup>(new EntityEventRefHandler<MobDoomedComponent, ComponentStartup>(this.OnDoomedAdded));
    this.SubscribeLocalEvent<MobDoomedComponent, ComponentShutdown>(new EntityEventRefHandler<MobDoomedComponent, ComponentShutdown>(this.OnDoomedRemoved));
  }

  protected virtual void OnDoomedAdded(Entity<MobDoomedComponent> ent, ref ComponentStartup args)
  {
  }

  protected virtual void OnDoomedRemoved(Entity<MobDoomedComponent> ent, ref ComponentShutdown args)
  {
  }

  protected virtual void OnDoomedLightAdded(
    Entity<LightDoomedComponent> ent,
    ref ComponentStartup args)
  {
    if (!this._pointLight.TryGetLight((EntityUid) ent, out SharedPointLightComponent _))
    {
      this.RemCompDeferred<LightDoomedComponent>((EntityUid) ent);
    }
    else
    {
      this._pointLight.SetEnabled(ent.Owner, false);
      ent.Comp.EndsAt = new TimeSpan?(this._timing.CurTime + ent.Comp.Duration);
    }
  }

  private void OnDoomedLightRemoved(Entity<LightDoomedComponent> ent, ref ComponentShutdown args)
  {
    if (!this._pointLight.TryGetLight((EntityUid) ent, out SharedPointLightComponent _))
      return;
    this._pointLight.SetEnabled(ent.Owner, ent.Comp.WasEnabled);
  }

  private void OnDoomedLightItemToggle(
    Entity<LightDoomedComponent> ent,
    ref ItemToggleActivateAttemptEvent args)
  {
    if (!this.HasComp<ItemTogglePointLightComponent>((EntityUid) ent))
      return;
    args.Popup = this.Loc.GetString("rmc-doomed-fail");
    args.Cancelled = true;
  }

  private void OnDoomedLightAttemptToggle(
    Entity<LightDoomedComponent> ent,
    ref AttemptPointLightToggleEvent args)
  {
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? endsAt = ent.Comp.EndsAt;
    if ((endsAt.HasValue ? (curTime >= endsAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    if (ent.Comp.DoomActivated)
      ent.Comp.WasEnabled = args.Enabled;
    if (!args.Enabled)
      return;
    args.Cancelled = true;
  }

  private void OnDoomedLightToggle(Entity<LightDoomedComponent> ent, ref PointLightToggleEvent args)
  {
    if (!args.Enabled)
      return;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? endsAt = ent.Comp.EndsAt;
    if ((endsAt.HasValue ? (curTime >= endsAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      return;
    ent.Comp.DoomActivated = true;
    this._pointLight.SetEnabled(ent.Owner, false);
  }

  protected virtual void OnXenoDoomAction(
    Entity<XenoDoomComponent> xeno,
    ref XenoDoomActionEvent args)
  {
    if (args.Handled || !this._rmcAction.TryUseAction((InstantActionEvent) args))
      return;
    args.Handled = true;
    this._audio.PlayPredicted(xeno.Comp.Sound, (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    this.PredictedSpawnAttachedTo((string) xeno.Comp.Effect, xeno.Owner.ToCoordinates(), rotation: new Angle());
    this.PredictedSpawnAtPosition((string) xeno.Comp.Smoke, xeno.Owner.ToCoordinates());
    this._mobs.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(this.Transform((EntityUid) xeno).Coordinates, xeno.Comp.Range, this._mobs);
    foreach (Entity<MobStateComponent> mob in this._mobs)
    {
      if (this._examine.InRangeUnOccluded((EntityUid) xeno, (EntityUid) mob) && this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) mob))
      {
        this._status.TryAddStatusEffect<RMCBlindedComponent>((EntityUid) mob, "Blinded", xeno.Comp.DazeTime, true);
        this._daze.TryDaze((EntityUid) mob, xeno.Comp.DazeTime);
        this._slow.TrySuperSlowdown((EntityUid) mob, xeno.Comp.SlowTime, ignoreDurationModifier: true);
        Entity<SolutionComponent>? entity;
        Solution solution;
        if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) mob.Owner, xeno.Comp.TargetSolution, out entity, out solution))
        {
          if (solution == null || !entity.HasValue)
            break;
          foreach (ReagentPrototype key in solution.GetReagentPrototypes(this._prototypeManager).Keys)
            this._solution.RemoveReagent(entity.Value, key.ID, xeno.Comp.RemovalPerReagent);
        }
        this._cameraShake.ShakeCamera((EntityUid) mob, 6, xeno.Comp.CameraShakeStrength);
        this.EnsureComp<MobDoomedComponent>((EntityUid) mob).EndsAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.OverlayTime);
      }
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<LightDoomedComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<LightDoomedComponent>();
    EntityUid uid1;
    LightDoomedComponent comp1_1;
    TimeSpan? endsAt;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (comp1_1.EndsAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        endsAt = comp1_1.EndsAt;
        if ((endsAt.HasValue ? (timeSpan < endsAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          continue;
      }
      this.RemCompDeferred<LightDoomedComponent>(uid1);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<WaitingDoomComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<WaitingDoomComponent>();
    EntityUid uid2;
    WaitingDoomComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(curTime < comp1_2.DoomAt))
      {
        this.EnsureComp<LightDoomedComponent>(uid2, out LightDoomedComponent _);
        this.RemCompDeferred<WaitingDoomComponent>(uid2);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<MobDoomedComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<MobDoomedComponent>();
    EntityUid uid3;
    MobDoomedComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      if (comp1_3.EndsAt.HasValue)
      {
        TimeSpan timeSpan = curTime;
        endsAt = comp1_3.EndsAt;
        if ((endsAt.HasValue ? (timeSpan < endsAt.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          continue;
      }
      this.RemCompDeferred<MobDoomedComponent>(uid3);
    }
  }
}
