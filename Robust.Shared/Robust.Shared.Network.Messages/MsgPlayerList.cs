using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.Enums;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages;

public sealed class MsgPlayerList : NetMessage
{
	public override MsgGroups MsgGroup => MsgGroups.Core;

	public List<SessionState> Plyrs { get; set; }

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadInt32();
		Plyrs = new List<SessionState>(num);
		for (int i = 0; i < num; i++)
		{
			SessionState item = new SessionState
			{
				UserId = new NetUserId(buffer.ReadGuid()),
				Name = ((NetBuffer)buffer).ReadString(),
				Status = (SessionStatus)((NetBuffer)buffer).ReadByte()
			};
			Plyrs.Add(item);
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Plyrs.Count);
		foreach (SessionState plyr in Plyrs)
		{
			buffer.Write(plyr.UserId.UserId);
			((NetBuffer)buffer).Write(plyr.Name);
			((NetBuffer)buffer).Write((byte)plyr.Status);
		}
	}
}
