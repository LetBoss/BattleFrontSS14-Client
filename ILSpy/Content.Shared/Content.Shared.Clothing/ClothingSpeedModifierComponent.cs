using System;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Clothing;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] { typeof(ClothingSpeedModifierSystem) })]
public sealed class ClothingSpeedModifierComponent : Component, ISerializationGenerated<ClothingSpeedModifierComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float WalkModifier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float SprintModifier = 1f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ClothingSpeedModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ClothingSpeedModifierComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<ClothingSpeedModifierComponent>(this, ref target, hookCtx, false, context))
		{
			float WalkModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WalkModifier, ref WalkModifierTemp, hookCtx, false, context))
			{
				WalkModifierTemp = WalkModifier;
			}
			target.WalkModifier = WalkModifierTemp;
			float SprintModifierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(SprintModifier, ref SprintModifierTemp, hookCtx, false, context))
			{
				SprintModifierTemp = SprintModifier;
			}
			target.SprintModifier = SprintModifierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ClothingSpeedModifierComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingSpeedModifierComponent cast = (ClothingSpeedModifierComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingSpeedModifierComponent cast = (ClothingSpeedModifierComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ClothingSpeedModifierComponent def = (ClothingSpeedModifierComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ClothingSpeedModifierComponent Instantiate()
	{
		return new ClothingSpeedModifierComponent();
	}
}
