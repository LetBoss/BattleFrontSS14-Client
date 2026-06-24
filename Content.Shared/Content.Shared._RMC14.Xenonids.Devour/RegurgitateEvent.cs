using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Devour;

[Serializable]
[NetSerializable]
public sealed class RegurgitateEvent : EntityEventArgs
{
	public NetEntity NetRegurgitater;

	public NetEntity NetRegurgitated;

	public RegurgitateEvent(NetEntity netRegurgitater, NetEntity netRegurgitated)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		NetRegurgitater = netRegurgitater;
		NetRegurgitated = netRegurgitated;
	}
}
