using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Network.Messages;

public sealed class MsgStateRequestFull : NetMessage
{
	public GameTick Tick;

	public NetEntity MissingEntity;

	public override MsgGroups MsgGroup => MsgGroups.Entity;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)34;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Tick = buffer.ReadGameTick();
		MissingEntity = buffer.ReadNetEntity();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		buffer.Write(Tick);
		buffer.Write(MissingEntity);
	}
}
