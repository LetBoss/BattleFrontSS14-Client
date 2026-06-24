// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.TransferImplLidgren
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using Robust.Shared.Network.Messages.Transfer;
using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network.Transfer;

internal sealed class TransferImplLidgren(
  ISawmill sawmill,
  INetChannel channel,
  BaseTransferManager transferManager,
  INetManager netManager) : BaseTransferImpl(sawmill, transferManager, channel)
{
  private TaskCompletionSource? _serverInitTcs;
  private (BaseTransferImpl.TransferFlags Flags, long TransferId, ChannelWriter<ArraySegment<byte>> Channel)? _parsedHeader;

  public override Task ServerInit()
  {
    netManager.ServerSendMessage((NetMessage) new MsgTransferInit(), this.Channel);
    this._serverInitTcs = new TaskCompletionSource();
    return this._serverInitTcs.Task;
  }

  public override Task ClientInit(CancellationToken cancel)
  {
    netManager.ClientSendMessage((NetMessage) new MsgTransferAckInit());
    return Task.CompletedTask;
  }

  public override Stream StartTransfer(TransferStartInfo startInfo)
  {
    return (Stream) new TransferImplLidgren.SendStream(this.Channel, this, Interlocked.Increment(ref this.OutgoingIdCounter), startInfo.MessageKey);
  }

  protected override bool BoundedChannel => false;

  public void ReceiveInitAck() => this._serverInitTcs?.TrySetResult();

  public void ReceiveData(MsgTransferData data)
  {
    BaseTransferManager.ReceivedDataMetrics.Inc((double) data.Data.Count);
    if (!this._parsedHeader.HasValue)
    {
      BaseTransferImpl.TransferFlags flags;
      long transferId;
      ChannelWriter<ArraySegment<byte>> channel;
      this.HandleHeaderReceived((ReadOnlyMemory<byte>) data.Data, out flags, out transferId, out channel);
      ArrayPool<byte>.Shared.Return(data.Data.Array);
      if ((flags & BaseTransferImpl.TransferFlags.HasData) == BaseTransferImpl.TransferFlags.None)
        this.HandlePostData(flags, transferId, channel);
      else
        this._parsedHeader = new (BaseTransferImpl.TransferFlags, long, ChannelWriter<ArraySegment<byte>>)?((flags, transferId, channel));
    }
    else
    {
      (BaseTransferImpl.TransferFlags transferFlags, long num, ChannelWriter<ArraySegment<byte>> channelWriter) = this._parsedHeader.Value;
      this._parsedHeader = new (BaseTransferImpl.TransferFlags, long, ChannelWriter<ArraySegment<byte>>)?();
      channelWriter.WriteAsync(data.Data).AsTask().Wait();
      this.HandlePostData(transferFlags, num, channelWriter);
    }
  }

  private sealed class SendStream : BaseTransferImpl.ChunkedSendStream
  {
    private readonly INetChannel _channel;

    public SendStream(INetChannel channel, TransferImplLidgren parent, long id, string key)
      : base((BaseTransferImpl) parent, id, key)
    {
      this._channel = channel;
    }

    protected override async ValueTask SendChunkAsync(
      ArraySegment<byte> buffer,
      CancellationToken cancellationToken)
    {
      TransferImplLidgren.SendStream sendStream = this;
      if (!sendStream._channel.IsConnected)
        throw new InvalidOperationException("Channel is disconnected");
      BaseTransferManager.SentDataMetrics.Inc((double) buffer.Count);
      await sendStream.Parent.Parent.WaitToSend(sendStream._channel);
      MsgTransferData message = new MsgTransferData()
      {
        Data = buffer
      };
      sendStream._channel.SendMessage((NetMessage) message);
    }
  }
}
