using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Gateway;

[Serializable]
[NetSerializable]
public record struct GatewayDestinationData
{
	public NetEntity Entity;

	public FormattedMessage Name;

	public bool Portal;

	public bool Locked;
}
