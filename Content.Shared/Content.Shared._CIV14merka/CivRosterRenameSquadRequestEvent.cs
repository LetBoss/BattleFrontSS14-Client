using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public sealed class CivRosterRenameSquadRequestEvent : EntityEventArgs
{
	public int TeamId { get; }

	public int SquadId { get; }

	public string Name { get; }

	public CivRosterRenameSquadRequestEvent(int teamId, int squadId, string name)
	{
		TeamId = teamId;
		SquadId = squadId;
		Name = name;
	}
}
