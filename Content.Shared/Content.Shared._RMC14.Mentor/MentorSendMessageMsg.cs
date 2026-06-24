using System;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Mentor;

public sealed class MentorSendMessageMsg : NetMessage
{
	public string Message = string.Empty;

	public Guid To;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Message = ((NetBuffer)buffer).ReadString();
		To = NetMessageExt.ReadGuid(buffer);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Message);
		NetMessageExt.Write(buffer, To);
	}
}
