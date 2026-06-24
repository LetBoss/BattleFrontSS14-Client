using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[Serializable]
[NetSerializable]
public sealed class InstrumentSetMasterEvent : EntityEventArgs
{
	public NetEntity Uid { get; }

	public NetEntity? Master { get; }

	public InstrumentSetMasterEvent(NetEntity uid, NetEntity? master)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Uid = uid;
		Master = master;
	}
}
