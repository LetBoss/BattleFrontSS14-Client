// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetChannelArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;

#nullable enable
namespace Robust.Shared.Network;

[Virtual]
public class NetChannelArgs : EventArgs
{
  public readonly INetChannel Channel;

  public NetChannelArgs(INetChannel channel) => this.Channel = channel;
}
