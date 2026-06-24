using System;
using Robust.Shared.Audio.Midi;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[Serializable]
[NetSerializable]
public sealed class InstrumentMidiEventEvent : EntityEventArgs
{
	public NetEntity Uid { get; }

	public RobustMidiEvent[] MidiEvent { get; }

	public InstrumentMidiEventEvent(NetEntity uid, RobustMidiEvent[] midiEvent)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		MidiEvent = midiEvent;
	}
}
