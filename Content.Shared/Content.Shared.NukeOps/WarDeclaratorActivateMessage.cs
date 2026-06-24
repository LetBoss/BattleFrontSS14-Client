using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NukeOps;

[Serializable]
[NetSerializable]
public sealed class WarDeclaratorActivateMessage : BoundUserInterfaceMessage
{
	public string Message { get; }

	public WarDeclaratorActivateMessage(string msg)
	{
		Message = msg;
	}
}
