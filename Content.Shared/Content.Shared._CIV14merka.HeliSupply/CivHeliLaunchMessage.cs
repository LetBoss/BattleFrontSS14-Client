using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.HeliSupply;

[Serializable]
[NetSerializable]
public sealed class CivHeliLaunchMessage : EntityEventArgs
{
	public List<Vector2> Points = new List<Vector2>();

	public MapId MapId;
}
