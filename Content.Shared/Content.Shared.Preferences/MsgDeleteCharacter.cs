using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences;

public sealed class MsgDeleteCharacter : NetMessage
{
	public int Slot;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Slot = ((NetBuffer)buffer).ReadInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Slot);
	}
}
