using System;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Prying.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PryUnpoweredComponent : Component, ISerializationGenerated<PryUnpoweredComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float PryModifier = 0.1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PryUnpoweredComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PryUnpoweredComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PryUnpoweredComponent>(this, ref target, hookCtx, false, context))
		{
			float PryModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(PryModifier, ref PryModifierTemp, hookCtx, false, context))
			{
				PryModifierTemp = PryModifier;
			}
			target.PryModifier = PryModifierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PryUnpoweredComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PryUnpoweredComponent cast = (PryUnpoweredComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PryUnpoweredComponent cast = (PryUnpoweredComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PryUnpoweredComponent def = (PryUnpoweredComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PryUnpoweredComponent Instantiate()
	{
		return new PryUnpoweredComponent();
	}
}
