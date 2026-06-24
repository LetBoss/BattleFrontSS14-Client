// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgEntity
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.IO;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgEntity : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.EntityEvent;

  public EntityMessageType Type { get; set; }

  public EntityEventArgs SystemMessage { get; set; }

  public EntityUid EntityUid { get; set; }

  public uint NetId { get; set; }

  public uint Sequence { get; set; }

  public GameTick SourceTick { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Type = (EntityMessageType) ((NetBuffer) buffer).ReadByte();
    this.SourceTick = buffer.ReadGameTick();
    this.Sequence = ((NetBuffer) buffer).ReadUInt32();
    if (this.Type != EntityMessageType.SystemMessage)
      return;
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length))
    {
      buffer.ReadAlignedMemory(memoryStream, length);
      this.SystemMessage = serializer.Deserialize<EntityEventArgs>((Stream) memoryStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write((byte) this.Type);
    buffer.Write(this.SourceTick);
    ((NetBuffer) buffer).Write(this.Sequence);
    if (this.Type != EntityMessageType.SystemMessage)
      return;
    MemoryStream ms = new MemoryStream();
    serializer.Serialize((Stream) ms, (object) this.SystemMessage);
    ((NetBuffer) buffer).WriteVariableInt32((int) ms.Length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) ms.AsSpan());
  }

  public override string ToString()
  {
    string str = $"T: {this.SourceTick} S: {this.Sequence}";
    switch (this.Type)
    {
      case EntityMessageType.Error:
        return "MsgEntity Error";
      case EntityMessageType.SystemMessage:
        return $"MsgEntity Comp, {str}, {this.SystemMessage}";
      default:
        throw new ArgumentOutOfRangeException();
    }
  }
}
