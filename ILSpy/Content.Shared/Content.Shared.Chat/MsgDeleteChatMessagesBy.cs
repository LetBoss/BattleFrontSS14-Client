using System.Collections.Generic;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Chat;

public sealed class MsgDeleteChatMessagesBy : NetMessage
{
	public int Key;

	public HashSet<NetEntity> Entities;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Key = ((NetBuffer)buffer).ReadInt32();
		int entities = ((NetBuffer)buffer).ReadInt32();
		Entities = new HashSet<NetEntity>(entities);
		for (int i = 0; i < entities; i++)
		{
			Entities.Add(NetMessageExt.ReadNetEntity(buffer));
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		((NetBuffer)buffer).Write(Key);
		((NetBuffer)buffer).Write(Entities.Count);
		foreach (NetEntity ent in Entities)
		{
			NetMessageExt.Write(buffer, ent);
		}
	}
}
