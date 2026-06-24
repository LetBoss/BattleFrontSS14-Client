using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Roles.FindParasite;

[Serializable]
[NetSerializable]
public sealed class TakeParasiteRoleMessage : BoundUserInterfaceMessage
{
	public NetEntity Spawner;

	public TakeParasiteRoleMessage(NetEntity spawner)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Spawner = spawner;
	}
}
