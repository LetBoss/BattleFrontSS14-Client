using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleHideBuiMsg(NetEntity target, bool hide) : BoundUserInterfaceMessage
{
	public readonly NetEntity Target = target;

	public readonly bool Hide = hide;
}
