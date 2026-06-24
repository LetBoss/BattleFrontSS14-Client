using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Evolution;

[Serializable]
[NetSerializable]
public sealed class XenoEvolveBuiState(bool lackingOvipositor) : BoundUserInterfaceState
{
	public readonly bool LackingOvipositor = lackingOvipositor;
}
