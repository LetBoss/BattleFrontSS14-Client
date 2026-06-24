using System;
using System.IO;
using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Mapping;

public sealed class MappingMapDataMessage : NetMessage
{
	public ZStdCompressionContext Context;

	public string Yml;

	public override MsgGroups MsgGroup => (MsgGroups)4;

	public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod)34;

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		((NetMessage)this).MsgSize = ((NetBuffer)buffer).LengthBytes;
		int uncompressedLength = ((NetBuffer)buffer).ReadVariableInt32();
		int compressedLength = ((NetBuffer)buffer).ReadVariableInt32();
		MemoryStream stream = new MemoryStream(compressedLength);
		NetMessageExt.ReadAlignedMemory(buffer, stream, compressedLength);
		ZStdDecompressStream decompress = new ZStdDecompressStream((Stream)stream, true);
		try
		{
			using MemoryStream decompressed = new MemoryStream(uncompressedLength);
			((Stream)(object)decompress).CopyTo((Stream)decompressed, uncompressedLength);
			decompressed.Position = 0L;
			serializer.DeserializeDirect<string>((Stream)decompressed, ref Yml);
		}
		finally
		{
			((IDisposable)decompress)?.Dispose();
		}
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		MemoryStream stream = new MemoryStream();
		serializer.SerializeDirect<string>((Stream)stream, Yml);
		((NetBuffer)buffer).WriteVariableInt32((int)stream.Length);
		stream.Position = 0L;
		byte[] buf = new byte[ZStd.CompressBound((int)stream.Length)];
		int length = Context.Compress2((Span<byte>)buf, (ReadOnlySpan<byte>)StreamExt.AsSpan(stream));
		((NetBuffer)buffer).WriteVariableInt32(length);
		((NetBuffer)buffer).Write((ReadOnlySpan<byte>)buf.AsSpan(0, length));
	}
}
