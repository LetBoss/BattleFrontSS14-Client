using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications;

[Serializable]
[NetSerializable]
public sealed class CommunicationsConsoleBroadcastMessage : BoundUserInterfaceMessage
{
	public readonly string Message;

	public CommunicationsConsoleBroadcastMessage(string message)
	{
		Message = message;
	}
}
