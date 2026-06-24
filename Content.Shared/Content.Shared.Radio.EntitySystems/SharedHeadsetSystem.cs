using System;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Radio.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Radio.EntitySystems;

public abstract class SharedHeadsetSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<HeadsetComponent, InventoryRelayedEvent<GetDefaultRadioChannelEvent>>((ComponentEventHandler<HeadsetComponent, InventoryRelayedEvent<GetDefaultRadioChannelEvent>>)OnGetDefault, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadsetComponent, GotEquippedEvent>((ComponentEventHandler<HeadsetComponent, GotEquippedEvent>)OnGotEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<HeadsetComponent, GotUnequippedEvent>((ComponentEventHandler<HeadsetComponent, GotUnequippedEvent>)OnGotUnequipped, (Type[])null, (Type[])null);
	}

	private void OnGetDefault(EntityUid uid, HeadsetComponent component, InventoryRelayedEvent<GetDefaultRadioChannelEvent> args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		EncryptionKeyHolderComponent keyHolder = default(EncryptionKeyHolderComponent);
		if (component.Enabled && component.IsEquipped && ((EntitySystem)this).TryComp<EncryptionKeyHolderComponent>(uid, ref keyHolder))
		{
			GetDefaultRadioChannelEvent args2 = args.Args;
			if (args2.Channel == null)
			{
				args2.Channel = keyHolder.DefaultChannel;
			}
		}
	}

	protected virtual void OnGotEquipped(EntityUid uid, HeadsetComponent component, GotEquippedEvent args)
	{
		component.IsEquipped = args.SlotFlags.HasFlag(component.RequiredSlot);
	}

	protected virtual void OnGotUnequipped(EntityUid uid, HeadsetComponent component, GotUnequippedEvent args)
	{
		component.IsEquipped = false;
	}
}
