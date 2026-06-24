using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat.V2.Repository;

[Serializable]
[NetSerializable]
public sealed class MessagePatchedEvent(uint id, string newMessage) : EntityEventArgs
{
	public uint MessageId = id;

	public string NewMessage = newMessage;
}
