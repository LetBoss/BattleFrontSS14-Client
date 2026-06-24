using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public abstract class BoundUserInterfaceMessage : BaseBoundUserInterfaceEvent
{
	public NetEntity Entity { get; set; } = NetEntity.Invalid;
}
