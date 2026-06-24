// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgMapStrStrings
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Robust.Shared.Network.Messages;

internal sealed class MsgMapStrStrings : NetMessage
{
  public override MsgGroups MsgGroup => MsgGroups.Core;

  public byte[]? Package { get; set; }

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    int length = ((NetBuffer) buffer).ReadVariableInt32();
    ((NetBuffer) buffer).ReadBytes((Span<byte>) (this.Package = new byte[length]));
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    if (this.Package == null)
      throw new InvalidOperationException("Package has not been specified.");
    ((NetBuffer) buffer).WriteVariableInt32(this.Package.Length);
    int lengthBytes = ((NetBuffer) buffer).LengthBytes;
    ((NetBuffer) buffer).Write(this.Package);
    if (((NetBuffer) buffer).LengthBytes - lengthBytes != this.Package.Length)
      throw new InvalidOperationException("Not all of the bytes were written to the message.");
  }
}
