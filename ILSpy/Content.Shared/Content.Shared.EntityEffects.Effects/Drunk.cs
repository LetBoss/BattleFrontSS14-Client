using System;
using Content.Shared.Drunk;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects;

public sealed class Drunk : EntityEffect, ISerializationGenerated<Drunk>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float BoozePower = 3f;

	[DataField(null, false, 1, false, false, null)]
	public bool SlurSpeech = true;

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-drunk", new(string, object)[1] { ("chance", Probability) });
	}

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float boozePower = BoozePower;
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			boozePower *= reagentArgs.Scale.Float();
		}
		args.EntityManager.EntitySysManager.GetEntitySystem<SharedDrunkSystem>().TryApplyDrunkenness(args.TargetEntity, boozePower, SlurSpeech);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Drunk target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Drunk)definitionCast;
		if (!serialization.TryCustomCopy<Drunk>(this, ref target, hookCtx, false, context))
		{
			float BoozePowerTemp = 0f;
			if (!serialization.TryCustomCopy<float>(BoozePower, ref BoozePowerTemp, hookCtx, false, context))
			{
				BoozePowerTemp = BoozePower;
			}
			target.BoozePower = BoozePowerTemp;
			bool SlurSpeechTemp = false;
			if (!serialization.TryCustomCopy<bool>(SlurSpeech, ref SlurSpeechTemp, hookCtx, false, context))
			{
				SlurSpeechTemp = SlurSpeech;
			}
			target.SlurSpeech = SlurSpeechTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Drunk target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Drunk cast = (Drunk)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Drunk cast = (Drunk)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Drunk Instantiate()
	{
		return new Drunk();
	}
}
