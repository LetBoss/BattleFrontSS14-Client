using System;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA;

[Serializable]
[NetSerializable]
public enum Note : byte
{
	A,
	Asharp,
	B,
	C,
	Csharp,
	D,
	Dsharp,
	E,
	F,
	Fsharp,
	G,
	Gsharp
}
