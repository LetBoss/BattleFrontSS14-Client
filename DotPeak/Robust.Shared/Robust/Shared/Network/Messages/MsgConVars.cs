// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgConVars
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Log;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Network.Messages;

public sealed class MsgConVars : NetMessage
{
  private const int MaxMessageSize = 32768 /*0x8000*/;
  private const int MaxNameSize = 128 /*0x80*/;
  private const int MaxStringValSize = 1024 /*0x0400*/;
  public GameTick Tick;
  public List<(string name, object value)> NetworkedVars;

  public override MsgGroups MsgGroup => MsgGroups.String;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    if (((NetBuffer) buffer).LengthBytes > 32768 /*0x8000*/)
      Logger.WarningS("net", $"{this.MsgChannel}: received a large {nameof (MsgConVars)}, {((NetBuffer) buffer).LengthBytes}B > {32768 /*0x8000*/}B");
    this.Tick = new GameTick(((NetBuffer) buffer).ReadVariableUInt32());
    short capacity = ((NetBuffer) buffer).ReadInt16();
    this.NetworkedVars = new List<(string, object)>((int) capacity);
    for (int index = 0; index < (int) capacity; ++index)
    {
      int num1 = ((NetBuffer) buffer).PeekStringSize();
      if (0 >= num1 || num1 > 128 /*0x80*/)
        throw new InvalidOperationException($"Cvar name size '{num1}' is out of bounds (1-{128 /*0x80*/} bytes).");
      string str = ((NetBuffer) buffer).ReadString();
      MsgConVars.CvarType actualValue = (MsgConVars.CvarType) ((NetBuffer) buffer).ReadByte();
      object obj;
      switch (actualValue)
      {
        case MsgConVars.CvarType.Int:
          obj = (object) ((NetBuffer) buffer).ReadInt32();
          break;
        case MsgConVars.CvarType.Long:
          obj = (object) ((NetBuffer) buffer).ReadInt64();
          break;
        case MsgConVars.CvarType.Bool:
          obj = (object) ((NetBuffer) buffer).ReadBoolean();
          break;
        case MsgConVars.CvarType.String:
          int num2 = ((NetBuffer) buffer).PeekStringSize();
          if (0 > num2 || num2 > 1024 /*0x0400*/)
            throw new InvalidOperationException($"Cvar string value size '{num1}' for cvar '{str}' is out of bounds (0-{1024 /*0x0400*/} bytes).");
          obj = (object) ((NetBuffer) buffer).ReadString();
          break;
        case MsgConVars.CvarType.Float:
          obj = (object) ((NetBuffer) buffer).ReadFloat();
          break;
        case MsgConVars.CvarType.Double:
          obj = (object) ((NetBuffer) buffer).ReadDouble();
          break;
        default:
          throw new ArgumentOutOfRangeException("value", (object) actualValue, $"CVar {str} is not of a valid CVar type!");
      }
      this.NetworkedVars.Add((str, obj));
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (this.NetworkedVars == null)
      throw new InvalidOperationException("NetworkedVars collection is null.");
    if (this.NetworkedVars.Count > (int) short.MaxValue)
      throw new InvalidOperationException($"{"NetworkedVars"} collection count is greater than {short.MaxValue}.");
    ((NetBuffer) buffer).WriteVariableUInt32(this.Tick.Value);
    ((NetBuffer) buffer).Write((short) this.NetworkedVars.Count);
    foreach ((string name, object value) in this.NetworkedVars)
    {
      ((NetBuffer) buffer).Write(name);
      switch (value)
      {
        case int num1:
          ((NetBuffer) buffer).Write((byte) 1);
          ((NetBuffer) buffer).Write(num1);
          continue;
        case long num2:
          ((NetBuffer) buffer).Write((byte) 2);
          ((NetBuffer) buffer).Write(num2);
          continue;
        case bool flag:
          ((NetBuffer) buffer).Write((byte) 3);
          ((NetBuffer) buffer).Write(flag);
          continue;
        case string str:
          ((NetBuffer) buffer).Write((byte) 4);
          ((NetBuffer) buffer).Write(str);
          continue;
        case float num3:
          ((NetBuffer) buffer).Write((byte) 5);
          ((NetBuffer) buffer).Write(num3);
          continue;
        case double num4:
          ((NetBuffer) buffer).Write((byte) 6);
          ((NetBuffer) buffer).Write(num4);
          continue;
        default:
          throw new ArgumentOutOfRangeException("value", (object) value.GetType(), $"CVar {name} is not of a valid CVar type!");
      }
    }
  }

  private enum CvarType : byte
  {
    None,
    Int,
    Long,
    Bool,
    String,
    Float,
    Double,
  }
}
