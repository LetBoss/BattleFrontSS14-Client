using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public sealed class ReagentGrinderWorkStartedMessage : BoundUserInterfaceMessage
{
	public GrinderProgram GrinderProgram;

	public ReagentGrinderWorkStartedMessage(GrinderProgram grinderProgram)
	{
		GrinderProgram = grinderProgram;
	}
}
