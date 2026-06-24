using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.CartridgeLoader;

[Serializable]
[NetSerializable]
public abstract class CartridgeMessageEvent : EntityEventArgs
{
	[NonSerialized]
	public EntityUid User;

	public NetEntity LoaderUid;

	[NonSerialized]
	public EntityUid Actor;
}
