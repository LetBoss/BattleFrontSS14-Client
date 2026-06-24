using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ChemVomit : EventEntityEffect<ChemVomit>, ISerializationGenerated<ChemVomit>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float ThirstAmount = -8f;

	[DataField(null, false, 1, false, false, null)]
	public float HungerAmount = -8f;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-chem-vomit", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChemVomit target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<ChemVomit> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChemVomit)definitionCast;
		if (!serialization.TryCustomCopy<ChemVomit>(this, ref target, hookCtx, false, context))
		{
			float ThirstAmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ThirstAmount, ref ThirstAmountTemp, hookCtx, false, context))
			{
				ThirstAmountTemp = ThirstAmount;
			}
			target.ThirstAmount = ThirstAmountTemp;
			float HungerAmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HungerAmount, ref HungerAmountTemp, hookCtx, false, context))
			{
				HungerAmountTemp = HungerAmount;
			}
			target.HungerAmount = HungerAmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChemVomit target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<ChemVomit> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemVomit cast = (ChemVomit)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemVomit cast = (ChemVomit)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChemVomit Instantiate()
	{
		return new ChemVomit();
	}
}
