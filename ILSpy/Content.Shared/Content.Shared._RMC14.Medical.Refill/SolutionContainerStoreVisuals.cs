using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Medical.Refill;

[Serializable]
[NetSerializable]
public enum SolutionContainerStoreVisuals : byte
{
	Base,
	Color
}
