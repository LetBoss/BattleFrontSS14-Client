using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class AdjustTemperature : EventEntityEffect<AdjustTemperature>, ISerializationGenerated<AdjustTemperature>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Amount;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-adjust-temperature", new(string, object)[3]
		{
			("chance", Probability),
			("deltasign", MathF.Sign(Amount)),
			("amount", MathF.Abs(Amount))
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AdjustTemperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<AdjustTemperature> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (AdjustTemperature)definitionCast;
		if (!serialization.TryCustomCopy<AdjustTemperature>(this, ref target, hookCtx, false, context))
		{
			float AmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AdjustTemperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<AdjustTemperature> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustTemperature cast = (AdjustTemperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AdjustTemperature cast = (AdjustTemperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override AdjustTemperature Instantiate()
	{
		return new AdjustTemperature();
	}
}
