using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.BUIStates;

[Serializable]
[NetSerializable]
public sealed class DockingPortState
{
	public string Name = string.Empty;

	public NetCoordinates Coordinates;

	public Angle Angle;

	public NetEntity Entity;

	public NetEntity? GridDockedWith;

	public bool Connected => GridDockedWith.HasValue;
}
