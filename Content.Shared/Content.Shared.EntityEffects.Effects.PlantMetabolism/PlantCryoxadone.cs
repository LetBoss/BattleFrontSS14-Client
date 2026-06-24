using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantCryoxadone : EventEntityEffect<PlantCryoxadone>, ISerializationGenerated<PlantCryoxadone>, ISerializationGenerated
{
	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-plant-cryoxadone", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PlantCryoxadone target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<PlantCryoxadone> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PlantCryoxadone)definitionCast;
		serialization.TryCustomCopy<PlantCryoxadone>(this, ref target, hookCtx, false, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PlantCryoxadone target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<PlantCryoxadone> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantCryoxadone cast = (PlantCryoxadone)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PlantCryoxadone cast = (PlantCryoxadone)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PlantCryoxadone Instantiate()
	{
		return new PlantCryoxadone();
	}
}
