using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Areas;

[Serializable]
[NetSerializable]
public enum AreaHijackEvacuationType
{
	None,
	Add,
	Multiply
}
