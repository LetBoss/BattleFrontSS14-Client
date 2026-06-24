using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesReqData : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint RequestId { get; set; }

	public uint SessionId { get; set; }

	public ViewVariablesRequest RequestMeta { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		RequestId = ((NetBuffer)buffer).ReadUInt32();
		SessionId = ((NetBuffer)buffer).ReadUInt32();
		int length = ((NetBuffer)buffer).ReadInt32();
		using MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length);
		buffer.ReadAlignedMemory(memoryStream, length);
		RequestMeta = serializer.Deserialize<ViewVariablesRequest>(memoryStream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(RequestId);
		((NetBuffer)buffer).Write(SessionId);
		MemoryStream memoryStream = new MemoryStream();
		serializer.Serialize(memoryStream, RequestMeta);
		((NetBuffer)buffer).Write((int)memoryStream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
	}
}
