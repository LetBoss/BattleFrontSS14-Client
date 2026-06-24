using Lidgren.Network;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesDenySession : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint RequestId { get; set; }

	public ViewVariablesResponseCode Reason { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		RequestId = ((NetBuffer)buffer).ReadUInt32();
		Reason = (ViewVariablesResponseCode)((NetBuffer)buffer).ReadUInt16();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(RequestId);
		((NetBuffer)buffer).Write((ushort)Reason);
	}
}
