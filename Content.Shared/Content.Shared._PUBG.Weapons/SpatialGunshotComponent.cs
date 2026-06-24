using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._PUBG.Weapons;

[RegisterComponent]
[NetworkedComponent]
public sealed class SpatialGunshotComponent : Component, ISerializationGenerated<SpatialGunshotComponent>, ISerializationGenerated
{
	[DataField(null, false, 1, false, false, null)]
	public SoundSpecifier? FarSound;

	[DataField(null, false, 1, false, false, null)]
	public float AudioRange = 50f;

	[DataField(null, false, 1, false, false, null)]
	public float NearAudioRange = 45f;

	[DataField(null, false, 1, false, false, null)]
	public float ConeAngle = 90f;

	[DataField(null, false, 1, false, false, null)]
	public float NearRange = 20f;

	[DataField(null, false, 1, false, false, null)]
	public float NearVolume = 6f;

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref SpatialGunshotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		Component definitionCast = (Component)(object)target;
		((Component)this).InternalCopy(ref definitionCast, serialization, hookCtx, context);
		target = (SpatialGunshotComponent)(object)definitionCast;
		if (!serialization.TryCustomCopy<SpatialGunshotComponent>(this, ref target, hookCtx, false, context))
		{
			SoundSpecifier FarSoundTemp = null;
			if (!serialization.TryCustomCopy<SoundSpecifier>(FarSound, ref FarSoundTemp, hookCtx, true, context))
			{
				FarSoundTemp = serialization.CreateCopy<SoundSpecifier>(FarSound, hookCtx, context, false);
			}
			target.FarSound = FarSoundTemp;
			float AudioRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(AudioRange, ref AudioRangeTemp, hookCtx, false, context))
			{
				AudioRangeTemp = AudioRange;
			}
			target.AudioRange = AudioRangeTemp;
			float NearAudioRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearAudioRange, ref NearAudioRangeTemp, hookCtx, false, context))
			{
				NearAudioRangeTemp = NearAudioRange;
			}
			target.NearAudioRange = NearAudioRangeTemp;
			float ConeAngleTemp = 0f;
			if (!serialization.TryCustomCopy<float>(ConeAngle, ref ConeAngleTemp, hookCtx, false, context))
			{
				ConeAngleTemp = ConeAngle;
			}
			target.ConeAngle = ConeAngleTemp;
			float NearRangeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearRange, ref NearRangeTemp, hookCtx, false, context))
			{
				NearRangeTemp = NearRange;
			}
			target.NearRange = NearRangeTemp;
			float NearVolumeTemp = 0f;
			if (!serialization.TryCustomCopy<float>(NearVolume, ref NearVolumeTemp, hookCtx, false, context))
			{
				NearVolumeTemp = NearVolume;
			}
			target.NearVolume = NearVolumeTemp;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref SpatialGunshotComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref Component target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpatialGunshotComponent cast = (SpatialGunshotComponent)(object)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = (Component)(object)cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpatialGunshotComponent cast = (SpatialGunshotComponent)target;
		Copy(ref cast, serialization, hookCtx, context);
		target = cast;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SpatialGunshotComponent def = (SpatialGunshotComponent)(object)target;
		Copy(ref def, serialization, hookCtx, context);
		target = (IComponent)(object)def;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override SpatialGunshotComponent Instantiate()
	{
		return new SpatialGunshotComponent();
	}
}
