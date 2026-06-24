using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgEncryptionRequest : NetMessage
{
	public byte[] VerifyToken;

	public byte[] PublicKey;

	public bool WantHwid;

	public override string MsgName => string.Empty;

	public override MsgGroups MsgGroup => MsgGroups.Core;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		VerifyToken = ((NetBuffer)buffer).ReadBytes(num);
		int num2 = ((NetBuffer)buffer).ReadVariableInt32();
		PublicKey = ((NetBuffer)buffer).ReadBytes(num2);
		WantHwid = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(VerifyToken.Length);
		((NetBuffer)buffer).Write(VerifyToken);
		((NetBuffer)buffer).WriteVariableInt32(PublicKey.Length);
		((NetBuffer)buffer).Write(PublicKey);
		((NetBuffer)buffer).Write(WantHwid);
	}
}
