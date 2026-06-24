using System;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;

namespace Content.Shared.Item;

public sealed class MultiHandedItemSystem : EntitySystem
{
	[Dependency]
	private IGameTiming _timing;

	[Dependency]
	private SharedHandsSystem _hands;

	[Dependency]
	private SharedPopupSystem _popup;

	[Dependency]
	private SharedVirtualItemSystem _virtualItem;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedItemComponent, GettingPickedUpAttemptEvent>((EntityEventRefHandler<MultiHandedItemComponent, GettingPickedUpAttemptEvent>)OnAttemptPickup, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedItemComponent, VirtualItemDeletedEvent>((EntityEventRefHandler<MultiHandedItemComponent, VirtualItemDeletedEvent>)OnVirtualItemDeleted, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedItemComponent, GotEquippedHandEvent>((EntityEventRefHandler<MultiHandedItemComponent, GotEquippedHandEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<MultiHandedItemComponent, GotUnequippedHandEvent>((EntityEventRefHandler<MultiHandedItemComponent, GotUnequippedHandEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnEquipped(Entity<MultiHandedItemComponent> ent, ref GotEquippedHandEvent args)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < ent.Comp.HandsNeeded - 1; i++)
		{
			_virtualItem.TrySpawnVirtualItemInHand(ent.Owner, args.User);
		}
	}

	private void OnUnequipped(Entity<MultiHandedItemComponent> ent, ref GotUnequippedHandEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		_virtualItem.DeleteInHandsMatching(args.User, ent.Owner);
	}

	private void OnAttemptPickup(Entity<MultiHandedItemComponent> ent, ref GettingPickedUpAttemptEvent args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (_hands.CountFreeHands(Entity<HandsComponent>.op_Implicit(args.User)) < ent.Comp.HandsNeeded)
		{
			((CancellableEntityEventArgs)args).Cancel();
			_popup.PopupPredictedCursor(base.Loc.GetString("multi-handed-item-pick-up-fail", (ValueTuple<string, object>)("number", ent.Comp.HandsNeeded - 1), (ValueTuple<string, object>)("item", ent.Owner)), args.User);
		}
	}

	private void OnVirtualItemDeleted(Entity<MultiHandedItemComponent> ent, ref VirtualItemDeletedEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (!(args.BlockingEntity != ent.Owner) && !_timing.ApplyingState)
		{
			_hands.TryDrop(Entity<HandsComponent>.op_Implicit(args.User), ent.Owner);
		}
	}
}
