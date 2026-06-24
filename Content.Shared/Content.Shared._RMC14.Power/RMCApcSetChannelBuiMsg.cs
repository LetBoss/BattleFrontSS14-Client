using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Power;

[Serializable]
[NetSerializable]
public sealed class RMCApcSetChannelBuiMsg(RMCPowerChannel channel, RMCApcButtonState state) : BoundUserInterfaceMessage
{
	public readonly RMCPowerChannel Channel = channel;

	public readonly RMCApcButtonState State = state;
}
