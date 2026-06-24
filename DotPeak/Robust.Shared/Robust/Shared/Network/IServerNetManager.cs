// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.IServerNetManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network;

[NotContentImplementable]
public interface IServerNetManager : INetManager
{
  byte[]? CryptoPublicKey { get; }

  AuthMode Auth { get; }

  Func<string, Task<NetUserId?>>? AssignUserIdCallback { get; set; }

  IServerNetManager.NetApprovalDelegate? HandleApprovalCallback { get; set; }

  void DisconnectChannel(INetChannel channel, string reason);

  delegate Task<NetApproval> NetApprovalDelegate(NetApprovalEventArgs eventArgs);
}
