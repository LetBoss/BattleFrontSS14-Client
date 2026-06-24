using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Supply;

[Serializable]
[NetSerializable]
public struct CivSupplyRefillEntry
{
	public string ProtoId;

	public string Name;

	public string Category;

	public int Count;

	public int Periodic;

	public int UnitPrice;
}
