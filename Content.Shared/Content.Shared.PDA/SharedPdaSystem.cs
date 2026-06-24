using System;
using Content.Shared.Access.Components;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.PDA;

public abstract class SharedPdaSystem : EntitySystem
{
	[Dependency]
	protected ItemSlotsSystem ItemSlotsSystem;

	[Dependency]
	protected SharedAppearanceSystem Appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PdaComponent, ComponentInit>((ComponentEventHandler<PdaComponent, ComponentInit>)OnComponentInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PdaComponent, ComponentRemove>((ComponentEventHandler<PdaComponent, ComponentRemove>)OnComponentRemove, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PdaComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<PdaComponent, EntInsertedIntoContainerMessage>)OnItemInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PdaComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<PdaComponent, EntRemovedFromContainerMessage>)OnItemRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PdaComponent, GetAdditionalAccessEvent>((ComponentEventRefHandler<PdaComponent, GetAdditionalAccessEvent>)OnGetAdditionalAccess, (Type[])null, (Type[])null);
	}

	protected virtual void OnComponentInit(EntityUid uid, PdaComponent pda, ComponentInit args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (pda.IdCard != null)
		{
			pda.IdSlot.StartingItem = pda.IdCard;
		}
		ItemSlotsSystem.AddItemSlot(uid, "PDA-id", pda.IdSlot);
		ItemSlotsSystem.AddItemSlot(uid, "PDA-pen", pda.PenSlot);
		ItemSlotsSystem.AddItemSlot(uid, "PDA-pai", pda.PaiSlot);
		UpdatePdaAppearance(uid, pda);
	}

	private void OnComponentRemove(EntityUid uid, PdaComponent pda, ComponentRemove args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		ItemSlotsSystem.RemoveItemSlot(uid, pda.IdSlot);
		ItemSlotsSystem.RemoveItemSlot(uid, pda.PenSlot);
		ItemSlotsSystem.RemoveItemSlot(uid, pda.PaiSlot);
	}

	protected virtual void OnItemInserted(EntityUid uid, PdaComponent pda, EntInsertedIntoContainerMessage args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID == "PDA-id")
		{
			pda.ContainedId = ((ContainerModifiedMessage)args).Entity;
		}
		UpdatePdaAppearance(uid, pda);
	}

	protected virtual void OnItemRemoved(EntityUid uid, PdaComponent pda, EntRemovedFromContainerMessage args)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((ContainerModifiedMessage)args).Container.ID == pda.IdSlot.ID)
		{
			pda.ContainedId = null;
		}
		UpdatePdaAppearance(uid, pda);
	}

	private void OnGetAdditionalAccess(EntityUid uid, PdaComponent component, ref GetAdditionalAccessEvent args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? containedId = component.ContainedId;
		if (containedId.HasValue)
		{
			EntityUid id = containedId.GetValueOrDefault();
			args.Entities.Add(id);
		}
	}

	private void UpdatePdaAppearance(EntityUid uid, PdaComponent pda)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Appearance.SetData(uid, (Enum)PdaVisuals.IdCardInserted, (object)pda.ContainedId.HasValue, (AppearanceComponent)null);
	}

	public virtual void UpdatePdaUi(EntityUid uid, PdaComponent? pda = null)
	{
	}
}
