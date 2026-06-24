using System;
using Content.Shared.Drunk;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Traits.Assorted;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SharedDrunkSystem) })]
public sealed class LightweightDrunkComponent : Component, ISerializationGenerated<LightweightDrunkComponent>, ISerializationGenerated
{
	[DataField("boozeStrengthMultiplier", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float BoozeStrengthMultiplier = 4f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LightweightDrunkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LightweightDrunkComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LightweightDrunkComponent>(this, ref target, hookCtx, false, context))
		{
			float BoozeStrengthMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BoozeStrengthMultiplier, ref BoozeStrengthMultiplierTemp, hookCtx, false, context))
			{
				BoozeStrengthMultiplierTemp = BoozeStrengthMultiplier;
			}
			target.BoozeStrengthMultiplier = BoozeStrengthMultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LightweightDrunkComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightweightDrunkComponent cast = (LightweightDrunkComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightweightDrunkComponent cast = (LightweightDrunkComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LightweightDrunkComponent def = (LightweightDrunkComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LightweightDrunkComponent Instantiate()
	{
		return new LightweightDrunkComponent();
	}
}
