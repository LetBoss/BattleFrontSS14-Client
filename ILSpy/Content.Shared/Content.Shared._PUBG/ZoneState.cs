using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG;

[Serializable]
[NetSerializable]
public enum ZoneState
{
	Waiting,
	Shrinking
}
