// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.TransferImplWebSocket
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network.Transfer;

internal abstract class TransferImplWebSocket(
  ISawmill sawmill,
  BaseTransferManager parent,
  INetChannel channel) : BaseTransferImpl(sawmill, parent, channel)
{
  internal const string KeyHeaderName = "RT-Key";
  internal const string UserIdHeaderName = "RT-UserId";
  internal const int RandomKeyBytes = 32 /*0x20*/;
  private readonly byte[] _headerBuffer = ArrayPool<byte>.Shared.Rent(128 /*0x80*/);
  private readonly CancellationTokenSource _readCancel = new CancellationTokenSource();
  public WebSocket? WebSocket;

  protected override bool BoundedChannel => true;

  public override Stream StartTransfer(TransferStartInfo startInfo)
  {
    if (this.WebSocket == null)
      throw new InvalidOperationException("Player not connected yet");
    return (Stream) new TransferImplWebSocket.SendStream(this, Interlocked.Increment(ref this.OutgoingIdCounter), startInfo.MessageKey);
  }

  protected async void ReadThread()
  {
    TransferImplWebSocket transferImplWebSocket = this;
    try
    {
      CancellationToken cancel = transferImplWebSocket._readCancel.Token;
      while (!cancel.IsCancellationRequested)
      {
        ValueWebSocketReceiveResult socketReceiveResult = await transferImplWebSocket.WebSocket.ReceiveAsync(transferImplWebSocket._headerBuffer.AsMemory<byte>(), cancel).ConfigureAwait(false);
        BaseTransferManager.ReceivedDataMetrics.Inc((double) socketReceiveResult.Count);
        if (!socketReceiveResult.EndOfMessage)
          throw new ProtocolViolationException("Header did not fit in one receive");
        if (socketReceiveResult.MessageType != WebSocketMessageType.Binary)
          throw new ProtocolViolationException("Data must be binary!");
        Memory<byte> data = transferImplWebSocket._headerBuffer.AsMemory<byte>(0, socketReceiveResult.Count);
        BaseTransferImpl.TransferFlags flags;
        long transferId;
        ChannelWriter<ArraySegment<byte>> channel;
        transferImplWebSocket.HandleHeaderReceived((ReadOnlyMemory<byte>) data, out flags, out transferId, out channel);
        if ((flags & BaseTransferImpl.TransferFlags.HasData) != BaseTransferImpl.TransferFlags.None)
          await TransferImplWebSocket.ReceiveTransferData(transferImplWebSocket.WebSocket, channel, cancel).ConfigureAwait(false);
        transferImplWebSocket.HandlePostData(flags, transferId, channel);
        channel = (ChannelWriter<ArraySegment<byte>>) null;
      }
      cancel = new CancellationToken();
    }
    catch (Exception ex)
    {
      transferImplWebSocket.Sawmill.Error($"Error reading transfer socket: {ex}");
      transferImplWebSocket.Channel.Disconnect("Error in transfer socket");
    }
  }

  private static async ValueTask ReceiveTransferData(
    WebSocket ws,
    ChannelWriter<ArraySegment<byte>> channel,
    CancellationToken cancel)
  {
    while (!cancel.IsCancellationRequested)
    {
      byte[] buf = ArrayPool<byte>.Shared.Rent(16384 /*0x4000*/);
      ValueWebSocketReceiveResult result = await ws.ReceiveAsync(buf.AsMemory<byte>(), cancel).ConfigureAwait(false);
      BaseTransferManager.ReceivedDataMetrics.Inc((double) result.Count);
      if (result.MessageType != WebSocketMessageType.Binary)
        throw new ProtocolViolationException("Data must be binary!");
      await channel.WriteAsync(new ArraySegment<byte>(buf, 0, result.Count), cancel).ConfigureAwait(false);
      if (result.EndOfMessage)
        break;
      buf = (byte[]) null;
    }
  }

  public override void Dispose()
  {
    base.Dispose();
    this.WebSocket?.Dispose();
    this._readCancel.Cancel();
    ArrayPool<byte>.Shared.Return(this._headerBuffer);
  }

  private sealed class SendStream(TransferImplWebSocket parent, long id, string key) : 
    BaseTransferImpl.ChunkedSendStream((BaseTransferImpl) parent, id, key)
  {
    protected override async ValueTask SendChunkAsync(
      ArraySegment<byte> buffer,
      CancellationToken cancel)
    {
      WebSocket webSocket = ((TransferImplWebSocket) this.Parent).WebSocket;
      BaseTransferManager.SentDataMetrics.Inc((double) buffer.Count);
      ArraySegment<byte> buffer1 = buffer;
      CancellationToken cancellationToken = cancel;
      await webSocket.SendAsync(buffer1, WebSocketMessageType.Binary, true, cancellationToken).ConfigureAwait(false);
    }
  }
}
