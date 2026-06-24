using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ModifyBleedAmount : EventEntityEffect<ModifyBleedAmount>, ISerializationGenerated<ModifyBleedAmount>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Scaled;

	[DataField(null, false, 1, false, false, null)]
	public float Amount = -1f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-modify-bleed-amount", new(string, object)[2]
		{
			("chance", Probability),
			("deltasign", MathF.Sign(Amount))
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ModifyBleedAmount target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<ModifyBleedAmount> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ModifyBleedAmount)definitionCast;
		if (!serialization.TryCustomCopy<ModifyBleedAmount>(this, ref target, hookCtx, false, context))
		{
			bool ScaledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Scaled, ref ScaledTemp, hookCtx, false, context))
			{
				ScaledTemp = Scaled;
			}
			target.Scaled = ScaledTemp;
			float AmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ModifyBleedAmount target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ModifyBleedAmount> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyBleedAmount cast = (ModifyBleedAmount)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyBleedAmount cast = (ModifyBleedAmount)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ModifyBleedAmount Instantiate()
	{
		return new ModifyBleedAmount();
	}
}
