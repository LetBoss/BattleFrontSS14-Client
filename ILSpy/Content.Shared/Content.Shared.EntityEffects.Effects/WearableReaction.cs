using System;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.EntityEffects.Effects;

public sealed class WearableReaction : EntityEffect, ISerializationGenerated<WearableReaction>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float AmountThreshold = 1f;

	[DataField(null, false, 1, true, false, null)]
	public string Slot;

	[DataField(null, false, 1, true, false, null)]
	public string PrototypeID;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return null;
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		if (args.EntityManager.System<InventorySystem>().SpawnItemInSlot(args.TargetEntity, Slot, PrototypeID) && args is EntityEffectReagentArgs { Reagent: not null } reagentArgs && !(reagentArgs.Quantity < AmountThreshold))
		{
			reagentArgs.Source?.RemoveReagent(reagentArgs.Reagent.ID, AmountThreshold);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref WearableReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (WearableReaction)definitionCast;
		if (!serialization.TryCustomCopy<WearableReaction>(this, ref target, hookCtx, false, context))
		{
			float AmountThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(AmountThreshold, ref AmountThresholdTemp, hookCtx, false, context))
			{
				AmountThresholdTemp = AmountThreshold;
			}
			target.AmountThreshold = AmountThresholdTemp;
			string SlotTemp = null;
			if (Slot == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Slot, ref SlotTemp, hookCtx, false, context))
			{
				SlotTemp = Slot;
			}
			target.Slot = SlotTemp;
			string PrototypeIDTemp = null;
			if (PrototypeID == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(PrototypeID, ref PrototypeIDTemp, hookCtx, false, context))
			{
				PrototypeIDTemp = PrototypeID;
			}
			target.PrototypeID = PrototypeIDTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref WearableReaction target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WearableReaction cast = (WearableReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		WearableReaction cast = (WearableReaction)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override WearableReaction Instantiate()
	{
		return new WearableReaction();
	}
}
