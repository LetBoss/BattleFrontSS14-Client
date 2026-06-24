using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public sealed class NetworkConfiguratorToggleLinkMessage : BoundUserInterfaceMessage
{
	public readonly string Source;

	public readonly string Sink;

	public NetworkConfiguratorToggleLinkMessage(string source, string sink)
	{
		Source = source;
		Sink = sink;
	}
}
