// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stealth.SharedStealthSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.Mobs;
using Content.Shared.Stealth.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Stealth;

public abstract class SharedStealthSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<StealthComponent, ComponentGetState>(new ComponentEventRefHandler<StealthComponent, ComponentGetState>(this.OnStealthGetState));
    this.SubscribeLocalEvent<StealthComponent, ComponentHandleState>(new ComponentEventRefHandler<StealthComponent, ComponentHandleState>(this.OnStealthHandleState));
    this.SubscribeLocalEvent<StealthOnMoveComponent, MoveEvent>(new ComponentEventRefHandler<StealthOnMoveComponent, MoveEvent>(this.OnMove));
    this.SubscribeLocalEvent<StealthOnMoveComponent, SharedStealthSystem.GetVisibilityModifiersEvent>(new ComponentEventHandler<StealthOnMoveComponent, SharedStealthSystem.GetVisibilityModifiersEvent>(this.OnGetVisibilityModifiers));
    this.SubscribeLocalEvent<StealthComponent, EntityPausedEvent>(new ComponentEventRefHandler<StealthComponent, EntityPausedEvent>(this.OnPaused));
    this.SubscribeLocalEvent<StealthComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<StealthComponent, EntityUnpausedEvent>(this.OnUnpaused));
    this.SubscribeLocalEvent<StealthComponent, ComponentInit>(new ComponentEventHandler<StealthComponent, ComponentInit>(this.OnInit));
    this.SubscribeLocalEvent<StealthComponent, ExamineAttemptEvent>(new ComponentEventHandler<StealthComponent, ExamineAttemptEvent>(this.OnExamineAttempt));
    this.SubscribeLocalEvent<StealthComponent, ExaminedEvent>(new ComponentEventHandler<StealthComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<StealthComponent, MobStateChangedEvent>(new ComponentEventHandler<StealthComponent, MobStateChangedEvent>(this.OnMobStateChanged));
  }

  private void OnExamineAttempt(
    EntityUid uid,
    StealthComponent component,
    ExamineAttemptEvent args)
  {
    if (!component.Enabled || (double) this.GetVisibility(uid, component) > (double) component.ExamineThreshold)
      return;
    EntityUid uid1 = args.Examiner;
    while (!(uid1 == uid))
    {
      uid1 = this.Transform(uid1).ParentUid;
      if (!uid1.IsValid())
      {
        args.Cancel();
        break;
      }
    }
  }

  private void OnExamined(EntityUid uid, StealthComponent component, ExaminedEvent args)
  {
    if (!component.Enabled)
      return;
    args.PushMarkup(this.Loc.GetString(component.ExaminedDesc, ("target", (object) uid)));
  }

  public virtual void SetEnabled(EntityUid uid, bool value, StealthComponent? component = null)
  {
    if (!this.Resolve<StealthComponent>(uid, ref component, false) || component.Enabled == value)
      return;
    component.Enabled = value;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnMobStateChanged(
    EntityUid uid,
    StealthComponent component,
    MobStateChangedEvent args)
  {
    component.Enabled = args.NewMobState != MobState.Dead || component.EnabledOnDeath;
    this.Dirty(uid, (IComponent) component);
  }

  private void OnPaused(EntityUid uid, StealthComponent component, ref EntityPausedEvent args)
  {
    component.LastVisibility = this.GetVisibility(uid, component);
    component.LastUpdated = new TimeSpan?();
    this.Dirty(uid, (IComponent) component);
  }

  private void OnUnpaused(EntityUid uid, StealthComponent component, ref EntityUnpausedEvent args)
  {
    component.LastUpdated = new TimeSpan?(this._timing.CurTime);
    this.Dirty(uid, (IComponent) component);
  }

  protected virtual void OnInit(EntityUid uid, StealthComponent component, ComponentInit args)
  {
    if (component.LastUpdated.HasValue || this.Paused(uid))
      return;
    component.LastUpdated = new TimeSpan?(this._timing.CurTime);
  }

  private void OnStealthGetState(
    EntityUid uid,
    StealthComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new StealthComponentState(component.LastVisibility, component.LastUpdated, component.Enabled);
  }

  private void OnStealthHandleState(
    EntityUid uid,
    StealthComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is StealthComponentState current))
      return;
    this.SetEnabled(uid, current.Enabled, component);
    component.LastVisibility = current.Visibility;
    component.LastUpdated = current.LastUpdated;
  }

  private void OnMove(EntityUid uid, StealthOnMoveComponent component, ref MoveEvent args)
  {
    if (this._timing.ApplyingState || args.NewPosition.EntityId != args.OldPosition.EntityId)
      return;
    float delta = component.MovementVisibilityRate * (args.NewPosition.Position - args.OldPosition.Position).Length();
    this.ModifyVisibility(uid, delta);
  }

  private void OnGetVisibilityModifiers(
    EntityUid uid,
    StealthOnMoveComponent component,
    SharedStealthSystem.GetVisibilityModifiersEvent args)
  {
    float num = args.SecondsSinceUpdate * component.PassiveVisibilityRate;
    args.FlatModifier += num;
  }

  public void ModifyVisibility(EntityUid uid, float delta, StealthComponent? component = null)
  {
    if ((double) delta == 0.0 || !this.Resolve<StealthComponent>(uid, ref component))
      return;
    if (component.LastUpdated.HasValue)
    {
      component.LastVisibility = this.GetVisibility(uid, component);
      component.LastUpdated = new TimeSpan?(this._timing.CurTime);
    }
    component.LastVisibility = Math.Clamp(component.LastVisibility + delta, component.MinVisibility, component.MaxVisibility);
    this.Dirty(uid, (IComponent) component);
  }

  public void SetVisibility(EntityUid uid, float value, StealthComponent? component = null)
  {
    if (!this.Resolve<StealthComponent>(uid, ref component))
      return;
    component.LastVisibility = Math.Clamp(value, component.MinVisibility, component.MaxVisibility);
    if (component.LastUpdated.HasValue)
      component.LastUpdated = new TimeSpan?(this._timing.CurTime);
    this.Dirty(uid, (IComponent) component);
  }

  public float GetVisibility(EntityUid uid, StealthComponent? component = null)
  {
    if (!this.Resolve<StealthComponent>(uid, ref component) || !component.Enabled)
      return 1f;
    if (!component.LastUpdated.HasValue)
      return component.LastVisibility;
    TimeSpan timeSpan = this._timing.CurTime - component.LastUpdated.Value;
    SharedStealthSystem.GetVisibilityModifiersEvent args = new SharedStealthSystem.GetVisibilityModifiersEvent(uid, component, (float) timeSpan.TotalSeconds, 0.0f);
    this.RaiseLocalEvent<SharedStealthSystem.GetVisibilityModifiersEvent>(uid, args);
    return Math.Clamp(component.LastVisibility + args.FlatModifier, component.MinVisibility, component.MaxVisibility);
  }

  private sealed class GetVisibilityModifiersEvent : EntityEventArgs
  {
    public readonly StealthComponent Stealth;
    public readonly float SecondsSinceUpdate;
    public float FlatModifier;

    public GetVisibilityModifiersEvent(
      EntityUid uid,
      StealthComponent stealth,
      float secondsSinceUpdate,
      float flatModifier)
    {
      this.Stealth = stealth;
      this.SecondsSinceUpdate = secondsSinceUpdate;
      this.FlatModifier = flatModifier;
    }
  }
}
