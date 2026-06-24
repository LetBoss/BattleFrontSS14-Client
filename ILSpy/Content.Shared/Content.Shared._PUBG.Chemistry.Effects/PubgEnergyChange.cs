using System;
using System.Text.Json.Serialization;
using Content.Shared._PUBG.Medicine;
using Content.Shared.EntityEffects;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Chemistry.Effects;

public sealed class PubgEnergyChange : EntityEffect, ISerializationGenerated<PubgEnergyChange>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	[JsonPropertyName("amount")]
	public float Amount;

	[DataField(null, false, 1, false, false, null)]
	[JsonPropertyName("scaleByQuantity")]
	public bool ScaleByQuantity = true;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("pubg-reagent-effect-energy", new(string, object)[1] { ("amount", Amount) });
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		PubgEnergyComponent energy = default(PubgEnergyComponent);
		if (!(args is EntityEffectReagentArgs reagentArgs) || !reagentArgs.EntityManager.TryGetComponent<PubgEnergyComponent>(reagentArgs.TargetEntity, ref energy))
		{
			return;
		}
		float amount = Amount;
		if (ScaleByQuantity)
		{
			amount *= reagentArgs.Quantity.Float();
		}
		if (!(amount <= 0f))
		{
			float newEnergy = MathF.Min(energy.MaxEnergy, energy.Energy + amount);
			if (!(MathF.Abs(newEnergy - energy.Energy) <= 0.001f))
			{
				energy.Energy = newEnergy;
				reagentArgs.EntityManager.Dirty(reagentArgs.TargetEntity, (IComponent)(object)energy, (MetaDataComponent)null);
				reagentArgs.EntityManager.System<MovementSpeedModifierSystem>().RefreshMovementSpeedModifiers(reagentArgs.TargetEntity);
			}
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgEnergyChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgEnergyChange)definitionCast;
		if (!serialization.TryCustomCopy<PubgEnergyChange>(this, ref target, hookCtx, false, context))
		{
			float AmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Amount, ref AmountTemp, hookCtx, false, context))
			{
				AmountTemp = Amount;
			}
			target.Amount = AmountTemp;
			bool ScaleByQuantityTemp = false;
			if (!serialization.TryCustomCopy<bool>(ScaleByQuantity, ref ScaleByQuantityTemp, hookCtx, false, context))
			{
				ScaleByQuantityTemp = ScaleByQuantity;
			}
			target.ScaleByQuantity = ScaleByQuantityTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgEnergyChange target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEnergyChange cast = (PubgEnergyChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgEnergyChange cast = (PubgEnergyChange)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgEnergyChange Instantiate()
	{
		return new PubgEnergyChange();
	}
}
