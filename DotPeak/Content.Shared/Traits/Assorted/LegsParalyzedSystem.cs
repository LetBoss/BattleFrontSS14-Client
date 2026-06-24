// Decompiled with JetBrains decompiler
// Type: Content.Shared.Traits.Assorted.LegsParalyzedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Systems;
using Content.Shared.Buckle.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Standing;
using Content.Shared.Throwing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Traits.Assorted;

public sealed class LegsParalyzedSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifierSystem;
  [Dependency]
  private StandingStateSystem _standingSystem;
  [Dependency]
  private SharedBodySystem _bodySystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<LegsParalyzedComponent, ComponentStartup>(new ComponentEventHandler<LegsParalyzedComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<LegsParalyzedComponent, ComponentShutdown>(new ComponentEventHandler<LegsParalyzedComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<LegsParalyzedComponent, BuckledEvent>(new ComponentEventRefHandler<LegsParalyzedComponent, BuckledEvent>(this.OnBuckled));
    this.SubscribeLocalEvent<LegsParalyzedComponent, UnbuckledEvent>(new ComponentEventRefHandler<LegsParalyzedComponent, UnbuckledEvent>(this.OnUnbuckled));
    this.SubscribeLocalEvent<LegsParalyzedComponent, ThrowPushbackAttemptEvent>(new ComponentEventHandler<LegsParalyzedComponent, ThrowPushbackAttemptEvent>(this.OnThrowPushbackAttempt));
    this.SubscribeLocalEvent<LegsParalyzedComponent, UpdateCanMoveEvent>(new ComponentEventHandler<LegsParalyzedComponent, UpdateCanMoveEvent>(this.OnUpdateCanMoveEvent));
  }

  private void OnStartup(EntityUid uid, LegsParalyzedComponent component, ComponentStartup args)
  {
    this._movementSpeedModifierSystem.ChangeBaseSpeed(uid, 0.0f, 0.0f, 20f);
  }

  private void OnShutdown(EntityUid uid, LegsParalyzedComponent component, ComponentShutdown args)
  {
    this._standingSystem.Stand(uid);
    this._bodySystem.UpdateMovementSpeed(uid);
  }

  private void OnBuckled(EntityUid uid, LegsParalyzedComponent component, ref BuckledEvent args)
  {
    this._standingSystem.Stand(uid);
  }

  private void OnUnbuckled(
    EntityUid uid,
    LegsParalyzedComponent component,
    ref UnbuckledEvent args)
  {
    this._standingSystem.Down(uid);
  }

  private void OnUpdateCanMoveEvent(
    EntityUid uid,
    LegsParalyzedComponent component,
    UpdateCanMoveEvent args)
  {
    args.Cancel();
  }

  private void OnThrowPushbackAttempt(
    EntityUid uid,
    LegsParalyzedComponent component,
    ThrowPushbackAttemptEvent args)
  {
    args.Cancel();
  }
}
