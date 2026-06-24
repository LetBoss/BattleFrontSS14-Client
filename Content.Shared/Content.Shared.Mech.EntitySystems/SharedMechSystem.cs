using System;
using System.Collections.Generic;
using System.Linq;
using Content.Shared.Access.Components;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.DragDrop;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mech.Components;
using Content.Shared.Mech.Equipment.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Mech.EntitySystems;

public abstract class SharedMechSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private INetManager _net;

	[Dependency]
	private ActionBlockerSystem _actionBlocker;

	[Dependency]
	private SharedActionsSystem _actions;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	[Dependency]
	private SharedContainerSystem _container;

	[Dependency]
	private SharedInteractionSystem _interaction;

	[Dependency]
	private SharedMoverController _mover;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, MechToggleEquipmentEvent>((ComponentEventHandler<MechComponent, MechToggleEquipmentEvent>)OnToggleEquipmentAction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, MechEjectPilotEvent>((ComponentEventHandler<MechComponent, MechEjectPilotEvent>)OnEjectPilotEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, UserActivateInWorldEvent>((ComponentEventHandler<MechComponent, UserActivateInWorldEvent>)RelayInteractionEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, ComponentStartup>((ComponentEventHandler<MechComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, DestructionEventArgs>((ComponentEventHandler<MechComponent, DestructionEventArgs>)OnDestruction, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, GetAdditionalAccessEvent>((ComponentEventRefHandler<MechComponent, GetAdditionalAccessEvent>)OnGetAdditionalAccess, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, DragDropTargetEvent>((ComponentEventRefHandler<MechComponent, DragDropTargetEvent>)OnDragDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, CanDropTargetEvent>((ComponentEventRefHandler<MechComponent, CanDropTargetEvent>)OnCanDragDrop, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechPilotComponent, GetMeleeWeaponEvent>((ComponentEventHandler<MechPilotComponent, GetMeleeWeaponEvent>)OnGetMeleeWeapon, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechPilotComponent, CanAttackFromContainerEvent>((ComponentEventHandler<MechPilotComponent, CanAttackFromContainerEvent>)OnCanAttackFromContainer, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MechPilotComponent, AttackAttemptEvent>((ComponentEventHandler<MechPilotComponent, AttackAttemptEvent>)OnAttackAttempt, (Type[])null, (Type[])null);
		InitializeRelay();
	}

	private void OnToggleEquipmentAction(EntityUid uid, MechComponent component, MechToggleEquipmentEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			CycleEquipment(uid);
		}
	}

	private void OnEjectPilotEvent(EntityUid uid, MechComponent component, MechEjectPilotEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled)
		{
			((HandledEntityEventArgs)args).Handled = true;
			TryEject(uid, component);
		}
	}

	private void RelayInteractionEvent(EntityUid uid, MechComponent component, UserActivateInWorldEvent args)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (component.PilotSlot.ContainedEntity.HasValue && _timing.IsFirstTimePredicted && component.CurrentSelectedEquipment.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<UserActivateInWorldEvent>(component.CurrentSelectedEquipment.Value, args, false);
		}
	}

	private void OnStartup(EntityUid uid, MechComponent component, ComponentStartup args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		component.PilotSlot = _container.EnsureContainer<ContainerSlot>(uid, component.PilotSlotId, (ContainerManagerComponent)null);
		component.EquipmentContainer = _container.EnsureContainer<Container>(uid, component.EquipmentContainerId, (ContainerManagerComponent)null);
		component.BatterySlot = _container.EnsureContainer<ContainerSlot>(uid, component.BatterySlotId, (ContainerManagerComponent)null);
		UpdateAppearance(uid, component);
	}

	private void OnDestruction(EntityUid uid, MechComponent component, DestructionEventArgs args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		BreakMech(uid, component);
	}

	private void OnGetAdditionalAccess(EntityUid uid, MechComponent component, ref GetAdditionalAccessEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? pilot = component.PilotSlot.ContainedEntity;
		if (pilot.HasValue)
		{
			args.Entities.Add(pilot.Value);
		}
	}

	private void SetupUser(EntityUid mech, EntityUid pilot, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MechComponent>(mech, ref component, true))
		{
			MechPilotComponent rider = ((EntitySystem)this).EnsureComp<MechPilotComponent>(pilot);
			InteractionRelayComponent irelay = ((EntitySystem)this).EnsureComp<InteractionRelayComponent>(pilot);
			_mover.SetRelay(pilot, mech);
			_interaction.SetRelay(pilot, mech, irelay);
			rider.Mech = mech;
			((EntitySystem)this).Dirty(pilot, (IComponent)(object)rider, (MetaDataComponent)null);
			if (!_net.IsClient)
			{
				_actions.AddAction(pilot, ref component.MechCycleActionEntity, EntProtoId.op_Implicit(component.MechCycleAction), mech);
				_actions.AddAction(pilot, ref component.MechUiActionEntity, EntProtoId.op_Implicit(component.MechUiAction), mech);
				_actions.AddAction(pilot, ref component.MechEjectActionEntity, EntProtoId.op_Implicit(component.MechEjectAction), mech);
			}
		}
	}

	private void RemoveUser(EntityUid mech, EntityUid pilot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).RemComp<MechPilotComponent>(pilot))
		{
			((EntitySystem)this).RemComp<RelayInputMoverComponent>(pilot);
			((EntitySystem)this).RemComp<InteractionRelayComponent>(pilot);
			_actions.RemoveProvidedActions(pilot, mech);
		}
	}

	public virtual void BreakMech(EntityUid uid, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			return;
		}
		TryEject(uid, component);
		foreach (EntityUid ent in new List<EntityUid>(((BaseContainer)component.EquipmentContainer).ContainedEntities))
		{
			RemoveEquipment(uid, ent, component, null, forced: true);
		}
		component.Broken = true;
		UpdateAppearance(uid, component);
	}

	public void CycleEquipment(EntityUid uid, MechComponent? component = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			List<EntityUid> allEquipment = ((BaseContainer)component.EquipmentContainer).ContainedEntities.ToList();
			int equipmentIndex = -1;
			if (component.CurrentSelectedEquipment.HasValue)
			{
				equipmentIndex = allEquipment.FindIndex(StartIndex);
			}
			equipmentIndex++;
			component.CurrentSelectedEquipment = ((equipmentIndex >= allEquipment.Count) ? ((EntityUid?)null) : new EntityUid?(allEquipment[equipmentIndex]));
			string popupString = (component.CurrentSelectedEquipment.HasValue ? base.Loc.GetString("mech-equipment-select-popup", (ValueTuple<string, object>)("item", component.CurrentSelectedEquipment)) : base.Loc.GetString("mech-equipment-select-none-popup"));
			if (_net.IsServer)
			{
				_popup.PopupEntity(popupString, uid);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		}
		bool StartIndex(EntityUid u)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			EntityUid? currentSelectedEquipment = component.CurrentSelectedEquipment;
			if (!currentSelectedEquipment.HasValue)
			{
				return false;
			}
			return u == currentSelectedEquipment.GetValueOrDefault();
		}
	}

	public void InsertEquipment(EntityUid uid, EntityUid toInsert, MechComponent? component = null, MechEquipmentComponent? equipmentComponent = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true) && ((EntitySystem)this).Resolve<MechEquipmentComponent>(toInsert, ref equipmentComponent, true) && ((BaseContainer)component.EquipmentContainer).ContainedEntities.Count < component.MaxEquipmentAmount && !_whitelistSystem.IsWhitelistFail(component.EquipmentWhitelist, toInsert))
		{
			equipmentComponent.EquipmentOwner = uid;
			_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toInsert), (BaseContainer)(object)component.EquipmentContainer, (TransformComponent)null, false);
			MechEquipmentInsertedEvent ev = new MechEquipmentInsertedEvent(uid);
			((EntitySystem)this).RaiseLocalEvent<MechEquipmentInsertedEvent>(toInsert, ref ev, false);
			UpdateUserInterface(uid, component);
		}
	}

	public void RemoveEquipment(EntityUid uid, EntityUid toRemove, MechComponent? component = null, MechEquipmentComponent? equipmentComponent = null, bool forced = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true) || !((EntitySystem)this).Resolve<MechEquipmentComponent>(toRemove, ref equipmentComponent, true))
		{
			return;
		}
		if (!forced)
		{
			AttemptRemoveMechEquipmentEvent attemptev = new AttemptRemoveMechEquipmentEvent();
			((EntitySystem)this).RaiseLocalEvent<AttemptRemoveMechEquipmentEvent>(toRemove, ref attemptev, false);
			if (attemptev.Cancelled)
			{
				return;
			}
		}
		MechEquipmentRemovedEvent ev = new MechEquipmentRemovedEvent(uid);
		((EntitySystem)this).RaiseLocalEvent<MechEquipmentRemovedEvent>(toRemove, ref ev, false);
		EntityUid? currentSelectedEquipment = component.CurrentSelectedEquipment;
		if (currentSelectedEquipment.HasValue && currentSelectedEquipment.GetValueOrDefault() == toRemove)
		{
			CycleEquipment(uid, component);
		}
		equipmentComponent.EquipmentOwner = null;
		_container.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(toRemove), (BaseContainer)(object)component.EquipmentContainer, true, false, (EntityCoordinates?)null, (Angle?)null);
		UpdateUserInterface(uid, component);
	}

	public virtual bool TryChangeEnergy(EntityUid uid, FixedPoint2 delta, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			return false;
		}
		if (component.Energy + delta < 0)
		{
			return false;
		}
		component.Energy = FixedPoint2.Clamp(component.Energy + delta, 0, component.MaxEnergy);
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		UpdateUserInterface(uid, component);
		return true;
	}

	public void SetIntegrity(EntityUid uid, FixedPoint2 value, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			component.Integrity = FixedPoint2.Clamp(value, 0, component.MaxIntegrity);
			if (component.Integrity <= 0)
			{
				BreakMech(uid, component);
			}
			else if (component.Broken)
			{
				component.Broken = false;
				UpdateAppearance(uid, component);
			}
			((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
			UpdateUserInterface(uid, component);
		}
	}

	public bool IsEmpty(MechComponent component)
	{
		return !component.PilotSlot.ContainedEntity.HasValue;
	}

	public bool CanInsert(EntityUid uid, EntityUid toInsert, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			return false;
		}
		if (IsEmpty(component))
		{
			return _actionBlocker.CanMove(toInsert);
		}
		return false;
	}

	public virtual void UpdateUserInterface(EntityUid uid, MechComponent? component = null)
	{
	}

	public bool TryInsert(EntityUid uid, EntityUid? toInsert, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			return false;
		}
		if (toInsert.HasValue)
		{
			EntityUid? containedEntity = component.PilotSlot.ContainedEntity;
			EntityUid? val = toInsert;
			if (containedEntity.HasValue != val.HasValue || (containedEntity.HasValue && !(containedEntity.GetValueOrDefault() == val.GetValueOrDefault())))
			{
				if (!CanInsert(uid, toInsert.Value, component))
				{
					return false;
				}
				SetupUser(uid, toInsert.Value);
				_container.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(toInsert.Value), (BaseContainer)(object)component.PilotSlot, (TransformComponent)null, false);
				UpdateAppearance(uid, component);
				return true;
			}
		}
		return false;
	}

	public bool TryEject(EntityUid uid, MechComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<MechComponent>(uid, ref component, true))
		{
			return false;
		}
		if (!component.PilotSlot.ContainedEntity.HasValue)
		{
			return false;
		}
		EntityUid pilot = component.PilotSlot.ContainedEntity.Value;
		RemoveUser(uid, pilot);
		_container.RemoveEntity(uid, pilot, (ContainerManagerComponent)null, (TransformComponent)null, (MetaDataComponent)null, true, false, (EntityCoordinates?)null, (Angle?)null);
		UpdateAppearance(uid, component);
		return true;
	}

	private void OnGetMeleeWeapon(EntityUid uid, MechPilotComponent component, GetMeleeWeaponEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		MechComponent mech = default(MechComponent);
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).TryComp<MechComponent>(component.Mech, ref mech))
		{
			EntityUid weapon = (EntityUid)(((_003F?)mech.CurrentSelectedEquipment) ?? component.Mech);
			args.Weapon = weapon;
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnCanAttackFromContainer(EntityUid uid, MechPilotComponent component, CanAttackFromContainerEvent args)
	{
		args.CanAttack = true;
	}

	private void OnAttackAttempt(EntityUid uid, MechPilotComponent component, AttackAttemptEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? target = args.Target;
		EntityUid mech = component.Mech;
		if (target.HasValue && target.GetValueOrDefault() == mech)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void UpdateAppearance(EntityUid uid, MechComponent? component = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<MechComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
		{
			_appearance.SetData(uid, (Enum)MechVisuals.Open, (object)IsEmpty(component), appearance);
			_appearance.SetData(uid, (Enum)MechVisuals.Broken, (object)component.Broken, appearance);
		}
	}

	private void OnDragDrop(EntityUid uid, MechComponent component, ref DragDropTargetEvent args)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!args.Handled)
		{
			args.Handled = true;
			DoAfterArgs doAfterEventArgs = new DoAfterArgs((IEntityManager)(object)base.EntityManager, args.Dragged, component.EntryDelay, new MechEntryEvent(), uid, uid)
			{
				BreakOnMove = true
			};
			_doAfter.TryStartDoAfter(doAfterEventArgs);
		}
	}

	private void OnCanDragDrop(EntityUid uid, MechComponent component, ref CanDropTargetEvent args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		args.Handled = true;
		args.CanDrop |= !component.Broken && CanInsert(uid, args.Dragged, component);
	}

	private void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<MechComponent, GettingAttackedAttemptEvent>((EntityEventRefHandler<MechComponent, GettingAttackedAttemptEvent>)RelayRefToPilot, (Type[])null, (Type[])null);
	}

	private void RelayToPilot<T>(Entity<MechComponent> uid, T args) where T : class
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? containedEntity = uid.Comp.PilotSlot.ContainedEntity;
		if (containedEntity.HasValue)
		{
			EntityUid pilot = containedEntity.GetValueOrDefault();
			MechPilotRelayedEvent<T> ev = new MechPilotRelayedEvent<T>(args);
			((EntitySystem)this).RaiseLocalEvent<MechPilotRelayedEvent<T>>(pilot, ref ev, false);
		}
	}

	private void RelayRefToPilot<T>(Entity<MechComponent> uid, ref T args) where T : struct
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? containedEntity = uid.Comp.PilotSlot.ContainedEntity;
		if (containedEntity.HasValue)
		{
			EntityUid pilot = containedEntity.GetValueOrDefault();
			MechPilotRelayedEvent<T> ev = new MechPilotRelayedEvent<T>(args);
			((EntitySystem)this).RaiseLocalEvent<MechPilotRelayedEvent<T>>(pilot, ref ev, false);
			args = ev.Args;
		}
	}
}
