using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.V2.Repository;

[Serializable]
[NetSerializable]
public sealed class MessageDeletedEvent(uint id) : EntityEventArgs
{
	public uint MessageId = id;
}
