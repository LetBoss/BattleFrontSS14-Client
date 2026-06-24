using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Roles.FindParasite;

[Serializable]
[NetSerializable]
public sealed class FindParasiteUIState : BoundUserInterfaceState
{
	public List<SpawnerData> ActiveParasiteSpawners = new List<SpawnerData>();
}
