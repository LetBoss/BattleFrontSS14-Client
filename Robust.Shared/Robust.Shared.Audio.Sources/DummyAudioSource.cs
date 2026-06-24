using System;
using System.Numerics;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Effects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Robust.Shared.Audio.Sources;

[Virtual]
[DataDefinition]
internal class DummyAudioSource : IAudioSource, IDisposable, ISerializationGenerated<DummyAudioSource>, ISerializationGenerated
{
	public static DummyAudioSource Instance { get; } = new DummyAudioSource();

	public bool Playing { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public bool Looping { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public bool Global { get; set; }

	public Vector2 Position { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public float Pitch { get; set; }

	public float Volume { get; set; }

	public float Gain { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public float MaxDistance { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public float RolloffFactor { get; set; }

	[DataField(null, false, 1, false, false, null)]
	public float ReferenceDistance { get; set; }

	public float Occlusion { get; set; }

	public float PlaybackPosition { get; set; }

	public Vector2 Velocity { get; set; }

	public void Dispose()
	{
	}

	public void Pause()
	{
	}

	public void StartPlaying()
	{
	}

	public void StopPlaying()
	{
	}

	public void Restart()
	{
	}

	public void SetAuxiliary(IAuxiliaryAudio? audio)
	{
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void InternalCopy(ref DummyAudioSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		if (!serialization.TryCustomCopy(this, ref target, hookCtx, hasHooks: false, context))
		{
			bool target2 = false;
			if (!serialization.TryCustomCopy(Looping, ref target2, hookCtx, hasHooks: false, context))
			{
				target2 = Looping;
			}
			target.Looping = target2;
			bool target3 = false;
			if (!serialization.TryCustomCopy(Global, ref target3, hookCtx, hasHooks: false, context))
			{
				target3 = Global;
			}
			target.Global = target3;
			float target4 = 0f;
			if (!serialization.TryCustomCopy(Pitch, ref target4, hookCtx, hasHooks: false, context))
			{
				target4 = Pitch;
			}
			target.Pitch = target4;
			float target5 = 0f;
			if (!serialization.TryCustomCopy(MaxDistance, ref target5, hookCtx, hasHooks: false, context))
			{
				target5 = MaxDistance;
			}
			target.MaxDistance = target5;
			float target6 = 0f;
			if (!serialization.TryCustomCopy(RolloffFactor, ref target6, hookCtx, hasHooks: false, context))
			{
				target6 = RolloffFactor;
			}
			target.RolloffFactor = target6;
			float target7 = 0f;
			if (!serialization.TryCustomCopy(ReferenceDistance, ref target7, hookCtx, hasHooks: false, context))
			{
				target7 = ReferenceDistance;
			}
			target.ReferenceDistance = target7;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref DummyAudioSource target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public virtual void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		DummyAudioSource target2 = (DummyAudioSource)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public virtual DummyAudioSource Instantiate()
	{
		return new DummyAudioSource();
	}
}
