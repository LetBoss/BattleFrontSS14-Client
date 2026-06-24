using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Materials.OreSilo;

[Serializable]
[NetSerializable]
public sealed class ToggleOreSiloClientMessage : BoundUserInterfaceMessage
{
	public readonly NetEntity Client;

	public ToggleOreSiloClientMessage(NetEntity client)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Client = client;
	}
}
