using System;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines;

[Serializable]
[NetSerializable]
public enum InventoryType : byte
{
	Regular,
	Emagged,
	Contraband
}
