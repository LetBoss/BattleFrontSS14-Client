using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.HeliSupply;

[Serializable]
[NetSerializable]
public sealed class CivHeliStockSection
{
	public string Name = string.Empty;

	public List<CivHeliStockEntry> Entries = new List<CivHeliStockEntry>();
}
