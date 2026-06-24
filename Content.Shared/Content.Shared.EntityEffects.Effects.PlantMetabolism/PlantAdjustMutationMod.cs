using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantAdjustMutationMod : PlantAdjustAttribute<PlantAdjustMutationMod>, ISerializationGenerated<PlantAdjustMutationMod>, ISerializationGenerated
{
	public override string GuidebookAttributeName { get; set; } = "plant-attribute-mutation-mod";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantAdjustMutationMod target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustAttribute<PlantAdjustMutationMod> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantAdjustMutationMod)definitionCast;
		serialization.TryCustomCopy<PlantAdjustMutationMod>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantAdjustMutationMod target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref PlantAdjustAttribute<PlantAdjustMutationMod> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustMutationMod cast = (PlantAdjustMutationMod)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustMutationMod cast = (PlantAdjustMutationMod)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantAdjustMutationMod Instantiate()
	{
		return new PlantAdjustMutationMod();
	}
}
