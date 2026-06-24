using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Kitchen;

[Serializable]
[NetSerializable]
public sealed class ReagentGrinderStartMessage : BoundUserInterfaceMessage
{
	public readonly GrinderProgram Program;

	public ReagentGrinderStartMessage(GrinderProgram program)
	{
		Program = program;
	}
}
