using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.StationAi;

[Serializable]
[NetSerializable]
public enum StationAiState : byte
{
	Empty,
	Occupied,
	Dead,
	Hologram
}
