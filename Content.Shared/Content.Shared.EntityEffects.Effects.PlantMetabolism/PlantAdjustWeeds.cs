using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantAdjustWeeds : PlantAdjustAttribute<PlantAdjustWeeds>, ISerializationGenerated<PlantAdjustWeeds>, ISerializationGenerated
{
	public override string GuidebookAttributeName { get; set; } = "plant-attribute-weeds";

	public override bool GuidebookIsAttributePositive { get; protected set; }

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantAdjustWeeds target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustAttribute<PlantAdjustWeeds> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantAdjustWeeds)definitionCast;
		serialization.TryCustomCopy<PlantAdjustWeeds>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantAdjustWeeds target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref PlantAdjustAttribute<PlantAdjustWeeds> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustWeeds cast = (PlantAdjustWeeds)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantAdjustWeeds cast = (PlantAdjustWeeds)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantAdjustWeeds Instantiate()
	{
		return new PlantAdjustWeeds();
	}
}
