using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[Serializable]
[NetSerializable]
public sealed class InstrumentSetChannelsEvent : EntityEventArgs
{
	public NetEntity Uid { get; }

	public MidiTrack?[] Tracks { get; set; }

	public InstrumentSetChannelsEvent(NetEntity uid, MidiTrack?[] tracks)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Tracks = tracks;
	}
}
