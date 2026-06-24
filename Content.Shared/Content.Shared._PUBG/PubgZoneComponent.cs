using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._PUBG;

[RegisterComponent]
public sealed class PubgZoneComponent : Component, ISerializationGenerated<PubgZoneComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public Vector2 CurrentCenter;

	[DataField(null, false, 1, false, false, null)]
	public float CurrentRadius;

	[DataField(null, false, 1, false, false, null)]
	public float InitialRadius;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 NextCenter;

	[DataField(null, false, 1, false, false, null)]
	public float NextRadius;

	[DataField(null, false, 1, false, false, null)]
	public int CurrentPhase;

	[DataField(null, false, 1, false, false, null)]
	public ZoneState State;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan? StateStartTime;

	[DataField(null, false, 1, false, false, null)]
	public Vector2 ShrinkStartCenter;

	[DataField(null, false, 1, false, false, null)]
	public float ShrinkStartRadius;

	[DataField(null, false, 1, false, false, null)]
	public float WaitDuration = 60f;

	[DataField(null, false, 1, false, false, null)]
	public float ShrinkDuration = 120f;

	[DataField(null, false, 1, false, false, null)]
	public bool Active;

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
	public bool PendingFirstZoneReveal;

	[DataField(null, false, 1, false, false, null)]
	public float FirstZoneRevealDelaySeconds;

	[DataField(null, false, 1, false, false, null)]
	public bool FirstZoneRevealRandomizeCenter = true;

	[DataField(null, false, 1, false, false, null)]
	public bool PendingInitialShrinkToFirstPhase;

	[DataField(null, false, 1, false, false, null)]
	public float DynamicSpeedMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float ZoneTimeModifierPercent;

	[DataField(null, false, 1, false, false, null)]
	public float CurrentStateTimeModifierPercent;

	[DataField(null, false, 1, false, false, null)]
	public bool UseDynamicSpeed = true;

	[DataField(null, false, 1, false, false, null)]
	public int DynamicTimeMaxPhase = 5;

	[DataField(null, false, 1, false, false, null)]
	public float DynamicTimeMinMultiplier = 0.1f;

	[DataField(null, false, 1, false, false, null)]
	public int InitialAlivePlayers;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? WarningSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? ShrinkStartSound = (SoundSpecifier?)new SoundPathSpecifier("/Audio/_PUBG/Effects/zone_warning.ogg", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public float[] WarningTimes = new float[2] { 30f, 10f };

	[DataField(null, false, 1, false, false, null)]
	public HashSet<float> PlayedWarnings = new HashSet<float>();

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref PubgZoneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_0645: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (PubgZoneComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<PubgZoneComponent>(this, ref target, hookCtx, false, context))
		{
			Vector2 CurrentCenterTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(CurrentCenter, ref CurrentCenterTemp, hookCtx, false, context))
			{
				CurrentCenterTemp = serialization.CreateCopy<Vector2>(CurrentCenter, hookCtx, context, false);
			}
			target.CurrentCenter = CurrentCenterTemp;
			float CurrentRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CurrentRadius, ref CurrentRadiusTemp, hookCtx, false, context))
			{
				CurrentRadiusTemp = CurrentRadius;
			}
			target.CurrentRadius = CurrentRadiusTemp;
			float InitialRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(InitialRadius, ref InitialRadiusTemp, hookCtx, false, context))
			{
				InitialRadiusTemp = InitialRadius;
			}
			target.InitialRadius = InitialRadiusTemp;
			Vector2 NextCenterTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(NextCenter, ref NextCenterTemp, hookCtx, false, context))
			{
				NextCenterTemp = serialization.CreateCopy<Vector2>(NextCenter, hookCtx, context, false);
			}
			target.NextCenter = NextCenterTemp;
			float NextRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NextRadius, ref NextRadiusTemp, hookCtx, false, context))
			{
				NextRadiusTemp = NextRadius;
			}
			target.NextRadius = NextRadiusTemp;
			int CurrentPhaseTemp = 0;
			if (!serialization.TryCustomCopy<int>(CurrentPhase, ref CurrentPhaseTemp, hookCtx, false, context))
			{
				CurrentPhaseTemp = CurrentPhase;
			}
			target.CurrentPhase = CurrentPhaseTemp;
			ZoneState StateTemp = ZoneState.Waiting;
			if (!serialization.TryCustomCopy<ZoneState>(State, ref StateTemp, hookCtx, false, context))
			{
				StateTemp = State;
			}
			target.State = StateTemp;
			TimeSpan? StateStartTimeTemp = null;
			if (!serialization.TryCustomCopy<TimeSpan?>(StateStartTime, ref StateStartTimeTemp, hookCtx, false, context))
			{
				StateStartTimeTemp = serialization.CreateCopy<TimeSpan?>(StateStartTime, hookCtx, context, false);
			}
			target.StateStartTime = StateStartTimeTemp;
			Vector2 ShrinkStartCenterTemp = default(Vector2);
			if (!serialization.TryCustomCopy<Vector2>(ShrinkStartCenter, ref ShrinkStartCenterTemp, hookCtx, false, context))
			{
				ShrinkStartCenterTemp = serialization.CreateCopy<Vector2>(ShrinkStartCenter, hookCtx, context, false);
			}
			target.ShrinkStartCenter = ShrinkStartCenterTemp;
			float ShrinkStartRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShrinkStartRadius, ref ShrinkStartRadiusTemp, hookCtx, false, context))
			{
				ShrinkStartRadiusTemp = ShrinkStartRadius;
			}
			target.ShrinkStartRadius = ShrinkStartRadiusTemp;
			float WaitDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WaitDuration, ref WaitDurationTemp, hookCtx, false, context))
			{
				WaitDurationTemp = WaitDuration;
			}
			target.WaitDuration = WaitDurationTemp;
			float ShrinkDurationTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShrinkDuration, ref ShrinkDurationTemp, hookCtx, false, context))
			{
				ShrinkDurationTemp = ShrinkDuration;
			}
			target.ShrinkDuration = ShrinkDurationTemp;
			bool ActiveTemp = false;
			if (!serialization.TryCustomCopy<bool>(Active, ref ActiveTemp, hookCtx, false, context))
			{
				ActiveTemp = Active;
			}
			target.Active = ActiveTemp;
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
			bool PendingFirstZoneRevealTemp = false;
			if (!serialization.TryCustomCopy<bool>(PendingFirstZoneReveal, ref PendingFirstZoneRevealTemp, hookCtx, false, context))
			{
				PendingFirstZoneRevealTemp = PendingFirstZoneReveal;
			}
			target.PendingFirstZoneReveal = PendingFirstZoneRevealTemp;
			float FirstZoneRevealDelaySecondsTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FirstZoneRevealDelaySeconds, ref FirstZoneRevealDelaySecondsTemp, hookCtx, false, context))
			{
				FirstZoneRevealDelaySecondsTemp = FirstZoneRevealDelaySeconds;
			}
			target.FirstZoneRevealDelaySeconds = FirstZoneRevealDelaySecondsTemp;
			bool FirstZoneRevealRandomizeCenterTemp = false;
			if (!serialization.TryCustomCopy<bool>(FirstZoneRevealRandomizeCenter, ref FirstZoneRevealRandomizeCenterTemp, hookCtx, false, context))
			{
				FirstZoneRevealRandomizeCenterTemp = FirstZoneRevealRandomizeCenter;
			}
			target.FirstZoneRevealRandomizeCenter = FirstZoneRevealRandomizeCenterTemp;
			bool PendingInitialShrinkToFirstPhaseTemp = false;
			if (!serialization.TryCustomCopy<bool>(PendingInitialShrinkToFirstPhase, ref PendingInitialShrinkToFirstPhaseTemp, hookCtx, false, context))
			{
				PendingInitialShrinkToFirstPhaseTemp = PendingInitialShrinkToFirstPhase;
			}
			target.PendingInitialShrinkToFirstPhase = PendingInitialShrinkToFirstPhaseTemp;
			float DynamicSpeedMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(DynamicSpeedMultiplier, ref DynamicSpeedMultiplierTemp, hookCtx, false, context))
			{
				DynamicSpeedMultiplierTemp = DynamicSpeedMultiplier;
			}
			target.DynamicSpeedMultiplier = DynamicSpeedMultiplierTemp;
			float ZoneTimeModifierPercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ZoneTimeModifierPercent, ref ZoneTimeModifierPercentTemp, hookCtx, false, context))
			{
				ZoneTimeModifierPercentTemp = ZoneTimeModifierPercent;
			}
			target.ZoneTimeModifierPercent = ZoneTimeModifierPercentTemp;
			float CurrentStateTimeModifierPercentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CurrentStateTimeModifierPercent, ref CurrentStateTimeModifierPercentTemp, hookCtx, false, context))
			{
				CurrentStateTimeModifierPercentTemp = CurrentStateTimeModifierPercent;
			}
			target.CurrentStateTimeModifierPercent = CurrentStateTimeModifierPercentTemp;
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
			int InitialAlivePlayersTemp = 0;
			if (!serialization.TryCustomCopy<int>(InitialAlivePlayers, ref InitialAlivePlayersTemp, hookCtx, false, context))
			{
				InitialAlivePlayersTemp = InitialAlivePlayers;
			}
			target.InitialAlivePlayers = InitialAlivePlayersTemp;
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
			HashSet<float> PlayedWarningsTemp = null;
			if (PlayedWarnings == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<HashSet<float>>(PlayedWarnings, ref PlayedWarningsTemp, hookCtx, true, context))
			{
				PlayedWarningsTemp = serialization.CreateCopy<HashSet<float>>(PlayedWarnings, hookCtx, context, false);
			}
			target.PlayedWarnings = PlayedWarningsTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref PubgZoneComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgZoneComponent cast = (PubgZoneComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgZoneComponent cast = (PubgZoneComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		PubgZoneComponent def = (PubgZoneComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override PubgZoneComponent Instantiate()
	{
		return new PubgZoneComponent();
	}
}
