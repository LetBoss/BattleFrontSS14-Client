using System;
using Content.Shared._RMC14.UniformAccessories;
using Content.Shared.Clothing.Components;
using Content.Shared.Clothing.EntitySystems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Clothing;

public sealed class RMCClothingSystem : EntitySystem
{
	[Dependency]
	private ClothingSystem _clothing;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedUniformAccessorySystem _uniformAccessories;

	[Dependency]
	private InventorySystem _inventory;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	[Dependency]
	private MovementSpeedModifierSystem _movementSpeed;

	private EntityQuery<ClothingLimitComponent> _clothingLimitQuery;

	public override void Initialize()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_clothingLimitQuery = ((EntitySystem)this).GetEntityQuery<ClothingLimitComponent>();
		((EntitySystem)this).SubscribeLocalEvent<ClothingLimitComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<ClothingLimitComponent, BeingEquippedAttemptEvent>)OnClothingLimitBeingEquippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingRequireEquippedComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<ClothingRequireEquippedComponent, BeingEquippedAttemptEvent>)OnRequireEquippedBeingEquippedAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ClothingComponent, DroppedEvent>((EntityEventRefHandler<ClothingComponent, DroppedEvent>)OnDropped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoClothingSlowdownComponent, ComponentStartup>((EntityEventRefHandler<NoClothingSlowdownComponent, ComponentStartup>)OnNoClothingSlowUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoClothingSlowdownComponent, DidEquipEvent>((EntityEventRefHandler<NoClothingSlowdownComponent, DidEquipEvent>)OnNoClothingSlowUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoClothingSlowdownComponent, DidUnequipEvent>((EntityEventRefHandler<NoClothingSlowdownComponent, DidUnequipEvent>)OnNoClothingSlowUpdate, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<NoClothingSlowdownComponent, RefreshMovementSpeedModifiersEvent>((EntityEventRefHandler<NoClothingSlowdownComponent, RefreshMovementSpeedModifiersEvent>)OnNoClothingSlowRefresh, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<RMCClothingFoldableComponent, GetVerbsEvent<AlternativeVerb>>((EntityEventRefHandler<RMCClothingFoldableComponent, GetVerbsEvent<AlternativeVerb>>)AddFoldVerb, (Type[])null, (Type[])null);
	}

	private void OnClothingLimitBeingEquippedAttempt(Entity<ClothingLimitComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (((CancellableEntityEventArgs)args).Cancelled || (args.SlotFlags & ent.Comp.Slot) == 0)
		{
			return;
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.EquipTarget), ent.Comp.Slot);
		ContainerSlot slot;
		ClothingLimitComponent otherLimit = default(ClothingLimitComponent);
		while (slots.MoveNext(out slot))
		{
			if (_clothingLimitQuery.TryComp(slot.ContainedEntity, ref otherLimit) && otherLimit.Id == ent.Comp.Id)
			{
				args.Reason = "rmc-clothing-limit";
				((CancellableEntityEventArgs)args).Cancel();
			}
		}
	}

	private void OnNoClothingSlowUpdate<T>(Entity<NoClothingSlowdownComponent> ent, ref T args) where T : EntityEventArgs
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.Active = !_inventory.TryGetSlotEntity(Entity<NoClothingSlowdownComponent>.op_Implicit(ent), ent.Comp.Slot, out var _);
		_movementSpeed.RefreshMovementSpeedModifiers(Entity<NoClothingSlowdownComponent>.op_Implicit(ent));
	}

	private void OnNoClothingSlowRefresh(Entity<NoClothingSlowdownComponent> ent, ref RefreshMovementSpeedModifiersEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (ent.Comp.Active)
		{
			args.ModifySpeed(ent.Comp.WalkModifier, ent.Comp.SprintModifier);
		}
	}

	private void OnRequireEquippedBeingEquippedAttempt(Entity<ClothingRequireEquippedComponent> ent, ref BeingEquippedAttemptEvent args)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (!((CancellableEntityEventArgs)args).Cancelled && !HasEquippedItemsWithinWhitelist(args.EquipTarget, ent.Comp.Whitelist))
		{
			((CancellableEntityEventArgs)args).Cancel();
			string denyReason = base.Loc.GetString(ent.Comp.DenyReason);
			_popup.PopupClient(denyReason, args.EquipTarget, args.EquipTarget, PopupType.SmallCaution);
		}
	}

	private void OnDropped(Entity<ClothingComponent> ent, ref DroppedEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(args.User));
		ContainerSlot slot;
		ClothingRequireEquippedComponent requiresEquipped = default(ClothingRequireEquippedComponent);
		while (slots.MoveNext(out slot))
		{
			EntityUid? containedEntity = slot.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				if (((EntitySystem)this).TryComp<ClothingRequireEquippedComponent>(contained, ref requiresEquipped) && requiresEquipped.AutoUnequip && _whitelist.IsWhitelistPassOrNull(requiresEquipped.Whitelist, ent.Owner) && !HasEquippedItemsWithinWhitelist(args.User, requiresEquipped.Whitelist))
				{
					_inventory.TryUnequip(args.User, ((BaseContainer)slot).ID);
				}
			}
		}
	}

	private bool HasEquippedItemsWithinWhitelist(EntityUid uid, EntityWhitelist? whitelist)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		foreach (EntityUid held in _hands.EnumerateHeld(Entity<HandsComponent>.op_Implicit(uid)))
		{
			if (_whitelist.IsWhitelistPassOrNull(whitelist, held))
			{
				return true;
			}
		}
		InventorySystem.InventorySlotEnumerator slots = _inventory.GetSlotEnumerator(Entity<InventoryComponent>.op_Implicit(uid));
		ContainerSlot slot;
		while (slots.MoveNext(out slot))
		{
			EntityUid? containedEntity = slot.ContainedEntity;
			if (containedEntity.HasValue)
			{
				EntityUid contained = containedEntity.GetValueOrDefault();
				if (_whitelist.IsWhitelistPassOrNull(whitelist, contained))
				{
					return true;
				}
			}
		}
		return false;
	}

	private void AddFoldVerb(Entity<RMCClothingFoldableComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Expected O, but got Unknown
		if (!args.CanAccess || !args.CanInteract || args.Hands == null)
		{
			return;
		}
		EntityUid user = args.User;
		foreach (FoldableType type in ent.Comp.Types)
		{
			AlternativeVerb verb = new AlternativeVerb
			{
				Act = delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					TryToggleFold(ent, type, user);
				},
				Text = base.Loc.GetString(LocId.op_Implicit(type.Name)),
				Icon = (SpriteSpecifier?)new Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png")),
				Priority = type.Priority
			};
			args.Verbs.Add(verb);
		}
	}

	public void TryToggleFold(Entity<RMCClothingFoldableComponent> ent, FoldableType type, EntityUid? user)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (type.Prefix == ent.Comp.ActivatedPrefix)
		{
			SetPrefix(ent, null, hideAccessories: false);
		}
		else if (type.BlacklistedPrefix == ent.Comp.ActivatedPrefix && ent.Comp.ActivatedPrefix != null)
		{
			if (type.BlacklistPopup.HasValue && user.HasValue)
			{
				ILocalizationManager loc = base.Loc;
				LocId? blacklistPopup = type.BlacklistPopup;
				string msg = loc.GetString(blacklistPopup.HasValue ? LocId.op_Implicit(blacklistPopup.GetValueOrDefault()) : null);
				_popup.PopupClient(msg, user.Value, user.Value, PopupType.SmallCaution);
			}
		}
		else
		{
			SetPrefix(ent, type.Prefix, type.HideAccessories);
		}
	}

	public void SetPrefix(Entity<RMCClothingFoldableComponent> ent, string? prefix, bool hideAccessories)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		ent.Comp.ActivatedPrefix = prefix;
		((EntitySystem)this).Dirty<RMCClothingFoldableComponent>(ent, (MetaDataComponent)null);
		_clothing.SetEquippedPrefix(ent.Owner, ent.Comp.ActivatedPrefix);
		_uniformAccessories.SetAccessoriesHidden(ent.Owner, hideAccessories);
	}
}
