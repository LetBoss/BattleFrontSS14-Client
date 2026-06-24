using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost;

[Serializable]
[NetSerializable]
public sealed class GhostWarpToTargetRequestEvent : EntityEventArgs
{
	public NetEntity Target { get; }

	public GhostWarpToTargetRequestEvent(NetEntity target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
	}
}
