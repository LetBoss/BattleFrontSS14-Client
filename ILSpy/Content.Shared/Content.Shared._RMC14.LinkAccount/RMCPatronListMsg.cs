using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.LinkAccount;

public sealed class RMCPatronListMsg : NetMessage
{
	public List<SharedRMCPatron> Patrons;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int count = ((NetBuffer)buffer).ReadVariableInt32();
		Patrons = new List<SharedRMCPatron>(count);
		for (int i = 0; i < count; i++)
		{
			string name = ((NetBuffer)buffer).ReadString();
			string tier = ((NetBuffer)buffer).ReadString();
			Patrons.Add(new SharedRMCPatron(name, tier));
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Patrons.Count);
		foreach (SharedRMCPatron patron in Patrons)
		{
			((NetBuffer)buffer).Write(patron.Name);
			((NetBuffer)buffer).Write(patron.Tier);
		}
	}
}
