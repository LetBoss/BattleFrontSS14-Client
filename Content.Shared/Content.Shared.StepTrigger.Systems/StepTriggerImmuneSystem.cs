using System;
using Content.Shared._RMC14.StepTrigger;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.StepTrigger.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Content.Shared.StepTrigger.Systems;

public sealed class StepTriggerImmuneSystem : EntitySystem
{
	[Dependency]
	private InventorySystem _inventory;

	public override void Initialize()
	{
		((EntitySystem)this).SubscribeLocalEvent<PreventableStepTriggerComponent, StepTriggerAttemptEvent>((EntityEventRefHandler<PreventableStepTriggerComponent, StepTriggerAttemptEvent>)OnStepTriggerClothingAttempt, (Type[])null, (Type[])null);
		((EntitySystem)this).SubscribeLocalEvent<PreventableStepTriggerComponent, ExaminedEvent>((ComponentEventHandler<PreventableStepTriggerComponent, ExaminedEvent>)OnExamined, (Type[])null, (Type[])null);
	}

	private void OnStepTriggerClothingAttempt(Entity<PreventableStepTriggerComponent> ent, ref StepTriggerAttemptEvent args)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (((EntitySystem)this).HasComp<ProtectedFromStepTriggersComponent>(args.Tripper) || _inventory.TryGetInventoryEntity<ProtectedFromStepTriggersComponent>(Entity<InventoryComponent>.op_Implicit(args.Tripper), out Entity<ProtectedFromStepTriggersComponent> _))
		{
			args.Cancelled = true;
		}
		if (((EntitySystem)this).HasComp<ImmuneToClothingRequiredStepTriggerComponent>(args.Tripper))
		{
			args.Cancelled = true;
		}
	}

	private void OnExamined(EntityUid uid, PreventableStepTriggerComponent component, ExaminedEvent args)
	{
		args.PushMarkup(base.Loc.GetString("clothing-required-step-trigger-examine"));
	}
}
