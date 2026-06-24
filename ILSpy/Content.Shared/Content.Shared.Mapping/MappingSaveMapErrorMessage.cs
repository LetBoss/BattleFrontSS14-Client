using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Mapping;

public sealed class MappingSaveMapErrorMessage : NetMessage
{
	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)34;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
	}
}
