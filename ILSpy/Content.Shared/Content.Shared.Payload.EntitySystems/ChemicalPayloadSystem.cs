using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Payload.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Payload.EntitySystems;

public sealed class ChemicalPayloadSystem : EntitySystem
{
	[Dependency]
	private ItemSlotsSystem _itemSlotsSystem;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ChemicalPayloadComponent, ComponentInit>((ComponentEventHandler<ChemicalPayloadComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChemicalPayloadComponent, ComponentRemove>((ComponentEventHandler<ChemicalPayloadComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChemicalPayloadComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<ChemicalPayloadComponent, EntInsertedIntoContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ChemicalPayloadComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<ChemicalPayloadComponent, EntRemovedFromContainerMessage>)OnContainerModified, (Type[])null, (Type[])null);
	}

	private void OnContainerModified(EntityUid uid, ChemicalPayloadComponent component, ContainerModifiedMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateAppearance(uid, component);
	}

	private void UpdateAppearance(EntityUid uid, ChemicalPayloadComponent? component = null, AppearanceComponent? appearance = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ChemicalPayloadComponent, AppearanceComponent>(uid, ref component, ref appearance, false))
		{
			ChemicalPayloadFilledSlots filled = ChemicalPayloadFilledSlots.None;
			if (component.BeakerSlotA.HasItem)
			{
				filled |= ChemicalPayloadFilledSlots.Left;
			}
			if (component.BeakerSlotB.HasItem)
			{
				filled |= ChemicalPayloadFilledSlots.Right;
			}
			_appearance.SetData(uid, (Enum)ChemicalPayloadVisuals.Slots, (object)filled, appearance);
		}
	}

	private void OnComponentInit(EntityUid uid, ChemicalPayloadComponent payload, ComponentInit args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.AddItemSlot(uid, "BeakerSlotA", payload.BeakerSlotA);
		_itemSlotsSystem.AddItemSlot(uid, "BeakerSlotB", payload.BeakerSlotB);
	}

	private void OnComponentRemove(EntityUid uid, ChemicalPayloadComponent payload, ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		_itemSlotsSystem.RemoveItemSlot(uid, payload.BeakerSlotA);
		_itemSlotsSystem.RemoveItemSlot(uid, payload.BeakerSlotB);
	}
}
