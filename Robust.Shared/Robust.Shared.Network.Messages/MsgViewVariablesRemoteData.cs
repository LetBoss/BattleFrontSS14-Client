using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesRemoteData : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint RequestId { get; set; }

	public ViewVariablesBlob Blob { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		RequestId = ((NetBuffer)buffer).ReadUInt32();
		int length = ((NetBuffer)buffer).ReadInt32();
		using MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length);
		buffer.ReadAlignedMemory(memoryStream, length);
		Blob = serializer.Deserialize<ViewVariablesBlob>(memoryStream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(RequestId);
		MemoryStream memoryStream = new MemoryStream();
		serializer.Serialize(memoryStream, Blob);
		((NetBuffer)buffer).Write((int)memoryStream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
	}
}
