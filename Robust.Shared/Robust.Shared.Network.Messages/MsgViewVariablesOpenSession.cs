using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesOpenSession : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint RequestId { get; set; }

	public uint SessionId { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		RequestId = ((NetBuffer)buffer).ReadUInt32();
		SessionId = ((NetBuffer)buffer).ReadUInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(RequestId);
		((NetBuffer)buffer).Write(SessionId);
	}
}
