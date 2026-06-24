using System;
using Robust.Shared.Serialization;

namespace Content.Shared.APC;

[Serializable]
[NetSerializable]
public enum ApcPanelState : sbyte
{
	Closed = 0,
	Open = 1,
	Error = -1
}
