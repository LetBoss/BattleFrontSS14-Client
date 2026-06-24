using System;
using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.HeliSupply;

[Serializable]
[NetSerializable]
public sealed class CivHeliStateMessage : EntityEventArgs
{
	public string SideId = string.Empty;

	public List<CivHeliStockSection> Sections = new List<CivHeliStockSection>();

	public List<CivHeliCargoEntry> Cargo = new List<CivHeliCargoEntry>();

	public int CargoCount;

	public int MaxCargo;

	public int MaxWaypoints;

	public bool HasOrigin;

	public Vector2 Origin;

	public MapId OriginMapId;
}
