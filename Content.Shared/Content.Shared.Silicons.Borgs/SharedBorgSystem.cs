using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.IdentityManagement;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.PowerCell.Components;
using Content.Shared.Silicons.Borgs.Components;
using Content.Shared.UserInterface;
using Content.Shared.Wires;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Silicons.Borgs;

public abstract class SharedBorgSystem : EntitySystem
{
	[Dependency]
	protected SharedContainerSystem Container;

	[Dependency]
	protected ItemSlotsSystem ItemSlots;

	[Dependency]
	protected ItemToggleSystem Toggle;

	[Dependency]
	protected SharedPopupSystem Popup;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, ComponentStartup>((ComponentEventHandler<BorgChassisComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, ItemSlotInsertAttemptEvent>((ComponentEventRefHandler<BorgChassisComponent, ItemSlotInsertAttemptEvent>)OnItemSlotInsertAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, ItemSlotEjectAttemptEvent>((ComponentEventRefHandler<BorgChassisComponent, ItemSlotEjectAttemptEvent>)OnItemSlotEjectAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<BorgChassisComponent, EntInsertedIntoContainerMessage>)OnInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<BorgChassisComponent, EntRemovedFromContainerMessage>)OnRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<BorgChassisComponent, RefreshMovementSpeedModifiersEvent>)OnRefreshMovementSpeedModifiers, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, ActivatableUIOpenAttemptEvent>((ComponentEventHandler<BorgChassisComponent, ActivatableUIOpenAttemptEvent>)OnUIOpenAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<TryGetIdentityShortInfoEvent>((EntityEventHandler<TryGetIdentityShortInfoEvent>)OnTryGetIdentityShortInfo, (Type[])null, (Type[])null);
		InitializeRelay();
	}

	private void OnTryGetIdentityShortInfo(TryGetIdentityShortInfoEvent args)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ((EntitySystem)this).HasComp<BorgChassisComponent>(args.ForActor))
		{
			args.Title = ((EntitySystem)this).Name(args.ForActor, (MetaDataComponent)null).Trim();
			((HandledEntityEventArgs)args).Handled = true;
		}
	}

	private void OnItemSlotInsertAttempt(EntityUid uid, BorgChassisComponent component, ref ItemSlotInsertAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		PowerCellSlotComponent cellSlotComp = default(PowerCellSlotComponent);
		WiresPanelComponent panel = default(WiresPanelComponent);
		if (args.Cancelled || !((EntitySystem)this).TryComp<PowerCellSlotComponent>(uid, ref cellSlotComp) || !((EntitySystem)this).TryComp<WiresPanelComponent>(uid, ref panel) || !ItemSlots.TryGetSlot(uid, cellSlotComp.CellSlotId, out ItemSlot cellSlot) || cellSlot != args.Slot)
		{
			return;
		}
		if (panel.Open)
		{
			EntityUid? user = args.User;
			if (!user.HasValue || !(user.GetValueOrDefault() == uid))
			{
				return;
			}
		}
		args.Cancelled = true;
	}

	private void OnItemSlotEjectAttempt(EntityUid uid, BorgChassisComponent component, ref ItemSlotEjectAttemptEvent args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		PowerCellSlotComponent cellSlotComp = default(PowerCellSlotComponent);
		WiresPanelComponent panel = default(WiresPanelComponent);
		if (args.Cancelled || !((EntitySystem)this).TryComp<PowerCellSlotComponent>(uid, ref cellSlotComp) || !((EntitySystem)this).TryComp<WiresPanelComponent>(uid, ref panel) || !ItemSlots.TryGetSlot(uid, cellSlotComp.CellSlotId, out ItemSlot cellSlot) || cellSlot != args.Slot)
		{
			return;
		}
		if (panel.Open)
		{
			EntityUid? user = args.User;
			if (!user.HasValue || !(user.GetValueOrDefault() == uid))
			{
				return;
			}
		}
		args.Cancelled = true;
	}

	private void OnStartup(EntityUid uid, BorgChassisComponent component, ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ContainerManagerComponent containerManager = default(ContainerManagerComponent);
		if (((EntitySystem)this).TryComp<ContainerManagerComponent>(uid, ref containerManager))
		{
			component.BrainContainer = Container.EnsureContainer<ContainerSlot>(uid, component.BrainContainerId, containerManager);
			component.ModuleContainer = Container.EnsureContainer<Container>(uid, component.ModuleContainerId, containerManager);
		}
	}

	private void OnUIOpenAttempt(EntityUid uid, BorgChassisComponent component, ActivatableUIOpenAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if (args.User == uid)
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	protected virtual void OnInserted(EntityUid uid, BorgChassisComponent component, EntInsertedIntoContainerMessage args)
	{
	}

	protected virtual void OnRemoved(EntityUid uid, BorgChassisComponent component, EntRemovedFromContainerMessage args)
	{
	}

	private void OnRefreshMovementSpeedModifiers(EntityUid uid, BorgChassisComponent component, RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		MovementSpeedModifierComponent movement = default(MovementSpeedModifierComponent);
		if (!Toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(uid)) && ((EntitySystem)this).TryComp<MovementSpeedModifierComponent>(uid, ref movement))
		{
			float sprintDif = movement.BaseWalkSpeed / movement.BaseSprintSpeed;
			args.ModifySpeed(1f, sprintDif);
		}
	}

	public void SetBorgModuleDefault(Entity<BorgModuleComponent> ent, bool newDefault)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.DefaultModule = newDefault;
		((EntitySystem)this).Dirty<BorgModuleComponent>(ent, (MetaDataComponent)null);
	}

	public void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<BorgChassisComponent, DamageModifyEvent>((ComponentEventHandler<BorgChassisComponent, DamageModifyEvent>)RelayToModule, (Type[])null, (Type[])null);
	}

	protected void RelayToModule<T>(EntityUid uid, BorgChassisComponent component, T args) where T : class
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		BorgModuleRelayedEvent<T> ev = new BorgModuleRelayedEvent<T>(args);
		foreach (EntityUid module in ((BaseContainer)component.ModuleContainer).ContainedEntities)
		{
			((EntitySystem)this).RaiseLocalEvent<BorgModuleRelayedEvent<T>>(module, ref ev, false);
		}
	}

	protected void RelayRefToModule<T>(EntityUid uid, BorgChassisComponent component, ref T args) where T : class
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BorgModuleRelayedEvent<T> ev = new BorgModuleRelayedEvent<T>(args);
		foreach (EntityUid module in ((BaseContainer)component.ModuleContainer).ContainedEntities)
		{
			((EntitySystem)this).RaiseLocalEvent<BorgModuleRelayedEvent<T>>(module, ref ev, false);
		}
	}
}
