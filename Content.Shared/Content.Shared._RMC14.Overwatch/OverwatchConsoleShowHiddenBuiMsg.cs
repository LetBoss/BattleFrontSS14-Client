using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public sealed class OverwatchConsoleShowHiddenBuiMsg(bool show) : BoundUserInterfaceMessage
{
	public readonly bool Show = show;
}
