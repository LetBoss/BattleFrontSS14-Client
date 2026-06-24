using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Cloning.CloningConsole;

[Serializable]
[NetSerializable]
public enum ClonerStatus : byte
{
	Ready,
	ScannerEmpty,
	ScannerOccupantAlive,
	OccupantMetaphyiscal,
	ClonerOccupied,
	NoClonerDetected,
	NoMindDetected
}
