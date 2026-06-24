using System;
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
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Player;
using Robust.Shared.Timing;

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
		((EntitySystem)this).SubscribeLocalEvent<RelayInputMoverComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<RelayInputMoverComponent, LocalPlayerAttachedEvent>)OnRelayPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RelayInputMoverComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<RelayInputMoverComponent, LocalPlayerDetachedEvent>)OnRelayPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, LocalPlayerAttachedEvent>((EntityEventRefHandler<InputMoverComponent, LocalPlayerAttachedEvent>)OnPlayerAttached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, LocalPlayerDetachedEvent>((EntityEventRefHandler<InputMoverComponent, LocalPlayerDetachedEvent>)OnPlayerDetached, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InputMoverComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<InputMoverComponent, UpdateIsPredictedEvent>)OnUpdatePredicted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MovementRelayTargetComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<MovementRelayTargetComponent, UpdateIsPredictedEvent>)OnUpdateRelayTargetPredicted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PullableComponent, UpdateIsPredictedEvent>((EntityEventRefHandler<PullableComponent, UpdateIsPredictedEvent>)OnUpdatePullablePredicted, (Type[])null, (Type[])null);
	}

	private void OnUpdatePredicted(Entity<InputMoverComponent> entity, ref UpdateIsPredictedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		EntityUid owner = entity.Owner;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && owner == localEntity.GetValueOrDefault())
		{
			args.IsPredicted = true;
		}
	}

	private void OnUpdateRelayTargetPredicted(Entity<MovementRelayTargetComponent> entity, ref UpdateIsPredictedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		EntityUid source = entity.Comp.Source;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue && source == localEntity.GetValueOrDefault())
		{
			args.IsPredicted = true;
		}
	}

	private void OnUpdatePullablePredicted(Entity<PullableComponent> entity, ref UpdateIsPredictedEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? puller = entity.Comp.Puller;
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (puller.HasValue == localEntity.HasValue && (!puller.HasValue || puller.GetValueOrDefault() == localEntity.GetValueOrDefault()))
		{
			args.IsPredicted = true;
		}
		else if (entity.Comp.Puller.HasValue)
		{
			args.BlockPrediction = true;
		}
	}

	private void OnRelayPlayerAttached(Entity<RelayInputMoverComponent> entity, ref LocalPlayerAttachedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Owner, (PhysicsComponent)null);
		((VirtualController)this).PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Comp.RelayEntity, (PhysicsComponent)null);
		InputMoverComponent item = default(InputMoverComponent);
		if (MoverQuery.TryGetComponent(entity.Comp.RelayEntity, ref item))
		{
			SetMoveInput(Entity<InputMoverComponent>.op_Implicit((entity.Comp.RelayEntity, item)), MoveButtons.None);
		}
	}

	private void OnRelayPlayerDetached(Entity<RelayInputMoverComponent> entity, ref LocalPlayerDetachedEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Owner, (PhysicsComponent)null);
		((VirtualController)this).PhysicsSystem.UpdateIsPredicted((EntityUid?)entity.Comp.RelayEntity, (PhysicsComponent)null);
		InputMoverComponent item = default(InputMoverComponent);
		if (MoverQuery.TryGetComponent(entity.Comp.RelayEntity, ref item))
		{
			SetMoveInput(Entity<InputMoverComponent>.op_Implicit((entity.Comp.RelayEntity, item)), MoveButtons.None);
		}
	}

	private void OnPlayerAttached(Entity<InputMoverComponent> entity, ref LocalPlayerAttachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetMoveInput(entity, MoveButtons.None);
	}

	private void OnPlayerDetached(Entity<InputMoverComponent> entity, ref LocalPlayerDetachedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetMoveInput(entity, MoveButtons.None);
	}

	public override void UpdateBeforeSolve(bool prediction, float frameTime)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		((VirtualController)this).UpdateBeforeSolve(prediction, frameTime);
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (!localEntity.HasValue)
		{
			return;
		}
		EntityUid valueOrDefault = localEntity.GetValueOrDefault();
		if (((EntityUid)(ref valueOrDefault)).Valid)
		{
			RelayInputMoverComponent relayInputMoverComponent = default(RelayInputMoverComponent);
			if (RelayQuery.TryGetComponent(valueOrDefault, ref relayInputMoverComponent))
			{
				HandleClientsideMovement(relayInputMoverComponent.RelayEntity, frameTime);
			}
			HandleClientsideMovement(valueOrDefault, frameTime);
		}
	}

	private void HandleClientsideMovement(EntityUid player, float frameTime)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		InputMoverComponent item = default(InputMoverComponent);
		if (MoverQuery.TryGetComponent(player, ref item))
		{
			HandleMobMovement(Entity<InputMoverComponent>.op_Implicit((player, item)), frameTime);
		}
	}

	protected override bool CanSound()
	{
		IGameTiming timing = _timing;
		if (timing != null && timing.IsFirstTimePredicted)
		{
			return timing.InSimulation;
		}
		return false;
	}

	public override void SetSprinting(Entity<InputMoverComponent> entity, ushort subTick, bool walking)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.SetSprinting(entity, subTick, walking);
		if (walking && _cfg.GetCVar<bool>(CCVars.ToggleWalk))
		{
			_alerts.ShowAlert(Entity<InputMoverComponent>.op_Implicit(entity), SharedMoverController.WalkingAlert, null, null, autoRemove: false, showCooldown: false);
		}
		else
		{
			_alerts.ClearAlert(Entity<InputMoverComponent>.op_Implicit(entity), SharedMoverController.WalkingAlert);
		}
	}
}
