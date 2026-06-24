using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public enum StorageVisuals : byte
{
	Open,
	HasContents,
	StorageUsed,
	Capacity
}
