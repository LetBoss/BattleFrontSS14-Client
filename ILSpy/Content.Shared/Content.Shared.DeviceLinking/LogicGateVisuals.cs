using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceLinking;

[Serializable]
[NetSerializable]
public enum LogicGateVisuals : byte
{
	Gate,
	InputA,
	InputB,
	Output
}
