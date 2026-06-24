using System;
using Content.Client.Inventory;
using Content.Shared.Cuffs.Components;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands;
using Content.Shared.Inventory.Events;
using Content.Shared.Strip;
using Content.Shared.Strip.Components;
using Robust.Shared.GameObjects;

namespace Content.Client.Strip;

public sealed class StrippableSystem : SharedStrippableSystem
{
	public override void Initialize()
	{
		base.Initialize();
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, CuffedStateChangeEvent>((ComponentEventRefHandler<StrippableComponent, CuffedStateChangeEvent>)OnCuffStateChange, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, DidEquipEvent>((ComponentEventHandler<StrippableComponent, DidEquipEvent>)UpdateUi, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, DidUnequipEvent>((ComponentEventHandler<StrippableComponent, DidUnequipEvent>)UpdateUi, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, DidEquipHandEvent>((ComponentEventHandler<StrippableComponent, DidEquipHandEvent>)UpdateUi, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, DidUnequipHandEvent>((ComponentEventHandler<StrippableComponent, DidUnequipHandEvent>)UpdateUi, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<StrippableComponent, EnsnaredChangedEvent>((ComponentEventHandler<StrippableComponent, EnsnaredChangedEvent>)UpdateUi, (Type[])null, (Type[])null);
	}

	private void OnCuffStateChange(EntityUid uid, StrippableComponent component, ref CuffedStateChangeEvent args)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateUi(uid, component);
	}

	public void UpdateUi(EntityUid uid, StrippableComponent? component = null, EntityEventArgs? args = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UserInterfaceComponent val = default(UserInterfaceComponent);
		if (!((EntitySystem)this).TryComp<UserInterfaceComponent>(uid, ref val))
		{
			return;
		}
		foreach (BoundUserInterface value in val.ClientOpenInterfaces.Values)
		{
			if (value is StrippableBoundUserInterface strippableBoundUserInterface)
			{
				strippableBoundUserInterface.DirtyMenu();
			}
		}
	}
}
