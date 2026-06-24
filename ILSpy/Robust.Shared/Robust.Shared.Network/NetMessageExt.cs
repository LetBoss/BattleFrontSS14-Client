using System;
using System.IO;
using System.Numerics;
using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;

namespace Robust.Shared.Network;

public static class NetMessageExt
{
	public static NetCoordinates ReadNetCoordinates(this NetIncomingMessage message)
	{
		NetEntity netEntity = message.ReadNetEntity();
		Vector2 position = message.ReadVector2();
		return new NetCoordinates(netEntity, position);
	}

	public static void Write(this NetOutgoingMessage message, NetCoordinates coordinates)
	{
		message.Write(coordinates.NetEntity);
		message.Write(coordinates.Position);
	}

	public static Vector2 ReadVector2(this NetIncomingMessage message)
	{
		float x = ((NetBuffer)message).ReadFloat();
		float y = ((NetBuffer)message).ReadFloat();
		return new Vector2(x, y);
	}

	public static void Write(this NetOutgoingMessage message, Vector2 vector2)
	{
		((NetBuffer)message).Write(vector2.X);
		((NetBuffer)message).Write(vector2.Y);
	}

	public static NetEntity ReadNetEntity(this NetIncomingMessage message)
	{
		return new NetEntity(((NetBuffer)message).ReadInt32());
	}

	public static void Write(this NetOutgoingMessage message, NetEntity entity)
	{
		((NetBuffer)message).Write((int)entity);
	}

	public static GameTick ReadGameTick(this NetIncomingMessage message)
	{
		return new GameTick(((NetBuffer)message).ReadUInt32());
	}

	public static void Write(this NetOutgoingMessage message, GameTick tick)
	{
		((NetBuffer)message).Write(tick.Value);
	}

	public static Guid ReadGuid(this NetIncomingMessage message)
	{
		Span<byte> span = stackalloc byte[16];
		((NetBuffer)message).ReadBytes(span);
		return new Guid(span);
	}

	public static void Write(this NetOutgoingMessage message, Guid guid)
	{
		Span<byte> span = stackalloc byte[16];
		guid.TryWriteBytes(span);
		((NetBuffer)message).Write((ReadOnlySpan<byte>)span);
	}

	public static Color ReadColor(this NetIncomingMessage message)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		byte num = ((NetBuffer)message).ReadByte();
		byte b = ((NetBuffer)message).ReadByte();
		byte b2 = ((NetBuffer)message).ReadByte();
		byte b3 = ((NetBuffer)message).ReadByte();
		return new Color(num, b, b2, b3);
	}

	public static void Write(this NetOutgoingMessage message, Color color)
	{
		((NetBuffer)message).Write(((Color)(ref color)).RByte);
		((NetBuffer)message).Write(((Color)(ref color)).GByte);
		((NetBuffer)message).Write(((Color)(ref color)).BByte);
		((NetBuffer)message).Write(((Color)(ref color)).AByte);
	}

	public static void ReadAlignedMemory(this NetIncomingMessage message, MemoryStream memStream, int length)
	{
		if ((((NetBuffer)message).Position & 7) != 0L)
		{
			throw new ArgumentException("Read position in message must be byte-aligned", "message");
		}
		memStream.Write(((NetBuffer)message).Data, ((NetBuffer)message).PositionInBytes, length);
		memStream.Position = 0L;
		((NetBuffer)message).Position = ((NetBuffer)message).Position + length * 8;
	}

	public static TimeSpan ReadTimeSpan(this NetIncomingMessage message)
	{
		return TimeSpan.FromTicks(((NetBuffer)message).ReadInt64());
	}

	public static void Write(this NetOutgoingMessage message, TimeSpan timeSpan)
	{
		((NetBuffer)message).Write(timeSpan.Ticks);
	}
}
