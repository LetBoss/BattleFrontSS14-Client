using System;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Atmos;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RMCFire : ISerializationGenerated<RMCFire>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public EntProtoId Type = EntProtoId.op_Implicit("RMCTileFire");

	[DataField(null, false, 1, false, false, null)]
	public int Range;

	[DataField(null, false, 1, false, false, null)]
	public int CardinalRange;

	[DataField(null, false, 1, false, false, null)]
	public int OrdinalRange;

	[DataField(null, false, 1, false, false, null)]
	public int? Intensity;

	[DataField(null, false, 1, false, false, null)]
	public int? Duration;

	[DataField(null, false, 1, false, false, null)]
	public int? Total;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCFire target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RMCFire>(this, ref target, hookCtx, false, context))
		{
			EntProtoId TypeTemp = default(EntProtoId);
			if (!serialization.TryCustomCopy<EntProtoId>(Type, ref TypeTemp, hookCtx, false, context))
			{
				TypeTemp = serialization.CreateCopy<EntProtoId>(Type, hookCtx, context, false);
			}
			target.Type = TypeTemp;
			int RangeTemp = 0;
			if (!serialization.TryCustomCopy<int>(Range, ref RangeTemp, hookCtx, false, context))
			{
				RangeTemp = Range;
			}
			target.Range = RangeTemp;
			int CardinalRangeTemp = 0;
			if (!serialization.TryCustomCopy<int>(CardinalRange, ref CardinalRangeTemp, hookCtx, false, context))
			{
				CardinalRangeTemp = CardinalRange;
			}
			target.CardinalRange = CardinalRangeTemp;
			int OrdinalRangeTemp = 0;
			if (!serialization.TryCustomCopy<int>(OrdinalRange, ref OrdinalRangeTemp, hookCtx, false, context))
			{
				OrdinalRangeTemp = OrdinalRange;
			}
			target.OrdinalRange = OrdinalRangeTemp;
			int? IntensityTemp = null;
			if (!serialization.TryCustomCopy<int?>(Intensity, ref IntensityTemp, hookCtx, false, context))
			{
				IntensityTemp = Intensity;
			}
			target.Intensity = IntensityTemp;
			int? DurationTemp = null;
			if (!serialization.TryCustomCopy<int?>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = Duration;
			}
			target.Duration = DurationTemp;
			int? TotalTemp = null;
			if (!serialization.TryCustomCopy<int?>(Total, ref TotalTemp, hookCtx, false, context))
			{
				TotalTemp = Total;
			}
			target.Total = TotalTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCFire target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCFire cast = (RMCFire)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCFire Instantiate()
	{
		return new RMCFire();
	}
}
