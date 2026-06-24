using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Communications;

[Serializable]
[NetSerializable]
public sealed class CommunicationsConsoleSelectAlertLevelMessage : BoundUserInterfaceMessage
{
	public readonly string Level;

	public CommunicationsConsoleSelectAlertLevelMessage(string level)
	{
		Level = level;
	}
}
