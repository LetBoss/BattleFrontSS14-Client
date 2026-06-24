using System;
using System.Collections.Generic;
using Content.Shared.Weather;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._RMC14.Weather;

[Serializable]
[DataDefinition]
[NetSerializable]
public sealed class RMCWeatherEvent : ISerializationGenerated<RMCWeatherEvent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public string Name = "rmcWeatherEvent";

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan Duration;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DurationRemaining;

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<WeatherPrototype> WeatherType;

	[DataField(null, false, 1, false, false, null)]
	public float LightningChance;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LightningDuration = TimeSpan.FromSeconds(2L);

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LightningCooldown;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan LightningCooldownDuration = TimeSpan.FromSeconds(5L);

	[DataField(null, false, 1, false, false, null)]
	public List<string> LightningEffects = new List<string> { "RMCColorSequenceLightningSharpPeak", "RMCColorSequenceLightningFlicker" };

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier LightningSound = (SoundSpecifier)new SoundCollectionSpecifier("RMCThunder", (AudioParams?)null);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref RMCWeatherEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		if (!serialization.TryCustomCopy<RMCWeatherEvent>(this, ref target, hookCtx, false, context))
		{
			string NameTemp = null;
			if (Name == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<string>(Name, ref NameTemp, hookCtx, false, context))
			{
				NameTemp = Name;
			}
			target.Name = NameTemp;
			TimeSpan DurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(Duration, ref DurationTemp, hookCtx, false, context))
			{
				DurationTemp = serialization.CreateCopy<TimeSpan>(Duration, hookCtx, context, false);
			}
			target.Duration = DurationTemp;
			TimeSpan DurationRemainingTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DurationRemaining, ref DurationRemainingTemp, hookCtx, false, context))
			{
				DurationRemainingTemp = serialization.CreateCopy<TimeSpan>(DurationRemaining, hookCtx, context, false);
			}
			target.DurationRemaining = DurationRemainingTemp;
			ProtoId<WeatherPrototype> WeatherTypeTemp = default(ProtoId<WeatherPrototype>);
			if (!serialization.TryCustomCopy<ProtoId<WeatherPrototype>>(WeatherType, ref WeatherTypeTemp, hookCtx, false, context))
			{
				WeatherTypeTemp = serialization.CreateCopy<ProtoId<WeatherPrototype>>(WeatherType, hookCtx, context, false);
			}
			target.WeatherType = WeatherTypeTemp;
			float LightningChanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(LightningChance, ref LightningChanceTemp, hookCtx, false, context))
			{
				LightningChanceTemp = LightningChance;
			}
			target.LightningChance = LightningChanceTemp;
			TimeSpan LightningDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LightningDuration, ref LightningDurationTemp, hookCtx, false, context))
			{
				LightningDurationTemp = serialization.CreateCopy<TimeSpan>(LightningDuration, hookCtx, context, false);
			}
			target.LightningDuration = LightningDurationTemp;
			TimeSpan LightningCooldownTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LightningCooldown, ref LightningCooldownTemp, hookCtx, false, context))
			{
				LightningCooldownTemp = serialization.CreateCopy<TimeSpan>(LightningCooldown, hookCtx, context, false);
			}
			target.LightningCooldown = LightningCooldownTemp;
			TimeSpan LightningCooldownDurationTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(LightningCooldownDuration, ref LightningCooldownDurationTemp, hookCtx, false, context))
			{
				LightningCooldownDurationTemp = serialization.CreateCopy<TimeSpan>(LightningCooldownDuration, hookCtx, context, false);
			}
			target.LightningCooldownDuration = LightningCooldownDurationTemp;
			List<string> LightningEffectsTemp = null;
			if (LightningEffects == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<List<string>>(LightningEffects, ref LightningEffectsTemp, hookCtx, true, context))
			{
				LightningEffectsTemp = serialization.CreateCopy<List<string>>(LightningEffects, hookCtx, context, false);
			}
			target.LightningEffects = LightningEffectsTemp;
			SoundSpecifier LightningSoundTemp = null;
			if (LightningSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(LightningSound, ref LightningSoundTemp, hookCtx, true, context))
			{
				LightningSoundTemp = serialization.CreateCopy<SoundSpecifier>(LightningSound, hookCtx, context, false);
			}
			target.LightningSound = LightningSoundTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref RMCWeatherEvent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		RMCWeatherEvent cast = (RMCWeatherEvent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public RMCWeatherEvent Instantiate()
	{
		return new RMCWeatherEvent();
	}
}
