using System;
using System.Collections.Generic;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class SlowOnDamageComponent : Component, ISerializationGenerated<SlowOnDamageComponent>, ISerializationGenerated
{
	[DataField("speedModifierThresholds", false, 1, true, false, null)]
	public Dictionary<FixedPoint2, float> SpeedModifierThresholds;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SlowOnDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SlowOnDamageComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SlowOnDamageComponent>(this, ref target, hookCtx, false, context))
		{
			Dictionary<FixedPoint2, float> SpeedModifierThresholdsTemp = null;
			if (SpeedModifierThresholds == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<Dictionary<FixedPoint2, float>>(SpeedModifierThresholds, ref SpeedModifierThresholdsTemp, hookCtx, true, context))
			{
				SpeedModifierThresholdsTemp = serialization.CreateCopy<Dictionary<FixedPoint2, float>>(SpeedModifierThresholds, hookCtx, context, false);
			}
			target.SpeedModifierThresholds = SpeedModifierThresholdsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SlowOnDamageComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlowOnDamageComponent cast = (SlowOnDamageComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlowOnDamageComponent cast = (SlowOnDamageComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SlowOnDamageComponent def = (SlowOnDamageComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SlowOnDamageComponent Instantiate()
	{
		return new SlowOnDamageComponent();
	}
}
