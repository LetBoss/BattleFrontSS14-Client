using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader;

[Serializable]
[NetSerializable]
public sealed class CartridgeLoaderUiMessage : BoundUserInterfaceMessage
{
	public readonly NetEntity CartridgeUid;

	public readonly CartridgeUiMessageAction Action;

	public CartridgeLoaderUiMessage(NetEntity cartridgeUid, CartridgeUiMessageAction action)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		CartridgeUid = cartridgeUid;
		Action = action;
	}
}
