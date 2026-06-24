using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public sealed class NetworkConfiguratorLinksSaveMessage : BoundUserInterfaceMessage
{
	public readonly List<(string source, string sink)> Links;

	public NetworkConfiguratorLinksSaveMessage(List<(string source, string sink)> links)
	{
		Links = links;
	}
}
