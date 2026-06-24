using System;
using System.Collections;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[Serializable]
[NetSerializable]
public sealed class InstrumentComponentState : ComponentState
{
	public bool Playing;

	public byte InstrumentProgram;

	public byte InstrumentBank;

	public bool AllowPercussion;

	public bool AllowProgramChange;

	public bool RespectMidiLimits;

	public NetEntity? Master;

	public BitArray FilteredChannels;
}
