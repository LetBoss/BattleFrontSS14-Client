using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Explosion.Implosion;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RMCImplosion : ISerializationGenerated<RMCImplosion>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float PullRange;

	[DataField(null, false, 1, false, false, null)]
	public float PullDistance;

	[DataField(null, false, 1, false, false, null)]
	public float PullSpeed;

	[DataField(null, false, 1, false, false, null)]
	public bool IgnoreSize = true;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCImplosion target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<RMCImplosion>(this, ref target, hookCtx, false, context))
		{
			float PullRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PullRange, ref PullRangeTemp, hookCtx, false, context))
			{
				PullRangeTemp = PullRange;
			}
			target.PullRange = PullRangeTemp;
			float PullDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PullDistance, ref PullDistanceTemp, hookCtx, false, context))
			{
				PullDistanceTemp = PullDistance;
			}
			target.PullDistance = PullDistanceTemp;
			float PullSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PullSpeed, ref PullSpeedTemp, hookCtx, false, context))
			{
				PullSpeedTemp = PullSpeed;
			}
			target.PullSpeed = PullSpeedTemp;
			bool IgnoreSizeTemp = false;
			if (!serialization.TryCustomCopy<bool>(IgnoreSize, ref IgnoreSizeTemp, hookCtx, false, context))
			{
				IgnoreSizeTemp = IgnoreSize;
			}
			target.IgnoreSize = IgnoreSizeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCImplosion target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCImplosion cast = (RMCImplosion)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCImplosion Instantiate()
	{
		return new RMCImplosion();
	}
}
