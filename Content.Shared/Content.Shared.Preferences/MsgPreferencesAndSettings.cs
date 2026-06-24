using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Preferences;

public sealed class MsgPreferencesAndSettings : NetMessage
{
	public PlayerPreferences Preferences;

	public GameSettings Settings;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		int length = ((NetBuffer)buffer).ReadVariableInt32();
		using (MemoryStream stream = new MemoryStream())
		{
			NetMessageExt.ReadAlignedMemory(buffer, stream, length);
			serializer.DeserializeDirect<PlayerPreferences>((Stream)stream, ref Preferences);
		}
		length = ((NetBuffer)buffer).ReadVariableInt32();
		using MemoryStream stream2 = new MemoryStream();
		NetMessageExt.ReadAlignedMemory(buffer, stream2, length);
		serializer.DeserializeDirect<GameSettings>((Stream)stream2, ref Settings);
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		using (MemoryStream stream = new MemoryStream())
		{
			serializer.SerializeDirect<PlayerPreferences>((Stream)stream, Preferences);
			((NetBuffer)buffer).WriteVariableInt32((int)stream.Length);
			stream.TryGetBuffer(out var segment);
			((NetBuffer)buffer).Write((ReadOnlySpan<byte>)segment);
		}
		using MemoryStream stream2 = new MemoryStream();
		serializer.SerializeDirect<GameSettings>((Stream)stream2, Settings);
		((NetBuffer)buffer).WriteVariableInt32((int)stream2.Length);
		stream2.TryGetBuffer(out var segment2);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)segment2);
	}
}
