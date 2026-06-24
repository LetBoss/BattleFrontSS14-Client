using System;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Eye.Blinding.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class ChemHealEyeDamage : EntityEffect, ISerializationGenerated<ChemHealEyeDamage>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int Amount = -1;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-cure-eye-damage", new(string, object)[2]
		{
			("chance", Probability),
			("deltasign", MathF.Sign(Amount))
		});
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!(args is EntityEffectReagentArgs reagentArgs) || !(reagentArgs.Scale != 1f))
		{
			args.EntityManager.EntitySysManager.GetEntitySystem<BlindableSystem>().AdjustEyeDamage(Entity<BlindableComponent>.op_Implicit(args.TargetEntity), Amount);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ChemHealEyeDamage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (ChemHealEyeDamage)definitionCast;
		if (!serialization.TryCustomCopy<ChemHealEyeDamage>(this, ref target, hookCtx, false, context))
		{
			int AmountTemp = 0;
			if (!serialization.TryCustomCopy<int>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ChemHealEyeDamage target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemHealEyeDamage cast = (ChemHealEyeDamage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ChemHealEyeDamage cast = (ChemHealEyeDamage)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override ChemHealEyeDamage Instantiate()
	{
		return new ChemHealEyeDamage();
	}
}
