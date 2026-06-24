using System;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Item;
using Content.Shared.Popups;
using Content.Shared.Whitelist;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared._RMC14.Item;

public sealed class MultiHandedHolderSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedVirtualItemSystem _virtualItem;

	[Dependency]
	private EntityWhitelistSystem _whitelist;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedHolderComponent, PickupAttemptEvent>((EntityEventRefHandler<MultiHandedHolderComponent, PickupAttemptEvent>)OnPickupAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedHolderComponent, VirtualItemDeletedEvent>((EntityEventRefHandler<MultiHandedHolderComponent, VirtualItemDeletedEvent>)OnVirtualItemDeleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedHolderComponent, DidEquipHandEvent>((EntityEventRefHandler<MultiHandedHolderComponent, DidEquipHandEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedHolderComponent, DidUnequipHandEvent>((EntityEventRefHandler<MultiHandedHolderComponent, DidUnequipHandEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnPickupAttempt(Entity<MultiHandedHolderComponent> holder, ref PickupAttemptEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		int? handsNeeded = GetHandsNeeded(holder, args.Item);
		if (!handsNeeded.HasValue)
		{
			return;
		}
		int needed = handsNeeded.GetValueOrDefault();
		if (!((EntitySystem)this).HasComp<HandsComponent>(args.User) || _hands.CountFreeHands(Entity<HandsComponent>.op_Implicit(args.User)) < needed)
		{
			((CancellableEntityEventArgs)args).Cancel();
			if (_timing.IsFirstTimePredicted)
			{
				_popup.PopupCursor(base.Loc.GetString("multi-handed-item-pick-up-fail", (ValueTuple<string, object>)("number", needed - 1), (ValueTuple<string, object>)("item", args.Item)), args.User);
			}
		}
	}

	private void OnVirtualItemDeleted(Entity<MultiHandedHolderComponent> ent, ref VirtualItemDeletedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.User != ent.Owner))
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), args.BlockingEntity);
		}
	}

	private void OnEquipped(Entity<MultiHandedHolderComponent> holder, ref DidEquipHandEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		int? handsNeeded = GetHandsNeeded(holder, args.Equipped);
		if (handsNeeded.HasValue)
		{
			int hands = handsNeeded.GetValueOrDefault();
			for (int i = 0; i < hands - 1; i++)
			{
				_virtualItem.TrySpawnVirtualItemInHand(args.Equipped, args.User);
			}
		}
	}

	private void OnUnequipped(Entity<MultiHandedHolderComponent> holder, ref DidUnequipHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		_virtualItem.DeleteInHandsMatching(args.User, args.Unequipped);
	}

	private int? GetHandsNeeded(Entity<MultiHandedHolderComponent> holder, EntityUid item)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		foreach (var (hands, whitelist) in holder.Comp.Items)
		{
			if (_whitelist.IsValid(whitelist, item))
			{
				return hands;
			}
		}
		return null;
	}
}
