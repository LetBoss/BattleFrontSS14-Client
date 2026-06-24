using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.GlobalMap;

[Serializable]
[NetSerializable]
public enum CivGlobalMapMarkerType : byte
{
	Attack,
	Defense,
	Enemy,
	Help,
	Allies,
	PointCapture
}
