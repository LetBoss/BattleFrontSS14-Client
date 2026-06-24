using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.LinkAccount;

public sealed class LinkAccountCodeMsg : NetMessage
{
	public Guid Code;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Code = NetMessageExt.ReadGuid(buffer);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		NetMessageExt.Write(buffer, Code);
	}
}
