using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DeviceNetwork;

[Serializable]
[NetSerializable]
public sealed class NetworkConfiguratorButtonPressedMessage : BoundUserInterfaceMessage
{
	public readonly NetworkConfiguratorButtonKey ButtonKey;

	public NetworkConfiguratorButtonPressedMessage(NetworkConfiguratorButtonKey buttonKey)
	{
		ButtonKey = buttonKey;
	}
}
