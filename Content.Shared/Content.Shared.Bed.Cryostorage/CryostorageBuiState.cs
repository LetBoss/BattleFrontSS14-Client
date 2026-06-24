using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Bed.Cryostorage;

[Serializable]
[NetSerializable]
public sealed class CryostorageBuiState : BoundUserInterfaceState
{
	public List<CryostorageContainedPlayerData> PlayerData;

	public CryostorageBuiState(List<CryostorageContainedPlayerData> playerData)
	{
		PlayerData = playerData;
	}
}
