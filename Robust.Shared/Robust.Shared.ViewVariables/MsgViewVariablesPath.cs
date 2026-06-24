using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPath : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Command;

	public uint RequestId { get; set; }

	public string Path { get; set; } = string.Empty;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		RequestId = ((NetBuffer)buffer).ReadUInt32();
		Path = ((NetBuffer)buffer).ReadString();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(RequestId);
		((NetBuffer)buffer).Write(Path);
	}
}
