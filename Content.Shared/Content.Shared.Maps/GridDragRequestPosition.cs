using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Maps;

[Serializable]
[NetSerializable]
public sealed class GridDragRequestPosition : EntityEventArgs
{
	public NetEntity Grid;

	public Vector2 WorldPosition;
}
