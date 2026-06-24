using System;
using Robust.Shared.Serialization;

namespace Content.Shared.APC;

[Serializable]
[NetSerializable]
public enum ApcVisuals : byte
{
	LockState,
	ChannelState,
	ChargeState
}
