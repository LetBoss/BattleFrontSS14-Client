using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleSendMessageBuiMsg(string message) : BoundUserInterfaceMessage
{
	public readonly string Message = message;
}
