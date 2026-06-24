using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleLocationCommentBuiMsg(int index, string comment) : BoundUserInterfaceMessage
{
	public readonly int Index = index;

	public readonly string Comment = comment;
}
