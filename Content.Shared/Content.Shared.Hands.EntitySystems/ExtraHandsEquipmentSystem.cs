using System;
using System.Collections.Generic;
using Content.Shared.Hands.Components;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.Hands.EntitySystems;

public sealed class ExtraHandsEquipmentSystem : EntitySystem
{
	[Dependency]
	private SharedHandsSystem _hands;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ExtraHandsEquipmentComponent, GotEquippedEvent>((EntityEventRefHandler<ExtraHandsEquipmentComponent, GotEquippedEvent>)OnEquipped, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ExtraHandsEquipmentComponent, GotUnequippedEvent>((EntityEventRefHandler<ExtraHandsEquipmentComponent, GotUnequippedEvent>)OnUnequipped, (Type[])null, (Type[])null);
	}

	private void OnEquipped(Entity<ExtraHandsEquipmentComponent> ent, ref GotEquippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(args.Equipee, ref handsComp))
		{
			return;
		}
		foreach (KeyValuePair<string, Hand> hand2 in ent.Comp.Hands)
		{
			hand2.Deconstruct(out var key, out var value);
			string handName = key;
			Hand hand = value;
			string handId = $"{((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null).Id}-{handName}";
			_hands.AddHand(Entity<HandsComponent>.op_Implicit((args.Equipee, handsComp)), handId, hand.Location);
		}
	}

	private void OnUnequipped(Entity<ExtraHandsEquipmentComponent> ent, ref GotUnequippedEvent args)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		HandsComponent handsComp = default(HandsComponent);
		if (!((EntitySystem)this).TryComp<HandsComponent>(args.Equipee, ref handsComp))
		{
			return;
		}
		foreach (string handName in ent.Comp.Hands.Keys)
		{
			string handId = $"{((EntitySystem)this).GetNetEntity(ent.Owner, (MetaDataComponent)null).Id}-{handName}";
			_hands.RemoveHand(Entity<HandsComponent>.op_Implicit((args.Equipee, handsComp)), handId);
		}
	}
}
