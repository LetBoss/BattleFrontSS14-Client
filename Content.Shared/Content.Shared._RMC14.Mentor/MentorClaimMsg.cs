using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorClaimMsg : NetMessage
{
	public string Author = string.Empty;

	public Guid Destination;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Author = ((NetBuffer)buffer).ReadString();
		Destination = NetMessageExt.ReadGuid(buffer);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Author);
		NetMessageExt.Write(buffer, Destination);
	}
}
