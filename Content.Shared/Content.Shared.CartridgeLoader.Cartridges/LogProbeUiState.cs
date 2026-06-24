using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader.Cartridges;

[Serializable]
[NetSerializable]
public sealed class LogProbeUiState : BoundUserInterfaceState
{
	public string EntityName;

	public List<PulledAccessLog> PulledLogs;

	public LogProbeUiState(string entityName, List<PulledAccessLog> pulledLogs)
	{
		EntityName = entityName;
		PulledLogs = pulledLogs;
	}
}
