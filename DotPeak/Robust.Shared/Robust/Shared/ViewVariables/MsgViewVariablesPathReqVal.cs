// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.MsgViewVariablesPathReqVal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPathReqVal : MsgViewVariablesPathReq
{
  public string? Value { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    base.ReadFromBuffer(buffer, serializer);
    this.Value = ((NetBuffer) buffer).ReadString();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    base.WriteToBuffer(buffer, serializer);
    ((NetBuffer) buffer).Write(this.Value);
  }
}
