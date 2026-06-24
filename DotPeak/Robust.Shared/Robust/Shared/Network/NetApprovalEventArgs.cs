// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.NetApprovalEventArgs
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using System;

#nullable enable
namespace Robust.Shared.Network;

public sealed class NetApprovalEventArgs : EventArgs
{
  public NetConnection Connection { get; }

  public NetApprovalEventArgs(NetConnection connection) => this.Connection = connection;
}
