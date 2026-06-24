using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Network.Messages;

public sealed class MsgStateLeavePvs : NetMessage
{
	public List<NetEntity> Entities;

	public GameTick Tick;

	public override MsgGroups MsgGroup => MsgGroups.Entity;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)34;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Tick = buffer.ReadGameTick();
		int num = ((NetBuffer)buffer).ReadInt32();
		Entities = new List<NetEntity>(num);
		for (int i = 0; i < num; i++)
		{
			Entities.Add(buffer.ReadNetEntity());
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		buffer.Write(Tick);
		((NetBuffer)buffer).Write(Entities.Count);
		foreach (NetEntity entity in Entities)
		{
			buffer.Write(entity);
		}
	}
}
