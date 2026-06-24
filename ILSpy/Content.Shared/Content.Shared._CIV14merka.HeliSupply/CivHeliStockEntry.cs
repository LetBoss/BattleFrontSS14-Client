using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.HeliSupply;

[Serializable]
[NetSerializable]
public sealed class CivHeliStockEntry
{
	public string ProtoId = string.Empty;

	public string? Name;

	public int Amount;
}
