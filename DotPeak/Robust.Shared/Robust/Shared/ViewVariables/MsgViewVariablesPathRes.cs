// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.MsgViewVariablesPathRes
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPathRes : MsgViewVariablesPath
{
  public string[] Response { get; set; } = Array.Empty<string>();

  public ViewVariablesResponseCode ResponseCode { get; set; } = ViewVariablesResponseCode.Ok;

  internal MsgViewVariablesPathRes()
  {
  }

  internal MsgViewVariablesPathRes(MsgViewVariablesPathReq req)
  {
    this.Path = req.Path;
    this.RequestId = req.RequestId;
  }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    base.ReadFromBuffer(buffer, serializer);
    this.ResponseCode = (ViewVariablesResponseCode) ((NetBuffer) buffer).ReadUInt16();
    int length = ((NetBuffer) buffer).ReadInt32();
    this.Response = new string[length];
    for (int index = 0; index < length; ++index)
      this.Response[index] = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    base.WriteToBuffer(buffer, serializer);
    ((NetBuffer) buffer).Write((ushort) this.ResponseCode);
    ((NetBuffer) buffer).Write(this.Response.Length);
    foreach (string str in this.Response)
      ((NetBuffer) buffer).Write(str);
  }
}
