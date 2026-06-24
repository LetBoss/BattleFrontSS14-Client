using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask;

[Serializable]
[NetSerializable]
public sealed class VoiceMaskBuiState : BoundUserInterfaceState
{
	public readonly string Name;

	public readonly string? Verb;

	public VoiceMaskBuiState(string name, string? verb)
	{
		Name = name;
		Verb = verb;
	}
}
