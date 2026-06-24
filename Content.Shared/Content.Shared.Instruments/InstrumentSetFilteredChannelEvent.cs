using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[Serializable]
[NetSerializable]
public sealed class InstrumentSetFilteredChannelEvent : EntityEventArgs
{
	public NetEntity Uid { get; }

	public int Channel { get; }

	public bool Value { get; }

	public InstrumentSetFilteredChannelEvent(NetEntity uid, int channel, bool value)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Channel = channel;
		Value = value;
	}
}
