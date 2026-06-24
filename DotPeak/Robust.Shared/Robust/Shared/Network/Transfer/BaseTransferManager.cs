// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.BaseTransferManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Lidgren.Network;
using Prometheus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Collections;
using Robust.Shared.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network.Transfer;

internal abstract class BaseTransferManager
{
  internal static readonly Counter SentDataMetrics = Metrics.CreateCounter("robust_transfer_sent_bytes", "Number of bytes sent via the transfer system", (CounterConfiguration) null);
  internal static readonly Counter ReceivedDataMetrics = Metrics.CreateCounter("robust_transfer_received_bytes", "Number of bytes received via the transfer system", (CounterConfiguration) null);
  private readonly NetMessageAccept _side;
  private readonly ITaskManager _taskManager;
  protected readonly Dictionary<string, BaseTransferManager.RegisteredKey> RegisteredKeys = new Dictionary<string, BaseTransferManager.RegisteredKey>();
  protected readonly ISawmill Sawmill;
  private readonly Lock _waitingSendChannelLock = new Lock();
  private readonly Dictionary<INetChannel, TaskCompletionSource> _waitingSendChannels = new Dictionary<INetChannel, TaskCompletionSource>();
  private ValueList<(INetChannel, TaskCompletionSource)> _sendChannelQueue;

  private protected BaseTransferManager(
    ILogManager logManager,
    NetMessageAccept side,
    ITaskManager taskManager)
  {
    this._side = side;
    this._taskManager = taskManager;
    this.Sawmill = logManager.GetSawmill("net.transfer");
  }

  public void RegisterTransferMessage(
    string key,
    Action<TransferReceivedEvent>? rxCallback = null,
    NetMessageAccept accept = NetMessageAccept.Both)
  {
    if ((accept & ~NetMessageAccept.Both) != NetMessageAccept.None)
      throw new ArgumentException("Invalid accept given: must be client, server, or both");
    bool exists;
    ref BaseTransferManager.RegisteredKey local = ref CollectionsMarshal.GetValueRefOrAddDefault<string, BaseTransferManager.RegisteredKey>(this.RegisteredKeys, key, out exists);
    if (exists)
      throw new InvalidOperationException($"Key '{key}' was already registered!");
    local = new BaseTransferManager.RegisteredKey();
    if ((accept & this._side) <= NetMessageAccept.None)
      return;
    local.Callback = rxCallback;
  }

  internal void TransferReceived(string key, INetChannel channel, Stream stream)
  {
    BaseTransferManager.RegisteredKey registered;
    if (!this.RegisteredKeys.TryGetValue(key, out registered))
      throw new Exception("Unknown key: " + key);
    if (registered.Callback == null)
      throw new Exception("Key is send-only: " + key);
    this._taskManager.RunOnMainThread((Action) (() => registered.Callback(new TransferReceivedEvent(key, channel, stream))));
  }

  public void FrameUpdate()
  {
    lock (this._waitingSendChannelLock)
    {
      foreach ((INetChannel netChannel, TaskCompletionSource completionSource) in this._waitingSendChannels)
      {
        if (!netChannel.IsConnected || BaseTransferManager.SendCheck(netChannel))
          this._sendChannelQueue.Add((netChannel, completionSource));
      }
      foreach ((INetChannel, TaskCompletionSource) sendChannel in this._sendChannelQueue)
        this._waitingSendChannels.Remove(sendChannel.Item1);
    }
    foreach ((INetChannel netChannel, TaskCompletionSource completionSource) in this._sendChannelQueue)
    {
      if (!netChannel.IsConnected)
        completionSource.TrySetException((Exception) new BaseTransferManager.NetChannelClosedException("Channel closed"));
      else
        completionSource.TrySetResult();
    }
    this._sendChannelQueue.Clear();
  }

  public async ValueTask WaitToSend(INetChannel channel)
  {
    if (BaseTransferManager.SendCheck(channel))
      return;
    TaskCompletionSource completionSource;
    lock (this._waitingSendChannelLock)
    {
      ref TaskCompletionSource local = ref CollectionsMarshal.GetValueRefOrAddDefault<INetChannel, TaskCompletionSource>(this._waitingSendChannels, channel, out bool _);
      if (local == null)
        local = new TaskCompletionSource();
      completionSource = local;
    }
    await completionSource.Task;
  }

  private static bool SendCheck(INetChannel channel)
  {
    return channel.CanSendImmediately((NetDeliveryMethod) 67, 16 /*0x10*/);
  }

  protected sealed class RegisteredKey
  {
    public Action<TransferReceivedEvent>? Callback;
  }

  private sealed class NetChannelClosedException(string message) : Exception(message)
  {
  }
}
