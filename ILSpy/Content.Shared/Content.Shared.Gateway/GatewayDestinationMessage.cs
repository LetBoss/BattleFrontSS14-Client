using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Gateway;

[Serializable]
[NetSerializable]
public sealed class GatewayDestinationMessage : EntityEventArgs
{
	public int Index;
}
