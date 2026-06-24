using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesCloseSession : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint SessionId { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		SessionId = ((NetBuffer)buffer).ReadUInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(SessionId);
	}
}
