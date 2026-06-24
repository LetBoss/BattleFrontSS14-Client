using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Eui;

public sealed class MsgEuiState : NetMessage
{
	public uint Id;

	public EuiStateBase State;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer ser)
	{
		Id = ((NetBuffer)buffer).ReadUInt32();
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		using MemoryStream stream = new MemoryStream(length);
		NetMessageExt.ReadAlignedMemory(buffer, stream, length);
		State = ser.Deserialize<EuiStateBase>((Stream)stream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer ser)
	{
		((NetBuffer)buffer).Write(Id);
		MemoryStream stream = new MemoryStream();
		ser.Serialize((Stream)stream, (object)State);
		int length = (int)stream.Length;
		((NetBuffer)buffer).WriteVariableInt32(length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)stream.GetBuffer().AsSpan(0, length));
	}
}
