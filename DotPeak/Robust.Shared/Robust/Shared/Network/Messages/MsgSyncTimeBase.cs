// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Messages.MsgSyncTimeBase
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.Network.Messages;

internal sealed class MsgSyncTimeBase : NetMessage
{
  public GameTick Tick;
  public TimeSpan Time;

  public override MsgGroups MsgGroup => MsgGroups.String;

  public override void ReadFromBuffer(NetIncomingMessage buffer, IRobustSerializer serializer)
  {
    this.Tick = buffer.ReadGameTick();
    this.Time = buffer.ReadTimeSpan();
  }

  public override void WriteToBuffer(NetOutgoingMessage buffer, IRobustSerializer serializer)
  {
    buffer.Write(this.Tick);
    buffer.Write(this.Time);
  }
}
