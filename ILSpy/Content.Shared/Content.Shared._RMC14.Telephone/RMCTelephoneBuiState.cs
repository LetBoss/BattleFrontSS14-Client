using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Telephone;

[Serializable]
[NetSerializable]
public sealed class RMCTelephoneBuiState(List<RMCPhone> phones, bool canDnd, bool dnd) : BoundUserInterfaceState
{
	public readonly List<RMCPhone> Phones = phones;

	public readonly bool CanDnd = canDnd;

	public readonly bool Dnd = dnd;
}
