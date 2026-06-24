using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantAdjustPests : PlantAdjustAttribute<PlantAdjustPests>, ISerializationGenerated<PlantAdjustPests>, ISerializationGenerated
{
	public override string GuidebookAttributeName { get; set; } = "plant-attribute-pests";

	public override bool GuidebookIsAttributePositive { get; protected set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantAdjustPests target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustAttribute<PlantAdjustPests> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantAdjustPests)definitionCast;
		serialization.TryCustomCopy<PlantAdjustPests>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantAdjustPests target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref PlantAdjustAttribute<PlantAdjustPests> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustPests cast = (PlantAdjustPests)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustPests cast = (PlantAdjustPests)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantAdjustPests Instantiate()
	{
		return new PlantAdjustPests();
	}
}
