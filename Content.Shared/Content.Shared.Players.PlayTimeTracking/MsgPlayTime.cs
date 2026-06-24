using System;
using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Players.PlayTimeTracking;

public sealed class MsgPlayTime : NetMessage
{
	public Dictionary<string, TimeSpan> Trackers = new Dictionary<string, TimeSpan>();

	public override MsgGroups MsgGroup => (MsgGroups)5;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int count = ((NetBuffer)buffer).ReadVariableInt32();
		Trackers.EnsureCapacity(count);
		for (int i = 0; i < count; i++)
		{
			Trackers.Add(((NetBuffer)buffer).ReadString(), NetMessageExt.ReadTimeSpan(buffer));
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Trackers.Count);
		foreach (var (role, time) in Trackers)
		{
			((NetBuffer)buffer).Write(role);
			NetMessageExt.Write(buffer, time);
		}
	}
}
