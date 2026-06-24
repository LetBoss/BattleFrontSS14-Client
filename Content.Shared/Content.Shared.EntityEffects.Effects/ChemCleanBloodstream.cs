using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ChemCleanBloodstream : EventEntityEffect<ChemCleanBloodstream>, ISerializationGenerated<ChemCleanBloodstream>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float CleanseRate = 3f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-chem-clean-bloodstream", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChemCleanBloodstream target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<ChemCleanBloodstream> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChemCleanBloodstream)definitionCast;
		if (!serialization.TryCustomCopy<ChemCleanBloodstream>(this, ref target, hookCtx, false, context))
		{
			float CleanseRateTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CleanseRate, ref CleanseRateTemp, hookCtx, false, context))
			{
				CleanseRateTemp = CleanseRate;
			}
			target.CleanseRate = CleanseRateTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChemCleanBloodstream target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ChemCleanBloodstream> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemCleanBloodstream cast = (ChemCleanBloodstream)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemCleanBloodstream cast = (ChemCleanBloodstream)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChemCleanBloodstream Instantiate()
	{
		return new ChemCleanBloodstream();
	}
}
