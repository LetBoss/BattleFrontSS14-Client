using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Players;

public sealed class MsgRoleBans : NetMessage
{
	public List<string> Bans = new List<string>();

	public override MsgGroups MsgGroup => (MsgGroups)5;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int count = ((NetBuffer)buffer).ReadVariableInt32();
		Bans.EnsureCapacity(count);
		for (int i = 0; i < count; i++)
		{
			Bans.Add(((NetBuffer)buffer).ReadString());
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Bans.Count);
		foreach (string ban in Bans)
		{
			((NetBuffer)buffer).Write(ban);
		}
	}
}
