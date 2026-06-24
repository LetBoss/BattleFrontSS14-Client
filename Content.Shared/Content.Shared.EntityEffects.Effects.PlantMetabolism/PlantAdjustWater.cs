using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantAdjustWater : PlantAdjustAttribute<PlantAdjustWater>, ISerializationGenerated<PlantAdjustWater>, ISerializationGenerated
{
	public override string GuidebookAttributeName { get; set; } = "plant-attribute-water";

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantAdjustWater target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustAttribute<PlantAdjustWater> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantAdjustWater)definitionCast;
		serialization.TryCustomCopy<PlantAdjustWater>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantAdjustWater target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref PlantAdjustAttribute<PlantAdjustWater> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustWater cast = (PlantAdjustWater)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustWater cast = (PlantAdjustWater)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantAdjustWater Instantiate()
	{
		return new PlantAdjustWater();
	}
}
