using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._RMC14.Hands;
using Content.Shared.Armor;
using Content.Shared.Atmos;
using Content.Shared.Chat;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Hypospray.Events;
using Content.Shared.Climbing.Events;
using Content.Shared.Clothing.Components;
using Content.Shared.Contraband;
using Content.Shared.Damage;
using Content.Shared.Damage.Events;
using Content.Shared.DoAfter;
using Content.Shared.Electrocution;
using Content.Shared.Explosion;
using Content.Shared.Eye.Blinding.Systems;
using Content.Shared.Flash;
using Content.Shared.Gravity;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Implants;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.NameModifier.EntitySystems;
using Content.Shared.Overlays;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Radio;
using Content.Shared.Slippery;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Strip;
using Content.Shared.Strip.Components;
using Content.Shared.Temperature;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Whitelist;
using Content.Shared.Wieldable;
using Content.Shared.Zombies;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Inventory;

public abstract class InventorySystem : EntitySystem
{
	public struct InventorySlotEnumerator
	{
		private readonly SlotDefinition[] _slots;

		private readonly ContainerSlot[] _containers;

		private readonly SlotFlags _flags;

		private int _nextIdx;

		public static InventorySlotEnumerator Empty = new InventorySlotEnumerator(Array.Empty<SlotDefinition>(), Array.Empty<ContainerSlot>());

		public InventorySlotEnumerator(InventoryComponent inventory, SlotFlags flags = SlotFlags.All)
			: this(inventory.Slots, inventory.Containers, flags)
		{
		}

		public InventorySlotEnumerator(SlotDefinition[] slots, ContainerSlot[] containers, SlotFlags flags = SlotFlags.All)
		{
			_nextIdx = 0;
			_flags = flags;
			_slots = slots;
			_containers = containers;
		}

		public bool MoveNext([NotNullWhen(true)] out ContainerSlot? container)
		{
			while (_nextIdx < _slots.Length)
			{
				int i = _nextIdx++;
				if ((_slots[i].SlotFlags & _flags) != SlotFlags.NONE)
				{
					container = _containers[i];
					return true;
				}
			}
			container = null;
			return false;
		}

		public bool NextItem(out EntityUid item)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			while (_nextIdx < _slots.Length)
			{
				int i = _nextIdx++;
				if ((_slots[i].SlotFlags & _flags) != SlotFlags.NONE)
				{
					EntityUid? containedEntity = _containers[i].ContainedEntity;
					if (containedEntity.HasValue)
					{
						EntityUid uid = containedEntity.GetValueOrDefault();
						item = uid;
						return true;
					}
				}
			}
			item = default(EntityUid);
			return false;
		}

		public bool NextItem(out EntityUid item, [NotNullWhen(true)] out SlotDefinition? slot)
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			while (_nextIdx < _slots.Length)
			{
				int i = _nextIdx++;
				slot = _slots[i];
				if ((slot.SlotFlags & _flags) != SlotFlags.NONE)
				{
					EntityUid? containedEntity = _containers[i].ContainedEntity;
					if (containedEntity.HasValue)
					{
						EntityUid uid = containedEntity.GetValueOrDefault();
						item = uid;
						return true;
					}
				}
			}
			item = default(EntityUid);
			slot = null;
			return false;
		}
	}

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	[Dependency]
	private SharedInteractionSystem _interactionSystem;

	[Dependency]
	private SharedItemSystem _item;

	[Dependency]
	private SharedAudioSystem _audio;

	[Dependency]
	private SharedContainerSystem _containerSystem;

	[Dependency]
	private SharedDoAfterSystem _doAfter;

	[Dependency]
	private SharedHandsSystem _handsSystem;

	[Dependency]
	private IGameTiming _gameTiming;

	[Dependency]
	private SharedTransformSystem _transform;

	[Dependency]
	private EntityWhitelistSystem _whitelistSystem;

	[Dependency]
	private SharedStrippableSystem _strippable;

	[Dependency]
	private RMCHandsSystem _rmcHands;

	private static readonly ProtoId<ItemSizePrototype> PocketableItemSize = ProtoId<ItemSizePrototype>.op_Implicit("Small");

	[Dependency]
	private SharedStorageSystem _storageSystem;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IViewVariablesManager _vvm;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		InitializeEquip();
		InitializeRelay();
		InitializeSlots();
	}

	public override void Shutdown()
	{
		((EntitySystem)this).Shutdown();
		ShutdownSlots();
	}

	private void InitializeEquip()
	{
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, EntInsertedIntoContainerMessage>((ComponentEventHandler<InventoryComponent, EntInsertedIntoContainerMessage>)OnEntInserted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, EntRemovedFromContainerMessage>((ComponentEventHandler<InventoryComponent, EntRemovedFromContainerMessage>)OnEntRemoved, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<UseSlotNetworkMessage>((EntitySessionEventHandler<UseSlotNetworkMessage>)OnUseSlot, (Type[])null, (Type[])null);
	}

	private void OnEntRemoved(EntityUid uid, InventoryComponent component, EntRemovedFromContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlot(uid, ((ContainerModifiedMessage)args).Container.ID, out SlotDefinition slotDef, component))
		{
			DidUnequipEvent unequippedEvent = new DidUnequipEvent(uid, ((ContainerModifiedMessage)args).Entity, slotDef);
			((EntitySystem)this).RaiseLocalEvent<DidUnequipEvent>(uid, unequippedEvent, true);
			GotUnequippedEvent gotUnequippedEvent = new GotUnequippedEvent(uid, ((ContainerModifiedMessage)args).Entity, slotDef);
			((EntitySystem)this).RaiseLocalEvent<GotUnequippedEvent>(((ContainerModifiedMessage)args).Entity, gotUnequippedEvent, true);
		}
	}

	private void OnEntInserted(EntityUid uid, InventoryComponent component, EntInsertedIntoContainerMessage args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetSlot(uid, ((ContainerModifiedMessage)args).Container.ID, out SlotDefinition slotDef, component))
		{
			DidEquipEvent equippedEvent = new DidEquipEvent(uid, ((ContainerModifiedMessage)args).Entity, slotDef);
			((EntitySystem)this).RaiseLocalEvent<DidEquipEvent>(uid, equippedEvent, true);
			GotEquippedEvent gotEquippedEvent = new GotEquippedEvent(uid, ((ContainerModifiedMessage)args).Entity, slotDef);
			((EntitySystem)this).RaiseLocalEvent<GotEquippedEvent>(((ContainerModifiedMessage)args).Entity, gotEquippedEvent, true);
		}
	}

	private void OnUseSlot(UseSlotNetworkMessage ev, EntitySessionEventArgs eventArgs)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref eventArgs)).SenderSession.AttachedEntity;
		if (!attachedEntity.HasValue)
		{
			return;
		}
		EntityUid actor = attachedEntity.GetValueOrDefault();
		InventoryComponent inventory = default(InventoryComponent);
		HandsComponent hands = default(HandsComponent);
		if (!((EntityUid)(ref actor)).Valid || !((EntitySystem)this).TryComp<InventoryComponent>(actor, ref inventory) || !((EntitySystem)this).TryComp<HandsComponent>(actor, ref hands))
		{
			return;
		}
		EntityUid? held = _handsSystem.GetActiveItem(Entity<HandsComponent>.op_Implicit((actor, hands)));
		TryGetSlotEntity(actor, ev.Slot, out var itemUid, inventory);
		if (held.HasValue && itemUid.HasValue)
		{
			_interactionSystem.InteractUsing(actor, held.Value, itemUid.Value, ((EntitySystem)this).Transform(itemUid.Value).Coordinates);
		}
		else if (itemUid.HasValue)
		{
			if (!_rmcHands.TryStorageEjectHand(actor, itemUid.Value) && TryUnequip(actor, ev.Slot, out var item, silent: false, force: false, predicted: true, inventory, null, reparent: true, checkDoafter: true, triggerHandContact: true))
			{
				_handsSystem.PickupOrDrop(actor, item.Value);
			}
		}
		else if (held.HasValue)
		{
			if (!CanEquip(actor, held.Value, ev.Slot, out string reason))
			{
				_popup.PopupCursor(base.Loc.GetString(reason));
			}
			else if (_handsSystem.CanDropHeld(actor, hands.ActiveHandId, checkActionBlocker: false))
			{
				((EntitySystem)this).RaiseLocalEvent<HandDeselectedEvent>(held.Value, new HandDeselectedEvent(actor), false);
				TryEquip(actor, actor, held.Value, ev.Slot, silent: false, force: true, predicted: true, inventory, null, checkDoafter: true, triggerHandContact: true);
			}
		}
	}

	public bool TryEquip(EntityUid uid, EntityUid itemUid, string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool checkDoafter = false, bool triggerHandContact = false, bool doRangeCheck = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return TryEquip(uid, uid, itemUid, slot, silent, force, predicted, inventory, clothing, checkDoafter, triggerHandContact, doRangeCheck);
	}

	public bool TryEquip(EntityUid actor, EntityUid target, EntityUid itemUid, string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool checkDoafter = false, bool triggerHandContact = false, bool doRangeCheck = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InventoryComponent>(target, ref inventory, false))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString("inventory-component-can-equip-cannot"));
			}
			return false;
		}
		((EntitySystem)this).Resolve<ClothingComponent>(itemUid, ref clothing, false);
		if (!TryGetSlotContainer(target, slot, out ContainerSlot slotContainer, out SlotDefinition slotDefinition, inventory))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString("inventory-component-can-equip-cannot"));
			}
			return false;
		}
		if (!force && !CanEquip(actor, target, itemUid, slot, out string reason, slotDefinition, inventory, clothing, null, doRangeCheck))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString(reason));
			}
			return false;
		}
		if (checkDoafter && clothing != null && clothing.EquipDelay > TimeSpan.Zero && (clothing.Slots & slotDefinition.SlotFlags) != SlotFlags.NONE && _containerSystem.CanInsert(itemUid, (BaseContainer)(object)slotContainer, false, (TransformComponent)null))
		{
			DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)base.EntityManager, actor, clothing.EquipDelay, new ClothingEquipDoAfterEvent(slot), itemUid, target, itemUid)
			{
				BreakOnMove = true,
				NeedHand = true
			};
			_doAfter.TryStartDoAfter(args);
			return false;
		}
		if (!_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(itemUid), (BaseContainer)(object)slotContainer, (TransformComponent)null, false))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString("inventory-component-can-unequip-cannot"));
			}
			return false;
		}
		if (!silent && clothing != null)
		{
			_audio.PlayPredicted(clothing.EquipSound, target, (EntityUid?)actor, (AudioParams?)null);
		}
		if (triggerHandContact && (slotDefinition.SlotFlags & SlotFlags.GLOVES) != SlotFlags.NONE)
		{
			TriggerHandContactInteraction(target);
		}
		((EntitySystem)this).Dirty(target, (IComponent)(object)inventory, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(target);
		return true;
	}

	public bool CanAccess(EntityUid actor, EntityUid target, EntityUid itemUid, bool doRangeCheck = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		AttachedClothingComponent attachedComp = default(AttachedClothingComponent);
		if (((EntitySystem)this).TryComp<AttachedClothingComponent>(itemUid, ref attachedComp))
		{
			itemUid = attachedComp.AttachedUid;
		}
		if (actor != target && (!doRangeCheck || !_interactionSystem.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(actor), Entity<TransformComponent>.op_Implicit(target)) || !_containerSystem.IsInSameOrParentContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit(actor), Entity<TransformComponent, MetaDataComponent>.op_Implicit(target))))
		{
			return false;
		}
		if (_interactionSystem.InRangeAndAccessible(Entity<TransformComponent>.op_Implicit(actor), Entity<TransformComponent>.op_Implicit(itemUid)))
		{
			return true;
		}
		if (actor != target && ((EntitySystem)this).HasComp<StrippableComponent>(target) && ((EntitySystem)this).HasComp<StrippingComponent>(actor))
		{
			return ((EntitySystem)this).HasComp<HandsComponent>(actor);
		}
		return false;
	}

	public bool CanEquip(EntityUid uid, EntityUid itemUid, string slot, [NotNullWhen(false)] out string? reason, SlotDefinition? slotDefinition = null, InventoryComponent? inventory = null, ClothingComponent? clothing = null, ItemComponent? item = null, bool doRangeCheck = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return CanEquip(uid, uid, itemUid, slot, out reason, slotDefinition, inventory, clothing, item, doRangeCheck);
	}

	public bool CanEquip(EntityUid actor, EntityUid target, EntityUid itemUid, string slot, [NotNullWhen(false)] out string? reason, SlotDefinition? slotDefinition = null, InventoryComponent? inventory = null, ClothingComponent? clothing = null, ItemComponent? item = null, bool doRangeCheck = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		reason = "inventory-component-can-equip-cannot";
		if (!((EntitySystem)this).Resolve<InventoryComponent>(target, ref inventory, false))
		{
			return false;
		}
		((EntitySystem)this).Resolve<ClothingComponent, ItemComponent>(itemUid, ref clothing, ref item, false);
		if (slotDefinition == null && !TryGetSlot(target, slot, out slotDefinition, inventory))
		{
			return false;
		}
		if (slotDefinition.DependsOn != null)
		{
			if (!TryGetSlotEntity(target, slotDefinition.DependsOn, out var slotEntity, inventory))
			{
				return false;
			}
			ComponentRegistry componentRegistry = slotDefinition.DependsOnComponents;
			if (componentRegistry != null)
			{
				AllowSuitStorageComponent comp = default(AllowSuitStorageComponent);
				foreach (var (_, entry) in (Dictionary<string, ComponentRegistryEntry>)(object)componentRegistry)
				{
					if (!((EntitySystem)this).HasComp(slotEntity, ((object)entry.Component).GetType()))
					{
						return false;
					}
					if (((EntitySystem)this).TryComp<AllowSuitStorageComponent>(slotEntity, ref comp) && _whitelistSystem.IsWhitelistFailOrNull(comp.Whitelist, itemUid))
					{
						return false;
					}
				}
			}
		}
		bool fittingInPocket = slotDefinition.SlotFlags.HasFlag(SlotFlags.POCKET) && item != null && _item.GetSizePrototype(item.Size) <= _item.GetSizePrototype(PocketableItemSize);
		if ((clothing == null && !fittingInPocket) || (clothing != null && !clothing.Slots.HasFlag(slotDefinition.SlotFlags) && !fittingInPocket))
		{
			reason = "inventory-component-can-equip-does-not-fit";
			return false;
		}
		if (!CanAccess(actor, target, itemUid, doRangeCheck))
		{
			reason = "interaction-system-user-interaction-cannot-reach";
			return false;
		}
		if (_whitelistSystem.IsWhitelistFail(slotDefinition.Whitelist, itemUid) || _whitelistSystem.IsBlacklistPass(slotDefinition.Blacklist, itemUid))
		{
			reason = "inventory-component-can-equip-does-not-fit";
			return false;
		}
		IsEquippingAttemptEvent attemptEvent = new IsEquippingAttemptEvent(actor, target, itemUid, slotDefinition);
		((EntitySystem)this).RaiseLocalEvent<IsEquippingAttemptEvent>(actor, attemptEvent, true);
		if (((CancellableEntityEventArgs)attemptEvent).Cancelled)
		{
			reason = attemptEvent.Reason ?? reason;
			return false;
		}
		IsEquippingTargetAttemptEvent targetAttemptEvent = new IsEquippingTargetAttemptEvent(actor, target, itemUid, slotDefinition);
		((EntitySystem)this).RaiseLocalEvent<IsEquippingTargetAttemptEvent>(target, targetAttemptEvent, true);
		if (((CancellableEntityEventArgs)targetAttemptEvent).Cancelled)
		{
			reason = targetAttemptEvent.Reason ?? reason;
			return false;
		}
		BeingEquippedAttemptEvent itemAttemptEvent = new BeingEquippedAttemptEvent(actor, target, itemUid, slotDefinition);
		((EntitySystem)this).RaiseLocalEvent<BeingEquippedAttemptEvent>(itemUid, itemAttemptEvent, true);
		if (((CancellableEntityEventArgs)itemAttemptEvent).Cancelled)
		{
			reason = itemAttemptEvent.Reason ?? reason;
			return false;
		}
		return true;
	}

	public bool TryUnequip(EntityUid uid, string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool reparent = true, bool checkDoafter = false, bool triggerHandContact = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TryUnequip(uid, uid, slot, silent, force, predicted, inventory, clothing, reparent, checkDoafter, triggerHandContact);
	}

	public bool TryUnequip(EntityUid actor, EntityUid target, string slot, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool reparent = true, bool checkDoafter = false, bool triggerHandContact = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? removedItem;
		return TryUnequip(actor, target, slot, out removedItem, silent, force, predicted, inventory, clothing, reparent, checkDoafter, triggerHandContact);
	}

	public bool TryUnequip(EntityUid uid, string slot, [NotNullWhen(true)] out EntityUid? removedItem, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool reparent = true, bool checkDoafter = false, bool triggerHandContact = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TryUnequip(uid, uid, slot, out removedItem, silent, force, predicted, inventory, clothing, reparent, checkDoafter, triggerHandContact);
	}

	public bool TryUnequip(EntityUid actor, EntityUid target, string slot, [NotNullWhen(true)] out EntityUid? removedItem, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool reparent = true, bool checkDoafter = false, bool triggerHandContact = false)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		int itemsDropped = 0;
		return TryUnequip(actor, target, slot, out removedItem, ref itemsDropped, silent, force, predicted, inventory, clothing, reparent, checkDoafter);
	}

	private bool TryUnequip(EntityUid actor, EntityUid target, string slot, [NotNullWhen(true)] out EntityUid? removedItem, ref int itemsDropped, bool silent = false, bool force = false, bool predicted = false, InventoryComponent? inventory = null, ClothingComponent? clothing = null, bool reparent = true, bool checkDoafter = false, bool triggerHandContact = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0312: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		removedItem = null;
		if (((EntitySystem)this).TerminatingOrDeleted(target, (MetaDataComponent)null))
		{
			return false;
		}
		if (!((EntitySystem)this).Resolve<InventoryComponent>(target, ref inventory, false))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString("inventory-component-can-unequip-cannot"));
			}
			return false;
		}
		if (!TryGetSlotContainer(target, slot, out ContainerSlot slotContainer, out SlotDefinition slotDefinition, inventory))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString("inventory-component-can-unequip-cannot"));
			}
			return false;
		}
		removedItem = slotContainer.ContainedEntity;
		if (!removedItem.HasValue || ((EntitySystem)this).TerminatingOrDeleted(removedItem.Value, (MetaDataComponent)null))
		{
			return false;
		}
		if (!force && !CanUnequip(actor, target, slot, out string reason, slotContainer, slotDefinition, inventory))
		{
			if (!silent)
			{
				_popup.PopupCursor(base.Loc.GetString(reason));
			}
			return false;
		}
		if (!force && !_containerSystem.CanRemove(removedItem.Value, (BaseContainer)(object)slotContainer))
		{
			return false;
		}
		if (checkDoafter && ((EntitySystem)this).Resolve<ClothingComponent>(removedItem.Value, ref clothing, false) && (clothing.Slots & slotDefinition.SlotFlags) != SlotFlags.NONE && clothing.UnequipDelay > TimeSpan.Zero)
		{
			DoAfterArgs args = new DoAfterArgs((IEntityManager)(object)base.EntityManager, actor, clothing.UnequipDelay, new ClothingUnequipDoAfterEvent(slot), removedItem.Value, target, removedItem.Value)
			{
				BreakOnMove = true,
				NeedHand = true
			};
			_doAfter.TryStartDoAfter(args);
			return false;
		}
		SharedContainerSystem containerSystem = _containerSystem;
		Entity<TransformComponent, MetaDataComponent> val = Entity<TransformComponent, MetaDataComponent>.op_Implicit(removedItem.Value);
		ContainerSlot obj = slotContainer;
		bool flag = force;
		if (!containerSystem.Remove(val, (BaseContainer)(object)obj, reparent, flag, (EntityCoordinates?)null, (Angle?)null))
		{
			return false;
		}
		bool firstRun = itemsDropped == 0;
		itemsDropped++;
		SlotDefinition[] slots = inventory.Slots;
		foreach (SlotDefinition slotDef in slots)
		{
			if (slotDef != slotDefinition && slotDef.DependsOn == slotDefinition.Name)
			{
				TryUnequip(actor, target, slotDef.Name, out var _, ref itemsDropped, silent: true, force: true, predicted, inventory, null, reparent);
			}
		}
		if (!silent && _gameTiming.IsFirstTimePredicted && firstRun && itemsDropped > 1)
		{
			_popup.PopupClient(base.Loc.GetString("inventory-component-dropped-from-unequip", (ValueTuple<string, object>)("items", itemsDropped - 1)), target, target);
		}
		if (!_containerSystem.IsEntityInContainer(removedItem.Value, (MetaDataComponent)null))
		{
			_transform.DropNextTo(Entity<TransformComponent>.op_Implicit(removedItem.Value), Entity<TransformComponent>.op_Implicit(target));
		}
		if (!silent && ((EntitySystem)this).Resolve<ClothingComponent>(removedItem.Value, ref clothing, false) && clothing.UnequipSound != null)
		{
			_audio.PlayPredicted(clothing.UnequipSound, target, (EntityUid?)actor, (AudioParams?)null);
		}
		if (triggerHandContact && (slotDefinition.SlotFlags & SlotFlags.GLOVES) != SlotFlags.NONE)
		{
			TriggerHandContactInteraction(target);
		}
		((EntitySystem)this).Dirty(target, (IComponent)(object)inventory, (MetaDataComponent)null);
		_movementSpeed.RefreshMovementSpeedModifiers(target);
		return true;
	}

	public bool CanUnequip(EntityUid uid, string slot, [NotNullWhen(false)] out string? reason, ContainerSlot? containerSlot = null, SlotDefinition? slotDefinition = null, InventoryComponent? inventory = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return CanUnequip(uid, uid, slot, out reason, containerSlot, slotDefinition, inventory);
	}

	public bool CanUnequip(EntityUid actor, EntityUid target, string slot, [NotNullWhen(false)] out string? reason, ContainerSlot? containerSlot = null, SlotDefinition? slotDefinition = null, InventoryComponent? inventory = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		reason = "inventory-component-can-unequip-cannot";
		if (!((EntitySystem)this).Resolve<InventoryComponent>(target, ref inventory, false))
		{
			return false;
		}
		if ((containerSlot == null || slotDefinition == null) && !TryGetSlotContainer(target, slot, out containerSlot, out slotDefinition, inventory))
		{
			return false;
		}
		EntityUid? containedEntity = containerSlot.ContainedEntity;
		if (containedEntity.HasValue)
		{
			EntityUid itemUid = containedEntity.GetValueOrDefault();
			if (!_containerSystem.CanRemove(itemUid, (BaseContainer)(object)containerSlot))
			{
				return false;
			}
			if (!CanAccess(actor, target, itemUid))
			{
				reason = "interaction-system-user-interaction-cannot-reach";
				return false;
			}
			IsUnequippingAttemptEvent attemptEvent = new IsUnequippingAttemptEvent(actor, target, itemUid, slotDefinition);
			((EntitySystem)this).RaiseLocalEvent<IsUnequippingAttemptEvent>(actor, attemptEvent, true);
			if (((CancellableEntityEventArgs)attemptEvent).Cancelled)
			{
				reason = attemptEvent.Reason ?? reason;
				return false;
			}
			IsUnequippingTargetAttemptEvent targetAttemptEvent = new IsUnequippingTargetAttemptEvent(actor, target, itemUid, slotDefinition);
			((EntitySystem)this).RaiseLocalEvent<IsUnequippingTargetAttemptEvent>(target, targetAttemptEvent, true);
			if (((CancellableEntityEventArgs)targetAttemptEvent).Cancelled)
			{
				reason = targetAttemptEvent.Reason ?? reason;
				return false;
			}
			BeingUnequippedAttemptEvent itemAttemptEvent = new BeingUnequippedAttemptEvent(actor, target, itemUid, slotDefinition);
			((EntitySystem)this).RaiseLocalEvent<BeingUnequippedAttemptEvent>(itemUid, itemAttemptEvent, true);
			if (((CancellableEntityEventArgs)itemAttemptEvent).Cancelled)
			{
				reason = itemAttemptEvent.Reason ?? reason;
				return false;
			}
			return true;
		}
		return false;
	}

	public bool TryGetSlotEntity(EntityUid uid, string slot, [NotNullWhen(true)] out EntityUid? entityUid, InventoryComponent? inventoryComponent = null, ContainerManagerComponent? containerManagerComponent = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		entityUid = null;
		if (!((EntitySystem)this).Resolve<InventoryComponent, ContainerManagerComponent>(uid, ref inventoryComponent, ref containerManagerComponent, false) || !TryGetSlotContainer(uid, slot, out ContainerSlot container, out SlotDefinition _, inventoryComponent, containerManagerComponent))
		{
			return false;
		}
		entityUid = container.ContainedEntity;
		return entityUid.HasValue;
	}

	public void TriggerHandContactInteraction(EntityUid uid)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid item in _handsSystem.EnumerateHeld(Entity<HandsComponent>.op_Implicit(uid)))
		{
			_interactionSystem.DoContactInteraction(uid, item);
		}
	}

	public IEnumerable<EntityUid> GetHandOrInventoryEntities(Entity<HandsComponent?, InventoryComponent?> user, SlotFlags flags = SlotFlags.All)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).Resolve<HandsComponent>(user.Owner, ref user.Comp1, false))
		{
			foreach (EntityUid item2 in _handsSystem.EnumerateHeld(Entity<HandsComponent, InventoryComponent>.op_Implicit(user)))
			{
				yield return item2;
			}
		}
		if (((EntitySystem)this).Resolve<InventoryComponent>(user.Owner, ref user.Comp2, false))
		{
			InventorySlotEnumerator slotEnumerator = new InventorySlotEnumerator(user.Comp2, flags);
			EntityUid item;
			while (slotEnumerator.NextItem(out item))
			{
				yield return item;
			}
		}
	}

	public bool TryGetContainingSlot(Entity<TransformComponent?, MetaDataComponent?> entity, [NotNullWhen(true)] out SlotDefinition? slot)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		BaseContainer container = default(BaseContainer);
		if (!_containerSystem.TryGetContainingContainer(entity, ref container))
		{
			slot = null;
			return false;
		}
		return TryGetSlot(container.Owner, container.ID, out slot);
	}

	public bool InSlotWithFlags(Entity<TransformComponent?, MetaDataComponent?> entity, SlotFlags flags)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetContainingSlot(entity, out SlotDefinition slot))
		{
			return (slot.SlotFlags & flags) == flags;
		}
		return false;
	}

	public bool SpawnItemInSlot(EntityUid uid, string slot, string prototype, bool silent = false, bool force = false, InventoryComponent? inventory = null)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InventoryComponent>(uid, ref inventory, false))
		{
			return false;
		}
		if (((EntitySystem)this).Deleted(uid, (MetaDataComponent)null))
		{
			return false;
		}
		if (!HasSlot(uid, slot) || TryGetSlotEntity(uid, slot, out var _, inventory))
		{
			return false;
		}
		if (!_prototypeManager.HasIndex<EntityPrototype>(prototype))
		{
			return false;
		}
		EntityUid item = ((EntitySystem)this).Spawn(prototype, ((EntitySystem)this).Transform(uid).Coordinates);
		if (!TryEquip(uid, item, slot, silent, force))
		{
			return DeleteItem();
		}
		return true;
		bool DeleteItem()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			((EntitySystem)this).Del((EntityUid?)item);
			return false;
		}
	}

	public void SpawnItemsOnEntity(EntityUid entity, List<string> items)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		foreach (string item in items)
		{
			SpawnItemOnEntity(entity, EntProtoId.op_Implicit(item));
		}
	}

	public void SpawnItemOnEntity(EntityUid entity, EntProtoId item)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).HasComp<TransformComponent>(entity))
		{
			return;
		}
		TransformComponent xform = ((EntitySystem)this).Transform(entity);
		MapCoordinates mapCoords = _transform.GetMapCoordinates(xform);
		EntityUid itemToSpawn = ((EntitySystem)this).Spawn(EntProtoId.op_Implicit(item), mapCoords, (ComponentRegistry)null, default(Angle));
		if (TryGetSlotContainer(entity, "back", out ContainerSlot backSlot, out SlotDefinition slotDefinition))
		{
			EntityUid? stackedEntity = backSlot.ContainedEntity;
			if (stackedEntity.HasValue && _storageSystem.Insert(backSlot.ContainedEntity.Value, itemToSpawn, out stackedEntity))
			{
				return;
			}
		}
		if ((!TryGetSlotContainer(entity, "pocket1", out ContainerSlot pocket1, out slotDefinition) || !_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(itemToSpawn), (BaseContainer)(object)pocket1, (TransformComponent)null, false)) && (!TryGetSlotContainer(entity, "pocket2", out ContainerSlot pocket2, out slotDefinition) || !_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(itemToSpawn), (BaseContainer)(object)pocket2, (TransformComponent)null, false)) && (!TryGetSlotContainer(entity, "belt", out ContainerSlot belt, out slotDefinition) || !_containerSystem.Insert(Entity<TransformComponent, MetaDataComponent, PhysicsComponent>.op_Implicit(itemToSpawn), (BaseContainer)(object)belt, (TransformComponent)null, false)))
		{
			_handsSystem.PickupOrDrop(entity, itemToSpawn, checkActionBlocker: false);
		}
	}

	public void InitializeRelay()
	{
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, DamageModifyEvent>((ComponentEventHandler<InventoryComponent, DamageModifyEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ElectrocutionAttemptEvent>((ComponentEventHandler<InventoryComponent, ElectrocutionAttemptEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, SlipAttemptEvent>((ComponentEventHandler<InventoryComponent, SlipAttemptEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshMovementSpeedModifiersEvent>((ComponentEventHandler<InventoryComponent, RefreshMovementSpeedModifiersEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, BeforeStripEvent>((ComponentEventHandler<InventoryComponent, BeforeStripEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, SeeIdentityAttemptEvent>((ComponentEventHandler<InventoryComponent, SeeIdentityAttemptEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ModifyChangedTemperatureEvent>((ComponentEventHandler<InventoryComponent, ModifyChangedTemperatureEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetDefaultRadioChannelEvent>((ComponentEventHandler<InventoryComponent, GetDefaultRadioChannelEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshNameModifiersEvent>((ComponentEventHandler<InventoryComponent, RefreshNameModifiersEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, TransformSpeakerNameEvent>((ComponentEventHandler<InventoryComponent, TransformSpeakerNameEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, SelfBeforeHyposprayInjectsEvent>((ComponentEventHandler<InventoryComponent, SelfBeforeHyposprayInjectsEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, TargetBeforeHyposprayInjectsEvent>((ComponentEventHandler<InventoryComponent, TargetBeforeHyposprayInjectsEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, SelfBeforeGunShotEvent>((ComponentEventHandler<InventoryComponent, SelfBeforeGunShotEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, SelfBeforeClimbEvent>((ComponentEventHandler<InventoryComponent, SelfBeforeClimbEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, CoefficientQueryEvent>((ComponentEventHandler<InventoryComponent, CoefficientQueryEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ZombificationResistanceQueryEvent>((ComponentEventHandler<InventoryComponent, ZombificationResistanceQueryEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, IsEquippingTargetAttemptEvent>((ComponentEventHandler<InventoryComponent, IsEquippingTargetAttemptEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, IsUnequippingTargetAttemptEvent>((ComponentEventHandler<InventoryComponent, IsUnequippingTargetAttemptEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ChameleonControllerOutfitSelectedEvent>((ComponentEventHandler<InventoryComponent, ChameleonControllerOutfitSelectedEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshFrictionModifiersEvent>((ComponentEventRefHandler<InventoryComponent, RefreshFrictionModifiersEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, BeforeStaminaDamageEvent>((ComponentEventRefHandler<InventoryComponent, BeforeStaminaDamageEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetExplosionResistanceEvent>((ComponentEventRefHandler<InventoryComponent, GetExplosionResistanceEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, IsWeightlessEvent>((ComponentEventRefHandler<InventoryComponent, IsWeightlessEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetSpeedModifierContactCapEvent>((ComponentEventRefHandler<InventoryComponent, GetSpeedModifierContactCapEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetSlowedOverSlipperyModifierEvent>((ComponentEventRefHandler<InventoryComponent, GetSlowedOverSlipperyModifierEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ModifySlowOnDamageSpeedEvent>((ComponentEventRefHandler<InventoryComponent, ModifySlowOnDamageSpeedEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ExtinguishEvent>((ComponentEventRefHandler<InventoryComponent, ExtinguishEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ProjectileReflectAttemptEvent>((ComponentEventRefHandler<InventoryComponent, ProjectileReflectAttemptEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, HitScanReflectAttemptEvent>((ComponentEventRefHandler<InventoryComponent, HitScanReflectAttemptEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetContrabandDetailsEvent>((ComponentEventRefHandler<InventoryComponent, GetContrabandDetailsEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, FlashAttemptEvent>((ComponentEventRefHandler<InventoryComponent, FlashAttemptEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, WieldAttemptEvent>((ComponentEventRefHandler<InventoryComponent, WieldAttemptEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, UnwieldAttemptEvent>((ComponentEventRefHandler<InventoryComponent, UnwieldAttemptEvent>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, CanSeeAttemptEvent>((ComponentEventHandler<InventoryComponent, CanSeeAttemptEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetEyeProtectionEvent>((ComponentEventHandler<InventoryComponent, GetEyeProtectionEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetBlurEvent>((ComponentEventHandler<InventoryComponent, GetBlurEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, SolutionScanEvent>((ComponentEventHandler<InventoryComponent, SolutionScanEvent>)RelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowJobIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowJobIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthBarsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthBarsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowHealthIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowHungerIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowHungerIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowThirstIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowThirstIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowMindShieldIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowMindShieldIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowSyndicateIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowSyndicateIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<ShowCriminalRecordIconsComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<ShowCriminalRecordIconsComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<BlackAndWhiteOverlayComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, RefreshEquipmentHudEvent<NoirOverlayComponent>>((ComponentEventRefHandler<InventoryComponent, RefreshEquipmentHudEvent<NoirOverlayComponent>>)RefRelayInventoryEvent, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetVerbsEvent<EquipmentVerb>>((ComponentEventHandler<InventoryComponent, GetVerbsEvent<EquipmentVerb>>)OnGetEquipmentVerbs, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, GetVerbsEvent<InnateVerb>>((ComponentEventHandler<InventoryComponent, GetVerbsEvent<InnateVerb>>)OnGetInnateVerbs, (Type[])null, (Type[])null);
	}

	protected void RefRelayInventoryEvent<T>(EntityUid uid, InventoryComponent component, ref T args) where T : IInventoryRelayEvent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RelayEvent(Entity<InventoryComponent>.op_Implicit((uid, component)), ref args);
	}

	protected void RelayInventoryEvent<T>(EntityUid uid, InventoryComponent component, T args) where T : IInventoryRelayEvent
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		RelayEvent(Entity<InventoryComponent>.op_Implicit((uid, component)), args);
	}

	public void RelayEvent<T>(Entity<InventoryComponent> inventory, ref T args) where T : IInventoryRelayEvent
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (args.TargetSlots != SlotFlags.NONE)
		{
			InventoryRelayedEvent<T> ev = new InventoryRelayedEvent<T>(args);
			InventorySlotEnumerator enumerator = new InventorySlotEnumerator(Entity<InventoryComponent>.op_Implicit(inventory), args.TargetSlots);
			EntityUid item;
			while (enumerator.NextItem(out item))
			{
				((EntitySystem)this).RaiseLocalEvent<InventoryRelayedEvent<T>>(item, ev, false);
			}
			args = ev.Args;
		}
	}

	public void RelayEvent<T>(Entity<InventoryComponent> inventory, T args) where T : IInventoryRelayEvent
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (args.TargetSlots != SlotFlags.NONE)
		{
			InventoryRelayedEvent<T> ev = new InventoryRelayedEvent<T>(args);
			InventorySlotEnumerator enumerator = new InventorySlotEnumerator(Entity<InventoryComponent>.op_Implicit(inventory), args.TargetSlots);
			EntityUid item;
			while (enumerator.NextItem(out item))
			{
				((EntitySystem)this).RaiseLocalEvent<InventoryRelayedEvent<T>>(item, ev, false);
			}
		}
	}

	private void OnGetEquipmentVerbs(EntityUid uid, InventoryComponent component, GetVerbsEvent<EquipmentVerb> args)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>> ev = new InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>(args);
		InventorySlotEnumerator enumerator = new InventorySlotEnumerator(component);
		EntityUid item;
		SlotDefinition slotDef;
		while (enumerator.NextItem(out item, out slotDef))
		{
			if (!_strippable.IsStripHidden(slotDef, args.User) || args.User == uid)
			{
				((EntitySystem)this).RaiseLocalEvent<InventoryRelayedEvent<GetVerbsEvent<EquipmentVerb>>>(item, ev, false);
			}
		}
	}

	private void OnGetInnateVerbs(EntityUid uid, InventoryComponent component, GetVerbsEvent<InnateVerb> args)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		InventoryRelayedEvent<GetVerbsEvent<InnateVerb>> ev = new InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>(args);
		InventorySlotEnumerator enumerator = new InventorySlotEnumerator(component, SlotFlags.WITHOUT_POCKET);
		EntityUid item;
		while (enumerator.NextItem(out item))
		{
			((EntitySystem)this).RaiseLocalEvent<InventoryRelayedEvent<GetVerbsEvent<InnateVerb>>>(item, ev, false);
		}
	}

	private void InitializeSlots()
	{
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, ComponentInit>((ComponentEventHandler<InventoryComponent, ComponentInit>)OnInit, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeAllEvent<OpenSlotStorageNetworkMessage>((EntitySessionEventHandler<OpenSlotStorageNetworkMessage>)OnOpenSlotStorage, (Type[])null, (Type[])null);
		_vvm.GetTypeHandler<InventoryComponent>().AddHandler((HandleTypePathComponent<InventoryComponent>)HandleViewVariablesSlots, (ListTypeCustomPathsComponent<InventoryComponent>)ListViewVariablesSlots);
		((EntitySystem)this).SubscribeLocalEvent<InventoryComponent, AfterAutoHandleStateEvent>((EntityEventRefHandler<InventoryComponent, AfterAutoHandleStateEvent>)AfterAutoState, (Type[])null, (Type[])null);
	}

	private void ShutdownSlots()
	{
		_vvm.GetTypeHandler<InventoryComponent>().RemoveHandler((HandleTypePathComponent<InventoryComponent>)HandleViewVariablesSlots, (ListTypeCustomPathsComponent<InventoryComponent>)ListViewVariablesSlots);
	}

	public bool TryGetInventoryEntity<T>(Entity<InventoryComponent?> entity, out Entity<T?> target) where T : IComponent, IClothingSlots
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(entity.Owner), out var containerSlotEnumerator))
		{
			EntityUid item;
			SlotDefinition slot;
			T required = default(T);
			while (containerSlotEnumerator.NextItem(out item, out slot))
			{
				if (((EntitySystem)this).TryComp<T>(item, ref required) && (required.Slots & slot.SlotFlags) != SlotFlags.NONE)
				{
					target = Entity<T>.op_Implicit((item, required));
					return true;
				}
			}
		}
		target = Entity<T>.op_Implicit(EntityUid.Invalid);
		return false;
	}

	protected virtual void OnInit(EntityUid uid, InventoryComponent component, ComponentInit args)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		InventoryTemplatePrototype invTemplate = default(InventoryTemplatePrototype);
		if (_prototypeManager.TryIndex<InventoryTemplatePrototype>(component.TemplateId, ref invTemplate))
		{
			component.Slots = invTemplate.Slots;
			component.Containers = (ContainerSlot[])(object)new ContainerSlot[component.Slots.Length];
			for (int i = 0; i < component.Containers.Length; i++)
			{
				SlotDefinition slot = component.Slots[i];
				ContainerSlot container = _containerSystem.EnsureContainer<ContainerSlot>(uid, slot.Name, (ContainerManagerComponent)null);
				((BaseContainer)container).OccludesLight = false;
				component.Containers[i] = container;
			}
		}
	}

	private void AfterAutoState(Entity<InventoryComponent> ent, ref AfterAutoHandleStateEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateInventoryTemplate(ent);
	}

	protected virtual void UpdateInventoryTemplate(Entity<InventoryComponent> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		InventoryTemplatePrototype invTemplate = default(InventoryTemplatePrototype);
		if ((int)((Component)ent.Comp).LifeStage >= 4 && _prototypeManager.TryIndex<InventoryTemplatePrototype>(ent.Comp.TemplateId, ref invTemplate))
		{
			ent.Comp.Slots = invTemplate.Slots;
			InventoryTemplateUpdated ev = default(InventoryTemplateUpdated);
			((EntitySystem)this).RaiseLocalEvent<InventoryTemplateUpdated>(Entity<InventoryComponent>.op_Implicit(ent), ref ev, false);
		}
	}

	public void RefreshInventoryTemplate(Entity<InventoryComponent> ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateInventoryTemplate(ent);
	}

	private void OnOpenSlotStorage(OpenSlotStorageNetworkMessage ev, EntitySessionEventArgs args)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? attachedEntity = ((EntitySessionEventArgs)(ref args)).SenderSession.AttachedEntity;
		if (attachedEntity.HasValue)
		{
			EntityUid uid = attachedEntity.GetValueOrDefault();
			StorageComponent storageComponent = default(StorageComponent);
			if (((EntityUid)(ref uid)).Valid && TryGetSlotEntity(uid, ev.Slot, out var entityUid) && ((EntitySystem)this).TryComp<StorageComponent>(entityUid, ref storageComponent))
			{
				_storageSystem.OpenStorageUI(entityUid.Value, uid, storageComponent, silent: false);
			}
		}
	}

	public bool TryGetSlotContainer(EntityUid uid, string slot, [NotNullWhen(true)] out ContainerSlot? containerSlot, [NotNullWhen(true)] out SlotDefinition? slotDefinition, InventoryComponent? inventory = null, ContainerManagerComponent? containerComp = null)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Invalid comparison between Unknown and I4
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		containerSlot = null;
		slotDefinition = null;
		if (!((EntitySystem)this).Resolve<InventoryComponent, ContainerManagerComponent>(uid, ref inventory, ref containerComp, false))
		{
			return false;
		}
		if (!TryGetSlot(uid, slot, out slotDefinition, inventory))
		{
			return false;
		}
		BaseContainer container = default(BaseContainer);
		if (!_containerSystem.TryGetContainer(uid, slotDefinition.Name, ref container, containerComp))
		{
			if ((int)((Component)inventory).LifeStage >= 4)
			{
				((EntitySystem)this).Log.Error($"Missing inventory container {slot} on entity {((EntitySystem)this).ToPrettyString(Entity<MetaDataComponent>.op_Implicit(uid))}");
			}
			return false;
		}
		ContainerSlot containerSlotChecked = (ContainerSlot)(object)((container is ContainerSlot) ? container : null);
		if (containerSlotChecked == null)
		{
			return false;
		}
		containerSlot = containerSlotChecked;
		return true;
	}

	public bool HasSlot(EntityUid uid, string slot, InventoryComponent? component = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SlotDefinition slotDefinition;
		return TryGetSlot(uid, slot, out slotDefinition, component);
	}

	public bool TryGetSlot(EntityUid uid, string slot, [NotNullWhen(true)] out SlotDefinition? slotDefinition, InventoryComponent? inventory = null)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		slotDefinition = null;
		if (!((EntitySystem)this).Resolve<InventoryComponent>(uid, ref inventory, false))
		{
			return false;
		}
		SlotDefinition[] slots = inventory.Slots;
		foreach (SlotDefinition slotDef in slots)
		{
			if (slotDef.Name.Equals(slot))
			{
				slotDefinition = slotDef;
				return true;
			}
		}
		return false;
	}

	public bool TryGetContainerSlotEnumerator(Entity<InventoryComponent?> entity, out InventorySlotEnumerator containerSlotEnumerator, SlotFlags flags = SlotFlags.All)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InventoryComponent>(entity.Owner, ref entity.Comp, false))
		{
			containerSlotEnumerator = default(InventorySlotEnumerator);
			return false;
		}
		containerSlotEnumerator = new InventorySlotEnumerator(entity.Comp, flags);
		return true;
	}

	public InventorySlotEnumerator GetSlotEnumerator(Entity<InventoryComponent?> entity, SlotFlags flags = SlotFlags.All)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!((EntitySystem)this).Resolve<InventoryComponent>(entity.Owner, ref entity.Comp, false))
		{
			return InventorySlotEnumerator.Empty;
		}
		return new InventorySlotEnumerator(entity.Comp, flags);
	}

	public bool TryGetSlots(EntityUid uid, [NotNullWhen(true)] out SlotDefinition[]? slotDefinitions)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inv = default(InventoryComponent);
		if (!((EntitySystem)this).TryComp<InventoryComponent>(uid, ref inv))
		{
			slotDefinitions = null;
			return false;
		}
		slotDefinitions = inv.Slots;
		return true;
	}

	private ViewVariablesPath? HandleViewVariablesSlots(EntityUid uid, InventoryComponent comp, string relativePath)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetSlotEntity(uid, relativePath, out var entity, comp))
		{
			return null;
		}
		return (ViewVariablesPath?)(object)ViewVariablesPath.FromObject((object)entity);
	}

	private IEnumerable<string> ListViewVariablesSlots(EntityUid uid, InventoryComponent comp)
	{
		SlotDefinition[] slots = comp.Slots;
		foreach (SlotDefinition slotDef in slots)
		{
			yield return slotDef.Name;
		}
	}

	public void SetTemplateId(Entity<InventoryComponent> ent, ProtoId<InventoryTemplatePrototype> newTemplate)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		if (!_prototypeManager.Index<InventoryTemplatePrototype>(newTemplate).Slots.Select((SlotDefinition x) => x.Name).SequenceEqual(ent.Comp.Slots.Select((SlotDefinition x) => x.Name)))
		{
			throw new ArgumentException("Incompatible inventory template!");
		}
		ent.Comp.TemplateId = ProtoId<InventoryTemplatePrototype>.op_Implicit(newTemplate);
		((EntitySystem)this).Dirty<InventoryComponent>(ent, (MetaDataComponent)null);
	}

	public bool ForceSetTemplateId(Entity<InventoryComponent?> ent, ProtoId<InventoryTemplatePrototype> newTemplate)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inventory = ent.Comp;
		if (!((EntitySystem)this).Resolve<InventoryComponent>(ent.Owner, ref inventory, false))
		{
			return false;
		}
		InventoryTemplatePrototype inventoryTemplatePrototype = default(InventoryTemplatePrototype);
		if (!_prototypeManager.TryIndex<InventoryTemplatePrototype>(newTemplate, ref inventoryTemplatePrototype))
		{
			return false;
		}
		bool num = ProtoId<InventoryTemplatePrototype>.op_Implicit(inventory.TemplateId) != newTemplate;
		if (num)
		{
			inventory.TemplateId = ProtoId<InventoryTemplatePrototype>.op_Implicit(newTemplate);
		}
		bool slotsChanged = EnsureTemplateSlots(Entity<InventoryComponent>.op_Implicit((ent.Owner, inventory)));
		if (num || slotsChanged)
		{
			((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)inventory, (MetaDataComponent)null);
		}
		return num || slotsChanged;
	}

	public bool EnsureTemplateSlots(Entity<InventoryComponent?> ent)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		InventoryComponent inventory = ent.Comp;
		if (!((EntitySystem)this).Resolve<InventoryComponent>(ent.Owner, ref inventory, false))
		{
			return false;
		}
		InventoryTemplatePrototype invTemplate = default(InventoryTemplatePrototype);
		if (!_prototypeManager.TryIndex<InventoryTemplatePrototype>(inventory.TemplateId, ref invTemplate))
		{
			return false;
		}
		if (inventory.Slots.Length == invTemplate.Slots.Length)
		{
			bool matches = true;
			for (int i = 0; i < invTemplate.Slots.Length; i++)
			{
				SlotDefinition current = inventory.Slots[i];
				SlotDefinition template = invTemplate.Slots[i];
				if (!(current.Name == template.Name) || current.SlotFlags != template.SlotFlags || !(current.SlotGroup == template.SlotGroup) || current.ShowInWindow != template.ShowInWindow || !(current.UIWindowPosition == template.UIWindowPosition) || !(current.StrippingWindowPos == template.StrippingWindowPos))
				{
					matches = false;
					break;
				}
			}
			if (matches)
			{
				return false;
			}
		}
		Dictionary<string, ContainerSlot> containersByName = new Dictionary<string, ContainerSlot>(inventory.Slots.Length);
		for (int j = 0; j < inventory.Slots.Length && j < inventory.Containers.Length; j++)
		{
			containersByName[inventory.Slots[j].Name] = inventory.Containers[j];
		}
		ContainerSlot[] containers = (ContainerSlot[])(object)new ContainerSlot[invTemplate.Slots.Length];
		for (int k = 0; k < invTemplate.Slots.Length; k++)
		{
			SlotDefinition slot = invTemplate.Slots[k];
			if (containersByName.TryGetValue(slot.Name, out var existing))
			{
				containers[k] = existing;
				continue;
			}
			ContainerSlot container = _containerSystem.EnsureContainer<ContainerSlot>(ent.Owner, slot.Name, (ContainerManagerComponent)null);
			((BaseContainer)container).OccludesLight = false;
			containers[k] = container;
		}
		inventory.Slots = invTemplate.Slots;
		inventory.Containers = containers;
		((EntitySystem)this).Dirty(ent.Owner, (IComponent)(object)inventory, (MetaDataComponent)null);
		return true;
	}
}
