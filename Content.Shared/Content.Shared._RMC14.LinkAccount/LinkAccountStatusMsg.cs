using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.LinkAccount;

public sealed class LinkAccountStatusMsg : NetMessage
{
	public SharedRMCPatronFull? Patron;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		if (!((NetBuffer)buffer).ReadBoolean())
		{
			return;
		}
		((NetBuffer)buffer).ReadPadBits();
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		using MemoryStream stream = new MemoryStream(length);
		NetMessageExt.ReadAlignedMemory(buffer, stream, length);
		Patron = serializer.Deserialize<SharedRMCPatronFull>((Stream)stream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		if (Patron == null)
		{
			((NetBuffer)buffer).Write(false);
			return;
		}
		((NetBuffer)buffer).Write(true);
		((NetBuffer)buffer).WritePadBits();
		using MemoryStream stream = new MemoryStream();
		serializer.Serialize((Stream)stream, (object)Patron);
		((NetBuffer)buffer).WriteVariableInt32((int)stream.Length);
		stream.TryGetBuffer(out var segment);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)segment);
	}
}
