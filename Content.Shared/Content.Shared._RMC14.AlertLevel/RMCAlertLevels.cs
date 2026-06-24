using System;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.AlertLevel;

[Serializable]
[NetSerializable]
public enum RMCAlertLevels
{
	Green,
	Blue,
	Red,
	Delta
}
