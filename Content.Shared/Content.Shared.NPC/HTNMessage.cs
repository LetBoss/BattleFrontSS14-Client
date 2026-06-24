using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.NPC;

[Serializable]
[NetSerializable]
public sealed class HTNMessage : EntityEventArgs
{
	public NetEntity Uid;

	public string Text = string.Empty;
}
