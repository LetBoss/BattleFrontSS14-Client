using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Players.JobWhitelist;

public sealed class MsgJobWhitelist : NetMessage
{
	public HashSet<string> Whitelist = new HashSet<string>();

	public override MsgGroups MsgGroup => (MsgGroups)5;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int count = ((NetBuffer)buffer).ReadVariableInt32();
		Whitelist.EnsureCapacity(count);
		for (int i = 0; i < count; i++)
		{
			Whitelist.Add(((NetBuffer)buffer).ReadString());
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Whitelist.Count);
		foreach (string ban in Whitelist)
		{
			((NetBuffer)buffer).Write(ban);
		}
	}
}
