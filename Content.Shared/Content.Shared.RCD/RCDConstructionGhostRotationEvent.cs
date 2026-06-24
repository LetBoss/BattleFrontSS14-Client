using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.RCD;

[Serializable]
[NetSerializable]
public sealed class RCDConstructionGhostRotationEvent(NetEntity netEntity, Direction direction) : EntityEventArgs
{
	public readonly NetEntity NetEntity = netEntity;

	public readonly Direction Direction = direction;
}
