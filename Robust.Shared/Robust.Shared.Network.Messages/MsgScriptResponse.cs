using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptResponse : NetMessage
{
	public FormattedMessage Echo;

	public FormattedMessage Response;

	public override MsgGroups MsgGroup => MsgGroups.Command;

	public int ScriptSession { get; set; }

	public bool WasComplete { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		ScriptSession = ((NetBuffer)buffer).ReadInt32();
		WasComplete = ((NetBuffer)buffer).ReadBoolean();
		if (WasComplete)
		{
			((NetBuffer)buffer).ReadPadBits();
			int length = ((NetBuffer)buffer).ReadVariableInt32();
			using MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length);
			buffer.ReadAlignedMemory(memoryStream, length);
			serializer.DeserializeDirect<FormattedMessage>(memoryStream, out Echo);
			serializer.DeserializeDirect<FormattedMessage>(memoryStream, out Response);
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(ScriptSession);
		((NetBuffer)buffer).Write(WasComplete);
		if (WasComplete)
		{
			((NetBuffer)buffer).WritePadBits();
			MemoryStream memoryStream = new MemoryStream();
			serializer.SerializeDirect(memoryStream, Echo);
			serializer.SerializeDirect(memoryStream, Response);
			((NetBuffer)buffer).WriteVariableInt32((int)memoryStream.Length);
			((NetBuffer)buffer).Write((ReadOnlySpan<byte>)memoryStream.AsSpan());
		}
	}
}
