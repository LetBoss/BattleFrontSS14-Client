// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgConCmdAck
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

public sealed class MsgConCmdAck : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public FormattedMessage Text { get; set; }

  public bool Error { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length))
    {
      buffer.ReadAlignedMemory(memoryStream, length);
      this.Text = serializer.Deserialize<FormattedMessage>((Stream) memoryStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    MemoryStream ms = new MemoryStream();
    serializer.Serialize((Stream) ms, (object) this.Text);
    ((NetBuffer) buffer).WriteVariableInt32((int) ms.Length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) ms.AsSpan());
  }
}
