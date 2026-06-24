// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.LinkAccount.LinkAccountStatusMsg
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.IO;

#nullable enable
namespace Content.Shared._RMC14.LinkAccount;

public sealed class LinkAccountStatusMsg : NetMessage
{
  public SharedRMCPatronFull? Patron;

  public override MsgGroups MsgGroup => MsgGroups.Core;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    if (!((NetBuffer) buffer).ReadBoolean())
      return;
    ((NetBuffer) buffer).ReadPadBits();
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memStream = new MemoryStream(num))
    {
      buffer.ReadAlignedMemory(memStream, num);
      this.Patron = serializer.Deserialize<SharedRMCPatronFull>((Stream) memStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (this.Patron == (SharedRMCPatronFull) null)
    {
      ((NetBuffer) buffer).Write(false);
    }
    else
    {
      ((NetBuffer) buffer).Write(true);
      ((NetBuffer) buffer).WritePadBits();
      using (MemoryStream memoryStream = new MemoryStream())
      {
        serializer.Serialize((Stream) memoryStream, (object) this.Patron);
        ((NetBuffer) buffer).WriteVariableInt32((int) memoryStream.Length);
        ArraySegment<byte> buffer1;
        memoryStream.TryGetBuffer(out buffer1);
        ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) buffer1);
      }
    }
  }
}
