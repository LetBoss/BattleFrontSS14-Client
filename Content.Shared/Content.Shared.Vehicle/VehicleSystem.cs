using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Access.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.Buckle.Components;
using Content.Shared.Damage;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Vehicle.Components;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Timing;

namespace Content.Shared.Vehicle;

public sealed class VehicleSystem : EntitySystem
{
	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private DamageableSystem _damageable;

	[Dependency]
	private EntityWhitelistSystem _entityWhitelist;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private IGameTiming _timing;

	public override void Initialize()
	{
		InitializeOperator();
		InitializeKey();
		((EntitySystem)this).SubscribeLocalEvent<VehicleComponent, BeforeDamageChangedEvent>((EntityEventRefHandler<VehicleComponent, BeforeDamageChangedEvent>)OnBeforeDamageChanged, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleComponent, UpdateCanMoveEvent>((EntityEventRefHandler<VehicleComponent, UpdateCanMoveEvent>)OnVehicleUpdateCanMove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleComponent, ComponentShutdown>((EntityEventRefHandler<VehicleComponent, ComponentShutdown>)OnVehicleShutdown, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleComponent, GetAdditionalAccessEvent>((EntityEventRefHandler<VehicleComponent, GetAdditionalAccessEvent>)OnVehicleGetAdditionalAccess, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<VehicleOperatorComponent, ComponentShutdown>((EntityEventRefHandler<VehicleOperatorComponent, ComponentShutdown>)OnOperatorShutdown, (Type[])null, (Type[])null);
	}

	private void OnBeforeDamageChanged(Entity<VehicleComponent> ent, ref BeforeDamageChangedEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (args.Cancelled || !ent.Comp.TransferDamage || !args.Damage.AnyPositive())
		{
			return;
		}
		EntityUid? val = ent.Comp.Operator;
		if (!val.HasValue)
		{
			return;
		}
		EntityUid operatorUid = val.GetValueOrDefault();
		MapId mapID = ((EntitySystem)this).Transform(ent.Owner).MapID;
		MapId operatorMap = ((EntitySystem)this).Transform(operatorUid).MapID;
		if (!(mapID != operatorMap))
		{
			DamageSpecifier damage = DamageSpecifier.GetPositive(args.Damage);
			DamageModifierSet modifierSet = ent.Comp.TransferDamageModifier;
			if (modifierSet != null)
			{
				damage = DamageSpecifier.ApplyModifierSet(damage, modifierSet);
			}
			_damageable.TryChangeDamage(operatorUid, damage, ignoreResistances: false, interruptsDoAfters: true, null, args.Origin);
		}
	}

	private void OnVehicleUpdateCanMove(Entity<VehicleComponent> ent, ref UpdateCanMoveEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		VehicleCanRunEvent ev = new VehicleCanRunEvent(ent);
		((EntitySystem)this).RaiseLocalEvent<VehicleCanRunEvent>(Entity<VehicleComponent>.op_Implicit(ent), ref ev, false);
		if (!ev.CanRun)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnVehicleShutdown(Entity<VehicleComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		TryRemoveOperator(ent);
	}

	private void OnVehicleGetAdditionalAccess(Entity<VehicleComponent> ent, ref GetAdditionalAccessEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? val = ent.Comp.Operator;
		if (val.HasValue)
		{
			EntityUid operatorUid = val.GetValueOrDefault();
			args.Entities.Add(operatorUid);
		}
	}

	private void OnOperatorShutdown(Entity<VehicleOperatorComponent> ent, ref ComponentShutdown args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TryRemoveOperator(Entity<VehicleOperatorComponent>.op_Implicit((Entity<VehicleOperatorComponent>.op_Implicit(ent), Entity<VehicleOperatorComponent>.op_Implicit(ent))));
	}

	public bool TrySetOperator(Entity<VehicleComponent> entity, EntityUid? uid, bool removeExisting = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		if (!entity.Comp.Operator.HasValue && !uid.HasValue)
		{
			return false;
		}
		VehicleOperatorComponent eOperator = default(VehicleOperatorComponent);
		EntityUid? vehicle;
		if (((EntitySystem)this).TryComp<VehicleOperatorComponent>(uid, ref eOperator))
		{
			vehicle = eOperator.Vehicle;
			EntityUid owner = entity.Owner;
			if (!vehicle.HasValue)
			{
				return false;
			}
			return vehicle.GetValueOrDefault() == owner;
		}
		if (!removeExisting)
		{
			vehicle = entity.Comp.Operator;
			if (vehicle.HasValue)
			{
				return false;
			}
		}
		if (uid.HasValue && !CanOperate(entity.AsNullable(), uid.Value))
		{
			return false;
		}
		EntityUid? oldOperator = entity.Comp.Operator;
		vehicle = entity.Comp.Operator;
		if (vehicle.HasValue)
		{
			EntityUid currentOperator = vehicle.GetValueOrDefault();
			VehicleOperatorComponent currentOperatorComponent = default(VehicleOperatorComponent);
			if (((EntitySystem)this).TryComp<VehicleOperatorComponent>(currentOperator, ref currentOperatorComponent))
			{
				OnVehicleExitedEvent exitEvent = new OnVehicleExitedEvent(entity, currentOperator);
				((EntitySystem)this).RaiseLocalEvent<OnVehicleExitedEvent>(currentOperator, ref exitEvent, false);
				currentOperatorComponent.Vehicle = null;
				((EntitySystem)this).RemCompDeferred<VehicleOperatorComponent>(currentOperator);
				((EntitySystem)this).RemCompDeferred<RelayInputMoverComponent>(currentOperator);
				((EntitySystem)this).RemCompDeferred<GridVehicleOperatorComponent>(currentOperator);
			}
		}
		entity.Comp.Operator = uid;
		if (uid.HasValue)
		{
			VehicleOperatorComponent vehicleOperator = ((EntitySystem)this).AddComp<VehicleOperatorComponent>(uid.Value);
			vehicleOperator.Vehicle = entity.Owner;
			((EntitySystem)this).Dirty(uid.Value, (IComponent)(object)vehicleOperator, (MetaDataComponent)null);
			if (entity.Comp.MovementKind == VehicleMovementKind.Standard)
			{
				_mover.SetRelay(uid.Value, Entity<VehicleComponent>.op_Implicit(entity));
			}
			else
			{
				VehicleMovementKind movementKind = entity.Comp.MovementKind;
				if (movementKind - 1 <= VehicleMovementKind.Free)
				{
					((EntitySystem)this).EnsureComp<GridVehicleMoverComponent>(entity.Owner);
					((EntitySystem)this).EnsureComp<GridVehicleOperatorComponent>(uid.Value);
					((EntitySystem)this).RemCompDeferred<RelayInputMoverComponent>(uid.Value);
					((EntitySystem)this).RemCompDeferred<MovementRelayTargetComponent>(Entity<VehicleComponent>.op_Implicit(entity));
				}
			}
			OnVehicleEnteredEvent enterEvent = new OnVehicleEnteredEvent(entity, uid.Value);
			((EntitySystem)this).RaiseLocalEvent<OnVehicleEnteredEvent>(uid.Value, ref enterEvent, false);
		}
		else
		{
			((EntitySystem)this).RemCompDeferred<MovementRelayTargetComponent>(Entity<VehicleComponent>.op_Implicit(entity));
		}
		RefreshCanRun(Entity<VehicleComponent>.op_Implicit((Entity<VehicleComponent>.op_Implicit(entity), entity.Comp)));
		VehicleOperatorSetEvent setEvent = new VehicleOperatorSetEvent(uid, oldOperator);
		((EntitySystem)this).RaiseLocalEvent<VehicleOperatorSetEvent>(Entity<VehicleComponent>.op_Implicit(entity), ref setEvent, false);
		((EntitySystem)this).Dirty<VehicleComponent>(entity, (MetaDataComponent)null);
		return true;
	}

	public bool TryRemoveOperator(Entity<VehicleComponent> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TrySetOperator(entity, null);
	}

	public bool TryRemoveOperator(Entity<VehicleOperatorComponent?> operatorEntity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VehicleOperatorComponent>(Entity<VehicleOperatorComponent>.op_Implicit(operatorEntity), ref operatorEntity.Comp, false))
		{
			return true;
		}
		VehicleComponent vehicle = default(VehicleComponent);
		if (!((EntitySystem)this).TryComp<VehicleComponent>(operatorEntity.Comp.Vehicle, ref vehicle))
		{
			return true;
		}
		return TrySetOperator(Entity<VehicleComponent>.op_Implicit((operatorEntity.Comp.Vehicle.Value, vehicle)), null);
	}

	public bool TryGetOperator(Entity<VehicleComponent?> entity, [NotNullWhen(true)] out Entity<VehicleOperatorComponent>? operatorEnt)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		operatorEnt = null;
		if (!((EntitySystem)this).Resolve<VehicleComponent>(Entity<VehicleComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		EntityUid? val = entity.Comp.Operator;
		if (val.HasValue)
		{
			EntityUid operatorUid = val.GetValueOrDefault();
			VehicleOperatorComponent operatorComponent = default(VehicleOperatorComponent);
			if (!((EntitySystem)this).TryComp<VehicleOperatorComponent>(operatorUid, ref operatorComponent))
			{
				return false;
			}
			operatorEnt = Entity<VehicleOperatorComponent>.op_Implicit((operatorUid, operatorComponent));
			return true;
		}
		return false;
	}

	public EntityUid? GetOperatorOrNull(Entity<VehicleComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		TryGetOperator(entity, out Entity<VehicleOperatorComponent>? operatorEnt);
		Entity<VehicleOperatorComponent>? val = operatorEnt;
		if (!val.HasValue)
		{
			return null;
		}
		return Entity<VehicleOperatorComponent>.op_Implicit(val.GetValueOrDefault());
	}

	public bool HasOperator(Entity<VehicleComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Entity<VehicleOperatorComponent>? operatorEnt;
		return TryGetOperator(entity, out operatorEnt);
	}

	public bool CanOperate(Entity<VehicleComponent?> entity, EntityUid uid)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<VehicleComponent>(Entity<VehicleComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			return false;
		}
		if (_entityWhitelist.IsWhitelistFail(entity.Comp.OperatorWhitelist, uid))
		{
			return false;
		}
		return _actionBlocker.CanConsciouslyPerformAction(uid);
	}

	public void RefreshCanRun(Entity<VehicleComponent?> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).TerminatingOrDeleted(Entity<VehicleComponent>.op_Implicit(entity), (MetaDataComponent)null) && ((EntitySystem)this).Resolve<VehicleComponent>(Entity<VehicleComponent>.op_Implicit(entity), ref entity.Comp, true))
		{
			_actionBlocker.UpdateCanMove(Entity<VehicleComponent>.op_Implicit(entity));
			UpdateAppearance(Entity<VehicleComponent>.op_Implicit((Entity<VehicleComponent>.op_Implicit(entity), entity.Comp)));
		}
	}

	private void UpdateAppearance(Entity<VehicleComponent> entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		AppearanceComponent appearance = default(AppearanceComponent);
		if (((EntitySystem)this).TryComp<AppearanceComponent>(Entity<VehicleComponent>.op_Implicit(entity), ref appearance))
		{
			InputMoverComponent inputMover = default(InputMoverComponent);
			if (((EntitySystem)this).TryComp<InputMoverComponent>(Entity<VehicleComponent>.op_Implicit(entity), ref inputMover))
			{
				_appearance.SetData(Entity<VehicleComponent>.op_Implicit(entity), (Enum)VehicleVisuals.CanRun, (object)inputMover.CanMove, appearance);
			}
			SharedAppearanceSystem appearance2 = _appearance;
			EntityUid val = Entity<VehicleComponent>.op_Implicit(entity);
			object obj = VehicleVisuals.HasOperator;
			EntityUid? val2 = entity.Comp.Operator;
			appearance2.SetData(val, (Enum)obj, (object)val2.HasValue, appearance);
		}
	}

	public void InitializeKey()
	{
		((EntitySystem)this).SubscribeLocalEvent<GenericKeyedVehicleComponent, ContainerIsInsertingAttemptEvent>((EntityEventRefHandler<GenericKeyedVehicleComponent, ContainerIsInsertingAttemptEvent>)OnGenericKeyedInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenericKeyedVehicleComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<GenericKeyedVehicleComponent, EntInsertedIntoContainerMessage>)OnGenericKeyedEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenericKeyedVehicleComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<GenericKeyedVehicleComponent, EntRemovedFromContainerMessage>)OnGenericKeyedEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<GenericKeyedVehicleComponent, VehicleCanRunEvent>((EntityEventRefHandler<GenericKeyedVehicleComponent, VehicleCanRunEvent>)OnGenericKeyedCanRun, (Type[])null, (Type[])null);
	}

	private void OnGenericKeyedInsertAttempt(Entity<GenericKeyedVehicleComponent> ent, ref ContainerIsInsertingAttemptEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !_timing.ApplyingState && ent.Comp.PreventInvalidInsertion && !(((ContainerAttemptEventBase)args).Container.ID != ent.Comp.ContainerId) && !_entityWhitelist.IsWhitelistPass(ent.Comp.KeyWhitelist, ((ContainerAttemptEventBase)args).EntityUid))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnGenericKeyedEntInserted(Entity<GenericKeyedVehicleComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId))
		{
			RefreshCanRun(Entity<VehicleComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnGenericKeyedEntRemoved(Entity<GenericKeyedVehicleComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!_timing.ApplyingState && !(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId))
		{
			RefreshCanRun(Entity<VehicleComponent>.op_Implicit(ent.Owner));
		}
	}

	private void OnGenericKeyedCanRun(Entity<GenericKeyedVehicleComponent> ent, ref VehicleCanRunEvent args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (!args.CanRun)
		{
			return;
		}
		args.CanRun = false;
		BaseContainer container = default(BaseContainer);
		if (!_container.TryGetContainer(ent.Owner, ent.Comp.ContainerId, ref container, (ContainerManagerComponent)null))
		{
			return;
		}
		foreach (EntityUid contained in container.ContainedEntities)
		{
			if (!_entityWhitelist.IsWhitelistFail(ent.Comp.KeyWhitelist, contained))
			{
				args.CanRun = true;
				break;
			}
		}
	}

	public void InitializeOperator()
	{
		((EntitySystem)this).SubscribeLocalEvent<StrapVehicleComponent, StrappedEvent>((EntityEventRefHandler<StrapVehicleComponent, StrappedEvent>)OnVehicleStrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrapVehicleComponent, UnstrappedEvent>((EntityEventRefHandler<StrapVehicleComponent, UnstrappedEvent>)OnVehicleUnstrapped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainerVehicleComponent, EntInsertedIntoContainerMessage>((EntityEventRefHandler<ContainerVehicleComponent, EntInsertedIntoContainerMessage>)OnContainerEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ContainerVehicleComponent, EntRemovedFromContainerMessage>((EntityEventRefHandler<ContainerVehicleComponent, EntRemovedFromContainerMessage>)OnContainerEntRemoved, (Type[])null, (Type[])null);
	}

	private void OnVehicleStrapped(Entity<StrapVehicleComponent> ent, ref StrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicle = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(Entity<StrapVehicleComponent>.op_Implicit(ent), ref vehicle))
		{
			TrySetOperator(Entity<VehicleComponent>.op_Implicit((Entity<StrapVehicleComponent>.op_Implicit(ent), vehicle)), Entity<BuckleComponent>.op_Implicit(args.Buckle));
		}
	}

	private void OnVehicleUnstrapped(Entity<StrapVehicleComponent> ent, ref UnstrappedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicle = default(VehicleComponent);
		if (((EntitySystem)this).TryComp<VehicleComponent>(Entity<StrapVehicleComponent>.op_Implicit(ent), ref vehicle))
		{
			TrySetOperator(Entity<VehicleComponent>.op_Implicit((Entity<StrapVehicleComponent>.op_Implicit(ent), vehicle)), null);
		}
	}

	private void OnContainerEntInserted(Entity<ContainerVehicleComponent> ent, ref EntInsertedIntoContainerMessage args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicle = default(VehicleComponent);
		if (!_timing.ApplyingState && !(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId) && ((EntitySystem)this).TryComp<VehicleComponent>(Entity<ContainerVehicleComponent>.op_Implicit(ent), ref vehicle))
		{
			TrySetOperator(Entity<VehicleComponent>.op_Implicit((Entity<ContainerVehicleComponent>.op_Implicit(ent), vehicle)), ((ContainerModifiedMessage)args).Entity, removeExisting: false);
		}
	}

	private void OnContainerEntRemoved(Entity<ContainerVehicleComponent> ent, ref EntRemovedFromContainerMessage args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		VehicleComponent vehicle = default(VehicleComponent);
		if (!_timing.ApplyingState && !(((ContainerModifiedMessage)args).Container.ID != ent.Comp.ContainerId) && ((EntitySystem)this).TryComp<VehicleComponent>(Entity<ContainerVehicleComponent>.op_Implicit(ent), ref vehicle))
		{
			EntityUid? val = vehicle.Operator;
			EntityUid entity = ((ContainerModifiedMessage)args).Entity;
			if (val.HasValue && !(val.GetValueOrDefault() != entity))
			{
				TryRemoveOperator(Entity<VehicleComponent>.op_Implicit((Entity<ContainerVehicleComponent>.op_Implicit(ent), vehicle)));
			}
		}
	}
}
