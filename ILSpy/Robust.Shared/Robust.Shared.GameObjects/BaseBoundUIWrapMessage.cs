using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
internal abstract class BaseBoundUIWrapMessage(NetEntity entity, BoundUserInterfaceMessage message, Enum uiKey) : EntityEventArgs
{
	public readonly NetEntity Entity = entity;

	public readonly BoundUserInterfaceMessage Message = message;

	public readonly Enum UiKey = uiKey;
}
