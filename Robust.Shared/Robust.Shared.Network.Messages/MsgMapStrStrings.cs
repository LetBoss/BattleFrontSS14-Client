using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

internal sealed class MsgMapStrStrings : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Core;

	public byte[]? Package { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		byte[] array = (Package = new byte[num]);
		((NetBuffer)buffer).ReadBytes((Span<byte>)array);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		if (Package == null)
		{
			throw new InvalidOperationException("Package has not been specified.");
		}
		((NetBuffer)buffer).WriteVariableInt32(Package.Length);
		int lengthBytes = ((NetBuffer)buffer).LengthBytes;
		((NetBuffer)buffer).Write(Package);
		if (((NetBuffer)buffer).LengthBytes - lengthBytes != Package.Length)
		{
			throw new InvalidOperationException("Not all of the bytes were written to the message.");
		}
	}
}
