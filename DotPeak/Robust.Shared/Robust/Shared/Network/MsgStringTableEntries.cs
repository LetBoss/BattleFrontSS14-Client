// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.MsgStringTableEntries
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Network;

public sealed class MsgStringTableEntries : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public MsgStringTableEntries.Entry[] Entries { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    uint length = ((NetBuffer) buffer).ReadUInt32();
    this.Entries = new MsgStringTableEntries.Entry[(int) length];
    for (int index = 0; (long) index < (long) length; ++index)
    {
      this.Entries[index].Id = ((NetBuffer) buffer).ReadVariableInt32();
      this.Entries[index].String = ((NetBuffer) buffer).ReadString();
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (this.Entries == null)
      throw new InvalidOperationException("Entries is null!");
    ((NetBuffer) buffer).Write(this.Entries.Length);
    foreach (MsgStringTableEntries.Entry entry in this.Entries)
    {
      ((NetBuffer) buffer).WriteVariableInt32(entry.Id);
      ((NetBuffer) buffer).Write(entry.String);
    }
  }

  public struct Entry
  {
    public string String { get; set; }

    public int Id { get; set; }
  }
}
