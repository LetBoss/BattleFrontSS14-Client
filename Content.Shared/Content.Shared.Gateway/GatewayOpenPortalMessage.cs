using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Gateway;

[Serializable]
[NetSerializable]
public sealed class GatewayOpenPortalMessage : BoundUserInterfaceMessage
{
	public NetEntity Destination;

	public GatewayOpenPortalMessage(NetEntity destination)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Destination = destination;
	}
}
