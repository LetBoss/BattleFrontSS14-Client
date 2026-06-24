// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.BaseTransferImpl
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Network.Transfer;

internal abstract class BaseTransferImpl(
  ISawmill sawmill,
  BaseTransferManager parent,
  INetChannel channel) : IDisposable
{
  internal const int BufferSize = 16384 /*0x4000*/;
  internal const int MaxKeySize = 96 /*0x60*/;
  internal const int MaxHeaderSize = 128 /*0x80*/;
  protected readonly INetChannel Channel = channel;
  protected readonly ISawmill Sawmill = sawmill;
  protected long OutgoingIdCounter;
  public int MaxChannelCount = int.MaxValue;
  private readonly Dictionary<long, ChannelWriter<ArraySegment<byte>>> _receivingChannels = new Dictionary<long, ChannelWriter<ArraySegment<byte>>>();
  private readonly SemaphoreSlim _socketSemaphore = new SemaphoreSlim(1, 1);
  internal readonly BaseTransferManager Parent = parent;

  public abstract Task ServerInit();

  public abstract Task ClientInit(CancellationToken cancel);

  public abstract Stream StartTransfer(TransferStartInfo startInfo);

  protected abstract bool BoundedChannel { get; }

  private void TransferReceived(string key, ChannelReader<ArraySegment<byte>> reader)
  {
    if (this._receivingChannels.Count >= this.MaxChannelCount)
    {
      this.Sawmill.Warning($"Disconnecting client {this.Channel} for breaching max channel count of {this._receivingChannels}");
      this.Channel.Disconnect("Reached max transfer channel count");
    }
    else
    {
      BaseTransferImpl.ReceiveStream receiveStream = new BaseTransferImpl.ReceiveStream(reader);
      this.Parent.TransferReceived(key, this.Channel, (Stream) receiveStream);
    }
  }

  protected void HandleHeaderReceived(
    ReadOnlyMemory<byte> data,
    out BaseTransferImpl.TransferFlags flags,
    out long transferId,
    out ChannelWriter<ArraySegment<byte>> channel)
  {
    string key;
    BaseTransferImpl.ParseHeader(data.Span, out flags, out transferId, out key);
    if (this._receivingChannels.TryGetValue(transferId, out channel))
      return;
    if ((flags & BaseTransferImpl.TransferFlags.Start) == BaseTransferImpl.TransferFlags.None)
      throw new ProtocolViolationException($"Received data for unknown transfer {transferId}");
    this.Sawmill.Verbose($"Starting transfer stream {transferId} with key {key}");
    System.Threading.Channels.Channel<ArraySegment<byte>> channel1;
    if (!this.BoundedChannel)
    {
      UnboundedChannelOptions options = new UnboundedChannelOptions();
      options.SingleReader = true;
      options.SingleWriter = true;
      channel1 = System.Threading.Channels.Channel.CreateUnbounded<ArraySegment<byte>>(options);
    }
    else
    {
      BoundedChannelOptions options = new BoundedChannelOptions(4);
      options.SingleReader = true;
      options.SingleWriter = true;
      channel1 = System.Threading.Channels.Channel.CreateBounded<ArraySegment<byte>>(options);
    }
    System.Threading.Channels.Channel<ArraySegment<byte>> channel2 = channel1;
    channel = channel2.Writer;
    this._receivingChannels.Add(transferId, channel);
    this.TransferReceived(key, channel2.Reader);
  }

  protected void HandlePostData(
    BaseTransferImpl.TransferFlags flags,
    long transferId,
    ChannelWriter<ArraySegment<byte>> channel)
  {
    if ((flags & BaseTransferImpl.TransferFlags.Finish) == BaseTransferImpl.TransferFlags.None)
      return;
    this.Sawmill.Verbose($"Finishing transfer stream {transferId}");
    channel.Complete();
    this._receivingChannels.Remove(transferId);
  }

  private static void ParseHeader(
    ReadOnlySpan<byte> buf,
    out BaseTransferImpl.TransferFlags flags,
    out long transferId,
    out string? key)
  {
    flags = (BaseTransferImpl.TransferFlags) buf[1];
    transferId = BinaryPrimitives.ReadInt64LittleEndian(buf.Slice(2, 8));
    if ((flags & BaseTransferImpl.TransferFlags.Start) != BaseTransferImpl.TransferFlags.None)
    {
      byte length = buf[10];
      key = Encoding.UTF8.GetString(buf.Slice(11, (int) length));
    }
    else
      key = (string) null;
  }

  public virtual void Dispose()
  {
    foreach (ChannelWriter<ArraySegment<byte>> channelWriter in this._receivingChannels.Values)
      channelWriter.Complete();
  }

  private sealed class ReceiveStream : SaneStream
  {
    private readonly ChannelReader<ArraySegment<byte>> _bufferChannel;
    private ArraySegment<byte> _currentBuffer;

    public override bool CanRead => true;

    public ReceiveStream(ChannelReader<ArraySegment<byte>> bufferChannel)
    {
      this._bufferChannel = bufferChannel;
    }

    public override int Read(Span<byte> buffer)
    {
      int num = 0;
      Span<byte> destination = buffer;
      while (destination.Length > 0)
      {
        if (this._currentBuffer.Array == null || this._currentBuffer.Count <= 0)
        {
          if (this._currentBuffer.Array != null)
          {
            ArrayPool<byte>.Shared.Return(this._currentBuffer.Array);
            this._currentBuffer = new ArraySegment<byte>();
          }
          if (!this._bufferChannel.TryRead(out this._currentBuffer) && (num > 0 || !this.ReadNewBufferSync()))
            return num;
        }
        int count = this._currentBuffer.Count;
        int length = Math.Min(destination.Length, count);
        this._currentBuffer.AsSpan<byte>(0, length).CopyTo(destination);
        ref Span<byte> local1 = ref destination;
        int start = length;
        destination = local1.Slice(start, local1.Length - start);
        ref ArraySegment<byte> local2 = ref this._currentBuffer;
        int index = length;
        this._currentBuffer = local2.Slice(index, local2.Count - index);
        num += length;
      }
      return num;
    }

    public override async ValueTask<int> ReadAsync(
      Memory<byte> buffer,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BaseTransferImpl.ReceiveStream receiveStream1 = this;
      int read = 0;
      Memory<byte> remainingSpan = buffer;
      while (remainingSpan.Length > 0)
      {
        if (receiveStream1._currentBuffer.Array == null || receiveStream1._currentBuffer.Count <= 0)
        {
          if (receiveStream1._currentBuffer.Array != null)
          {
            ArrayPool<byte>.Shared.Return(receiveStream1._currentBuffer.Array);
            receiveStream1._currentBuffer = new ArraySegment<byte>();
          }
          if (!receiveStream1._bufferChannel.TryRead(out receiveStream1._currentBuffer))
          {
            bool flag = read > 0;
            if (!flag)
              flag = !await receiveStream1.ReadNewBufferAsync();
            if (flag)
              return read;
          }
        }
        int length = Math.Min(remainingSpan.Length, receiveStream1._currentBuffer.Count);
        receiveStream1._currentBuffer.AsMemory<byte>(0, length).CopyTo(remainingSpan);
        ref Memory<byte> local1 = ref remainingSpan;
        int start = length;
        remainingSpan = local1.Slice(start, local1.Length - start);
        BaseTransferImpl.ReceiveStream receiveStream2 = receiveStream1;
        ref ArraySegment<byte> local2 = ref receiveStream1._currentBuffer;
        int index = length;
        ArraySegment<byte> arraySegment = local2.Slice(index, local2.Count - index);
        receiveStream2._currentBuffer = arraySegment;
        read += length;
      }
      return read;
    }

    private bool ReadNewBufferSync()
    {
      return this._bufferChannel.WaitToReadAsync().AsTask().Result && this._bufferChannel.TryRead(out this._currentBuffer);
    }

    private async Task<bool> ReadNewBufferAsync()
    {
      return await this._bufferChannel.WaitToReadAsync() && this._bufferChannel.TryRead(out this._currentBuffer);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing || this._currentBuffer.Array == null)
        return;
      ArrayPool<byte>.Shared.Return(this._currentBuffer.Array);
    }
  }

  protected abstract class ChunkedSendStream : SaneStream
  {
    protected readonly BaseTransferImpl Parent;
    private readonly long _id;
    private readonly string _key;
    private readonly byte[] _headerBuffer;
    private readonly byte[] _dataBuffer;
    private bool _isFirstTransmission = true;
    private int _bufferPos;

    public override bool CanWrite => true;

    public ChunkedSendStream(BaseTransferImpl parent, long id, string key)
    {
      if (Encoding.UTF8.GetByteCount(key) > 96 /*0x60*/)
        throw new ArgumentException("Key too long");
      this.Parent = parent;
      this._id = id;
      this._key = key;
      this._headerBuffer = ArrayPool<byte>.Shared.Rent(128 /*0x80*/);
      this._dataBuffer = ArrayPool<byte>.Shared.Rent(16384 /*0x4000*/);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
      // ISSUE: variable of a reference type
      ReadOnlySpan<byte>& local;
      int start;
      for (; buffer.Length > 0; buffer = local.Slice(start, local.Length - start))
      {
        Span<byte> destination = this._dataBuffer.AsSpan<byte>(this._bufferPos);
        int length = Math.Min(destination.Length, buffer.Length);
        buffer.Slice(0, length).CopyTo(destination);
        this._bufferPos += length;
        if (this._bufferPos == this._dataBuffer.Length)
          this.Flush();
        local = ref buffer;
        start = length;
      }
    }

    public override async ValueTask WriteAsync(
      ReadOnlyMemory<byte> buffer,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BaseTransferImpl.ChunkedSendStream chunkedSendStream = this;
      // ISSUE: variable of a reference type
      ReadOnlyMemory<byte>& local;
      int start;
      for (; buffer.Length > 0; buffer = local.Slice(start, local.Length - start))
      {
        Span<byte> destination = chunkedSendStream._dataBuffer.AsSpan<byte>(chunkedSendStream._bufferPos);
        int thisChunk = Math.Min(destination.Length, buffer.Length);
        buffer.Slice(0, thisChunk).Span.CopyTo(destination);
        chunkedSendStream._bufferPos += thisChunk;
        if (chunkedSendStream._bufferPos == chunkedSendStream._dataBuffer.Length)
          await chunkedSendStream.FlushAsync(cancellationToken).ConfigureAwait(false);
        local = ref buffer;
        start = thisChunk;
      }
    }

    public override void Flush() => this.FlushAsync().Wait();

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
      await this.FlushAsync(false, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask FlushAsync(bool finish, CancellationToken cancel = default (CancellationToken))
    {
      int headerLength = 10;
      BaseTransferImpl.Opcode opcode = BaseTransferImpl.Opcode.Transfer;
      BaseTransferImpl.TransferFlags transferFlags = BaseTransferImpl.TransferFlags.None;
      if (this._isFirstTransmission)
        transferFlags |= BaseTransferImpl.TransferFlags.Start;
      if (this._bufferPos > 0)
        transferFlags |= BaseTransferImpl.TransferFlags.HasData;
      if (finish)
        transferFlags |= BaseTransferImpl.TransferFlags.Finish;
      if (transferFlags == BaseTransferImpl.TransferFlags.None)
        return;
      this._headerBuffer[0] = (byte) opcode;
      this._headerBuffer[1] = (byte) transferFlags;
      BinaryPrimitives.WriteInt64LittleEndian(this._headerBuffer.AsSpan<byte>(new Range((Index) 2, (Index) 10)), this._id);
      if (this._isFirstTransmission)
      {
        int bytes = Encoding.UTF8.GetBytes(this._key.AsSpan(), this._headerBuffer.AsSpan<byte>(Range.StartAt((Index) 11)));
        this._headerBuffer[10] = (byte) bytes;
        ++headerLength;
        headerLength += bytes;
      }
      LockUtility.SemaphoreGuard semaphoreGuard = await this.Parent._socketSemaphore.WaitGuardAsync().ConfigureAwait(false);
      try
      {
        ConfiguredValueTaskAwaitable valueTaskAwaitable = this.SendChunkAsync(new ArraySegment<byte>(this._headerBuffer, 0, headerLength), cancel).ConfigureAwait(false);
        await valueTaskAwaitable;
        if (this._bufferPos > 0)
        {
          valueTaskAwaitable = this.SendChunkAsync(new ArraySegment<byte>(this._dataBuffer, 0, this._bufferPos), cancel).ConfigureAwait(false);
          await valueTaskAwaitable;
          this._bufferPos = 0;
        }
      }
      finally
      {
        semaphoreGuard.Dispose();
      }
      semaphoreGuard = new LockUtility.SemaphoreGuard();
      this._isFirstTransmission = false;
    }

    protected abstract ValueTask SendChunkAsync(
      ArraySegment<byte> buffer,
      CancellationToken cancellationToken);

    protected override void Dispose(bool disposing)
    {
      this.FlushAsync(true).AsTask().Wait();
      this.DisposeCore();
    }

    public override async ValueTask DisposeAsync()
    {
      BaseTransferImpl.ChunkedSendStream chunkedSendStream = this;
      GC.SuppressFinalize((object) chunkedSendStream);
      await chunkedSendStream.FlushAsync(true).ConfigureAwait(false);
      chunkedSendStream.DisposeCore();
    }

    private void DisposeCore()
    {
      ArrayPool<byte>.Shared.Return(this._dataBuffer);
      ArrayPool<byte>.Shared.Return(this._headerBuffer);
    }

    ~ChunkedSendStream() => this.FlushAsync(true).AsTask().Wait();
  }

  protected enum Opcode : byte
  {
    Transfer,
  }

  [Flags]
  protected enum TransferFlags : byte
  {
    None = 0,
    Start = 1,
    Finish = 2,
    HasData = 4,
  }
}
