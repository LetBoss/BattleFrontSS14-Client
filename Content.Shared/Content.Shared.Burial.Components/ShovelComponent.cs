using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Burial.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ShovelComponent : Component, ISerializationGenerated<ShovelComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public float SpeedModifier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ShovelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ShovelComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ShovelComponent>(this, ref target, hookCtx, false, context))
		{
			float SpeedModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SpeedModifier, ref SpeedModifierTemp, hookCtx, false, context))
			{
				SpeedModifierTemp = SpeedModifier;
			}
			target.SpeedModifier = SpeedModifierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ShovelComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShovelComponent cast = (ShovelComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShovelComponent cast = (ShovelComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ShovelComponent def = (ShovelComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ShovelComponent Instantiate()
	{
		return new ShovelComponent();
	}
}
