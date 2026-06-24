using System;
using Content.Shared._RMC14.Temperature;
using Content.Shared.EntityEffects;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Chemistry.Effects;

public sealed class StabilizeTemperature : EntityEffect, ISerializationGenerated<StabilizeTemperature>, ISerializationGenerated
{
	[DataField(null, false, 1, true, false, null)]
	public float Stable;

	[DataField(null, false, 1, true, false, null)]
	public float Change;

	protected override string ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return $"Stabilizes the temperature of the body that it is in to {Stable} degrees, by {Change} degrees at a time";
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		SharedRMCTemperatureSystem sys = args.EntityManager.EntitySysManager.GetEntitySystem<SharedRMCTemperatureSystem>();
		float current = sys.GetTemperature(args.TargetEntity);
		if (!((double)Math.Abs(current - Stable) < 0.01))
		{
			float change = Change;
			if (args is EntityEffectReagentArgs reagentArgs)
			{
				change *= reagentArgs.Scale.Float();
			}
			float temp = ((current > Stable) ? Math.Max(Stable, current - change) : Math.Min(Stable, current + change));
			sys.ForceChangeTemperature(args.TargetEntity, temp);
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref StabilizeTemperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (StabilizeTemperature)definitionCast;
		if (!serialization.TryCustomCopy<StabilizeTemperature>(this, ref target, hookCtx, false, context))
		{
			float StableTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Stable, ref StableTemp, hookCtx, false, context))
			{
				StableTemp = Stable;
			}
			target.Stable = StableTemp;
			float ChangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Change, ref ChangeTemp, hookCtx, false, context))
			{
				ChangeTemp = Change;
			}
			target.Change = ChangeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref StabilizeTemperature target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StabilizeTemperature cast = (StabilizeTemperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		StabilizeTemperature cast = (StabilizeTemperature)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override StabilizeTemperature Instantiate()
	{
		return new StabilizeTemperature();
	}
}
