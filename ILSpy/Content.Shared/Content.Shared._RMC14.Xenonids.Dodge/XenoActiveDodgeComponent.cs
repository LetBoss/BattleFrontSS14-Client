using System;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Xenonids.Dodge;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(XenoDodgeSystem) })]
public sealed class XenoActiveDodgeComponent : Component, ISerializationGenerated<XenoActiveDodgeComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 SpeedMult = FixedPoint2.New(0.25);

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 CrowdSpeedAddMult = FixedPoint2.New(0.25);

	[DataField(null, false, 1, false, false, null)]
	public float CrowdRange = 1.5f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan ExpiresAt;

	[DataField(null, false, 1, false, false, null)]
	public bool InCrowd;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref XenoActiveDodgeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (XenoActiveDodgeComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<XenoActiveDodgeComponent>(this, ref target, hookCtx, false, context))
		{
			FixedPoint2 SpeedMultTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(SpeedMult, ref SpeedMultTemp, hookCtx, false, context))
			{
				SpeedMultTemp = serialization.CreateCopy<FixedPoint2>(SpeedMult, hookCtx, context, false);
			}
			target.SpeedMult = SpeedMultTemp;
			FixedPoint2 CrowdSpeedAddMultTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(CrowdSpeedAddMult, ref CrowdSpeedAddMultTemp, hookCtx, false, context))
			{
				CrowdSpeedAddMultTemp = serialization.CreateCopy<FixedPoint2>(CrowdSpeedAddMult, hookCtx, context, false);
			}
			target.CrowdSpeedAddMult = CrowdSpeedAddMultTemp;
			float CrowdRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CrowdRange, ref CrowdRangeTemp, hookCtx, false, context))
			{
				CrowdRangeTemp = CrowdRange;
			}
			target.CrowdRange = CrowdRangeTemp;
			TimeSpan ExpiresAtTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(ExpiresAt, ref ExpiresAtTemp, hookCtx, false, context))
			{
				ExpiresAtTemp = serialization.CreateCopy<TimeSpan>(ExpiresAt, hookCtx, context, false);
			}
			target.ExpiresAt = ExpiresAtTemp;
			bool InCrowdTemp = false;
			if (!serialization.TryCustomCopy<bool>(InCrowd, ref InCrowdTemp, hookCtx, false, context))
			{
				InCrowdTemp = InCrowd;
			}
			target.InCrowd = InCrowdTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref XenoActiveDodgeComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoActiveDodgeComponent cast = (XenoActiveDodgeComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoActiveDodgeComponent cast = (XenoActiveDodgeComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		XenoActiveDodgeComponent def = (XenoActiveDodgeComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override XenoActiveDodgeComponent Instantiate()
	{
		return new XenoActiveDodgeComponent();
	}
}
