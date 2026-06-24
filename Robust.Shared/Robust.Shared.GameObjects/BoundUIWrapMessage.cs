using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
internal sealed class BoundUIWrapMessage : BaseBoundUIWrapMessage
{
	public BoundUIWrapMessage(NetEntity entity, BoundUserInterfaceMessage message, Enum uiKey)
		: base(entity, message, uiKey)
	{
	}
}
