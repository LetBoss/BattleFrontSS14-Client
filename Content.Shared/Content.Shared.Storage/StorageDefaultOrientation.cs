using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage;

[Serializable]
[NetSerializable]
public enum StorageDefaultOrientation : byte
{
	Horizontal,
	Vertical
}
