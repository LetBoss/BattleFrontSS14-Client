using System;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Atmos.Prototypes;
using Content.Shared.Database;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class CreateGas : EventEntityEffect<CreateGas>, ISerializationGenerated<CreateGas>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public Gas Gas;

	[DataField(null, false, 1, false, false, null)]
	public float Multiplier = 3f;

	public override bool ShouldLog => true;

	public override LogImpact LogImpact => LogImpact.High;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		GasPrototype gasProto = entSys.GetEntitySystem<SharedAtmosphereSystem>().GetGas(Gas);
		return Loc.GetString("reagent-effect-guidebook-create-gas", new(string, object)[3]
		{
			("chance", Probability),
			("moles", Multiplier),
			("gas", gasProto.Name)
		});
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CreateGas target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EventEntityEffect<CreateGas> definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CreateGas)definitionCast;
		if (!serialization.TryCustomCopy<CreateGas>(this, ref target, hookCtx, false, context))
		{
			Gas GasTemp = Gas.Oxygen;
			if (!serialization.TryCustomCopy<Gas>(Gas, ref GasTemp, hookCtx, false, context))
			{
				GasTemp = Gas;
			}
			target.Gas = GasTemp;
			float MultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Multiplier, ref MultiplierTemp, hookCtx, false, context))
			{
				MultiplierTemp = Multiplier;
			}
			target.Multiplier = MultiplierTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CreateGas target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EventEntityEffect<CreateGas> target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreateGas cast = (CreateGas)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CreateGas cast = (CreateGas)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CreateGas Instantiate()
	{
		return new CreateGas();
	}
}
