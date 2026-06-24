using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences;

public sealed class MsgUpdateCharacter : NetMessage
{
	public int Slot;

	public ICharacterProfile Profile;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		Slot = ((NetBuffer)buffer).ReadInt32();
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		using MemoryStream stream = new MemoryStream(length);
		NetMessageExt.ReadAlignedMemory(buffer, stream, length);
		Profile = serializer.Deserialize<ICharacterProfile>((Stream)stream);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).Write(Slot);
		using MemoryStream stream = new MemoryStream();
		serializer.Serialize((Stream)stream, (object)Profile);
		((NetBuffer)buffer).WriteVariableInt32((int)stream.Length);
		stream.TryGetBuffer(out var segment);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)segment);
	}
}
