using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.LinkAccount;

public sealed class RMCChangeMarineShoutoutMsg : NetMessage
{
	public string? Name;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		if (((NetBuffer)buffer).PeekStringSize() <= 10000)
		{
			Name = ((NetBuffer)buffer).ReadString();
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Name);
	}
}
