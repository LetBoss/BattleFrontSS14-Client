using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Audio;

[Serializable]
[NetSerializable]
[DataDefinition]
public struct AudioParams : ISerializationGenerated<AudioParams>, ISerializationGenerated
{
	private float _volume = Default.Volume;

	private float _pitch = Default.Pitch;

	public static readonly AudioParams Default = new AudioParams(0f, 1f, 15f, 1f, 1f, loop: false, 0f);

	[DataField(null, false, 1, false, false, null)]
	public float Volume
	{
		get
		{
			return _volume;
		}
		set
		{
			if (float.IsNaN(value))
			{
				value = float.NegativeInfinity;
			}
			_volume = value;
		}
	}

	[DataField(null, false, 1, false, false, null)]
	public float Pitch
	{
		get
		{
			return _pitch;
		}
		set
		{
			_pitch = MathF.Max(0f, value);
		}
	}

	[DataField(null, false, 1, false, false, null)]
	public float MaxDistance { get; set; } = Default.MaxDistance;

	[DataField(null, false, 1, false, false, null)]
	public float RolloffFactor { get; set; } = Default.RolloffFactor;

	[DataField(null, false, 1, false, false, null)]
	public float ReferenceDistance { get; set; } = Default.ReferenceDistance;

	[DataField(null, false, 1, false, false, null)]
	public bool Loop { get; set; } = Default.Loop;

	[DataField(null, false, 1, false, false, null)]
	public float PlayOffsetSeconds { get; set; } = Default.PlayOffsetSeconds;

	[DataField(null, false, 1, false, false, null)]
	public float? Variation { get; set; } = null;

	public AudioParams()
	{
	}

	public AudioParams(float volume, float pitch, float maxDistance, float refDistance, bool loop, float playOffsetSeconds, float? variation = null)
		: this(volume, pitch, maxDistance, 1f, refDistance, loop, playOffsetSeconds, variation)
	{
	}

	public AudioParams(float volume, float pitch, float maxDistance, float rolloffFactor, float refDistance, bool loop, float playOffsetSeconds, float? variation = null)
		: this()
	{
		Volume = volume;
		Pitch = pitch;
		MaxDistance = maxDistance;
		RolloffFactor = rolloffFactor;
		ReferenceDistance = refDistance;
		Loop = loop;
		PlayOffsetSeconds = playOffsetSeconds;
		Variation = variation;
	}

	public readonly AudioParams WithVolume(float volume)
	{
		AudioParams result = this;
		result.Volume = volume;
		return result;
	}

	public readonly AudioParams AddVolume(float volume)
	{
		AudioParams result = this;
		result.Volume += volume;
		return result;
	}

	public readonly AudioParams WithVariation(float? variation)
	{
		AudioParams result = this;
		result.Variation = variation;
		return result;
	}

	public readonly AudioParams WithPitchScale(float pitch)
	{
		AudioParams result = this;
		result.Pitch = pitch;
		return result;
	}

	public readonly AudioParams WithMaxDistance(float dist)
	{
		AudioParams result = this;
		result.MaxDistance = dist;
		return result;
	}

	public readonly AudioParams WithRolloffFactor(float rolloffFactor)
	{
		AudioParams result = this;
		result.RolloffFactor = rolloffFactor;
		return result;
	}

	public readonly AudioParams WithReferenceDistance(float refDistance)
	{
		AudioParams result = this;
		result.ReferenceDistance = refDistance;
		return result;
	}

	public readonly AudioParams WithLoop(bool loop)
	{
		AudioParams result = this;
		result.Loop = loop;
		return result;
	}

	public readonly AudioParams WithPlayOffset(float offset)
	{
		AudioParams result = this;
		result.PlayOffsetSeconds = offset;
		return result;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref AudioParams target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			float target2 = 0f;
			if (!serialization.TryCustomCopy(Volume, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = Volume;
			}
			float target3 = 0f;
			if (!serialization.TryCustomCopy(Pitch, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Pitch;
			}
			float target4 = 0f;
			if (!serialization.TryCustomCopy(MaxDistance, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = MaxDistance;
			}
			float target5 = 0f;
			if (!serialization.TryCustomCopy(RolloffFactor, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = RolloffFactor;
			}
			float target6 = 0f;
			if (!serialization.TryCustomCopy(ReferenceDistance, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = ReferenceDistance;
			}
			bool target7 = false;
			if (!serialization.TryCustomCopy(Loop, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = Loop;
			}
			float target8 = 0f;
			if (!serialization.TryCustomCopy(PlayOffsetSeconds, ref target8, hookCtx, hasHooks: false, context))
			{
				target8 = PlayOffsetSeconds;
			}
			float? target9 = null;
			if (!serialization.TryCustomCopy(Variation, ref target9, hookCtx, hasHooks: false, context))
			{
				target9 = Variation;
			}
			AudioParams audioParams = target;
			audioParams.Volume = target2;
			audioParams.Pitch = target3;
			audioParams.MaxDistance = target4;
			audioParams.RolloffFactor = target5;
			audioParams.ReferenceDistance = target6;
			audioParams.Loop = target7;
			audioParams.PlayOffsetSeconds = target8;
			audioParams.Variation = target9;
			target = audioParams;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref AudioParams target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		AudioParams target2 = (AudioParams)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public AudioParams Instantiate()
	{
		return new AudioParams();
	}
}
