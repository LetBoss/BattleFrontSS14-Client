using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgLoginStart : NetMessage
{
	public string UserName;

	public bool CanAuth;

	public bool NeedPubKey;

	public bool Encrypt;

	public override string MsgName => string.Empty;

	public override MsgGroups MsgGroup => MsgGroups.Core;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		UserName = ((NetBuffer)buffer).ReadString();
		CanAuth = ((NetBuffer)buffer).ReadBoolean();
		NeedPubKey = ((NetBuffer)buffer).ReadBoolean();
		Encrypt = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(UserName);
		((NetBuffer)buffer).Write(CanAuth);
		((NetBuffer)buffer).Write(NeedPubKey);
		((NetBuffer)buffer).Write(Encrypt);
	}
}
