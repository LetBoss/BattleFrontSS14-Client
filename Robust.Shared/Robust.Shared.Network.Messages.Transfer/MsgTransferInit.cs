using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Transfer;

internal sealed class MsgTransferInit : NetMessage
{
	public (string EndpointUrl, byte[] Key)? HttpInfo;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		if (!((NetBuffer)buffer).ReadBoolean())
		{
			HttpInfo = null;
			return;
		}
		((NetBuffer)buffer).SkipPadBits();
		string item = ((NetBuffer)buffer).ReadString();
		byte[] item2 = ((NetBuffer)buffer).ReadBytes(32);
		HttpInfo = (item, item2);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		(string, byte[])? httpInfo = HttpInfo;
		if (!httpInfo.HasValue)
		{
			((NetBuffer)buffer).Write(false);
			return;
		}
		((NetBuffer)buffer).Write(true);
		((NetBuffer)buffer).WritePadBits();
		var (text, array) = HttpInfo.Value;
		((NetBuffer)buffer).Write(text);
		((NetBuffer)buffer).Write(array);
	}
}
