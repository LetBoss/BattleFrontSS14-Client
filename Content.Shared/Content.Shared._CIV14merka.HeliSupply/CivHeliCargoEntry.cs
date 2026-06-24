using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.HeliSupply;

[Serializable]
[NetSerializable]
public sealed class CivHeliCargoEntry
{
	public string ProtoId = string.Empty;

	public int Count;
}
