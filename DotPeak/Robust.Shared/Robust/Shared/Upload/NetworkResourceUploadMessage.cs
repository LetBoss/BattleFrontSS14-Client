// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Upload.NetworkResourceUploadMessage
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Upload;

[Obsolete("The engine no longer uses this message")]
public sealed class NetworkResourceUploadMessage : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.String;

  public byte[] Data { get; set; } = Array.Empty<byte>();

  public ResPath RelativePath { get; set; } = ResPath.Self;

  public NetworkResourceUploadMessage()
  {
  }

  public NetworkResourceUploadMessage(byte[] data, ResPath relativePath)
  {
    this.Data = data;
    this.RelativePath = relativePath;
  }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int num = ((NetBuffer) buffer).ReadVariableInt32();
    this.Data = ((NetBuffer) buffer).ReadBytes(num);
    this.RelativePath = new ResPath(((NetBuffer) buffer).ReadString());
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    ((NetBuffer) buffer).WriteVariableInt32(this.Data.Length);
    ((NetBuffer) buffer).Write(this.Data);
    ((NetBuffer) buffer).Write(this.RelativePath.ToString());
    ((NetBuffer) buffer).Write((ushort) 47);
  }
}
