using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Messages;

public sealed class MsgConCmdAck : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.String;

	public FormattedMessage Text { get; set; }

	public bool Error { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		using MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length);
		buffer.ReadAlignedMemory(memoryStream, length);
		Text = serializer.Deserialize<FormattedMessage>(memoryStream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		MemoryStream memoryStream = new MemoryStream();
		serializer.Serialize(memoryStream, Text);
		((NetBuffer)buffer).WriteVariableInt32((int)memoryStream.Length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
	}
}
