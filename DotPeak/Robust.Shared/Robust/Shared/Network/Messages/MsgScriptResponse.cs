// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgScriptResponse
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.IO;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgScriptResponse : NetMessage
{
  public FormattedMessage Echo;
  public FormattedMessage Response;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public int ScriptSession { get; set; }

  public bool WasComplete { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.ScriptSession = ((NetBuffer) buffer).ReadInt32();
    this.WasComplete = ((NetBuffer) buffer).ReadBoolean();
    if (!this.WasComplete)
      return;
    ((NetBuffer) buffer).ReadPadBits();
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length))
    {
      buffer.ReadAlignedMemory(memoryStream, length);
      serializer.DeserializeDirect<FormattedMessage>((Stream) memoryStream, out this.Echo);
      serializer.DeserializeDirect<FormattedMessage>((Stream) memoryStream, out this.Response);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.ScriptSession);
    ((NetBuffer) buffer).Write(this.WasComplete);
    if (!this.WasComplete)
      return;
    ((NetBuffer) buffer).WritePadBits();
    MemoryStream ms = new MemoryStream();
    serializer.SerializeDirect<FormattedMessage>((Stream) ms, this.Echo);
    serializer.SerializeDirect<FormattedMessage>((Stream) ms, this.Response);
    ((NetBuffer) buffer).WriteVariableInt32((int) ms.Length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) ms.AsSpan());
  }
}
