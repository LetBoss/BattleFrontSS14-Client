using System;
using System.Buffers;
using System.IO;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Messages;

public sealed class MsgState : NetMessage
{
	public const int ReliableThreshold = 488;

	public const int CompressionThreshold = 256;

	public GameState State;

	public MemoryStream StateStream;

	public ZStdCompressionContext CompressionContext;

	internal bool HasWritten;

	internal bool ForceSendReliably;

	public override MsgGroups MsgGroup => MsgGroups.Entity;

	public override NetDeliveryMethod DeliveryMethod
	{
		get
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (!ShouldSendReliably())
			{
				return base.DeliveryMethod;
			}
			return (NetDeliveryMethod)34;
		}
	}

	public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
	{
		base.MsgSize = ((NetBuffer)buffer).LengthBytes;
		int num = ((NetBuffer)buffer).ReadVariableInt32();
		int num2 = ((NetBuffer)buffer).ReadVariableInt32();
		MemoryStream memoryStream2;
		if (num2 > 0)
		{
			MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(num2);
			buffer.ReadAlignedMemory(memoryStream, num2);
			using ZStdDecompressStream zStdDecompressStream = new ZStdDecompressStream(memoryStream);
			memoryStream2 = RobustMemoryManager.GetMemoryStream(num);
			memoryStream2.SetLength(num);
			zStdDecompressStream.CopyTo(memoryStream2, num);
			memoryStream2.Position = 0L;
		}
		else
		{
			memoryStream2 = RobustMemoryManager.GetMemoryStream(num);
			buffer.ReadAlignedMemory(memoryStream2, num);
		}
		try
		{
			serializer.DeserializeDirect<GameState>(memoryStream2, out State);
		}
		finally
		{
			memoryStream2.Dispose();
		}
		State.PayloadSize = num;
	}

	public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
	{
		((NetBuffer)buffer).WriteVariableInt32((int)StateStream.Length);
		if (StateStream.Length > 256)
		{
			StateStream.Position = 0L;
			byte[] array = ArrayPool<byte>.Shared.Rent(ZStd.CompressBound((int)StateStream.Length));
			int num = CompressionContext.Compress2(array, StateStream.AsSpan());
			((NetBuffer)buffer).WriteVariableInt32(num);
			((NetBuffer)buffer).Write((ReadOnlySpan<byte>)array.AsSpan(0, num));
			ArrayPool<byte>.Shared.Return(array);
		}
		else
		{
			((NetBuffer)buffer).WriteVariableInt32(0);
			((NetBuffer)buffer).Write((ReadOnlySpan<byte>)StateStream.AsSpan());
		}
		HasWritten = true;
		base.MsgSize = ((NetBuffer)buffer).LengthBytes;
	}

	public bool ShouldSendReliably()
	{
		if (!ForceSendReliably)
		{
			return base.MsgSize > 488;
		}
		return true;
	}
}
