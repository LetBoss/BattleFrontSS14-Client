// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.MsgViewVariablesPathReq
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.ViewVariables;

internal abstract class MsgViewVariablesPathReq : MsgViewVariablesPath
{
  public Guid Session { get; set; } = Guid.Empty;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    base.ReadFromBuffer(buffer, serializer);
    this.Session = buffer.ReadGuid();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    base.WriteToBuffer(buffer, serializer);
    buffer.Write(this.Session);
  }
}
