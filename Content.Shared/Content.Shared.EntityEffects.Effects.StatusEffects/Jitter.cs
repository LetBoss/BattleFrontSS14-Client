using System;
using Content.Shared.Jittering;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.EntityEffects.Effects.StatusEffects;

public sealed class Jitter : EntityEffect, ISerializationGenerated<Jitter>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public float Amplitude = 10f;

	[DataField(null, false, 1, false, false, null)]
	public float Frequency = 4f;

	[DataField(null, false, 1, false, false, null)]
	public float Time = 2f;

	[DataField(null, false, 1, false, false, null)]
	public bool Refresh = true;

	public override void Effect(EntityEffectBaseArgs args)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float time = Time;
		if (args is EntityEffectReagentArgs reagentArgs)
		{
			time *= reagentArgs.Scale.Float();
		}
		args.EntityManager.EntitySysManager.GetEntitySystem<SharedJitteringSystem>().DoJitter(args.TargetEntity, TimeSpan.FromSeconds(time), Refresh, Amplitude, Frequency);
	}

	protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
	{
		return Loc.GetString("reagent-effect-guidebook-jittering", new(string, object)[1] { ("chance", Probability) });
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref Jitter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		EntityEffect definitionCast = target;
		base.InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (Jitter)definitionCast;
		if (!serialization.TryCustomCopy<Jitter>(this, ref target, hookCtx, false, context))
		{
			float AmplitudeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Amplitude, ref AmplitudeTemp, hookCtx, false, context))
			{
				AmplitudeTemp = Amplitude;
			}
			target.Amplitude = AmplitudeTemp;
			float FrequencyTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Frequency, ref FrequencyTemp, hookCtx, false, context))
			{
				FrequencyTemp = Frequency;
			}
			target.Frequency = FrequencyTemp;
			float TimeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(Time, ref TimeTemp, hookCtx, false, context))
			{
				TimeTemp = Time;
			}
			target.Time = TimeTemp;
			bool RefreshTemp = false;
			if (!serialization.TryCustomCopy<bool>(Refresh, ref RefreshTemp, hookCtx, false, context))
			{
				RefreshTemp = Refresh;
			}
			target.Refresh = RefreshTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref Jitter target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref EntityEffect target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Jitter cast = (Jitter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Jitter cast = (Jitter)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override Jitter Instantiate()
	{
		return new Jitter();
	}
}
