using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public enum CargoOrderConsoleMode : byte
{
	DirectOrder,
	PrintSlip,
	SendToPrimary
}
