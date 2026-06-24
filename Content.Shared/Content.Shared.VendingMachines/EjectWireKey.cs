using System;
using Robust.Shared.Serialization;

namespace Content.Shared.VendingMachines;

[Serializable]
[NetSerializable]
public enum EjectWireKey : byte
{
	StatusKey
}
