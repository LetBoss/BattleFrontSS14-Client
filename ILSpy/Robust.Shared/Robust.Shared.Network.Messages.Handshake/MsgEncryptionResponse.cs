using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgEncryptionResponse : NetMessage
{
	public Guid UserId;

	public byte[] SealedData;

	public byte[] LegacyHwid;

	public override string MsgName => string.Empty;

	public override MsgGroups MsgGroup => MsgGroups.Core;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		UserId = buffer.ReadGuid();
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		SealedData = ((NetBuffer)buffer).ReadBytes(num);
		int num2 = ((NetBuffer)buffer).ReadVariableInt32();
		LegacyHwid = ((NetBuffer)buffer).ReadBytes(num2);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		buffer.Write(UserId);
		((NetBuffer)buffer).WriteVariableInt32(SealedData.Length);
		((NetBuffer)buffer).Write(SealedData);
		((NetBuffer)buffer).WriteVariableInt32(LegacyHwid.Length);
		((NetBuffer)buffer).Write(LegacyHwid);
	}
}
