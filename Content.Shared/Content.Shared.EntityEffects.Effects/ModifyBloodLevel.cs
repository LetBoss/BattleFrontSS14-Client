using System;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ModifyBloodLevel : EventEntityEffect<ModifyBloodLevel>, ISerializationGenerated<ModifyBloodLevel>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public bool Scaled;

	[DataField(null, false, 1, false, false, null)]
	public FixedPoint2 Amount = 1f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-modify-blood-level", new(string, object)[2]
		{
			("chance", Probability),
			("deltasign", MathF.Sign(Amount.Float()))
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ModifyBloodLevel target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<ModifyBloodLevel> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ModifyBloodLevel)definitionCast;
		if (!serialization.TryCustomCopy<ModifyBloodLevel>(this, ref target, hookCtx, false, context))
		{
			bool ScaledTemp = false;
			if (!serialization.TryCustomCopy<bool>(Scaled, ref ScaledTemp, hookCtx, false, context))
			{
				ScaledTemp = Scaled;
			}
			target.Scaled = ScaledTemp;
			FixedPoint2 AmountTemp = default(FixedPoint2);
			if (!serialization.TryCustomCopy<FixedPoint2>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = serialization.CreateCopy<FixedPoint2>(Amount, hookCtx, context, false);
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ModifyBloodLevel target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ModifyBloodLevel> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyBloodLevel cast = (ModifyBloodLevel)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ModifyBloodLevel cast = (ModifyBloodLevel)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ModifyBloodLevel Instantiate()
	{
		return new ModifyBloodLevel();
	}
}
