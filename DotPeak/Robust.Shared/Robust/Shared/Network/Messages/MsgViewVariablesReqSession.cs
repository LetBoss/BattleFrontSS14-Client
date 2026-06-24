// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgViewVariablesReqSession
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;
using System;
using System.IO;

#nullable disable
namespace Robust.Shared.Network.Messages;

public sealed class MsgViewVariablesReqSession : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Command;

  public uint RequestId { get; set; }

  public ViewVariablesObjectSelector Selector { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.RequestId = ((NetBuffer) buffer).ReadUInt32();
    int length = ((NetBuffer) buffer).ReadInt32();
    using (MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length))
    {
      buffer.ReadAlignedMemory(memoryStream, length);
      this.Selector = serializer.Deserialize<ViewVariablesObjectSelector>((Stream) memoryStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).Write(this.RequestId);
    MemoryStream ms = new MemoryStream();
    serializer.Serialize((Stream) ms, (object) this.Selector);
    ((NetBuffer) buffer).Write((int) ms.Length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) ms.AsSpan());
  }
}
