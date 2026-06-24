using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Payload.Components;

[Serializable]
[Flags]
[NetSerializable]
public enum ChemicalPayloadFilledSlots : byte
{
	None = 0,
	Left = 1,
	Right = 2,
	Both = 3
}
