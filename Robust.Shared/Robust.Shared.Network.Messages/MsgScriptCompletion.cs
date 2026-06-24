using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptCompletion : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public int ScriptSession { get; set; }

	public int Cursor { get; set; }

	public string Code { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		ScriptSession = ((NetBuffer)buffer).ReadInt32();
		Cursor = ((NetBuffer)buffer).ReadInt32();
		Code = ((NetBuffer)buffer).ReadString();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(ScriptSession);
		((NetBuffer)buffer).Write(Cursor);
		((NetBuffer)buffer).Write(Code);
	}
}
