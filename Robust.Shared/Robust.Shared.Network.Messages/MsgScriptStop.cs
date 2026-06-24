using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptStop : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public int ScriptSession { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		ScriptSession = ((NetBuffer)buffer).ReadInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(ScriptSession);
	}
}
