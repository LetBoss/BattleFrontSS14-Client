// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.TransferReceivedEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.IO;

#nullable enable
namespace Robust.Shared.Network.Transfer;

public sealed class TransferReceivedEvent
{
  public readonly string Key;
  public readonly Stream DataStream;
  public readonly INetChannel Channel;

  internal TransferReceivedEvent(string key, INetChannel channel, Stream stream)
  {
    this.Key = key;
    this.DataStream = stream;
    this.Channel = channel;
  }
}
