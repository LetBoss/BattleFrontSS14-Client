using System;
using System.Collections.Generic;
using Content.Shared.Instruments;
using Robust.Client.Audio.Midi;
using Robust.Shared.Audio.Midi;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Instruments;

[RegisterComponent]
public sealed class InstrumentComponent : SharedInstrumentComponent, ISerializationGenerated<InstrumentComponent>, ISerializationGenerated
{
	[ViewVariables]
	public IMidiRenderer? Renderer;

	[ViewVariables]
	public uint SequenceDelay;

	[ViewVariables]
	public uint SequenceStartTick;

	[ViewVariables]
	public TimeSpan LastMeasured = TimeSpan.MinValue;

	[ViewVariables]
	public int SentWithinASec;

	[ViewVariables]
	public readonly List<RobustMidiEvent> MidiEventBuffer = new List<RobustMidiEvent>();

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public bool LoopMidi { get; set; }

	[DataField("handheld", false, 1, false, false, null)]
	public bool Handheld { get; set; }

	[ViewVariables]
	public bool IsMidiOpen
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			IMidiRenderer? renderer = Renderer;
			if (renderer == null)
			{
				return false;
			}
			return (int)renderer.Status == 2;
		}
	}

	[ViewVariables]
	public bool IsInputOpen
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Invalid comparison between Unknown and I4
			IMidiRenderer? renderer = Renderer;
			if (renderer == null)
			{
				return false;
			}
			return (int)renderer.Status == 1;
		}
	}

	[ViewVariables]
	public bool IsRendererAlive => Renderer != null;

	[ViewVariables]
	public int PlayerTotalTick
	{
		get
		{
			IMidiRenderer? renderer = Renderer;
			if (renderer == null)
			{
				return 0;
			}
			return renderer.PlayerTotalTick;
		}
	}

	[ViewVariables]
	public int PlayerTick
	{
		get
		{
			IMidiRenderer? renderer = Renderer;
			if (renderer == null)
			{
				return 0;
			}
			return renderer.PlayerTick;
		}
	}

	public event Action? OnMidiPlaybackEnded;

	public void PlaybackEndedInvoke()
	{
		this.OnMidiPlaybackEnded?.Invoke();
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void InternalCopy(ref InstrumentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		SharedInstrumentComponent target2 = target;
		base.InternalCopy(ref target2, serialization, hookCtx, context);
		target = (InstrumentComponent)target2;
		if (!serialization.TryCustomCopy<InstrumentComponent>(this, ref target, hookCtx, false, context))
		{
			bool handheld = false;
			if (!serialization.TryCustomCopy<bool>(Handheld, ref handheld, hookCtx, false, context))
			{
				handheld = Handheld;
			}
			target.Handheld = handheld;
		}
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public void Copy(ref InstrumentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref SharedInstrumentComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstrumentComponent target2 = (InstrumentComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref object target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstrumentComponent target2 = (InstrumentComponent)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void InternalCopy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		InstrumentComponent target2 = (InstrumentComponent)(object)target;
		Copy(ref target2, serialization, hookCtx, context);
		target = (IComponent)(object)target2;
	}

	[Obsolete("Use ISerializationManager.CopyTo instead")]
	public override void Copy(ref IComponent target, ISerializationManager serialization, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		((Component)this).InternalCopy(ref target, serialization, hookCtx, context);
	}

	[Obsolete("Use ISerializationManager.CreateCopy instead")]
	public override InstrumentComponent Instantiate()
	{
		return new InstrumentComponent();
	}
}
