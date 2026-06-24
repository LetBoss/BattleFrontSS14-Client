using System;
using System.Diagnostics.CodeAnalysis;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Nutrition.EntitySystems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Cabinet;

public sealed class ItemCabinetSystem : EntitySystem
{
	[Dependency]
	private ItemSlotsSystem _slots;

	[Dependency]
	private OpenableSystem _openable;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemCabinetComponent, ComponentStartup>((EntityEventRefHandler<ItemCabinetComponent, ComponentStartup>)OnStartup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCabinetComponent, MapInitEvent>((EntityEventRefHandler<ItemCabinetComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCabinetComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ItemCabinetComponent, EntInsertedIntoContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCabinetComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ItemCabinetComponent, EntRemovedFromContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCabinetComponent, OpenableOpenedEvent>((EntityEventRefHandler<ItemCabinetComponent, OpenableOpenedEvent>)OnOpened, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ItemCabinetComponent, OpenableClosedEvent>((EntityEventRefHandler<ItemCabinetComponent, OpenableClosedEvent>)OnClosed, (Type[])null, (Type[])null);
	}

	private void OnStartup(Entity<ItemCabinetComponent> ent, ref ComponentStartup args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(ent);
	}

	private void OnMapInit(Entity<ItemCabinetComponent> ent, ref MapInitEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		SetSlotLock(ent, !_openable.IsOpen(Entity<ItemCabinetComponent>.op_Implicit(ent)));
	}

	private void UpdateAppearance(Entity<ItemCabinetComponent> ent)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_appearance.SetData(Entity<ItemCabinetComponent>.op_Implicit(ent), (Enum)ItemCabinetVisuals.ContainsItem, (object)HasItem(ent), (AppearanceComponent)null);
	}

	private void OnContainerModified(EntityUid uid, ItemCabinetComponent component, ContainerModifiedMessage args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (args.Container.ID == component.Slot)
		{
			UpdateAppearance(Entity<ItemCabinetComponent>.op_Implicit((uid, component)));
		}
	}

	private void OnOpened(Entity<ItemCabinetComponent> ent, ref OpenableOpenedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetSlotLock(ent, closed: false);
	}

	private void OnClosed(Entity<ItemCabinetComponent> ent, ref OpenableClosedEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetSlotLock(ent, closed: true);
	}

	public bool TryGetSlot(Entity<ItemCabinetComponent> ent, [NotNullWhen(true)] out ItemSlot? slot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		slot = null;
		ItemSlotsComponent slots = default(ItemSlotsComponent);
		if (!((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<ItemCabinetComponent>.op_Implicit(ent), ref slots))
		{
			return false;
		}
		return _slots.TryGetSlot(Entity<ItemCabinetComponent>.op_Implicit(ent), ent.Comp.Slot, out slot, slots);
	}

	public bool HasItem(Entity<ItemCabinetComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlot(ent, out ItemSlot slot))
		{
			return slot.HasItem;
		}
		return false;
	}

	public void SetSlotLock(Entity<ItemCabinetComponent> ent, bool closed)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsComponent slots = default(ItemSlotsComponent);
		if (((EntitySystem)this).TryComp<ItemSlotsComponent>(Entity<ItemCabinetComponent>.op_Implicit(ent), ref slots) && _slots.TryGetSlot(Entity<ItemCabinetComponent>.op_Implicit(ent), ent.Comp.Slot, out ItemSlot slot, slots))
		{
			_slots.SetLock(Entity<ItemCabinetComponent>.op_Implicit(ent), slot, closed, slots);
		}
	}
}
