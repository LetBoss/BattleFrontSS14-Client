using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Radio;

[Serializable]
[NetSerializable]
public sealed class SelectIntercomChannelMessage : BoundUserInterfaceMessage
{
	public string Channel;

	public SelectIntercomChannelMessage(string channel)
	{
		Channel = channel;
	}
}
