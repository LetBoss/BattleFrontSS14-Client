using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Wires;

[Serializable]
[NetSerializable]
public sealed class WiresBoundUserInterfaceState : BoundUserInterfaceState
{
	public string BoardName { get; }

	public string? SerialNumber { get; }

	public ClientWire[] WiresList { get; }

	public StatusEntry[] Statuses { get; }

	public int WireSeed { get; }

	public WiresBoundUserInterfaceState(ClientWire[] wiresList, StatusEntry[] statuses, string boardName, string? serialNumber, int wireSeed)
	{
		BoardName = boardName;
		SerialNumber = serialNumber;
		WireSeed = wireSeed;
		WiresList = wiresList;
		Statuses = statuses;
	}
}
