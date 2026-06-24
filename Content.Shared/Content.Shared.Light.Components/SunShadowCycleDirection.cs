using System;
using System.Numerics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Light.Components;

[Serializable]
[DataDefinition]
[NetSerializable]
public record struct SunShadowCycleDirection : ISerializationGenerated<SunShadowCycleDirection>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Ratio;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 Direction;

	[DataField(null, false, 1, false, false, null)]
	public float Alpha;

	public SunShadowCycleDirection(float ratio, Vector2 direction, float alpha)
	{
		Ratio = ratio;
		Direction = direction;
		Alpha = alpha;
	}

	public SunShadowCycleDirection()
	{
		Ratio = 0f;
		Direction = default(Vector2);
		Alpha = 0f;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SunShadowCycleDirection target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<SunShadowCycleDirection>(this, ref target, hookCtx, false, context))
		{
			float RatioTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Ratio, ref RatioTemp, hookCtx, false, context))
			{
				RatioTemp = Ratio;
			}
			Vector2 DirectionTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(Direction, ref DirectionTemp, hookCtx, false, context))
			{
				DirectionTemp = serialization.CreateCopy<Vector2>(Direction, hookCtx, context, false);
			}
			float AlphaTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Alpha, ref AlphaTemp, hookCtx, false, context))
			{
				AlphaTemp = Alpha;
			}
			target = target with
			{
				Ratio = RatioTemp,
				Direction = DirectionTemp,
				Alpha = AlphaTemp
			};
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SunShadowCycleDirection target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SunShadowCycleDirection cast = (SunShadowCycleDirection)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SunShadowCycleDirection Instantiate()
	{
		return new SunShadowCycleDirection();
	}
}
