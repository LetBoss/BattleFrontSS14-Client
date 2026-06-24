using System;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.Events;

public sealed class XenoExpandWeedsActionEvent : WorldTargetActionEvent, ISerializationGenerated<XenoExpandWeedsActionEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Expand = EntProtoId.op_Implicit("XenoWeeds");

	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Source = EntProtoId.op_Implicit("XenoWeedsSource");

	[DataField(null, false, 1, false, false, null)]
	[AutoNetworkedField]
	public FixedPoint2 PlasmaCost = 50;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoExpandWeedsActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		WorldTargetActionEvent definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoExpandWeedsActionEvent)definitionCast;
		if (!serialization.TryCustomCopy<XenoExpandWeedsActionEvent>(this, ref target, hookCtx, false, context))
		{
			EntProtoId ExpandTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Expand, ref ExpandTemp, hookCtx, false, context))
			{
				ExpandTemp = serialization.CreateCopy<EntProtoId>(Expand, hookCtx, context, false);
			}
			target.Expand = ExpandTemp;
			EntProtoId SourceTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Source, ref SourceTemp, hookCtx, false, context))
			{
				SourceTemp = serialization.CreateCopy<EntProtoId>(Source, hookCtx, context, false);
			}
			target.Source = SourceTemp;
			FixedPoint2 PlasmaCostTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(PlasmaCost, ref PlasmaCostTemp, hookCtx, false, context))
			{
				PlasmaCostTemp = serialization.CreateCopy<FixedPoint2>(PlasmaCost, hookCtx, context, false);
			}
			target.PlasmaCost = PlasmaCostTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoExpandWeedsActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref WorldTargetActionEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoExpandWeedsActionEvent cast = (XenoExpandWeedsActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoExpandWeedsActionEvent cast = (XenoExpandWeedsActionEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoExpandWeedsActionEvent Instantiate()
	{
		return new XenoExpandWeedsActionEvent();
	}
}
