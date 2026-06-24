using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Salvage.Expeditions;

[Serializable]
[NetSerializable]
public sealed class SalvageExpeditionConsoleState : BoundUserInterfaceState
{
	public TimeSpan NextOffer;

	public bool Claimed;

	public bool Cooldown;

	public ushort ActiveMission;

	public List<SalvageMissionParams> Missions;

	public SalvageExpeditionConsoleState(TimeSpan nextOffer, bool claimed, bool cooldown, ushort activeMission, List<SalvageMissionParams> missions)
	{
		NextOffer = nextOffer;
		Claimed = claimed;
		Cooldown = cooldown;
		ActiveMission = activeMission;
		Missions = missions;
	}
}
