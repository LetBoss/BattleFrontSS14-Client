using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(SlowOnDamageSystem) })]
public sealed class ClothingSlowOnDamageModifierComponent : Component, ISerializationGenerated<ClothingSlowOnDamageModifierComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Modifier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClothingSlowOnDamageModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ClothingSlowOnDamageModifierComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ClothingSlowOnDamageModifierComponent>(this, ref target, hookCtx, false, context))
		{
			float ModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Modifier, ref ModifierTemp, hookCtx, false, context))
			{
				ModifierTemp = Modifier;
			}
			target.Modifier = ModifierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClothingSlowOnDamageModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingSlowOnDamageModifierComponent cast = (ClothingSlowOnDamageModifierComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingSlowOnDamageModifierComponent cast = (ClothingSlowOnDamageModifierComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingSlowOnDamageModifierComponent def = (ClothingSlowOnDamageModifierComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClothingSlowOnDamageModifierComponent Instantiate()
	{
		return new ClothingSlowOnDamageModifierComponent();
	}
}
