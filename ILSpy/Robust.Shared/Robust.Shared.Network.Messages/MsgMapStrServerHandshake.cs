using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

internal sealed class MsgMapStrServerHandshake : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.String;

	public byte[]? Hash { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		if (num > 64)
		{
			throw new InvalidOperationException("Hash too long.");
		}
		byte[] array = (Hash = new byte[num]);
		((NetBuffer)buffer).ReadBytes((Span<byte>)array);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		if (Hash == null)
		{
			throw new InvalidOperationException("Package has not been specified.");
		}
		((NetBuffer)buffer).WriteVariableInt32(Hash.Length);
		((NetBuffer)buffer).Write(Hash);
	}
}
