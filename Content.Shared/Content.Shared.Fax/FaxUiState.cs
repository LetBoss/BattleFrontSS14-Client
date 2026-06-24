using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fax;

[Serializable]
[NetSerializable]
public sealed class FaxUiState : BoundUserInterfaceState
{
	public string DeviceName { get; }

	public Dictionary<string, string> AvailablePeers { get; }

	public string? DestinationAddress { get; }

	public bool IsPaperInserted { get; }

	public bool CanSend { get; }

	public bool CanCopy { get; }

	public FaxUiState(string deviceName, Dictionary<string, string> peers, bool canSend, bool canCopy, bool isPaperInserted, string? destAddress)
	{
		DeviceName = deviceName;
		AvailablePeers = peers;
		IsPaperInserted = isPaperInserted;
		CanSend = canSend;
		CanCopy = canCopy;
		DestinationAddress = destAddress;
	}
}
