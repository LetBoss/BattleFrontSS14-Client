using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences;

public sealed class MsgSelectCharacter : NetMessage
{
	public int SelectedCharacterIndex;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		SelectedCharacterIndex = ((NetBuffer)buffer).ReadVariableInt32();
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32(SelectedCharacterIndex);
	}
}
