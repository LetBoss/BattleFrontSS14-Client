using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

internal sealed class MsgMapStrClientHandshake : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.String;

	public bool NeedsStrings { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		NeedsStrings = ((NetBuffer)buffer).ReadBoolean();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(NeedsStrings);
	}
}
