using System;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Salvage.Expeditions;

[Serializable]
[NetSerializable]
public sealed record SalvageMissionParams
{
	[ViewVariables]
	public ushort Index;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public int Seed;

	public string Difficulty = string.Empty;
}
