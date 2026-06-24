// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.MsgViewVariablesListPathReq
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;
using System.IO;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal sealed class MsgViewVariablesListPathReq : MsgViewVariablesPathReq
{
  public VVListPathOptions Options { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    base.ReadFromBuffer(buffer, serializer);
    int length = ((NetBuffer) buffer).ReadInt32();
    using (MemoryStream memoryStream = RobustMemoryManager.GetMemoryStream(length))
    {
      buffer.ReadAlignedMemory(memoryStream, length);
      this.Options = serializer.Deserialize<VVListPathOptions>((Stream) memoryStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    base.WriteToBuffer(buffer, serializer);
    MemoryStream ms = new MemoryStream();
    serializer.Serialize((Stream) ms, (object) this.Options);
    ((NetBuffer) buffer).Write((int) ms.Length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) ms.AsSpan());
  }
}
