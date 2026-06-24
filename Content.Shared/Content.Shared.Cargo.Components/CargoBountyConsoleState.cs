using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Cargo.Components;

[Serializable]
[NetSerializable]
public sealed class CargoBountyConsoleState : BoundUserInterfaceState
{
	public List<CargoBountyData> Bounties;

	public List<CargoBountyHistoryData> History;

	public TimeSpan UntilNextSkip;

	public CargoBountyConsoleState(List<CargoBountyData> bounties, List<CargoBountyHistoryData> history, TimeSpan untilNextSkip)
	{
		Bounties = bounties;
		History = history;
		UntilNextSkip = untilNextSkip;
	}
}
