// Decompiled with JetBrains decompiler
// Type: Content.Client.PhysicsSystem.Controllers.MoverController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Alert;
using Content.Shared.CCVar;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Movement.Systems;
using Robust.Client.Physics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.PhysicsSystem.Controllers;

public sealed class MoverController : SharedMoverController
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private IConfigurationManager _cfg;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<RelayInputMoverComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<RelayInputMoverComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnRelayPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<RelayInputMoverComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<RelayInputMoverComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnRelayPlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<InputMoverComponent, LocalPlayerAttachedEvent>(new EntityEventRefHandler<InputMoverComponent, LocalPlayerAttachedEvent>((object) this, __methodptr(OnPlayerAttached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<InputMoverComponent, LocalPlayerDetachedEvent>(new EntityEventRefHandler<InputMoverComponent, LocalPlayerDetachedEvent>((object) this, __methodptr(OnPlayerDetached)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<InputMoverComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<InputMoverComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnUpdatePredicted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<MovementRelayTargetComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<MovementRelayTargetComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnUpdateRelayTargetPredicted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<PullableComponent, UpdateIsPredictedEvent>(new EntityEventRefHandler<PullableComponent, UpdateIsPredictedEvent>((object) this, __methodptr(OnUpdatePullablePredicted)), (Type[]) null, (Type[]) null);
  }

  private void OnUpdatePredicted(
    Entity<InputMoverComponent> entity,
    ref UpdateIsPredictedEvent args)
  {
    EntityUid owner = entity.Owner;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(owner, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    args.IsPredicted = true;
  }

  private void OnUpdateRelayTargetPredicted(
    Entity<MovementRelayTargetComponent> entity,
    ref UpdateIsPredictedEvent args)
  {
    EntityUid source = entity.Comp.Source;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((localEntity.HasValue ? (EntityUid.op_Equality(source, localEntity.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    args.IsPredicted = true;
  }

  private void OnUpdatePullablePredicted(
    Entity<PullableComponent> entity,
    ref UpdateIsPredictedEvent args)
  {
    EntityUid? puller = entity.Comp.Puller;
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if ((puller.HasValue == localEntity.HasValue ? (puller.HasValue ? (EntityUid.op_Equality(puller.GetValueOrDefault(), localEntity.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
    {
      args.IsPredicted = true;
    }
    else
    {
      if (!entity.Comp.Puller.HasValue)
        return;
      args.BlockPrediction = true;
    }
  }

  private void OnRelayPlayerAttached(
    Entity<RelayInputMoverComponent> entity,
    ref LocalPlayerAttachedEvent args)
  {
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Owner), (PhysicsComponent) null);
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Comp.RelayEntity), (PhysicsComponent) null);
    InputMoverComponent inputMoverComponent;
    if (!this.MoverQuery.TryGetComponent(entity.Comp.RelayEntity, ref inputMoverComponent))
      return;
    this.SetMoveInput(Entity<InputMoverComponent>.op_Implicit((entity.Comp.RelayEntity, inputMoverComponent)), MoveButtons.None);
  }

  private void OnRelayPlayerDetached(
    Entity<RelayInputMoverComponent> entity,
    ref LocalPlayerDetachedEvent args)
  {
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Owner), (PhysicsComponent) null);
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Comp.RelayEntity), (PhysicsComponent) null);
    InputMoverComponent inputMoverComponent;
    if (!this.MoverQuery.TryGetComponent(entity.Comp.RelayEntity, ref inputMoverComponent))
      return;
    this.SetMoveInput(Entity<InputMoverComponent>.op_Implicit((entity.Comp.RelayEntity, inputMoverComponent)), MoveButtons.None);
  }

  private void OnPlayerAttached(
    Entity<InputMoverComponent> entity,
    ref LocalPlayerAttachedEvent args)
  {
    this.SetMoveInput(entity, MoveButtons.None);
  }

  private void OnPlayerDetached(
    Entity<InputMoverComponent> entity,
    ref LocalPlayerDetachedEvent args)
  {
    this.SetMoveInput(entity, MoveButtons.None);
  }

  public virtual void UpdateBeforeSolve(bool prediction, float frameTime)
  {
    base.UpdateBeforeSolve(prediction, frameTime);
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid valueOrDefault = localEntity.GetValueOrDefault();
    if (!((EntityUid) ref valueOrDefault).Valid)
      return;
    RelayInputMoverComponent inputMoverComponent;
    if (this.RelayQuery.TryGetComponent(valueOrDefault, ref inputMoverComponent))
      this.HandleClientsideMovement(inputMoverComponent.RelayEntity, frameTime);
    this.HandleClientsideMovement(valueOrDefault, frameTime);
  }

  private void HandleClientsideMovement(EntityUid player, float frameTime)
  {
    InputMoverComponent inputMoverComponent;
    if (!this.MoverQuery.TryGetComponent(player, ref inputMoverComponent))
      return;
    this.HandleMobMovement(Entity<InputMoverComponent>.op_Implicit((player, inputMoverComponent)), frameTime);
  }

  protected override bool CanSound()
  {
    IGameTiming timing = this._timing;
    return timing != null && timing.IsFirstTimePredicted && timing.InSimulation;
  }

  public override void SetSprinting(
    Entity<InputMoverComponent> entity,
    ushort subTick,
    bool walking)
  {
    base.SetSprinting(entity, subTick, walking);
    if (walking && this._cfg.GetCVar<bool>(CCVars.ToggleWalk))
      this._alerts.ShowAlert(Entity<InputMoverComponent>.op_Implicit(entity), SharedMoverController.WalkingAlert, showCooldown: false);
    else
      this._alerts.ClearAlert(Entity<InputMoverComponent>.op_Implicit(entity), SharedMoverController.WalkingAlert);
  }
}
