using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsolePromoteLeaderBuiMsg(NetEntity target, Rsi icon) : BoundUserInterfaceMessage
{
	public readonly NetEntity Target = target;

	public readonly Rsi Icon = icon;
}
