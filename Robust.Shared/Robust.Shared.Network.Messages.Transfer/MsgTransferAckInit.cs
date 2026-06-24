using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Transfer;

internal sealed class MsgTransferAckInit : NetMessage
{
	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
	}
}
