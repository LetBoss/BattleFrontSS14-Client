// Decompiled with JetBrains decompiler
// Type: Content.Shared.Puppet.SharedVentriloquistPuppetSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Emoting;
using Content.Shared.Hands;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Puppet;

public abstract class SharedVentriloquistPuppetSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _blocker;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, UseAttemptEvent>(new ComponentEventHandler<VentriloquistPuppetComponent, UseAttemptEvent>(this.Cancel<UseAttemptEvent>));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, InteractionAttemptEvent>(new EntityEventRefHandler<VentriloquistPuppetComponent, InteractionAttemptEvent>(this.CancelInteract));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, DropAttemptEvent>(new ComponentEventHandler<VentriloquistPuppetComponent, DropAttemptEvent>(this.Cancel<DropAttemptEvent>));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, PickupAttemptEvent>(new ComponentEventHandler<VentriloquistPuppetComponent, PickupAttemptEvent>(this.Cancel<PickupAttemptEvent>));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, UpdateCanMoveEvent>(new ComponentEventHandler<VentriloquistPuppetComponent, UpdateCanMoveEvent>(this.Cancel<UpdateCanMoveEvent>));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, EmoteAttemptEvent>(new ComponentEventHandler<VentriloquistPuppetComponent, EmoteAttemptEvent>(this.Cancel<EmoteAttemptEvent>));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, ChangeDirectionAttemptEvent>(new ComponentEventHandler<VentriloquistPuppetComponent, ChangeDirectionAttemptEvent>(this.Cancel<ChangeDirectionAttemptEvent>));
    this.SubscribeLocalEvent<VentriloquistPuppetComponent, ComponentStartup>(new ComponentEventHandler<VentriloquistPuppetComponent, ComponentStartup>(this.OnStartup));
  }

  private void CancelInteract(
    Entity<VentriloquistPuppetComponent> ent,
    ref InteractionAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnStartup(
    EntityUid uid,
    VentriloquistPuppetComponent component,
    ComponentStartup args)
  {
    this._blocker.UpdateCanMove(uid);
  }

  private void Cancel<T>(EntityUid uid, VentriloquistPuppetComponent component, T args) where T : CancellableEntityEventArgs
  {
    args.Cancel();
  }
}
