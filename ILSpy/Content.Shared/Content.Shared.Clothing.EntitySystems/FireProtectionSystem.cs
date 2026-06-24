using System;
using Content.Shared.Armor;
using Content.Shared.Atmos;
using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;

namespace Content.Shared.Clothing.EntitySystems;

public sealed class FireProtectionSystem : EntitySystem
{
	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<FireProtectionComponent, InventoryRelayedEvent<GetFireProtectionEvent>>((EntityEventRefHandler<FireProtectionComponent, InventoryRelayedEvent<GetFireProtectionEvent>>)OnGetProtection, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<FireProtectionComponent, ArmorExamineEvent>((EntityEventRefHandler<FireProtectionComponent, ArmorExamineEvent>)OnArmorExamine, (Type[])null, (Type[])null);
	}

	private void OnGetProtection(Entity<FireProtectionComponent> ent, ref InventoryRelayedEvent<GetFireProtectionEvent> args)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		args.Args.Reduce(ent.Comp.Reduction);
	}

	private void OnArmorExamine(Entity<FireProtectionComponent> ent, ref ArmorExamineEvent args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		float value = MathF.Round(ent.Comp.Reduction * 100f, 1);
		if (value != 0f)
		{
			args.Msg.PushNewline();
			args.Msg.AddMarkupOrThrow(base.Loc.GetString(LocId.op_Implicit(ent.Comp.ExamineMessage), (ValueTuple<string, object>)("value", value)));
		}
	}
}
