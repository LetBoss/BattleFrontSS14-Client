using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Decor;

[Serializable]
[NetSerializable]
public enum PubgShellrackFill : byte
{
	Empty,
	Partial,
	Full
}
