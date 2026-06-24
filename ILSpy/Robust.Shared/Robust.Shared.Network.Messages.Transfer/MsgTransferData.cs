using System;
using System.Buffers;
using Lidgren.Network;
using Robust.Shared.Serialization;

namespace Robust.Shared.Network.Messages.Transfer;

internal sealed class MsgTransferData : NetMessage
{
	internal const NetDeliveryMethod Method = (NetDeliveryMethod)67;

	internal const int Channel = 16;

	public ArraySegment<byte> Data;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override int SequenceChannel => 16;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		if (num > 16384)
		{
			throw new Exception("Buffer size is too large");
		}
		byte[] array = ArrayPool<byte>.Shared.Rent(num);
		((NetBuffer)buffer).ReadBytes(array, 0, num);
		Data = new ArraySegment<byte>(array, 0, num);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(Data.Count);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)Data.AsSpan());
	}
}
