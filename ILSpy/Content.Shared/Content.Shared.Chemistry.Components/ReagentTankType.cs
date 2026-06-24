using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Components;

[Serializable]
[NetSerializable]
public enum ReagentTankType : byte
{
	Unspecified,
	Fuel
}
