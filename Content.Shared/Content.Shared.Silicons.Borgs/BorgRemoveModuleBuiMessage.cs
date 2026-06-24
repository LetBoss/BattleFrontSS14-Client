using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Silicons.Borgs;

[Serializable]
[NetSerializable]
public sealed class BorgRemoveModuleBuiMessage : BoundUserInterfaceMessage
{
	public NetEntity Module;

	public BorgRemoveModuleBuiMessage(NetEntity module)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Module = module;
	}
}
