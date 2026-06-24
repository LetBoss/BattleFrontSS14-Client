using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesModifyRemote : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint SessionId { get; set; }

	public bool ReinterpretValue { get; set; }

	public object[] PropertyIndex { get; set; }

	public object Value { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		SessionId = ((NetBuffer)buffer).ReadUInt32();
		int length = ((NetBuffer)buffer).ReadInt32();
		using (MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length))
		{
			buffer.ReadAlignedMemory(memoryStream, length);
			PropertyIndex = serializer.Deserialize<object[]>(memoryStream);
		}
		int length2 = ((NetBuffer)buffer).ReadInt32();
		using (MemoryStream memoryStream2 = RobustMemoryManager.GetMemoryStream(length2))
		{
			buffer.ReadAlignedMemory(memoryStream2, length2);
			Value = serializer.Deserialize(memoryStream2);
		}
		ReinterpretValue = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(SessionId);
		MemoryStream memoryStream = new MemoryStream();
		serializer.Serialize(memoryStream, PropertyIndex);
		((NetBuffer)buffer).Write((int)memoryStream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
		memoryStream.Position = 0L;
		serializer.Serialize(memoryStream, Value);
		((NetBuffer)buffer).Write((int)memoryStream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
		((NetBuffer)buffer).Write(ReinterpretValue);
	}
}
