using System;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Power.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Power.EntitySystems;

public sealed class ItemSlotRequiresPowerSystem : EntitySystem
{
	[Dependency]
	private SharedPowerReceiverSystem _receiver;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ItemSlotRequiresPowerComponent, ItemSlotInsertAttemptEvent>((EntityEventRefHandler<ItemSlotRequiresPowerComponent, ItemSlotInsertAttemptEvent>)OnInsertAttempt, (Type[])null, (Type[])null);
	}

	private void OnInsertAttempt(Entity<ItemSlotRequiresPowerComponent> ent, ref ItemSlotInsertAttemptEvent args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (!_receiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(ent.Owner)))
		{
			args.Cancelled = true;
		}
	}
}
