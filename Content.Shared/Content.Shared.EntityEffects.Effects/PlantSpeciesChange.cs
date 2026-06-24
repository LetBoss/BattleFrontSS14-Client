using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects;

public sealed class PlantSpeciesChange : EventEntityEffect<PlantSpeciesChange>, ISerializationGenerated<PlantSpeciesChange>, ISerializationGenerated
{
	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return "TODO";
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantSpeciesChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<PlantSpeciesChange> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantSpeciesChange)definitionCast;
		serialization.TryCustomCopy<PlantSpeciesChange>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantSpeciesChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<PlantSpeciesChange> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantSpeciesChange cast = (PlantSpeciesChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantSpeciesChange cast = (PlantSpeciesChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantSpeciesChange Instantiate()
	{
		return new PlantSpeciesChange();
	}
}
