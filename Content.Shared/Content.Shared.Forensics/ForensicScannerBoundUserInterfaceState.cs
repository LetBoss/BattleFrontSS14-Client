using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Forensics;

[Serializable]
[NetSerializable]
public sealed class ForensicScannerBoundUserInterfaceState : BoundUserInterfaceState
{
	public readonly List<string> Fingerprints = new List<string>();

	public readonly List<string> Fibers = new List<string>();

	public readonly List<string> TouchDNAs = new List<string>();

	public readonly List<string> SolutionDNAs = new List<string>();

	public readonly List<string> Residues = new List<string>();

	public readonly string LastScannedName = string.Empty;

	public readonly TimeSpan PrintCooldown = TimeSpan.Zero;

	public readonly TimeSpan PrintReadyAt = TimeSpan.Zero;

	public ForensicScannerBoundUserInterfaceState(List<string> fingerprints, List<string> fibers, List<string> touchDnas, List<string> solutionDnas, List<string> residues, string lastScannedName, TimeSpan printCooldown, TimeSpan printReadyAt)
	{
		Fingerprints = fingerprints;
		Fibers = fibers;
		TouchDNAs = touchDnas;
		SolutionDNAs = solutionDnas;
		Residues = residues;
		LastScannedName = lastScannedName;
		PrintCooldown = printCooldown;
		PrintReadyAt = printReadyAt;
	}
}
