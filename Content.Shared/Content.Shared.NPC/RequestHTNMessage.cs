using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public sealed class RequestHTNMessage : EntityEventArgs
{
	public bool Enabled;
}
