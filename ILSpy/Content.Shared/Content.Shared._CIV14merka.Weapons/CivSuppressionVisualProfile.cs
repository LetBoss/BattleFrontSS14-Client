using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Weapons;

[Serializable]
[NetSerializable]
public enum CivSuppressionVisualProfile : byte
{
	IncomingFire,
	Explosion,
	Mortar
}
