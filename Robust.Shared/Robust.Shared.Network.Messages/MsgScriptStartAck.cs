using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptStartAck : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public bool WasAccepted { get; set; }

	public int ScriptSession { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		WasAccepted = ((NetBuffer)buffer).ReadBoolean();
		ScriptSession = ((NetBuffer)buffer).ReadInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(WasAccepted);
		((NetBuffer)buffer).Write(ScriptSession);
	}
}
