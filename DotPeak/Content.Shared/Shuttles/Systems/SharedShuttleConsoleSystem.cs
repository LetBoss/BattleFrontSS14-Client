// Decompiled with JetBrains decompiler
// Type: Content.Shared.Shuttles.Systems.SharedShuttleConsoleSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.ActionBlocker;
using Content.Shared.Movement.Events;
using Content.Shared.Shuttles.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Shuttles.Systems;

public abstract class SharedShuttleConsoleSystem : EntitySystem
{
  [Dependency]
  protected ActionBlockerSystem ActionBlockerSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PilotComponent, UpdateCanMoveEvent>(new ComponentEventHandler<PilotComponent, UpdateCanMoveEvent>(this.HandleMovementBlock));
    this.SubscribeLocalEvent<PilotComponent, ComponentStartup>(new ComponentEventHandler<PilotComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<PilotComponent, ComponentShutdown>(new ComponentEventHandler<PilotComponent, ComponentShutdown>(this.HandlePilotShutdown));
  }

  protected virtual void HandlePilotShutdown(
    EntityUid uid,
    PilotComponent component,
    ComponentShutdown args)
  {
    this.ActionBlockerSystem.UpdateCanMove(uid);
  }

  private void OnStartup(EntityUid uid, PilotComponent component, ComponentStartup args)
  {
    this.ActionBlockerSystem.UpdateCanMove(uid);
  }

  private void HandleMovementBlock(
    EntityUid uid,
    PilotComponent component,
    UpdateCanMoveEvent args)
  {
    if (component.LifeStage > ComponentLifeStage.Running || !component.Console.HasValue)
      return;
    args.Cancel();
  }

  [NetSerializable]
  [Serializable]
  protected sealed class PilotComponentState : ComponentState
  {
    public NetEntity? Console { get; }

    public PilotComponentState(NetEntity? uid) => this.Console = uid;
  }
}
