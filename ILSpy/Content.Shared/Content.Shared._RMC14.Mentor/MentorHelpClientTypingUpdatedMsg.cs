using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorHelpClientTypingUpdatedMsg : NetMessage
{
	public Guid Destination;

	public bool Typing;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Destination = NetMessageExt.ReadGuid(buffer);
		Typing = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		NetMessageExt.Write(buffer, Destination);
		((NetBuffer)buffer).Write(Typing);
	}
}
