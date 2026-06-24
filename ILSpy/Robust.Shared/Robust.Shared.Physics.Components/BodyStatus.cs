using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Physics.Components;

[Serializable]
[NetSerializable]
public enum BodyStatus : byte
{
	OnGround,
	InAir
}
