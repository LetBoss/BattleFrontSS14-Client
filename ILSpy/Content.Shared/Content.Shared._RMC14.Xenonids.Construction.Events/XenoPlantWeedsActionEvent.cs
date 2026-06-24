using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Events;

public sealed class XenoPlantWeedsActionEvent : InstantActionEvent, ISerializationGenerated<XenoPlantWeedsActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 PlasmaCost = 75;

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Prototype = EntProtoId.op_Implicit("XenoWeedsSource");

	[DataField(null, false, 1, false, false, null)]
	public bool UseOnSemiWeedable;

	[DataField(null, false, 1, false, false, null)]
	public bool LimitDistance;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoPlantWeedsActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		InstantActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoPlantWeedsActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoPlantWeedsActionEvent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 PlasmaCostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = serialization.CreateCopy<FixedPoint2>(PlasmaCost, hookCtx, context, false);
			}
			target.PlasmaCost = PlasmaCostTemp;
			EntProtoId PrototypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Prototype, ref PrototypeTemp, hookCtx, false, context))
			{
				PrototypeTemp = serialization.CreateCopy<EntProtoId>(Prototype, hookCtx, context, false);
			}
			target.Prototype = PrototypeTemp;
			bool UseOnSemiWeedableTemp = false;
			if (!serialization.TryCustomCopy<bool>(UseOnSemiWeedable, ref UseOnSemiWeedableTemp, hookCtx, false, context))
			{
				UseOnSemiWeedableTemp = UseOnSemiWeedable;
			}
			target.UseOnSemiWeedable = UseOnSemiWeedableTemp;
			bool LimitDistanceTemp = false;
			if (!serialization.TryCustomCopy<bool>(LimitDistance, ref LimitDistanceTemp, hookCtx, false, context))
			{
				LimitDistanceTemp = LimitDistance;
			}
			target.LimitDistance = LimitDistanceTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoPlantWeedsActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref InstantActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlantWeedsActionEvent cast = (XenoPlantWeedsActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoPlantWeedsActionEvent cast = (XenoPlantWeedsActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoPlantWeedsActionEvent Instantiate()
	{
		return new XenoPlantWeedsActionEvent();
	}
}
