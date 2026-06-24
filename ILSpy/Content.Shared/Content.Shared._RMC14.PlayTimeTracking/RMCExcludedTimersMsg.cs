using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.PlayTimeTracking;

public sealed class RMCExcludedTimersMsg : NetMessage
{
	public HashSet<string> Trackers;

	public override MsgGroups MsgGroup => (MsgGroups)1;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int count = ((NetBuffer)buffer).ReadVariableInt32();
		Trackers = new HashSet<string>(count);
		for (int i = 0; i < count; i++)
		{
			Trackers.Add(((NetBuffer)buffer).ReadString());
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Trackers.Count);
		foreach (string tracker in Trackers)
		{
			((NetBuffer)buffer).Write(tracker);
		}
	}
}
