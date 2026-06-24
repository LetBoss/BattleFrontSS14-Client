// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetMessageExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.IO;
using System.Numerics;

#nullable enable
namespace Robust.Shared.Network;

public static class NetMessageExt
{
  public static NetCoordinates ReadNetCoordinates(this NetIncomingMessage message)
  {
    return new NetCoordinates(message.ReadNetEntity(), message.ReadVector2());
  }

  public static void Write(this NetOutgoingMessage message, NetCoordinates coordinates)
  {
    message.Write(coordinates.NetEntity);
    message.Write(coordinates.Position);
  }

  public static Vector2 ReadVector2(this NetIncomingMessage message)
  {
    return new Vector2(((NetBuffer) message).ReadFloat(), ((NetBuffer) message).ReadFloat());
  }

  public static void Write(this NetOutgoingMessage message, Vector2 vector2)
  {
    ((NetBuffer) message).Write(vector2.X);
    ((NetBuffer) message).Write(vector2.Y);
  }

  public static NetEntity ReadNetEntity(this NetIncomingMessage message)
  {
    return new NetEntity(((NetBuffer) message).ReadInt32());
  }

  public static void Write(this NetOutgoingMessage message, NetEntity entity)
  {
    ((NetBuffer) message).Write((int) entity);
  }

  public static GameTick ReadGameTick(this NetIncomingMessage message)
  {
    return new GameTick(((NetBuffer) message).ReadUInt32());
  }

  public static void Write(this NetOutgoingMessage message, GameTick tick)
  {
    ((NetBuffer) message).Write(tick.Value);
  }

  public static Guid ReadGuid(this NetIncomingMessage message)
  {
    Span<byte> b = stackalloc byte[16 /*0x10*/];
    ((NetBuffer) message).ReadBytes(b);
    return new Guid((ReadOnlySpan<byte>) b);
  }

  public static void Write(this NetOutgoingMessage message, Guid guid)
  {
    Span<byte> destination = stackalloc byte[16 /*0x10*/];
    guid.TryWriteBytes(destination);
    ((NetBuffer) message).Write((ReadOnlySpan<byte>) destination);
  }

  public static Color ReadColor(this NetIncomingMessage message)
  {
    int num1 = (int) ((NetBuffer) message).ReadByte();
    byte num2 = ((NetBuffer) message).ReadByte();
    byte num3 = ((NetBuffer) message).ReadByte();
    byte num4 = ((NetBuffer) message).ReadByte();
    int num5 = (int) num2;
    int num6 = (int) num3;
    int num7 = (int) num4;
    return new Color((byte) num1, (byte) num5, (byte) num6, (byte) num7);
  }

  public static void Write(this NetOutgoingMessage message, Color color)
  {
    ((NetBuffer) message).Write(((Color) ref color).RByte);
    ((NetBuffer) message).Write(((Color) ref color).GByte);
    ((NetBuffer) message).Write(((Color) ref color).BByte);
    ((NetBuffer) message).Write(((Color) ref color).AByte);
  }

  public static void ReadAlignedMemory(
    this NetIncomingMessage message,
    MemoryStream memStream,
    int length)
  {
    if ((((NetBuffer) message).Position & 7L) != 0L)
      throw new ArgumentException("Read position in message must be byte-aligned", nameof (message));
    memStream.Write(((NetBuffer) message).Data, ((NetBuffer) message).PositionInBytes, length);
    memStream.Position = 0L;
    NetIncomingMessage netIncomingMessage = message;
    ((NetBuffer) netIncomingMessage).Position = ((NetBuffer) netIncomingMessage).Position + (long) (length * 8);
  }

  public static TimeSpan ReadTimeSpan(this NetIncomingMessage message)
  {
    return TimeSpan.FromTicks(((NetBuffer) message).ReadInt64());
  }

  public static void Write(this NetOutgoingMessage message, TimeSpan timeSpan)
  {
    ((NetBuffer) message).Write(timeSpan.Ticks);
  }
}
