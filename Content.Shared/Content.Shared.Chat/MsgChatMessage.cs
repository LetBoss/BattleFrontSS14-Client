using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Chat;

public sealed class MsgChatMessage : NetMessage
{
	public ChatMessage Message;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		using MemoryStream stream = new MemoryStream(length);
		NetMessageExt.ReadAlignedMemory(buffer, stream, length);
		serializer.DeserializeDirect<ChatMessage>((Stream)stream, ref Message);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		MemoryStream stream = new MemoryStream();
		serializer.SerializeDirect<ChatMessage>((Stream)stream, Message);
		((NetBuffer)buffer).WriteVariableInt32((int)stream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)StreamExt.AsSpan(stream));
	}
}
