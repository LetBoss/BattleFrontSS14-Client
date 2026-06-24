using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Egg;

[Serializable]
[NetSerializable]
public sealed class XenoEggUseInHandEvent : HandledEntityEventArgs
{
	public NetEntity UsedEgg;

	public XenoEggUseInHandEvent(NetEntity usedEgg)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		UsedEgg = usedEgg;
	}
}
