using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceLinking;

[Serializable]
[NetSerializable]
public enum SignalState : byte
{
	Momentary,
	Low,
	High
}
