using System;
using Content.Shared.Inventory.Events;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared._RMC14.Marines.Armor;

public sealed class RMCBulkyArmorSystem : EntitySystem
{
	[Dependency]
	private SharedPopupSystem _popup;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<RMCBulkyArmorComponent, BeingEquippedAttemptEvent>((EntityEventRefHandler<RMCBulkyArmorComponent, BeingEquippedAttemptEvent>)OnBeingEquippedAttempt, (Type[])null, (Type[])null);
	}

	private void OnBeingEquippedAttempt(Entity<RMCBulkyArmorComponent> armor, ref BeingEquippedAttemptEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (armor.Comp.IsBulky && ((EntitySystem)this).HasComp<RMCUserBulkyArmorIncapableComponent>(args.EquipTarget))
		{
			if (args.EquipTarget == args.Equipee)
			{
				_popup.PopupClient(base.Loc.GetString("rmc-bulky-armor-user-unable", (ValueTuple<string, object>)("armor", armor)), args.Equipee, args.Equipee, PopupType.MediumCaution);
			}
			else
			{
				_popup.PopupEntity(base.Loc.GetString("rmc-bulky-armor-target-unable", (ValueTuple<string, object>)("target", args.EquipTarget), (ValueTuple<string, object>)("armor", armor)), args.Equipee, args.Equipee, PopupType.MediumCaution);
			}
			((CancellableEntityEventArgs)args).Cancel();
		}
	}
}
