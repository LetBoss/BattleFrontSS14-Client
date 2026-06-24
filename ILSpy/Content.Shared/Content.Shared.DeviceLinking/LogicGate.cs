using System;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceLinking;

[Serializable]
[NetSerializable]
public enum LogicGate : byte
{
	Or,
	And,
	Xor,
	Nor,
	Nand,
	Xnor
}
