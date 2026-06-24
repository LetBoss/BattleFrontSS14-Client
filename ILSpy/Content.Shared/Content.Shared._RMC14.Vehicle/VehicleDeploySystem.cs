using System;
using Content.Shared._RMC14.Chat;
using Content.Shared._RMC14.Sentry;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Buckle.Components;
using Content.Shared.Chat;
using Content.Shared.CombatMode;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Vehicle.Components;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Vehicle;

public sealed class VehicleDeploySystem : EntitySystem
{
	[Dependency]
	private readonly SharedActionsSystem _actions;

	[Dependency]
	private readonly SharedCMChatSystem _rmcChat;

	[Dependency]
	private readonly MetaDataSystem _meta;

	[Dependency]
	private readonly SharedPopupSystem _popup;

	[Dependency]
	private readonly VehicleSystem _vehicleSystem;

	[Dependency]
	private readonly SharedCombatModeSystem _combatMode;

	[Dependency]
	private readonly SharedSentryTargetingSystem _targeting;

	[Dependency]
	private readonly SharedGunSystem _guns;

	[Dependency]
	private readonly VehicleTurretSystem _turret;

	[Dependency]
	private readonly VehicleTopologySystem _topology;

	[Dependency]
	private readonly SharedTransformSystem _transform;

	[Dependency]
	private readonly SharedInteractionSystem _interaction;

	[Dependency]
	private readonly IGameTiming _timing;

	[Dependency]
	private readonly INetManager _net;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, StrappedEvent>((EntityEventRefHandler<StrapComponent, StrappedEvent>)OnDriverStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapComponent, UnstrappedEvent>((EntityEventRefHandler<StrapComponent, UnstrappedEvent>)OnDriverUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleDeployActionComponent, VehicleDeployActionEvent>((EntityEventRefHandler<VehicleDeployActionComponent, VehicleDeployActionEvent>)OnDeployAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleDeployActionComponent, ComponentShutdown>((EntityEventRefHandler<VehicleDeployActionComponent, ComponentShutdown>)OnDeployActionShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleDeployableComponent, VehicleCanRunEvent>((EntityEventRefHandler<VehicleDeployableComponent, VehicleCanRunEvent>)OnVehicleCanRun, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointSlotsChangedEvent>((EntityEventHandler<HardpointSlotsChangedEvent>)OnHardpointSlotsChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HardpointItemComponent, AttemptShootEvent>((EntityEventRefHandler<HardpointItemComponent, AttemptShootEvent>)OnDeployableAttemptShoot, (Type[])null, (Type[])null);
	}

	private void OnDriverStrapped(Entity<StrapComponent> ent, ref StrappedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
		if (!_net.IsClient && ((EntitySystem)this).HasComp<VehicleDriverSeatComponent>(ent.Owner) && _vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) && vehicle.HasValue && ((EntitySystem)this).TryComp<VehicleDeployableComponent>(vehicle.Value, ref deployable))
		{
			EnableDeployAction(args.Buckle.Owner, vehicle.Value, deployable);
		}
	}

	private void OnDriverUnstrapped(Entity<StrapComponent> ent, ref UnstrappedEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (!_net.IsClient && ((EntitySystem)this).HasComp<VehicleDriverSeatComponent>(ent.Owner) && _vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out var vehicle) && vehicle.HasValue)
		{
			DisableDeployAction(args.Buckle.Owner, vehicle.Value);
		}
	}

	private void EnableDeployAction(EntityUid user, EntityUid vehicle, VehicleDeployableComponent deployable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		VehicleDeployActionComponent actionComp = ((EntitySystem)this).EnsureComp<VehicleDeployActionComponent>(user);
		actionComp.Vehicle = vehicle;
		if (!actionComp.Action.HasValue)
		{
			actionComp.Action = _actions.AddAction(user, EntProtoId.op_Implicit(actionComp.ActionId));
		}
		UpdateDeployActionState(user, actionComp, deployable);
		((EntitySystem)this).Dirty(user, (IComponent)(object)actionComp, (MetaDataComponent)null);
	}

	private void DisableDeployAction(EntityUid user, EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		VehicleDeployActionComponent actionComp = default(VehicleDeployActionComponent);
		if (!((EntitySystem)this).TryComp<VehicleDeployActionComponent>(user, ref actionComp))
		{
			return;
		}
		EntityUid? vehicle2 = actionComp.Vehicle;
		if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != vehicle))
		{
			if (actionComp.Action.HasValue)
			{
				_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(user), Entity<ActionComponent>.op_Implicit(actionComp.Action.Value));
			}
			((EntitySystem)this).RemCompDeferred<VehicleDeployActionComponent>(user);
		}
	}

	private void OnDeployActionShutdown(Entity<VehicleDeployActionComponent> ent, ref ComponentShutdown args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Action.HasValue)
		{
			_actions.RemoveAction(Entity<ActionsComponent>.op_Implicit(ent.Owner), Entity<ActionComponent>.op_Implicit(ent.Comp.Action.Value));
		}
	}

	private void OnDeployAction(Entity<VehicleDeployActionComponent> ent, ref VehicleDeployActionEvent args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if (_net.IsClient || ((HandledEntityEventArgs)args).Handled || args.Performer != ent.Owner)
		{
			return;
		}
		((HandledEntityEventArgs)args).Handled = true;
		EntityUid? vehicle = ent.Comp.Vehicle;
		if (!vehicle.HasValue)
		{
			return;
		}
		EntityUid vehicle2 = vehicle.GetValueOrDefault();
		VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
		if (!((EntitySystem)this).TryComp<VehicleDeployableComponent>(vehicle2, ref deployable))
		{
			return;
		}
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(vehicle2, ref vehicleComp))
		{
			vehicle = vehicleComp.Operator;
			EntityUid turretUid = ent.Owner;
			if (vehicle.HasValue && !(vehicle.GetValueOrDefault() != turretUid))
			{
				if (deployable.Deploying)
				{
					return;
				}
				bool deployingTo = !deployable.Deployed;
				if (deployingTo && !TryGetVehicleTurret(vehicle2, out turretUid))
				{
					_popup.PopupClient(base.Loc.GetString("rmc-vehicle-deploy-requires-turret"), ent.Owner, ent.Owner, PopupType.SmallCaution);
					return;
				}
				deployable.Deploying = true;
				deployable.DeployingTo = deployingTo;
				deployable.Deployer = ent.Owner;
				TimeSpan delay = (deployingTo ? deployable.DeployTime : deployable.UndeployTime);
				deployable.DeployEndTime = _timing.CurTime + delay;
				deployable.AutoTarget = null;
				deployable.NextAutoTargetTime = TimeSpan.Zero;
				deployable.AutoSpinInitialized = false;
				((EntitySystem)this).Dirty(vehicle2, (IComponent)(object)deployable, (MetaDataComponent)null);
				if (!deployingTo && TryGetVehicleTurret(vehicle2, out var turretUid2))
				{
					Angle vehicleRot = _transform.GetWorldRotation(vehicle2);
					_turret.TrySetTargetRotationWorld(turretUid2, vehicleRot);
				}
				UpdateDeployActionState(ent.Owner, ent.Comp, deployable);
				EntityUid? actionEntity = ent.Comp.Action;
				if (actionEntity.HasValue)
				{
					_actions.SetCooldown(Entity<ActionComponent>.op_Implicit(actionEntity.Value), delay);
				}
				string popupKey = (deployingTo ? "rmc-vehicle-deploy-start" : "rmc-vehicle-undeploy-start");
				string startMsg = base.Loc.GetString(popupKey);
				_popup.PopupClient(startMsg, ent.Owner, ent.Owner);
				SendDeployChat(ent.Owner, vehicle2, startMsg);
				return;
			}
		}
		_popup.PopupClient(base.Loc.GetString("rmc-vehicle-deploy-not-driver"), ent.Owner, ent.Owner, PopupType.SmallCaution);
	}

	private void OnVehicleCanRun(Entity<VehicleDeployableComponent> ent, ref VehicleCanRunEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (args.CanRun && (ent.Comp.Deploying || ent.Comp.Deployed))
		{
			args.CanRun = false;
		}
	}

	private void UpdateDriverActionState(EntityUid vehicle, VehicleDeployableComponent deployable)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp) || !vehicleComp.Operator.HasValue)
		{
			return;
		}
		EntityUid driver = vehicleComp.Operator.Value;
		VehicleDeployActionComponent actionComp = default(VehicleDeployActionComponent);
		if (((EntitySystem)this).TryComp<VehicleDeployActionComponent>(driver, ref actionComp))
		{
			EntityUid? vehicle2 = actionComp.Vehicle;
			if (vehicle2.HasValue && !(vehicle2.GetValueOrDefault() != vehicle))
			{
				UpdateDeployActionState(driver, actionComp, deployable);
				((EntitySystem)this).Dirty(driver, (IComponent)(object)actionComp, (MetaDataComponent)null);
			}
		}
	}

	private void UpdateDeployActionState(EntityUid user, VehicleDeployActionComponent actionComp, VehicleDeployableComponent deployable)
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if (!actionComp.Action.HasValue)
		{
			return;
		}
		bool canDeploy = true;
		EntityUid? turretUid = null;
		EntityUid? vehicle = actionComp.Vehicle;
		if (vehicle.HasValue)
		{
			EntityUid vehicle2 = vehicle.GetValueOrDefault();
			EntityUid foundTurret;
			bool hasTurret = TryGetVehicleTurret(vehicle2, out foundTurret);
			canDeploy = deployable.Deployed || hasTurret;
			turretUid = (hasTurret ? new EntityUid?(foundTurret) : ((EntityUid?)null));
			vehicle = actionComp.Action;
			if (vehicle.HasValue)
			{
				EntityUid actionEntity = vehicle.GetValueOrDefault();
				ActionComponent actionComponent = default(ActionComponent);
				if (((EntitySystem)this).TryComp<ActionComponent>(actionEntity, ref actionComponent))
				{
					_actions.SetEntityIcon(Entity<ActionComponent>.op_Implicit((actionEntity, actionComponent)), turretUid ?? vehicle2);
				}
			}
		}
		EntityUid actionEntityUid = actionComp.Action.Value;
		_actions.SetToggled(Entity<ActionComponent>.op_Implicit(actionEntityUid), deployable.Deployed || deployable.Deploying);
		_actions.SetEnabled(Entity<ActionComponent>.op_Implicit(actionEntityUid), !deployable.Deploying && canDeploy);
		UpdateDeployActionText(actionEntityUid, deployable);
	}

	private void UpdateDeployActionText(EntityUid action, VehicleDeployableComponent deployable)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		string nameKey;
		string descKey;
		if (deployable.Deploying)
		{
			if (deployable.DeployingTo)
			{
				nameKey = "rmc-vehicle-deploy-action-name-deploying";
				descKey = "rmc-vehicle-deploy-action-desc-deploying";
			}
			else
			{
				nameKey = "rmc-vehicle-deploy-action-name-undeploying";
				descKey = "rmc-vehicle-deploy-action-desc-undeploying";
			}
		}
		else if (deployable.Deployed)
		{
			nameKey = "rmc-vehicle-deploy-action-name-undeploy";
			descKey = "rmc-vehicle-deploy-action-desc-undeploy";
		}
		else
		{
			nameKey = "rmc-vehicle-deploy-action-name-deploy";
			descKey = "rmc-vehicle-deploy-action-desc-deploy";
		}
		_meta.SetEntityName(action, base.Loc.GetString(nameKey), (MetaDataComponent)null, true);
		_meta.SetEntityDescription(action, base.Loc.GetString(descKey), (MetaDataComponent)null);
	}

	private void ClearDriverDeployCooldown(EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicleComp = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(vehicle, ref vehicleComp) && vehicleComp.Operator.HasValue)
		{
			EntityUid driver = vehicleComp.Operator.Value;
			VehicleDeployActionComponent actionComp = default(VehicleDeployActionComponent);
			if (((EntitySystem)this).TryComp<VehicleDeployActionComponent>(driver, ref actionComp) && actionComp.Action.HasValue)
			{
				_actions.ClearCooldown(Entity<ActionComponent>.op_Implicit(actionComp.Action.Value));
			}
		}
	}

	private void OnHardpointSlotsChanged(HardpointSlotsChangedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
		if (!_net.IsClient && ((EntitySystem)this).TryComp<VehicleDeployableComponent>(args.Vehicle, ref deployable))
		{
			UpdateDriverActionState(args.Vehicle, deployable);
		}
	}

	private void OnDeployableAttemptShoot(Entity<HardpointItemComponent> ent, ref AttemptShootEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if ((!_net.IsClient || _timing.IsFirstTimePredicted) && !args.Cancelled && string.Equals(ent.Comp.HardpointType, "Cannon", StringComparison.OrdinalIgnoreCase))
		{
			VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
			VehicleDeployGatedHardpointsComponent gated = default(VehicleDeployGatedHardpointsComponent);
			if (!TryGetVehicleFromContained(ent.Owner, out var vehicle))
			{
				args.Cancelled = true;
				args.ResetCooldown = true;
			}
			else if (((EntitySystem)this).TryComp<VehicleDeployableComponent>(vehicle, ref deployable) && ((EntitySystem)this).TryComp<VehicleDeployGatedHardpointsComponent>(vehicle, ref gated) && IsBlockedHardpoint(gated, ent.Comp.HardpointType) && !deployable.Deployed)
			{
				args.Cancelled = true;
				args.ResetCooldown = true;
			}
		}
	}

	private static bool IsBlockedHardpoint(VehicleDeployGatedHardpointsComponent gated, string hardpointType)
	{
		if (string.IsNullOrWhiteSpace(hardpointType))
		{
			return false;
		}
		foreach (string blockedHardpoint in gated.BlockedHardpoints)
		{
			if (string.Equals(blockedHardpoint, hardpointType, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public override void Update(float frameTime)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0316: Unknown result type (might be due to invalid IL or missing references)
		((EntitySystem)this).Update(frameTime);
		if (_net.IsClient)
		{
			return;
		}
		EntityQueryEnumerator<VehicleDeployableComponent, TransformComponent> query = ((EntitySystem)this).EntityQueryEnumerator<VehicleDeployableComponent, TransformComponent>();
		EntityUid vehicle = default(EntityUid);
		VehicleDeployableComponent deployable = default(VehicleDeployableComponent);
		TransformComponent val = default(TransformComponent);
		VehicleWeaponsComponent weapons = default(VehicleWeaponsComponent);
		while (query.MoveNext(ref vehicle, ref deployable, ref val))
		{
			TimeSpan now = _timing.CurTime;
			if (deployable.Deploying)
			{
				if (now >= deployable.DeployEndTime)
				{
					bool finishedDeploy = deployable.DeployingTo;
					EntityUid? deployer = deployable.Deployer;
					deployable.Deploying = false;
					deployable.DeployingTo = false;
					deployable.DeployEndTime = TimeSpan.Zero;
					deployable.Deployed = finishedDeploy;
					if (!deployable.Deployed)
					{
						deployable.Deployer = null;
						deployable.TargetingDeployer = null;
						deployable.AutoTarget = null;
						deployable.AutoSpinInitialized = false;
					}
					((EntitySystem)this).Dirty(vehicle, (IComponent)(object)deployable, (MetaDataComponent)null);
					UpdateDriverActionState(vehicle, deployable);
					ClearDriverDeployCooldown(vehicle);
					string popupKey = (finishedDeploy ? "rmc-vehicle-deploy-finish" : "rmc-vehicle-undeploy-finish");
					if (deployer.HasValue)
					{
						string finishMsg = base.Loc.GetString(popupKey);
						_popup.PopupClient(finishMsg, deployer.Value, deployer.Value);
						SendDeployChat(deployer.Value, vehicle, finishMsg);
					}
					else
					{
						_popup.PopupEntity(base.Loc.GetString(popupKey), vehicle);
					}
				}
				continue;
			}
			if (!deployable.Deployed || !deployable.AutoTurretEnabled)
			{
				deployable.AutoSpinInitialized = false;
				continue;
			}
			EntityUid? operatorUid = null;
			if (((EntitySystem)this).TryComp<VehicleWeaponsComponent>(vehicle, ref weapons))
			{
				operatorUid = weapons.Operator;
			}
			if (operatorUid.HasValue && _combatMode.IsInCombatMode(operatorUid))
			{
				deployable.AutoSpinInitialized = false;
			}
			else
			{
				if (!TryFindAutoGun(vehicle, out EntityUid gunUid, out GunComponent gunComp))
				{
					continue;
				}
				if (deployable.AutoTarget.HasValue && !IsValidAutoTarget(vehicle, deployable, deployable.AutoTarget.Value, deployable.AutoTargetRange))
				{
					deployable.AutoTarget = null;
					deployable.NextAutoTargetTime = TimeSpan.Zero;
					deployable.AutoSpinInitialized = false;
				}
				if (!deployable.AutoTarget.HasValue && (deployable.NextAutoTargetTime == TimeSpan.Zero || now >= deployable.NextAutoTargetTime))
				{
					deployable.NextAutoTargetTime = now + TimeSpan.FromSeconds(Math.Max(0f, deployable.AutoTargetCooldown));
					deployable.AutoTarget = FindAutoTarget(vehicle, deployable, deployable.AutoTargetRange);
				}
				EntityUid? autoTarget = deployable.AutoTarget;
				if (autoTarget.HasValue)
				{
					EntityUid target = autoTarget.GetValueOrDefault();
					deployable.AutoSpinInitialized = false;
					if (!_turret.TryAimAtTarget(gunUid, target, out var targetCoords))
					{
						deployable.AutoTarget = null;
						deployable.NextAutoTargetTime = TimeSpan.Zero;
					}
					else if (HasAmmo(gunUid))
					{
						EntityUid? previousTarget = _guns.SwapTarget(Entity<GunComponent>.op_Implicit((gunUid, gunComp)), target);
						_guns.AttemptShoot(Entity<GunComponent>.op_Implicit((gunUid, gunComp)), vehicle, targetCoords);
						_guns.SwapTarget(Entity<GunComponent>.op_Implicit((gunUid, gunComp)), previousTarget);
					}
				}
				else if (deployable.AutoSpinSpeed > 0f)
				{
					if (!deployable.AutoSpinInitialized)
					{
						deployable.AutoSpinWorldRotation = GetTurretWorldRotation(gunUid, vehicle);
						deployable.AutoSpinInitialized = true;
					}
					Angle delta = Angle.FromDegrees((double)(deployable.AutoSpinSpeed * frameTime));
					VehicleDeployableComponent vehicleDeployableComponent = deployable;
					Angle val2 = deployable.AutoSpinWorldRotation + delta;
					vehicleDeployableComponent.AutoSpinWorldRotation = ((Angle)(ref val2)).Reduced();
					_turret.TrySetTargetRotationWorld(gunUid, deployable.AutoSpinWorldRotation);
				}
			}
		}
	}

	private bool TryFindAutoGun(EntityUid vehicle, out EntityUid gunUid, out GunComponent gunComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		gunUid = default(EntityUid);
		gunComp = null;
		EntityUid? fallbackGun = null;
		GunComponent fallbackComp = null;
		foreach (VehicleMountedSlot mountedSlot2 in _topology.GetMountedSlots(vehicle))
		{
			EntityUid? item = mountedSlot2.Item;
			if (!item.HasValue)
			{
				continue;
			}
			EntityUid installed = item.GetValueOrDefault();
			if (TryGetGunCandidate(installed, out EntityUid directGun, out GunComponent directComp))
			{
				if (HasAmmo(directGun))
				{
					gunUid = directGun;
					gunComp = directComp;
					return true;
				}
				EntityUid valueOrDefault = fallbackGun.GetValueOrDefault();
				if (!fallbackGun.HasValue)
				{
					valueOrDefault = directGun;
					fallbackGun = valueOrDefault;
				}
				if (fallbackComp == null)
				{
					fallbackComp = directComp;
				}
			}
		}
		if (fallbackGun.HasValue && fallbackComp != null)
		{
			gunUid = fallbackGun.Value;
			gunComp = fallbackComp;
			return true;
		}
		return false;
	}

	private bool TryGetGunCandidate(EntityUid uid, out EntityUid gunUid, out GunComponent gunComp)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		gunUid = uid;
		gunComp = null;
		GunComponent gun = default(GunComponent);
		if (!((EntitySystem)this).TryComp<GunComponent>(uid, ref gun) || !((EntitySystem)this).HasComp<VehicleTurretComponent>(uid))
		{
			return false;
		}
		gunComp = gun;
		return true;
	}

	private bool HasAmmo(EntityUid gunUid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<GunComponent>(gunUid))
		{
			return false;
		}
		GetAmmoCountEvent ammoEv = default(GetAmmoCountEvent);
		((EntitySystem)this).RaiseLocalEvent<GetAmmoCountEvent>(gunUid, ref ammoEv, false);
		if (ammoEv.Capacity > 0)
		{
			return ammoEv.Count > 0;
		}
		return true;
	}

	private EntityUid? FindAutoTarget(EntityUid vehicle, VehicleDeployableComponent deployable, float range)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (range <= 0f)
		{
			return null;
		}
		SentryTargetingComponent targeting = ((EntitySystem)this).EnsureComp<SentryTargetingComponent>(vehicle);
		if (deployable.Deployer.HasValue)
		{
			EntityUid? deployer = deployable.Deployer;
			EntityUid? targetingDeployer = deployable.TargetingDeployer;
			if (deployer.HasValue != targetingDeployer.HasValue || (deployer.HasValue && deployer.GetValueOrDefault() != targetingDeployer.GetValueOrDefault()))
			{
				_targeting.ApplyDeployerFactions(vehicle, deployable.Deployer.Value);
				deployable.TargetingDeployer = deployable.Deployer;
			}
		}
		MapCoordinates vehicleCoords = _transform.GetMapCoordinates(vehicle, (TransformComponent)null);
		float bestDistance = float.MaxValue;
		EntityUid? bestTarget = null;
		foreach (EntityUid candidate in _targeting.GetNearbyIffHostiles(Entity<SentryTargetingComponent>.op_Implicit((vehicle, targeting)), range))
		{
			if (!(candidate == vehicle) && IsValidAutoTarget(vehicle, deployable, candidate, range, targeting))
			{
				float distance = (_transform.GetMapCoordinates(candidate, (TransformComponent)null).Position - vehicleCoords.Position).LengthSquared();
				if (!(distance >= bestDistance))
				{
					bestDistance = distance;
					bestTarget = candidate;
				}
			}
		}
		return bestTarget;
	}

	private bool TryGetVehicleTurret(EntityUid vehicle, out EntityUid turretUid, HardpointSlotsComponent? hardpoints = null, ItemSlotsComponent? itemSlots = null)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _topology.TryGetPrimaryTurret(vehicle, out turretUid, hardpoints, itemSlots);
	}

	private bool TryGetVehicleFromContained(EntityUid contained, out EntityUid vehicle)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return _topology.TryGetVehicle(contained, out vehicle);
	}

	private bool IsValidAutoTarget(EntityUid vehicle, VehicleDeployableComponent deployable, EntityUid target, float range)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		SentryTargetingComponent targeting = default(SentryTargetingComponent);
		if (!((EntitySystem)this).TryComp<SentryTargetingComponent>(vehicle, ref targeting))
		{
			targeting = ((EntitySystem)this).EnsureComp<SentryTargetingComponent>(vehicle);
		}
		return IsValidAutoTarget(vehicle, deployable, target, range, targeting);
	}

	private bool IsValidAutoTarget(EntityUid vehicle, VehicleDeployableComponent deployable, EntityUid target, float range, SentryTargetingComponent targeting)
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		if (deployable.Deployer.HasValue)
		{
			EntityUid? deployer = deployable.Deployer;
			EntityUid? targetingDeployer = deployable.TargetingDeployer;
			if (deployer.HasValue != targetingDeployer.HasValue || (deployer.HasValue && deployer.GetValueOrDefault() != targetingDeployer.GetValueOrDefault()))
			{
				_targeting.ApplyDeployerFactions(vehicle, deployable.Deployer.Value);
				deployable.TargetingDeployer = deployable.Deployer;
			}
		}
		if (!((EntitySystem)this).Exists(target))
		{
			return false;
		}
		MobStateComponent mobState = default(MobStateComponent);
		if (((EntitySystem)this).TryComp<MobStateComponent>(target, ref mobState) && mobState.CurrentState == MobState.Dead)
		{
			return false;
		}
		if (!_targeting.IsValidTarget(Entity<SentryTargetingComponent>.op_Implicit((vehicle, targeting)), target))
		{
			return false;
		}
		EntityCoordinates targetCoords = ((EntitySystem)this).Transform(target).Coordinates;
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (TryGetVehicleTurret(vehicle, out var turretUid) && ((EntitySystem)this).TryComp<VehicleTurretComponent>(turretUid, ref turret) && _turret.TryGetTurretOrigin(turretUid, turret, out var originCoords))
		{
			MapCoordinates originMap = _transform.ToMapCoordinates(originCoords, true);
			MapCoordinates targetMap = _transform.ToMapCoordinates(targetCoords, true);
			if (!_interaction.InRangeUnobstructed(originMap, targetMap, range))
			{
				return false;
			}
		}
		else if (!_interaction.InRangeUnobstructed(vehicle, targetCoords, range))
		{
			return false;
		}
		return true;
	}

	private Angle GetTurretWorldRotation(EntityUid turretUid, EntityUid vehicle)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		VehicleTurretComponent turret = default(VehicleTurretComponent);
		if (!((EntitySystem)this).TryComp<VehicleTurretComponent>(turretUid, ref turret))
		{
			return _transform.GetWorldRotation(vehicle);
		}
		Angle vehicleRot = _transform.GetWorldRotation(vehicle);
		Angle val = turret.WorldRotation + vehicleRot;
		return ((Angle)(ref val)).Reduced();
	}

	private void SendDeployChat(EntityUid deployer, EntityUid vehicle, string message)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		ActorComponent actor = default(ActorComponent);
		if (((EntitySystem)this).TryComp<ActorComponent>(deployer, ref actor))
		{
			MetaDataComponent meta = default(MetaDataComponent);
			string name = (((EntitySystem)this).TryComp(vehicle, ref meta) ? meta.EntityName : base.Loc.GetString("entity-unknown-name"));
			name = FormattedMessage.EscapeText(name);
			string wrappedMessage = base.Loc.GetString("chat-manager-entity-say-wrap-message", new(string, object)[5]
			{
				("entityName", name),
				("verb", base.Loc.GetString("chat-speech-verb-default")),
				("fontType", "Default"),
				("fontSize", 12),
				("message", FormattedMessage.EscapeText(message))
			});
			_rmcChat.ChatMessageToOne(ChatChannel.Local, message, wrappedMessage, vehicle, hideChat: false, actor.PlayerSession.Channel);
		}
	}
}
