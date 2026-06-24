using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.VoiceMask;

[Serializable]
[NetSerializable]
public sealed class VoiceMaskChangeNameMessage : BoundUserInterfaceMessage
{
	public readonly string Name;

	public VoiceMaskChangeNameMessage(string name)
	{
		Name = name;
	}
}
