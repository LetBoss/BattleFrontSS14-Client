using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Fruit.Events;

public sealed class XenoFruitPlantActionEvent : InstantActionEvent, ISerializationGenerated<XenoFruitPlantActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool CheckWeeds = true;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 PlasmaCost = 100;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 HealthCost = 50;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoFruitPlantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoFruitPlantActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoFruitPlantActionEvent>(this, ref target, hookCtx, false, context))
		{
			bool CheckWeedsTemp = false;
			if (!serialization.TryCustomCopy<bool>(CheckWeeds, ref CheckWeedsTemp, hookCtx, false, context))
			{
				CheckWeedsTemp = CheckWeeds;
			}
			target.CheckWeeds = CheckWeedsTemp;
			FixedPoint2 PlasmaCostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = serialization.CreateCopy<FixedPoint2>(PlasmaCost, hookCtx, context, false);
			}
			target.PlasmaCost = PlasmaCostTemp;
			FixedPoint2 HealthCostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(HealthCost, ref HealthCostTemp, hookCtx, false, context))
			{
				HealthCostTemp = serialization.CreateCopy<FixedPoint2>(HealthCost, hookCtx, context, false);
			}
			target.HealthCost = HealthCostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoFruitPlantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoFruitPlantActionEvent cast = (XenoFruitPlantActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoFruitPlantActionEvent cast = (XenoFruitPlantActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoFruitPlantActionEvent Instantiate()
	{
		return new XenoFruitPlantActionEvent();
	}
}
