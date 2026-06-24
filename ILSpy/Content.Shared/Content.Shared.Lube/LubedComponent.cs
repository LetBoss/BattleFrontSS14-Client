using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Lube;

[RegisterComponent]
public sealed class LubedComponent : Component, ISerializationGenerated<LubedComponent>, ISerializationGenerated
{
	[DataField("slipsLeft", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int SlipsLeft;

	[DataField("slipStrength", false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int SlipStrength;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref LubedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (LubedComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<LubedComponent>(this, ref target, hookCtx, false, context))
		{
			int SlipsLeftTemp = 0;
			if (!serialization.TryCustomCopy<int>(SlipsLeft, ref SlipsLeftTemp, hookCtx, false, context))
			{
				SlipsLeftTemp = SlipsLeft;
			}
			target.SlipsLeft = SlipsLeftTemp;
			int SlipStrengthTemp = 0;
			if (!serialization.TryCustomCopy<int>(SlipStrength, ref SlipStrengthTemp, hookCtx, false, context))
			{
				SlipStrengthTemp = SlipStrength;
			}
			target.SlipStrength = SlipStrengthTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref LubedComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LubedComponent cast = (LubedComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LubedComponent cast = (LubedComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		LubedComponent def = (LubedComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override LubedComponent Instantiate()
	{
		return new LubedComponent();
	}
}
