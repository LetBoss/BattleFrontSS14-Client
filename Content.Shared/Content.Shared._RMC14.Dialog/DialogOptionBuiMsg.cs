using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Dialog;

[Serializable]
[NetSerializable]
public sealed class DialogOptionBuiMsg(int index) : BoundUserInterfaceMessage
{
	public readonly int Index = index;
}
