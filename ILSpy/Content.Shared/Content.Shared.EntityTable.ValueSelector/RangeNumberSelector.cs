using System;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityTable.ValueSelector;

public sealed class RangeNumberSelector : NumberSelector, ISerializationGenerated<RangeNumberSelector>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Vector2i Range = new Vector2i(1, 1);

	public RangeNumberSelector(Vector2i range)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Range = range;
	}

	public override int Get(System.Random rand)
	{
		return rand.Next(Range.X, Range.Y + 1);
	}

	public RangeNumberSelector()
	{
	}//IL_0003: Unknown result type (might be due to invalid IL or missing references)
	//IL_0008: Unknown result type (might be due to invalid IL or missing references)


	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RangeNumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		NumberSelector definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (RangeNumberSelector)definitionCast;
		if (!serialization.TryCustomCopy<RangeNumberSelector>(this, ref target, hookCtx, false, context))
		{
			Vector2i RangeTemp = default(Vector2i);
			if (!serialization.TryCustomCopy<Vector2i>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = serialization.CreateCopy<Vector2i>(Range, hookCtx, context, false);
			}
			target.Range = RangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RangeNumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref NumberSelector target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RangeNumberSelector cast = (RangeNumberSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RangeNumberSelector cast = (RangeNumberSelector)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override RangeNumberSelector Instantiate()
	{
		return new RangeNumberSelector();
	}
}
