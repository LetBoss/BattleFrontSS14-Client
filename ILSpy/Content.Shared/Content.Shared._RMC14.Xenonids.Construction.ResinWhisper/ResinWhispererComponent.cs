using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Construction.ResinWhisper;

[RegisterComponent]
[NetworkedComponent]
public sealed class ResinWhispererComponent : Component, ISerializationGenerated<ResinWhispererComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? StandardConstructDelay;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2? MaxConstructDistance;

	[DataField(null, false, 1, false, false, null)]
	public float MaxRemoteConstructDistance = 100f;

	[DataField(null, false, 1, false, false, null)]
	public float RemoteConstructDelayMultiplier = 2.5f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ResinWhispererComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ResinWhispererComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ResinWhispererComponent>(this, ref target, hookCtx, false, context))
		{
			TimeSpan? StandardConstructDelayTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(StandardConstructDelay, ref StandardConstructDelayTemp, hookCtx, false, context))
			{
				StandardConstructDelayTemp = serialization.CreateCopy<TimeSpan?>(StandardConstructDelay, hookCtx, context, false);
			}
			target.StandardConstructDelay = StandardConstructDelayTemp;
			FixedPoint2? MaxConstructDistanceTemp = null;
			if (!serialization.TryCustomCopy<FixedPoint2?>(MaxConstructDistance, ref MaxConstructDistanceTemp, hookCtx, false, context))
			{
				MaxConstructDistanceTemp = serialization.CreateCopy<FixedPoint2?>(MaxConstructDistance, hookCtx, context, false);
			}
			target.MaxConstructDistance = MaxConstructDistanceTemp;
			float MaxRemoteConstructDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxRemoteConstructDistance, ref MaxRemoteConstructDistanceTemp, hookCtx, false, context))
			{
				MaxRemoteConstructDistanceTemp = MaxRemoteConstructDistance;
			}
			target.MaxRemoteConstructDistance = MaxRemoteConstructDistanceTemp;
			float RemoteConstructDelayMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(RemoteConstructDelayMultiplier, ref RemoteConstructDelayMultiplierTemp, hookCtx, false, context))
			{
				RemoteConstructDelayMultiplierTemp = RemoteConstructDelayMultiplier;
			}
			target.RemoteConstructDelayMultiplier = RemoteConstructDelayMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ResinWhispererComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResinWhispererComponent cast = (ResinWhispererComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResinWhispererComponent cast = (ResinWhispererComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ResinWhispererComponent def = (ResinWhispererComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ResinWhispererComponent Instantiate()
	{
		return new ResinWhispererComponent();
	}
}
