using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Eui;

public sealed class MsgEuiCtl : NetMessage
{
	public enum CtlType : byte
	{
		Open,
		Close
	}

	public CtlType Type;

	public string OpenType = string.Empty;

	public uint Id;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)67;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Id = ((NetBuffer)buffer).ReadUInt32();
		Type = (CtlType)((NetBuffer)buffer).ReadByte();
		if (Type == CtlType.Open)
		{
			OpenType = ((NetBuffer)buffer).ReadString();
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Id);
		((NetBuffer)buffer).Write((byte)Type);
		if (Type == CtlType.Open)
		{
			((NetBuffer)buffer).Write(OpenType);
		}
	}
}
