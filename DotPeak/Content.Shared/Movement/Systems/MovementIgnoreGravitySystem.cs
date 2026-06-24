// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.MovementIgnoreGravitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;

#nullable enable
namespace Content.Shared.Movement.Systems;

public sealed class MovementIgnoreGravitySystem : EntitySystem
{
  public override void Initialize()
  {
    this.SubscribeLocalEvent<MovementIgnoreGravityComponent, ComponentGetState>(new ComponentEventRefHandler<MovementIgnoreGravityComponent, ComponentGetState>(this.GetState));
    this.SubscribeLocalEvent<MovementIgnoreGravityComponent, ComponentHandleState>(new ComponentEventRefHandler<MovementIgnoreGravityComponent, ComponentHandleState>(this.HandleState));
    this.SubscribeLocalEvent<MovementAlwaysTouchingComponent, CanWeightlessMoveEvent>(new ComponentEventRefHandler<MovementAlwaysTouchingComponent, CanWeightlessMoveEvent>(this.OnWeightless));
  }

  private void OnWeightless(
    EntityUid uid,
    MovementAlwaysTouchingComponent component,
    ref CanWeightlessMoveEvent args)
  {
    args.CanMove = true;
  }

  private void HandleState(
    EntityUid uid,
    MovementIgnoreGravityComponent component,
    ref ComponentHandleState args)
  {
    if (args.Next == null)
      return;
    component.Weightless = ((MovementIgnoreGravityComponentState) args.Next).Weightless;
  }

  private void GetState(
    EntityUid uid,
    MovementIgnoreGravityComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new MovementIgnoreGravityComponentState(component);
  }
}
