// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eui.MsgEuiState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;
using System.IO;

#nullable enable
namespace Content.Shared.Eui;

public sealed class MsgEuiState : NetMessage
{
  public uint Id;
  public EuiStateBase State;

  public override MsgGroups MsgGroup => MsgGroups.Command;

  public override NetDeliveryMethod DeliveryMethod => (NetDeliveryMethod) 67;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer ser)
  {
    this.Id = ((NetBuffer) buffer).ReadUInt32();
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    using (MemoryStream memStream = new MemoryStream(num))
    {
      buffer.ReadAlignedMemory(memStream, num);
      this.State = ser.Deserialize<EuiStateBase>((Stream) memStream);
    }
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer ser)
  {
    ((NetBuffer) buffer).Write(this.Id);
    MemoryStream memoryStream = new MemoryStream();
    ser.Serialize((Stream) memoryStream, (object) this.State);
    int length = (int) memoryStream.Length;
    ((NetBuffer) buffer).WriteVariableInt32(length);
    ((NetBuffer) buffer).Write((ReadOnlySpan<byte>) memoryStream.GetBuffer().AsSpan<byte>(0, length));
  }
}
