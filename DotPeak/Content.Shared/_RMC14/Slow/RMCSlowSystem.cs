// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Slow.RMCSlowSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Movement;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Movement.Systems;
using Content.Shared.Rejuvenate;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Slow;

public sealed class RMCSlowSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private MovementSpeedModifierSystem _speed;
  [Dependency]
  private StandingStateSystem _standing;
  [Dependency]
  private TemporarySpeedModifiersSystem _temporarySpeed;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RMCSlowdownComponent, ComponentStartup>(new EntityEventRefHandler<RMCSlowdownComponent, ComponentStartup>(this.OnAdded<RMCSlowdownComponent>));
    this.SubscribeLocalEvent<RMCSuperSlowdownComponent, ComponentStartup>(new EntityEventRefHandler<RMCSuperSlowdownComponent, ComponentStartup>(this.OnAdded<RMCSuperSlowdownComponent>));
    this.SubscribeLocalEvent<RMCRootedComponent, ComponentStartup>(new EntityEventRefHandler<RMCRootedComponent, ComponentStartup>(this.OnAdded<RMCRootedComponent>));
    this.SubscribeLocalEvent<RMCSlowdownComponent, ComponentShutdown>(new EntityEventRefHandler<RMCSlowdownComponent, ComponentShutdown>(this.OnExpire<RMCSlowdownComponent>));
    this.SubscribeLocalEvent<RMCSuperSlowdownComponent, ComponentShutdown>(new EntityEventRefHandler<RMCSuperSlowdownComponent, ComponentShutdown>(this.OnExpire<RMCSuperSlowdownComponent>));
    this.SubscribeLocalEvent<RMCRootedComponent, ComponentShutdown>(new EntityEventRefHandler<RMCRootedComponent, ComponentShutdown>(this.OnExpire<RMCRootedComponent>));
    this.SubscribeLocalEvent<RMCSlowdownComponent, ComponentRemove>(new EntityEventRefHandler<RMCSlowdownComponent, ComponentRemove>(this.OnRemove<RMCSlowdownComponent>));
    this.SubscribeLocalEvent<RMCSuperSlowdownComponent, ComponentRemove>(new EntityEventRefHandler<RMCSuperSlowdownComponent, ComponentRemove>(this.OnRemove<RMCSuperSlowdownComponent>));
    this.SubscribeLocalEvent<RMCRootedComponent, ComponentRemove>(new EntityEventRefHandler<RMCRootedComponent, ComponentRemove>(this.OnRemove<RMCRootedComponent>));
    this.SubscribeLocalEvent<RMCSlowdownComponent, RejuvenateEvent>(new EntityEventRefHandler<RMCSlowdownComponent, RejuvenateEvent>(this.OnRejuvenate<RMCSlowdownComponent>));
    this.SubscribeLocalEvent<RMCSuperSlowdownComponent, RejuvenateEvent>(new EntityEventRefHandler<RMCSuperSlowdownComponent, RejuvenateEvent>(this.OnRejuvenate<RMCSuperSlowdownComponent>));
    this.SubscribeLocalEvent<RMCRootedComponent, RejuvenateEvent>(new EntityEventRefHandler<RMCRootedComponent, RejuvenateEvent>(this.OnRejuvenate<RMCRootedComponent>));
    this.SubscribeLocalEvent<RMCSlowdownComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<RMCSlowdownComponent, RefreshMovementSpeedModifiersEvent>(this.OnSlowdownRefresh));
    this.SubscribeLocalEvent<RMCSuperSlowdownComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<RMCSuperSlowdownComponent, RefreshMovementSpeedModifiersEvent>(this.OnSuperSlowdownRefresh));
    this.SubscribeLocalEvent<RMCRootedComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<RMCRootedComponent, RefreshMovementSpeedModifiersEvent>(this.OnRootRefresh));
    this.SubscribeLocalEvent<RMCSpeciesSlowdownModifierComponent, StunnedEvent>(new EntityEventRefHandler<RMCSpeciesSlowdownModifierComponent, StunnedEvent>(this.OnModifierStun));
    this.SubscribeLocalEvent<RMCSpeciesSlowdownModifierComponent, KnockedDownEvent>(new EntityEventRefHandler<RMCSpeciesSlowdownModifierComponent, KnockedDownEvent>(this.OnModifierKnockdown));
    this.SubscribeLocalEvent<RMCSpeciesSlowdownModifierComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<RMCSpeciesSlowdownModifierComponent, StatusEffectEndedEvent>(this.OnModifierEffectEnd));
  }

  public bool TrySlowdown(
    EntityUid ent,
    TimeSpan duration,
    bool refresh = true,
    bool ignoreDurationModifier = false)
  {
    RMCSpeciesSlowdownModifierComponent comp;
    if (!this.TryComp<RMCSpeciesSlowdownModifierComponent>(ent, out comp))
      return false;
    TimeSpan timeSpan = this._timing.CurTime + duration * (ignoreDurationModifier ? 1.0 : (double) comp.DurationMultiplier);
    RMCSlowdownComponent slowdownComponent = this.EnsureComp<RMCSlowdownComponent>(ent);
    if (refresh && timeSpan > slowdownComponent.ExpiresAt)
      slowdownComponent.ExpiresAt = timeSpan;
    else if (!refresh)
      slowdownComponent.ExpiresAt += duration;
    return true;
  }

  public bool TrySuperSlowdown(
    EntityUid ent,
    TimeSpan duration,
    bool refresh = true,
    bool ignoreDurationModifier = false)
  {
    RMCSpeciesSlowdownModifierComponent comp;
    if (this._timing.ApplyingState || !this.TryComp<RMCSpeciesSlowdownModifierComponent>(ent, out comp))
      return false;
    TimeSpan timeSpan = this._timing.CurTime + duration * (ignoreDurationModifier ? 1.0 : (double) comp.DurationMultiplier);
    RMCSuperSlowdownComponent slowdownComponent = this.EnsureComp<RMCSuperSlowdownComponent>(ent);
    if (refresh && timeSpan > slowdownComponent.ExpiresAt)
      slowdownComponent.ExpiresAt = timeSpan;
    else if (!refresh)
      slowdownComponent.ExpiresAt += duration;
    return true;
  }

  public bool TryRoot(EntityUid ent, TimeSpan duration, bool refresh = true)
  {
    TimeSpan timeSpan = this._timing.CurTime + duration;
    RMCRootedComponent rmcRootedComponent = this.EnsureComp<RMCRootedComponent>(ent);
    if (refresh && timeSpan > rmcRootedComponent.ExpiresAt)
      rmcRootedComponent.ExpiresAt = timeSpan;
    else if (!refresh)
      rmcRootedComponent.ExpiresAt += duration;
    return true;
  }

  private void OnAdded<T>(Entity<T> ent, ref ComponentStartup args) where T : IComponent
  {
    this._speed.RefreshMovementSpeedModifiers((EntityUid) ent);
    if (this.HasComp<XenoComponent>((EntityUid) ent))
      return;
    if (typeof (T) != typeof (RMCRootedComponent))
      this.EnsureComp<XenoSlowVisualsComponent>((EntityUid) ent);
    else
      this.EnsureComp<XenoImmobileVisualsComponent>((EntityUid) ent);
  }

  private void OnExpire<T>(Entity<T> ent, ref ComponentShutdown args) where T : IComponent
  {
    if (typeof (T) != typeof (RMCRootedComponent))
    {
      if (typeof (T) == typeof (RMCSlowdownComponent))
        this.MaybeRemoveSlowVisuals((EntityUid) ent);
      else
        this.MaybeRemoveSuperSlowVisuals((EntityUid) ent);
    }
    else
      this.MaybeRemoveStunVisuals((EntityUid) ent);
  }

  private void OnRemove<T>(Entity<T> ent, ref ComponentRemove args) where T : Component
  {
    if (this.TerminatingOrDeleted((EntityUid) ent))
      return;
    this._speed.RefreshMovementSpeedModifiers((EntityUid) ent);
  }

  private void OnRejuvenate<T>(Entity<T> ent, ref RejuvenateEvent args) where T : IComponent
  {
    this.RemCompDeferred<T>((EntityUid) ent);
  }

  private void OnSlowdownRefresh(
    Entity<RMCSlowdownComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    RMCSpeciesSlowdownModifierComponent comp1;
    if (!this.TryComp<RMCSpeciesSlowdownModifierComponent>((EntityUid) ent, out comp1) || !ent.Comp.Running)
      return;
    float? speedModifier = this._temporarySpeed.CalculateSpeedModifier((EntityUid) ent, comp1.SlowModifier);
    RMCSuperSlowdownComponent comp2;
    if (!speedModifier.HasValue || this.TryComp<RMCSuperSlowdownComponent>((EntityUid) ent, out comp2) && comp2.Running)
      return;
    args.ModifySpeed(speedModifier.Value, speedModifier.Value);
  }

  private void OnSuperSlowdownRefresh(
    Entity<RMCSuperSlowdownComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    RMCSpeciesSlowdownModifierComponent comp;
    if (!this.TryComp<RMCSpeciesSlowdownModifierComponent>((EntityUid) ent, out comp) || !ent.Comp.Running)
      return;
    float? speedModifier = this._temporarySpeed.CalculateSpeedModifier((EntityUid) ent, comp.SuperSlowModifier);
    if (!speedModifier.HasValue)
      return;
    args.ModifySpeed(speedModifier.Value, speedModifier.Value);
  }

  private void OnRootRefresh(
    Entity<RMCRootedComponent> ent,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    if (!ent.Comp.Running)
      return;
    args.ModifySpeed(0.0f, 0.0f);
  }

  private void MaybeRemoveSlowVisuals(EntityUid ent)
  {
    if (!this.HasComp<XenoSlowVisualsComponent>(ent) || this.HasComp<RMCSuperSlowdownComponent>(ent))
      return;
    this.RemCompDeferred<XenoSlowVisualsComponent>(ent);
  }

  private void MaybeRemoveSuperSlowVisuals(EntityUid ent)
  {
    if (!this.HasComp<XenoSlowVisualsComponent>(ent) || this.HasComp<RMCSlowdownComponent>(ent))
      return;
    this.RemCompDeferred<XenoSlowVisualsComponent>(ent);
  }

  private void MaybeRemoveStunVisuals(EntityUid ent)
  {
    if (!this.HasComp<XenoImmobileVisualsComponent>(ent) || this.HasComp<StunnedComponent>(ent) && !this._standing.IsDown(ent))
      return;
    this.RemCompDeferred<XenoImmobileVisualsComponent>(ent);
  }

  private void OnModifierStun(
    Entity<RMCSpeciesSlowdownModifierComponent> ent,
    ref StunnedEvent args)
  {
    if (this._standing.IsDown((EntityUid) ent))
      return;
    this.EnsureComp<XenoImmobileVisualsComponent>((EntityUid) ent);
  }

  private void OnModifierKnockdown(
    Entity<RMCSpeciesSlowdownModifierComponent> ent,
    ref KnockedDownEvent args)
  {
    if (!this.HasComp<XenoImmobileVisualsComponent>((EntityUid) ent))
      return;
    this.RemCompDeferred<XenoImmobileVisualsComponent>((EntityUid) ent);
  }

  private void OnModifierEffectEnd(
    Entity<RMCSpeciesSlowdownModifierComponent> ent,
    ref StatusEffectEndedEvent args)
  {
    if (!((IEnumerable<string>) ent.Comp.StatusesToUpdateOn).Contains<string>(args.Key))
      return;
    if (args.Key != "KnockedDown" && !this.HasComp<RMCRootedComponent>((EntityUid) ent))
    {
      this.RemCompDeferred<XenoImmobileVisualsComponent>((EntityUid) ent);
    }
    else
    {
      if (!(args.Key == "KnockedDown") && this._standing.IsDown((EntityUid) ent) || !this.HasComp<StunnedComponent>((EntityUid) ent))
        return;
      this.EnsureComp<XenoImmobileVisualsComponent>((EntityUid) ent);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCSlowdownComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCSlowdownComponent>();
    EntityUid uid1;
    RMCSlowdownComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(curTime < comp1_1.ExpiresAt))
      {
        this.RemCompDeferred<RMCSlowdownComponent>(uid1);
        this._speed.RefreshMovementSpeedModifiers(uid1);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCSuperSlowdownComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCSuperSlowdownComponent>();
    EntityUid uid2;
    RMCSuperSlowdownComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      if (!(curTime < comp1_2.ExpiresAt))
      {
        this.RemCompDeferred<RMCSuperSlowdownComponent>(uid2);
        this._speed.RefreshMovementSpeedModifiers(uid2);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCRootedComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<RMCRootedComponent>();
    EntityUid uid3;
    RMCRootedComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      if (!(curTime < comp1_3.ExpiresAt))
      {
        this.RemCompDeferred<RMCRootedComponent>(uid3);
        this._speed.RefreshMovementSpeedModifiers(uid3);
      }
    }
  }
}
