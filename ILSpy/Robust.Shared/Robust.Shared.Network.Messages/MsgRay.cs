using System;
using System.Numerics;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

[Serializable]
[NetSerializable]
public sealed class MsgRay : EntityEventArgs
{
	public Vector2 RayOrigin;

	public Vector2 RayHit;

	public bool DidHit;

	public MapId Map;
}
