using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Slippery;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class SlipperyEffectEntry : ISerializationGenerated<SlipperyEffectEntry>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan ParalyzeTime = TimeSpan.FromSeconds(1.5);

	[DataField(null, false, 1, false, false, null)]
	public float LaunchForwardsMultiplier = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public float RequiredSlipSpeed = 3.5f;

	[DataField(null, false, 1, false, false, null)]
	public bool SuperSlippery;

	[DataField(null, false, 1, false, false, null)]
	public float SlipFriction;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SlipperyEffectEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy<SlipperyEffectEntry>(this, ref target, hookCtx, false, context))
		{
			TimeSpan ParalyzeTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(ParalyzeTime, ref ParalyzeTimeTemp, hookCtx, false, context))
			{
				ParalyzeTimeTemp = serialization.CreateCopy<TimeSpan>(ParalyzeTime, hookCtx, context, false);
			}
			target.ParalyzeTime = ParalyzeTimeTemp;
			float LaunchForwardsMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LaunchForwardsMultiplier, ref LaunchForwardsMultiplierTemp, hookCtx, false, context))
			{
				LaunchForwardsMultiplierTemp = LaunchForwardsMultiplier;
			}
			target.LaunchForwardsMultiplier = LaunchForwardsMultiplierTemp;
			float RequiredSlipSpeedTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RequiredSlipSpeed, ref RequiredSlipSpeedTemp, hookCtx, false, context))
			{
				RequiredSlipSpeedTemp = RequiredSlipSpeed;
			}
			target.RequiredSlipSpeed = RequiredSlipSpeedTemp;
			bool SuperSlipperyTemp = false;
			if (!serialization.TryCustomCopy<bool>(SuperSlippery, ref SuperSlipperyTemp, hookCtx, false, context))
			{
				SuperSlipperyTemp = SuperSlippery;
			}
			target.SuperSlippery = SuperSlipperyTemp;
			float SlipFrictionTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SlipFriction, ref SlipFrictionTemp, hookCtx, false, context))
			{
				SlipFrictionTemp = SlipFriction;
			}
			target.SlipFriction = SlipFrictionTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SlipperyEffectEntry target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlipperyEffectEntry cast = (SlipperyEffectEntry)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public SlipperyEffectEntry Instantiate()
	{
		return new SlipperyEffectEntry();
	}
}
