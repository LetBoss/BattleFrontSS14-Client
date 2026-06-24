// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.ITransferManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.IO;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network.Transfer;

[NotContentImplementable]
public interface ITransferManager
{
  Stream StartTransfer(INetChannel channel, TransferStartInfo startInfo);

  void RegisterTransferMessage(
    string key,
    Action<TransferReceivedEvent>? rxCallback = null,
    NetMessageAccept accept = NetMessageAccept.Both);

  internal void Initialize();

  internal void FrameUpdate();

  internal Task ServerHandshake(INetChannel channel);

  internal event Action ClientHandshakeComplete;
}
