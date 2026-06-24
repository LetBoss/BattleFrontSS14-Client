using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._CIV14merka.Weapons;

[RegisterComponent]
[Access(new Type[] { typeof(SharedCivSuppressionSystem) })]
public sealed class CivSuppressionEmitterComponent : Component, ISerializationGenerated<CivSuppressionEmitterComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public CivSuppressionVisualProfile VisualProfile;

	[DataField(null, false, 1, false, false, null)]
	public float NearMissRadius = 1.75f;

	[DataField(null, false, 1, false, false, null)]
	public float NearMissAmount = 0.14f;

	[DataField(null, false, 1, false, false, null)]
	public float HitAmount = 0.32f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan DecayTime = TimeSpan.FromSeconds(2.799999952316284);

	[DataField(null, false, 1, false, false, null)]
	public float MaxIntensity = 1f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan NearMissCooldown = TimeSpan.FromSeconds(0.18000000715255737);

	[DataField(null, false, 1, false, false, null)]
	public float ShotPenaltyDegrees = 4f;

	[DataField(null, false, 1, false, false, null)]
	public float FreshTargetSuppressionMultiplier = 0.6f;

	[DataField(null, false, 1, false, false, null)]
	public float HighStressThreshold = 0.65f;

	[DataField(null, false, 1, false, false, null)]
	public float HighStressShotPenaltyMultiplier = 1.75f;

	[DataField(null, false, 1, false, false, null)]
	public float NearMissMinimumFactor = 0.2f;

	[DataField(null, false, 1, false, false, null)]
	public float NearMissExponent = 1.6f;

	[DataField(null, false, 1, false, false, null)]
	public float OccludedNearMissMultiplier = 0.35f;

	[DataField(null, false, 1, false, false, null)]
	public float WhizzDistance = 2.25f;

	[DataField(null, false, 1, false, false, null)]
	public float WhizzAudioRange = 24f;

	[DataField(null, false, 1, false, false, null)]
	public float WhizzMinVolume = -6f;

	[DataField(null, false, 1, false, false, null)]
	public float WhizzMaxVolume = 3f;

	[DataField(null, false, 1, false, false, null)]
	public float CrackThreshold = 0.78f;

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier WhizzSound = (SoundSpecifier)new SoundCollectionSpecifier("CivSuppressionWhizz", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier CrackSound = (SoundSpecifier)new SoundCollectionSpecifier("CivSuppressionCrack", (AudioParams?)null);

	[DataField(null, false, 1, false, false, null)]
	public float VisualShockMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public float VisualSwayMultiplier = 1f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan VisualRecoveryDelay = TimeSpan.FromSeconds(0.44999998807907104);

	[DataField(null, false, 1, false, false, null)]
	public float VisualRingThreshold = 0.72f;

	[DataField(null, false, 1, false, false, null)]
	public float VisualRingVolume = -11f;

	[DataField(null, false, 1, false, false, null)]
	public TimeSpan VisualRingCooldown = TimeSpan.FromSeconds(1.350000023841858);

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref CivSuppressionEmitterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (CivSuppressionEmitterComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<CivSuppressionEmitterComponent>(this, ref target, hookCtx, false, context))
		{
			CivSuppressionVisualProfile VisualProfileTemp = CivSuppressionVisualProfile.IncomingFire;
			if (!serialization.TryCustomCopy<CivSuppressionVisualProfile>(VisualProfile, ref VisualProfileTemp, hookCtx, false, context))
			{
				VisualProfileTemp = VisualProfile;
			}
			target.VisualProfile = VisualProfileTemp;
			float NearMissRadiusTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearMissRadius, ref NearMissRadiusTemp, hookCtx, false, context))
			{
				NearMissRadiusTemp = NearMissRadius;
			}
			target.NearMissRadius = NearMissRadiusTemp;
			float NearMissAmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearMissAmount, ref NearMissAmountTemp, hookCtx, false, context))
			{
				NearMissAmountTemp = NearMissAmount;
			}
			target.NearMissAmount = NearMissAmountTemp;
			float HitAmountTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HitAmount, ref HitAmountTemp, hookCtx, false, context))
			{
				HitAmountTemp = HitAmount;
			}
			target.HitAmount = HitAmountTemp;
			TimeSpan DecayTimeTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(DecayTime, ref DecayTimeTemp, hookCtx, false, context))
			{
				DecayTimeTemp = serialization.CreateCopy<TimeSpan>(DecayTime, hookCtx, context, false);
			}
			target.DecayTime = DecayTimeTemp;
			float MaxIntensityTemp = 0f;
			if (!serialization.TryCustomCopy<float>(MaxIntensity, ref MaxIntensityTemp, hookCtx, false, context))
			{
				MaxIntensityTemp = MaxIntensity;
			}
			target.MaxIntensity = MaxIntensityTemp;
			TimeSpan NearMissCooldownTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(NearMissCooldown, ref NearMissCooldownTemp, hookCtx, false, context))
			{
				NearMissCooldownTemp = serialization.CreateCopy<TimeSpan>(NearMissCooldown, hookCtx, context, false);
			}
			target.NearMissCooldown = NearMissCooldownTemp;
			float ShotPenaltyDegreesTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ShotPenaltyDegrees, ref ShotPenaltyDegreesTemp, hookCtx, false, context))
			{
				ShotPenaltyDegreesTemp = ShotPenaltyDegrees;
			}
			target.ShotPenaltyDegrees = ShotPenaltyDegreesTemp;
			float FreshTargetSuppressionMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(FreshTargetSuppressionMultiplier, ref FreshTargetSuppressionMultiplierTemp, hookCtx, false, context))
			{
				FreshTargetSuppressionMultiplierTemp = FreshTargetSuppressionMultiplier;
			}
			target.FreshTargetSuppressionMultiplier = FreshTargetSuppressionMultiplierTemp;
			float HighStressThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HighStressThreshold, ref HighStressThresholdTemp, hookCtx, false, context))
			{
				HighStressThresholdTemp = HighStressThreshold;
			}
			target.HighStressThreshold = HighStressThresholdTemp;
			float HighStressShotPenaltyMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(HighStressShotPenaltyMultiplier, ref HighStressShotPenaltyMultiplierTemp, hookCtx, false, context))
			{
				HighStressShotPenaltyMultiplierTemp = HighStressShotPenaltyMultiplier;
			}
			target.HighStressShotPenaltyMultiplier = HighStressShotPenaltyMultiplierTemp;
			float NearMissMinimumFactorTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearMissMinimumFactor, ref NearMissMinimumFactorTemp, hookCtx, false, context))
			{
				NearMissMinimumFactorTemp = NearMissMinimumFactor;
			}
			target.NearMissMinimumFactor = NearMissMinimumFactorTemp;
			float NearMissExponentTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearMissExponent, ref NearMissExponentTemp, hookCtx, false, context))
			{
				NearMissExponentTemp = NearMissExponent;
			}
			target.NearMissExponent = NearMissExponentTemp;
			float OccludedNearMissMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(OccludedNearMissMultiplier, ref OccludedNearMissMultiplierTemp, hookCtx, false, context))
			{
				OccludedNearMissMultiplierTemp = OccludedNearMissMultiplier;
			}
			target.OccludedNearMissMultiplier = OccludedNearMissMultiplierTemp;
			float WhizzDistanceTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WhizzDistance, ref WhizzDistanceTemp, hookCtx, false, context))
			{
				WhizzDistanceTemp = WhizzDistance;
			}
			target.WhizzDistance = WhizzDistanceTemp;
			float WhizzAudioRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WhizzAudioRange, ref WhizzAudioRangeTemp, hookCtx, false, context))
			{
				WhizzAudioRangeTemp = WhizzAudioRange;
			}
			target.WhizzAudioRange = WhizzAudioRangeTemp;
			float WhizzMinVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WhizzMinVolume, ref WhizzMinVolumeTemp, hookCtx, false, context))
			{
				WhizzMinVolumeTemp = WhizzMinVolume;
			}
			target.WhizzMinVolume = WhizzMinVolumeTemp;
			float WhizzMaxVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(WhizzMaxVolume, ref WhizzMaxVolumeTemp, hookCtx, false, context))
			{
				WhizzMaxVolumeTemp = WhizzMaxVolume;
			}
			target.WhizzMaxVolume = WhizzMaxVolumeTemp;
			float CrackThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(CrackThreshold, ref CrackThresholdTemp, hookCtx, false, context))
			{
				CrackThresholdTemp = CrackThreshold;
			}
			target.CrackThreshold = CrackThresholdTemp;
			SoundSpecifier WhizzSoundTemp = null;
			if (WhizzSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(WhizzSound, ref WhizzSoundTemp, hookCtx, true, context))
			{
				WhizzSoundTemp = serialization.CreateCopy<SoundSpecifier>(WhizzSound, hookCtx, context, false);
			}
			target.WhizzSound = WhizzSoundTemp;
			SoundSpecifier CrackSoundTemp = null;
			if (CrackSound == null)
			{
				throw new NullNotAllowedException();
			}
			if (!serialization.TryCustomCopy<SoundSpecifier>(CrackSound, ref CrackSoundTemp, hookCtx, true, context))
			{
				CrackSoundTemp = serialization.CreateCopy<SoundSpecifier>(CrackSound, hookCtx, context, false);
			}
			target.CrackSound = CrackSoundTemp;
			float VisualShockMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VisualShockMultiplier, ref VisualShockMultiplierTemp, hookCtx, false, context))
			{
				VisualShockMultiplierTemp = VisualShockMultiplier;
			}
			target.VisualShockMultiplier = VisualShockMultiplierTemp;
			float VisualSwayMultiplierTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VisualSwayMultiplier, ref VisualSwayMultiplierTemp, hookCtx, false, context))
			{
				VisualSwayMultiplierTemp = VisualSwayMultiplier;
			}
			target.VisualSwayMultiplier = VisualSwayMultiplierTemp;
			TimeSpan VisualRecoveryDelayTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(VisualRecoveryDelay, ref VisualRecoveryDelayTemp, hookCtx, false, context))
			{
				VisualRecoveryDelayTemp = serialization.CreateCopy<TimeSpan>(VisualRecoveryDelay, hookCtx, context, false);
			}
			target.VisualRecoveryDelay = VisualRecoveryDelayTemp;
			float VisualRingThresholdTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VisualRingThreshold, ref VisualRingThresholdTemp, hookCtx, false, context))
			{
				VisualRingThresholdTemp = VisualRingThreshold;
			}
			target.VisualRingThreshold = VisualRingThresholdTemp;
			float VisualRingVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(VisualRingVolume, ref VisualRingVolumeTemp, hookCtx, false, context))
			{
				VisualRingVolumeTemp = VisualRingVolume;
			}
			target.VisualRingVolume = VisualRingVolumeTemp;
			TimeSpan VisualRingCooldownTemp = default(TimeSpan);
			if (!serialization.TryCustomCopy<TimeSpan>(VisualRingCooldown, ref VisualRingCooldownTemp, hookCtx, false, context))
			{
				VisualRingCooldownTemp = serialization.CreateCopy<TimeSpan>(VisualRingCooldown, hookCtx, context, false);
			}
			target.VisualRingCooldown = VisualRingCooldownTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref CivSuppressionEmitterComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSuppressionEmitterComponent cast = (CivSuppressionEmitterComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSuppressionEmitterComponent cast = (CivSuppressionEmitterComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		CivSuppressionEmitterComponent def = (CivSuppressionEmitterComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override CivSuppressionEmitterComponent Instantiate()
	{
		return new CivSuppressionEmitterComponent();
	}
}
