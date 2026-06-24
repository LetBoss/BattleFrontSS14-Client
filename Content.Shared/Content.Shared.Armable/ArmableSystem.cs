using System;
using Content.Shared.Examine;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Content.Shared.Armable;

public sealed class ArmableSystem : EntitySystem
{
	[Dependency]
	private ItemToggleSystem _itemToggle;

	public override void Initialize()
	{
		((EntitySystem)this).Initialize();
		((EntitySystem)this).SubscribeLocalEvent<ArmableComponent, ExaminedEvent>((ComponentEventHandler<ArmableComponent, ExaminedEvent>)OnExamine, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<ArmableComponent, ItemToggledEvent>((EntityEventRefHandler<ArmableComponent, ItemToggledEvent>)ArmingDone, (Type[])null, (Type[])null);
	}

	private void OnExamine(EntityUid uid, ArmableComponent comp, ExaminedEvent args)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		ItemToggleComponent itemToggle = default(ItemToggleComponent);
		if (!args.IsInDetailsRange || !comp.ShowStatusOnExamination || !((EntitySystem)this).TryComp<ItemToggleComponent>(uid, ref itemToggle))
		{
			return;
		}
		if (itemToggle.Activated)
		{
			LocId? examineTextArmed = comp.ExamineTextArmed;
			if (!string.IsNullOrEmpty(examineTextArmed.HasValue ? LocId.op_Implicit(examineTextArmed.GetValueOrDefault()) : null))
			{
				ILocalizationManager loc = base.Loc;
				examineTextArmed = comp.ExamineTextArmed;
				args.PushMarkup(loc.GetString(examineTextArmed.HasValue ? LocId.op_Implicit(examineTextArmed.GetValueOrDefault()) : null, (ValueTuple<string, object>)("name", uid)));
			}
		}
		else
		{
			LocId? examineTextArmed = comp.ExamineTextNotArmed;
			if (!string.IsNullOrEmpty(examineTextArmed.HasValue ? LocId.op_Implicit(examineTextArmed.GetValueOrDefault()) : null))
			{
				ILocalizationManager loc2 = base.Loc;
				examineTextArmed = comp.ExamineTextNotArmed;
				args.PushMarkup(loc2.GetString(examineTextArmed.HasValue ? LocId.op_Implicit(examineTextArmed.GetValueOrDefault()) : null, (ValueTuple<string, object>)("name", uid)));
			}
		}
	}

	private void ArmingDone(Entity<ArmableComponent> entity, ref ItemToggledEvent args)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (args.Activated)
		{
			_itemToggle.SetOnActivate(Entity<ItemToggleComponent>.op_Implicit(entity.Owner), val: false);
		}
	}
}
