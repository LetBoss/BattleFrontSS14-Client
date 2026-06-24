using System;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Radio;

[Serializable]
[NetSerializable]
public sealed class RMCRadioFilterBuiMsg(ProtoId<RadioChannelPrototype> channel, bool toggle) : BoundUserInterfaceMessage
{
	public readonly ProtoId<RadioChannelPrototype> Channel = channel;

	public readonly bool Toggle = toggle;
}
