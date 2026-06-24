using System;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Handshake;

internal sealed class MsgLoginSuccess : NetMessage
{
	public NetUserData UserData;

	public LoginType Type;

	public override string MsgName => string.Empty;

	public override MsgGroups MsgGroup => MsgGroups.Core;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		string userName = ((NetBuffer)buffer).ReadString();
		Guid userId = buffer.ReadGuid();
		string text = ((NetBuffer)buffer).ReadString();
		if (text.Length == 0)
		{
			text = null;
		}
		UserData = new NetUserData(new NetUserId(userId), userName)
		{
			PatronTier = text
		};
		Type = (LoginType)((NetBuffer)buffer).ReadByte();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(UserData.UserName);
		buffer.Write((Guid)UserData.UserId);
		((NetBuffer)buffer).Write(UserData.PatronTier);
		((NetBuffer)buffer).Write((byte)Type);
		((NetBuffer)buffer).Write(new byte[100]);
	}
}
