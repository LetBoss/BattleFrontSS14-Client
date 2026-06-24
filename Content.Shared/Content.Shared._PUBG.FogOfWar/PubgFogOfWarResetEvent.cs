using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.FogOfWar;

[Serializable]
[NetSerializable]
public sealed class PubgFogOfWarResetEvent : EntityEventArgs
{
	public NetEntity GridId { get; }

	public PubgFogOfWarResetEvent(NetEntity gridId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GridId = gridId;
	}
}
