using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Watch;

[Serializable]
[NetSerializable]
public sealed class XenoWatchBuiState(List<Xeno> xenos, int burrowedLarva) : BoundUserInterfaceState
{
	public readonly List<Xeno> Xenos = xenos;

	public readonly int BurrowedLarva = burrowedLarva;
}
