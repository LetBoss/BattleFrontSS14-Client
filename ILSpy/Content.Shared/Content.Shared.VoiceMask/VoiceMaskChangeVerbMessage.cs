using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask;

[Serializable]
[NetSerializable]
public sealed class VoiceMaskChangeVerbMessage : BoundUserInterfaceMessage
{
	public readonly string? Verb;

	public VoiceMaskChangeVerbMessage(string? verb)
	{
		Verb = verb;
	}
}
