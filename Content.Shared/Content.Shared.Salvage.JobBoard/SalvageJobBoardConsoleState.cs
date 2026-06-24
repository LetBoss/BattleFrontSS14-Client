using System;
using System.Collections.Generic;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.JobBoard;

[Serializable]
[NetSerializable]
public sealed class SalvageJobBoardConsoleState : BoundUserInterfaceState
{
	public string Title;

	public float Progression;

	public List<ProtoId<CargoBountyPrototype>> AvailableJobs;

	public SalvageJobBoardConsoleState(string title, float progression, List<ProtoId<CargoBountyPrototype>> availableJobs)
	{
		Title = title;
		Progression = progression;
		AvailableJobs = availableJobs;
	}
}
