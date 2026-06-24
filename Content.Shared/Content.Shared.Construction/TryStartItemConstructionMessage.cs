using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Construction;

[Serializable]
[NetSerializable]
public sealed class TryStartItemConstructionMessage : EntityEventArgs
{
	public readonly string PrototypeName;

	public TryStartItemConstructionMessage(string prototypeName)
	{
		PrototypeName = prototypeName;
	}
}
