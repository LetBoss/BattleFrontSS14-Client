using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning.CloningConsole;

[Serializable]
[NetSerializable]
public sealed class CloningConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly string? ScannerBodyInfo;

	public readonly string? ClonerBodyInfo;

	public readonly bool MindPresent;

	public readonly ClonerStatus CloningStatus;

	public readonly bool ScannerConnected;

	public readonly bool ScannerInRange;

	public readonly bool ClonerConnected;

	public readonly bool ClonerInRange;

	public CloningConsoleBoundUserInterfaceState(string? scannerBodyInfo, string? cloningBodyInfo, bool mindPresent, ClonerStatus cloningStatus, bool scannerConnected, bool scannerInRange, bool clonerConnected, bool clonerInRange)
	{
		ScannerBodyInfo = scannerBodyInfo;
		ClonerBodyInfo = cloningBodyInfo;
		MindPresent = mindPresent;
		CloningStatus = cloningStatus;
		ScannerConnected = scannerConnected;
		ScannerInRange = scannerInRange;
		ClonerConnected = clonerConnected;
		ClonerInRange = clonerInRange;
	}
}
