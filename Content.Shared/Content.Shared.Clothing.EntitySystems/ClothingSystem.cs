using System;
using Content.Shared.Clothing.Components;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Strip.Components;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;

namespace Content.Shared.Clothing.EntitySystems;

public abstract class ClothingSystem : EntitySystem
{
	[Dependency]
	private SharedItemSystem _itemSys;

	[Dependency]
	private InventorySystem _invSystem;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, UseInHandEvent>((EntityEventRefHandler<ClothingComponent, UseInHandEvent>)OnUseInHand, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<ClothingComponent, AfterAutoHandleStateEvent>)AfterAutoHandleState, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, GotEquippedEvent>((ComponentEventHandler<ClothingComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, GotUnequippedEvent>((ComponentEventHandler<ClothingComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, ClothingEquipDoAfterEvent>((EntityEventRefHandler<ClothingComponent, ClothingEquipDoAfterEvent>)OnEquipDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, ClothingUnequipDoAfterEvent>((EntityEventRefHandler<ClothingComponent, ClothingUnequipDoAfterEvent>)OnUnequipDoAfter, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, BeforeItemStrippedEvent>((EntityEventRefHandler<ClothingComponent, BeforeItemStrippedEvent>)OnItemStripped, (Type[])null, (Type[])null);
	}

	private void OnUseInHand(Entity<ClothingComponent> ent, ref UseInHandEvent args)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && ent.Comp.QuickEquip)
		{
			EntityUid user = args.User;
			InventoryComponent inv = default(InventoryComponent);
			HandsComponent hands = default(HandsComponent);
			if (((EntitySystem)this).TryComp<InventoryComponent>(user, ref inv) && ((EntitySystem)this).TryComp<HandsComponent>(user, ref hands))
			{
				QuickEquip(ent, Entity<InventoryComponent, HandsComponent>.op_Implicit((user, inv, hands)));
				((HandledEntityEventArgs)args).Handled = true;
				args.ApplyDelay = false;
			}
		}
	}

	private void QuickEquip(Entity<ClothingComponent> toEquipEnt, Entity<InventoryComponent, HandsComponent> userEnt)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		SlotDefinition[] slots = userEnt.Comp1.Slots;
		ClothingComponent item = default(ClothingComponent);
		foreach (SlotDefinition slotDef in slots)
		{
			if (!_invSystem.CanEquip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), slotDef.Name, out string _, slotDef, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt)))
			{
				continue;
			}
			if (_invSystem.TryGetSlotEntity(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), slotDef.Name, out var slotEntity, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt)))
			{
				if ((!((EntitySystem)this).TryComp<ClothingComponent>(slotEntity, ref item) || item.QuickEquip) && _invSystem.TryUnequip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), slotDef.Name, silent: true, force: false, predicted: false, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), null, reparent: true, checkDoafter: true) && _invSystem.TryEquip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), slotDef.Name, silent: false, force: false, predicted: false, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), checkDoafter: true, triggerHandContact: true))
				{
					_handsSystem.PickupOrDrop(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), slotEntity.Value, checkActionBlocker: true, animateUser: false, animate: true, dropNear: false, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt));
					break;
				}
			}
			else if (_invSystem.TryEquip(Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), slotDef.Name, silent: false, force: false, predicted: false, Entity<InventoryComponent, HandsComponent>.op_Implicit(userEnt), Entity<ClothingComponent>.op_Implicit(toEquipEnt), checkDoafter: true, triggerHandContact: true))
			{
				break;
			}
		}
	}

	protected virtual void OnGotEquipped(EntityUid uid, ClothingComponent component, GotEquippedEvent args)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		component.InSlot = args.Slot;
		component.InSlotFlag = args.SlotFlags;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
		if ((component.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			ClothingGotEquippedEvent gotEquippedEvent = new ClothingGotEquippedEvent(args.Equipee, component);
			((EntitySystem)this).RaiseLocalEvent<ClothingGotEquippedEvent>(uid, ref gotEquippedEvent, false);
			ClothingDidEquippedEvent didEquippedEvent = new ClothingDidEquippedEvent(Entity<ClothingComponent>.op_Implicit((uid, component)));
			((EntitySystem)this).RaiseLocalEvent<ClothingDidEquippedEvent>(args.Equipee, ref didEquippedEvent, false);
		}
	}

	protected virtual void OnGotUnequipped(EntityUid uid, ClothingComponent component, GotUnequippedEvent args)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((component.Slots & args.SlotFlags) != SlotFlags.NONE)
		{
			ClothingGotUnequippedEvent gotUnequippedEvent = new ClothingGotUnequippedEvent(args.Equipee, component);
			((EntitySystem)this).RaiseLocalEvent<ClothingGotUnequippedEvent>(uid, ref gotUnequippedEvent, false);
			ClothingDidUnequippedEvent didUnequippedEvent = new ClothingDidUnequippedEvent(Entity<ClothingComponent>.op_Implicit((uid, component)));
			((EntitySystem)this).RaiseLocalEvent<ClothingDidUnequippedEvent>(args.Equipee, ref didUnequippedEvent, false);
		}
		component.InSlot = null;
		component.InSlotFlag = null;
		((EntitySystem)this).Dirty(uid, (IComponent)(object)component, (MetaDataComponent)null);
	}

	private void AfterAutoHandleState(Entity<ClothingComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_itemSys.VisualsChanged(ent.Owner);
	}

	private void OnEquipDoAfter(Entity<ClothingComponent> ent, ref ClothingEquipDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (!((HandledEntityEventArgs)args).Handled && !args.Cancelled)
		{
			EntityUid? target = args.Target;
			if (target.HasValue)
			{
				EntityUid target2 = target.GetValueOrDefault();
				((HandledEntityEventArgs)args).Handled = _invSystem.TryEquip(args.User, target2, Entity<ClothingComponent>.op_Implicit(ent), args.Slot, silent: false, force: false, predicted: true, null, ent.Comp);
			}
		}
	}

	private void OnUnequipDoAfter(Entity<ClothingComponent> ent, ref ClothingUnequipDoAfterEvent args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (((HandledEntityEventArgs)args).Handled || args.Cancelled)
		{
			return;
		}
		EntityUid? target = args.Target;
		if (target.HasValue)
		{
			EntityUid target2 = target.GetValueOrDefault();
			((HandledEntityEventArgs)args).Handled = _invSystem.TryUnequip(args.User, target2, args.Slot, silent: false, force: false, predicted: true, null, ent.Comp, reparent: true, checkDoafter: false, triggerHandContact: true);
			if (((HandledEntityEventArgs)args).Handled)
			{
				_handsSystem.TryPickup(args.User, Entity<ClothingComponent>.op_Implicit(ent));
			}
		}
	}

	private void OnItemStripped(Entity<ClothingComponent> ent, ref BeforeItemStrippedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		args.Additive += ent.Comp.StripDelay;
	}

	public void SetEquippedPrefix(EntityUid uid, string? prefix, ClothingComponent? clothing = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ClothingComponent>(uid, ref clothing, false) && !(clothing.EquippedPrefix == prefix))
		{
			clothing.EquippedPrefix = prefix;
			_itemSys.VisualsChanged(uid);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)clothing, (MetaDataComponent)null);
		}
	}

	public void SetSlots(EntityUid uid, SlotFlags slots, ClothingComponent? clothing = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ClothingComponent>(uid, ref clothing, true))
		{
			clothing.Slots = slots;
			((EntitySystem)this).Dirty(uid, (IComponent)(object)clothing, (MetaDataComponent)null);
		}
	}

	public void CopyVisuals(EntityUid uid, ClothingComponent otherClothing, ClothingComponent? clothing = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<ClothingComponent>(uid, ref clothing, true))
		{
			clothing.ClothingVisuals = otherClothing.ClothingVisuals;
			clothing.EquippedPrefix = otherClothing.EquippedPrefix;
			clothing.RsiPath = otherClothing.RsiPath;
			_itemSys.VisualsChanged(uid);
			((EntitySystem)this).Dirty(uid, (IComponent)(object)clothing, (MetaDataComponent)null);
		}
	}

	public void SetLayerColor(ClothingComponent clothing, string slot, string mapKey, Color? color)
	{
		foreach (PrototypeLayerData layer in clothing.ClothingVisuals[slot])
		{
			if (layer.MapKeys == null)
			{
				break;
			}
			if (layer.MapKeys.Contains(mapKey))
			{
				layer.Color = color;
			}
		}
	}

	public void SetLayerState(ClothingComponent clothing, string slot, string mapKey, string state)
	{
		foreach (PrototypeLayerData layer in clothing.ClothingVisuals[slot])
		{
			if (layer.MapKeys == null)
			{
				break;
			}
			if (layer.MapKeys.Contains(mapKey))
			{
				layer.State = state;
			}
		}
	}
}
