using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Tabletop.Events;

[Serializable]
[NetSerializable]
public sealed class TabletopStopPlayingEvent : EntityEventArgs
{
	public NetEntity TableUid;

	public TabletopStopPlayingEvent(NetEntity tableUid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TableUid = tableUid;
	}
}
