using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Upload;

internal sealed class NetworkResourceAckMessage : NetMessage
{
	public int Key;

	public override MsgGroups MsgGroup => MsgGroups.String;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Key = ((NetBuffer)buffer).ReadInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Key);
	}
}
