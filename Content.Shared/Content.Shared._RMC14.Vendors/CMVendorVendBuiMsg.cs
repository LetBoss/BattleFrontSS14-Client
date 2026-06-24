using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Vendors;

[Serializable]
[NetSerializable]
public sealed class CMVendorVendBuiMsg(int section, int entry, List<int> linkedEntries) : BoundUserInterfaceMessage
{
	public readonly int Section = section;

	public readonly int Entry = entry;

	public readonly List<int> LinkedEntries = linkedEntries;
}
