using System;
using Robust.Shared.Audio;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG;

[DataDefinition]
public sealed class ZoneSettings : ISerializationGenerated<ZoneSettings>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public int TotalPhases = 6;

	[DataField(null, false, 1, false, false, null)]
	public float[] PhaseRadiusPercents = new float[7] { 1f, 0.75f, 0.55f, 0.35f, 0.2f, 0.1f, 0.05f };

	[DataField(null, false, 1, false, false, null)]
	public float[] PhaseWaitDurations = new float[6] { 135f, 113f, 90f, 75f, 60f, 45f };

	[DataField(null, false, 1, false, false, null)]
	public float[] PhaseShrinkDurations = new float[6] { 90f, 75f, 60f, 45f, 38f, 30f };

	[DataField(null, false, 1, false, false, null)]
	public float[] PhaseDamage = new float[6] { 2f, 5f, 8f, 12f, 20f, 30f };

	[DataField(null, false, 1, false, false, null)]
	public int RandomizeCenterFromPhase = 1;

	[DataField(null, false, 1, false, false, null)]
	public FirstZoneRevealSettings? FirstZoneReveal;

	[DataField(null, false, 1, false, false, null)]
	public bool UseDynamicSpeed = true;

	[DataField(null, false, 1, false, false, null)]
	public int DynamicTimeMaxPhase = 5;

	[DataField(null, false, 1, false, false, null)]
	public float DynamicTimeMinMultiplier = 0.1f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? WarningSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? ShrinkStartSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public float[] WarningTimes = new float[2] { 30f, 10f };

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref ZoneSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		if (serialization.TryCustomCopy<ZoneSettings>(this, ref target, hookCtx, false, context))
		{
			return;
		}
		int TotalPhasesTemp = 0;
		if (!serialization.TryCustomCopy<int>(TotalPhases, ref TotalPhasesTemp, hookCtx, false, context))
		{
			TotalPhasesTemp = TotalPhases;
		}
		target.TotalPhases = TotalPhasesTemp;
		float[] PhaseRadiusPercentsTemp = null;
		if (PhaseRadiusPercents == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<float[]>(PhaseRadiusPercents, ref PhaseRadiusPercentsTemp, hookCtx, true, context))
		{
			PhaseRadiusPercentsTemp = serialization.CreateCopy<float[]>(PhaseRadiusPercents, hookCtx, context, false);
		}
		target.PhaseRadiusPercents = PhaseRadiusPercentsTemp;
		float[] PhaseWaitDurationsTemp = null;
		if (PhaseWaitDurations == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<float[]>(PhaseWaitDurations, ref PhaseWaitDurationsTemp, hookCtx, true, context))
		{
			PhaseWaitDurationsTemp = serialization.CreateCopy<float[]>(PhaseWaitDurations, hookCtx, context, false);
		}
		target.PhaseWaitDurations = PhaseWaitDurationsTemp;
		float[] PhaseShrinkDurationsTemp = null;
		if (PhaseShrinkDurations == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<float[]>(PhaseShrinkDurations, ref PhaseShrinkDurationsTemp, hookCtx, true, context))
		{
			PhaseShrinkDurationsTemp = serialization.CreateCopy<float[]>(PhaseShrinkDurations, hookCtx, context, false);
		}
		target.PhaseShrinkDurations = PhaseShrinkDurationsTemp;
		float[] PhaseDamageTemp = null;
		if (PhaseDamage == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<float[]>(PhaseDamage, ref PhaseDamageTemp, hookCtx, true, context))
		{
			PhaseDamageTemp = serialization.CreateCopy<float[]>(PhaseDamage, hookCtx, context, false);
		}
		target.PhaseDamage = PhaseDamageTemp;
		int RandomizeCenterFromPhaseTemp = 0;
		if (!serialization.TryCustomCopy<int>(RandomizeCenterFromPhase, ref RandomizeCenterFromPhaseTemp, hookCtx, false, context))
		{
			RandomizeCenterFromPhaseTemp = RandomizeCenterFromPhase;
		}
		target.RandomizeCenterFromPhase = RandomizeCenterFromPhaseTemp;
		FirstZoneRevealSettings FirstZoneRevealTemp = null;
		if (!serialization.TryCustomCopy<FirstZoneRevealSettings>(FirstZoneReveal, ref FirstZoneRevealTemp, hookCtx, false, context))
		{
			if (FirstZoneReveal == null)
			{
				FirstZoneRevealTemp = null;
			}
			else
			{
				serialization.CopyTo<FirstZoneRevealSettings>(FirstZoneReveal, ref FirstZoneRevealTemp, hookCtx, context, false);
			}
		}
		target.FirstZoneReveal = FirstZoneRevealTemp;
		bool UseDynamicSpeedTemp = false;
		if (!serialization.TryCustomCopy<bool>(UseDynamicSpeed, ref UseDynamicSpeedTemp, hookCtx, false, context))
		{
			UseDynamicSpeedTemp = UseDynamicSpeed;
		}
		target.UseDynamicSpeed = UseDynamicSpeedTemp;
		int DynamicTimeMaxPhaseTemp = 0;
		if (!serialization.TryCustomCopy<int>(DynamicTimeMaxPhase, ref DynamicTimeMaxPhaseTemp, hookCtx, false, context))
		{
			DynamicTimeMaxPhaseTemp = DynamicTimeMaxPhase;
		}
		target.DynamicTimeMaxPhase = DynamicTimeMaxPhaseTemp;
		float DynamicTimeMinMultiplierTemp = 0f;
		if (!serialization.TryCustomCopy<float>(DynamicTimeMinMultiplier, ref DynamicTimeMinMultiplierTemp, hookCtx, false, context))
		{
			DynamicTimeMinMultiplierTemp = DynamicTimeMinMultiplier;
		}
		target.DynamicTimeMinMultiplier = DynamicTimeMinMultiplierTemp;
		SoundSpecifier WarningSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(WarningSound, ref WarningSoundTemp, hookCtx, true, context))
		{
			WarningSoundTemp = serialization.CreateCopy<SoundSpecifier>(WarningSound, hookCtx, context, false);
		}
		target.WarningSound = WarningSoundTemp;
		SoundSpecifier ShrinkStartSoundTemp = null;
		if (!serialization.TryCustomCopy<SoundSpecifier>(ShrinkStartSound, ref ShrinkStartSoundTemp, hookCtx, true, context))
		{
			ShrinkStartSoundTemp = serialization.CreateCopy<SoundSpecifier>(ShrinkStartSound, hookCtx, context, false);
		}
		target.ShrinkStartSound = ShrinkStartSoundTemp;
		float[] WarningTimesTemp = null;
		if (WarningTimes == null)
		{
			throw new NullNotAllowedException();
		}
		if (!serialization.TryCustomCopy<float[]>(WarningTimes, ref WarningTimesTemp, hookCtx, true, context))
		{
			WarningTimesTemp = serialization.CreateCopy<float[]>(WarningTimes, hookCtx, context, false);
		}
		target.WarningTimes = WarningTimesTemp;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref ZoneSettings target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		ZoneSettings cast = (ZoneSettings)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public ZoneSettings Instantiate()
	{
		return new ZoneSettings();
	}
}
