using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Stealth.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class StealthOnMoveComponent : Component, ISerializationGenerated<StealthOnMoveComponent>, ISerializationGenerated
{
	[DataField("passiveVisibilityRate", false, 1, false, false, null)]
	public float PassiveVisibilityRate = -0.15f;

	[DataField("movementVisibilityRate", false, 1, false, false, null)]
	public float MovementVisibilityRate = 0.2f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StealthOnMoveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StealthOnMoveComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<StealthOnMoveComponent>(this, ref target, hookCtx, false, context))
		{
			float PassiveVisibilityRateTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PassiveVisibilityRate, ref PassiveVisibilityRateTemp, hookCtx, false, context))
			{
				PassiveVisibilityRateTemp = PassiveVisibilityRate;
			}
			target.PassiveVisibilityRate = PassiveVisibilityRateTemp;
			float MovementVisibilityRateTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MovementVisibilityRate, ref MovementVisibilityRateTemp, hookCtx, false, context))
			{
				MovementVisibilityRateTemp = MovementVisibilityRate;
			}
			target.MovementVisibilityRate = MovementVisibilityRateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StealthOnMoveComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealthOnMoveComponent cast = (StealthOnMoveComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealthOnMoveComponent cast = (StealthOnMoveComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StealthOnMoveComponent def = (StealthOnMoveComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StealthOnMoveComponent Instantiate()
	{
		return new StealthOnMoveComponent();
	}
}
