using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications;

[Serializable]
[NetSerializable]
public sealed class CommunicationsConsoleAnnounceMessage : BoundUserInterfaceMessage
{
	public readonly string Message;

	public CommunicationsConsoleAnnounceMessage(string message)
	{
		Message = message;
	}
}
