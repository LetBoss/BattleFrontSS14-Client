// Decompiled with JetBrains decompiler
// Type: Content.Shared.Administration.SharedAdminFrozenSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Emoting;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Pulling.Systems;
using Content.Shared.Speech;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Administration;

public abstract class SharedAdminFrozenSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private PullingSystem _pulling;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, UseAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, UseAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, PickupAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, PickupAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, ThrowAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, ThrowAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, InteractionAttemptEvent>(new EntityEventRefHandler<AdminFrozenComponent, InteractionAttemptEvent>((object) this, __methodptr(OnInteractAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, ComponentStartup>(new ComponentEventHandler<AdminFrozenComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, ComponentShutdown>(new ComponentEventHandler<AdminFrozenComponent, ComponentShutdown>((object) this, __methodptr(UpdateCanMove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, UpdateCanMoveEvent>(new ComponentEventHandler<AdminFrozenComponent, UpdateCanMoveEvent>((object) this, __methodptr(OnUpdateCanMove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, PullAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, PullAttemptEvent>((object) this, __methodptr(OnPullAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, AttackAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, AttackAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, ChangeDirectionAttemptEvent>((object) this, __methodptr(OnAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, EmoteAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, EmoteAttemptEvent>((object) this, __methodptr(OnEmoteAttempt)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AdminFrozenComponent, SpeakAttemptEvent>(new ComponentEventHandler<AdminFrozenComponent, SpeakAttemptEvent>((object) this, __methodptr(OnSpeakAttempt)), (Type[]) null, (Type[]) null);
  }

  private void OnInteractAttempt(Entity<AdminFrozenComponent> ent, ref InteractionAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnSpeakAttempt(
    EntityUid uid,
    AdminFrozenComponent component,
    SpeakAttemptEvent args)
  {
    if (!component.Muted)
      return;
    args.Cancel();
  }

  private void OnAttempt(
    EntityUid uid,
    AdminFrozenComponent component,
    CancellableEntityEventArgs args)
  {
    args.Cancel();
  }

  private void OnPullAttempt(EntityUid uid, AdminFrozenComponent component, PullAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnStartup(EntityUid uid, AdminFrozenComponent component, ComponentStartup args)
  {
    PullableComponent pullable;
    if (this.TryComp<PullableComponent>(uid, ref pullable))
      this._pulling.TryStopPull(uid, pullable);
    this.UpdateCanMove(uid, component, (EntityEventArgs) args);
  }

  private void OnUpdateCanMove(
    EntityUid uid,
    AdminFrozenComponent component,
    UpdateCanMoveEvent args)
  {
    if (component.LifeStage > 6)
      return;
    args.Cancel();
  }

  private void UpdateCanMove(EntityUid uid, AdminFrozenComponent component, EntityEventArgs args)
  {
    this._blocker.UpdateCanMove(uid);
  }

  private void OnEmoteAttempt(
    EntityUid uid,
    AdminFrozenComponent component,
    EmoteAttemptEvent args)
  {
    if (!component.Muted)
      return;
    args.Cancel();
  }
}
