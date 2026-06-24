using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka;

[Serializable]
[NetSerializable]
public enum ModeMenuMode : byte
{
	None,
	Pubg,
	Civ14
}
