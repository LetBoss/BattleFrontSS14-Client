using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.PowerCell.Components;
using Content.Shared.Rejuvenate;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.PowerCell;

public abstract class SharedPowerCellSystem : EntitySystem
{
	[Dependency]
	protected IGameTiming Timing;

	[Dependency]
	private ItemSlotsSystem _itemSlots;

	[Dependency]
	private SharedAppearanceSystem _appearance;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<PowerCellDrawComponent, MapInitEvent>((EntityEventRefHandler<PowerCellDrawComponent, MapInitEvent>)OnMapInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerCellSlotComponent, RejuvenateEvent>((ComponentEventHandler<PowerCellSlotComponent, RejuvenateEvent>)OnRejuvenate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerCellSlotComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<PowerCellSlotComponent, EntInsertedIntoContainerMessage>)OnCellInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerCellSlotComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<PowerCellSlotComponent, EntRemovedFromContainerMessage>)OnCellRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PowerCellSlotComponent, ContainerIsInsertingAttemptEvent>((ComponentEventHandler<PowerCellSlotComponent, ContainerIsInsertingAttemptEvent>)OnCellInsertAttempt, (Type[])null, (Type[])null);
	}

	private void OnMapInit(Entity<PowerCellDrawComponent> ent, ref MapInitEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.NextUpdateTime = Timing.CurTime + ent.Comp.Delay;
	}

	private void OnRejuvenate(EntityUid uid, PowerCellSlotComponent component, RejuvenateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (_itemSlots.TryGetSlot(uid, component.CellSlotId, out ItemSlot itemSlot) && itemSlot.Item.HasValue)
		{
			((EntitySystem)this).RaiseLocalEvent<RejuvenateEvent>(itemSlot.Item.Value, args, false);
		}
	}

	private void OnCellInsertAttempt(EntityUid uid, PowerCellSlotComponent component, ContainerIsInsertingAttemptEvent args)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)component).Initialized && !(((ContainerAttemptEventBase)args).Container.ID != component.CellSlotId) && !((EntitySystem)this).HasComp<PowerCellComponent>(((ContainerAttemptEventBase)args).EntityUid))
		{
			((CancellableEntityEventArgs)args).Cancel();
		}
	}

	private void OnCellInserted(EntityUid uid, PowerCellSlotComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (((Component)component).Initialized && !(((ContainerModifiedMessage)args).Container.ID != component.CellSlotId))
		{
			_appearance.SetData(uid, (Enum)PowerCellSlotVisuals.Enabled, (object)true, (AppearanceComponent)null);
			((EntitySystem)this).RaiseLocalEvent<PowerCellChangedEvent>(uid, new PowerCellChangedEvent(ejected: false), false);
		}
	}

	protected virtual void OnCellRemoved(EntityUid uid, PowerCellSlotComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!(((ContainerModifiedMessage)args).Container.ID != component.CellSlotId))
		{
			_appearance.SetData(uid, (Enum)PowerCellSlotVisuals.Enabled, (object)false, (AppearanceComponent)null);
			((EntitySystem)this).RaiseLocalEvent<PowerCellChangedEvent>(uid, new PowerCellChangedEvent(ejected: true), false);
		}
	}

	public void SetDrawEnabled(Entity<PowerCellDrawComponent?> ent, bool enabled)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<PowerCellDrawComponent>(Entity<PowerCellDrawComponent>.op_Implicit(ent), ref ent.Comp, false) && ent.Comp.Enabled != enabled)
		{
			if (enabled)
			{
				ent.Comp.NextUpdateTime = Timing.CurTime;
			}
			ent.Comp.Enabled = enabled;
			((EntitySystem)this).Dirty(Entity<PowerCellDrawComponent>.op_Implicit(ent), (IComponent)(object)ent.Comp, (MetaDataComponent)null);
		}
	}

	public abstract bool HasActivatableCharge(EntityUid uid, PowerCellDrawComponent? battery = null, PowerCellSlotComponent? cell = null, EntityUid? user = null);

	public abstract bool HasDrawCharge(EntityUid uid, PowerCellDrawComponent? battery = null, PowerCellSlotComponent? cell = null, EntityUid? user = null);
}
