using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderVisionResetEvent : EntityEventArgs
{
	public NetEntity GridId { get; }

	public CivCommanderVisionResetEvent(NetEntity gridId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GridId = gridId;
	}
}
