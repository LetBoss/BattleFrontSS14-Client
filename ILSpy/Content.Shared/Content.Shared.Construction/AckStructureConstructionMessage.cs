using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class AckStructureConstructionMessage : EntityEventArgs
{
	public readonly int GhostId;

	public readonly NetEntity? Uid;

	public AckStructureConstructionMessage(int ghostId, NetEntity? uid = null)
	{
		GhostId = ghostId;
		Uid = uid;
	}
}
