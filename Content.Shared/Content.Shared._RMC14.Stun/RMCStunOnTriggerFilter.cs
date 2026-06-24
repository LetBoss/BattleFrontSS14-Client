using System;
using Content.Shared.Whitelist;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Stun;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RMCStunOnTriggerFilter : ISerializationGenerated<RMCStunOnTriggerFilter>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float? Range;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? Stun;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? Paralyze;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? Flash;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? FlashAdditionalStunTime;

	[DataField(null, false, 1, false, false, null)]
	public float? Probability;

	[DataField(null, false, 1, false, false, null)]
	public EntityWhitelist Whitelist;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCStunOnTriggerFilter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<RMCStunOnTriggerFilter>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		float? RangeTemp = null;
		if (!serialization.TryCustomCopy<float?>(Range, ref RangeTemp, hookCtx, false, context))
		{
			RangeTemp = Range;
		}
		target.Range = RangeTemp;
		TimeSpan? StunTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(Stun, ref StunTemp, hookCtx, false, context))
		{
			StunTemp = serialization.CreateCopy<TimeSpan?>(Stun, hookCtx, context, false);
		}
		target.Stun = StunTemp;
		TimeSpan? ParalyzeTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(Paralyze, ref ParalyzeTemp, hookCtx, false, context))
		{
			ParalyzeTemp = serialization.CreateCopy<TimeSpan?>(Paralyze, hookCtx, context, false);
		}
		target.Paralyze = ParalyzeTemp;
		TimeSpan? FlashTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(Flash, ref FlashTemp, hookCtx, false, context))
		{
			FlashTemp = serialization.CreateCopy<TimeSpan?>(Flash, hookCtx, context, false);
		}
		target.Flash = FlashTemp;
		TimeSpan? FlashAdditionalStunTimeTemp = null;
		if (!serialization.TryCustomCopy<TimeSpan?>(FlashAdditionalStunTime, ref FlashAdditionalStunTimeTemp, hookCtx, false, context))
		{
			FlashAdditionalStunTimeTemp = serialization.CreateCopy<TimeSpan?>(FlashAdditionalStunTime, hookCtx, context, false);
		}
		target.FlashAdditionalStunTime = FlashAdditionalStunTimeTemp;
		float? ProbabilityTemp = null;
		if (!serialization.TryCustomCopy<float?>(Probability, ref ProbabilityTemp, hookCtx, false, context))
		{
			ProbabilityTemp = Probability;
		}
		target.Probability = ProbabilityTemp;
		EntityWhitelist WhitelistTemp = null;
		if (Whitelist == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, false, context))
		{
			if (Whitelist == null)
			{
				WhitelistTemp = null;
			}
			else
			{
				serialization.CopyTo<EntityWhitelist>(Whitelist, ref WhitelistTemp, hookCtx, context, true);
			}
		}
		target.Whitelist = WhitelistTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCStunOnTriggerFilter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCStunOnTriggerFilter cast = (RMCStunOnTriggerFilter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCStunOnTriggerFilter Instantiate()
	{
		return new RMCStunOnTriggerFilter();
	}
}
