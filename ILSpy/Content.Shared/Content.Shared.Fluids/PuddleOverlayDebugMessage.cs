using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Fluids;

[Serializable]
[NetSerializable]
public sealed class PuddleOverlayDebugMessage : EntityEventArgs
{
	public PuddleDebugOverlayData[] OverlayData { get; }

	public NetEntity GridUid { get; }

	public PuddleOverlayDebugMessage(NetEntity gridUid, PuddleDebugOverlayData[] overlayData)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GridUid = gridUid;
		OverlayData = overlayData;
	}
}
