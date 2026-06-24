using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public abstract class BaseBoundUserInterfaceEvent : EntityEventArgs
{
	public Enum UiKey;

	[NonSerialized]
	public EntityUid Actor;
}
