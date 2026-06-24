using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Gibbing.Events;

[Serializable]
[NetSerializable]
public enum GibType : byte
{
	Skip,
	Drop,
	Gib
}
