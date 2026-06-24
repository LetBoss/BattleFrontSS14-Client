using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Materials.OreSilo;

[Serializable]
[NetSerializable]
public sealed class OreSiloBuiState : BoundUserInterfaceState
{
	public readonly HashSet<(NetEntity, string, string)> Clients;

	public OreSiloBuiState(HashSet<(NetEntity, string, string)> clients)
	{
		Clients = clients;
	}
}
