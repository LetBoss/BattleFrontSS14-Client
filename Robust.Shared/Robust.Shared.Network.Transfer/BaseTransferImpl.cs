using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Transfer;

internal abstract class BaseTransferImpl(ISawmill sawmill, BaseTransferManager parent, INetChannel channel) : IDisposable
{
	private sealed class ReceiveStream : SaneStream
	{
		private readonly ChannelReader<ArraySegment<byte>> _bufferChannel;

		private ArraySegment<byte> _currentBuffer;

		public override bool CanRead => true;

		public ReceiveStream(ChannelReader<ArraySegment<byte>> bufferChannel)
		{
			_bufferChannel = bufferChannel;
		}

		public override int Read(Span<byte> buffer)
		{
			int num = 0;
			Span<byte> destination = buffer;
			while (destination.Length > 0)
			{
				if (_currentBuffer.Array == null || _currentBuffer.Count <= 0)
				{
					if (_currentBuffer.Array != null)
					{
						ArrayPool<byte>.Shared.Return(_currentBuffer.Array);
						_currentBuffer = default(ArraySegment<byte>);
					}
					if (!_bufferChannel.TryRead(out _currentBuffer) && (num > 0 || !ReadNewBufferSync()))
					{
						return num;
					}
				}
				int count = _currentBuffer.Count;
				int num2 = Math.Min(destination.Length, count);
				_currentBuffer.AsSpan(0, num2).CopyTo(destination);
				int num3 = num2;
				destination = destination.Slice(num3, destination.Length - num3);
				ref ArraySegment<byte> currentBuffer = ref _currentBuffer;
				num3 = num2;
				_currentBuffer = currentBuffer.Slice(num3, currentBuffer.Count - num3);
				num += num2;
			}
			return num;
		}

		public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			int read = 0;
			Memory<byte> remainingSpan = buffer;
			while (remainingSpan.Length > 0)
			{
				if (_currentBuffer.Array == null || _currentBuffer.Count <= 0)
				{
					if (_currentBuffer.Array != null)
					{
						ArrayPool<byte>.Shared.Return(_currentBuffer.Array);
						_currentBuffer = default(ArraySegment<byte>);
					}
					if (!_bufferChannel.TryRead(out _currentBuffer))
					{
						bool flag = read > 0;
						if (!flag)
						{
							flag = !(await ReadNewBufferAsync());
						}
						if (flag)
						{
							return read;
						}
					}
				}
				int count = _currentBuffer.Count;
				int num = Math.Min(remainingSpan.Length, count);
				_currentBuffer.AsMemory(0, num).CopyTo(remainingSpan);
				int num2 = num;
				remainingSpan = remainingSpan.Slice(num2, remainingSpan.Length - num2);
				ReceiveStream receiveStream = this;
				ref ArraySegment<byte> currentBuffer = ref _currentBuffer;
				num2 = num;
				receiveStream._currentBuffer = currentBuffer.Slice(num2, currentBuffer.Count - num2);
				read += num;
			}
			return read;
		}

		private bool ReadNewBufferSync()
		{
			if (!_bufferChannel.WaitToReadAsync().AsTask().Result)
			{
				return false;
			}
			return _bufferChannel.TryRead(out _currentBuffer);
		}

		private async Task<bool> ReadNewBufferAsync()
		{
			if (!(await _bufferChannel.WaitToReadAsync()))
			{
				return false;
			}
			return _bufferChannel.TryRead(out _currentBuffer);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing && _currentBuffer.Array != null)
			{
				ArrayPool<byte>.Shared.Return(_currentBuffer.Array);
			}
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
			if (Encoding.UTF8.GetByteCount(key) > 96)
			{
				throw new ArgumentException("Key too long");
			}
			Parent = parent;
			_id = id;
			_key = key;
			_headerBuffer = ArrayPool<byte>.Shared.Rent(128);
			_dataBuffer = ArrayPool<byte>.Shared.Rent(16384);
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			while (buffer.Length > 0)
			{
				Span<byte> destination = _dataBuffer.AsSpan(_bufferPos);
				int num = Math.Min(destination.Length, buffer.Length);
				buffer.Slice(0, num).CopyTo(destination);
				_bufferPos += num;
				if (_bufferPos == _dataBuffer.Length)
				{
					Flush();
				}
				int num2 = num;
				buffer = buffer.Slice(num2, buffer.Length - num2);
			}
		}

		public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			while (buffer.Length > 0)
			{
				Span<byte> destination = _dataBuffer.AsSpan(_bufferPos);
				int thisChunk = Math.Min(destination.Length, buffer.Length);
				buffer.Slice(0, thisChunk).Span.CopyTo(destination);
				_bufferPos += thisChunk;
				if (_bufferPos == _dataBuffer.Length)
				{
					await FlushAsync(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
				}
				int num = thisChunk;
				buffer = buffer.Slice(num, buffer.Length - num);
			}
		}

		public override void Flush()
		{
			FlushAsync().Wait();
		}

		public override async Task FlushAsync(CancellationToken cancellationToken)
		{
			await FlushAsync(finish: false, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		private async ValueTask FlushAsync(bool finish, CancellationToken cancel = default(CancellationToken))
		{
			int headerLength = 10;
			Opcode opcode = Opcode.Transfer;
			TransferFlags transferFlags = TransferFlags.None;
			if (_isFirstTransmission)
			{
				transferFlags |= TransferFlags.Start;
			}
			if (_bufferPos > 0)
			{
				transferFlags |= TransferFlags.HasData;
			}
			if (finish)
			{
				transferFlags |= TransferFlags.Finish;
			}
			if (transferFlags == TransferFlags.None)
			{
				return;
			}
			_headerBuffer[0] = (byte)opcode;
			_headerBuffer[1] = (byte)transferFlags;
			BinaryPrimitives.WriteInt64LittleEndian(_headerBuffer.AsSpan(2..10), _id);
			if (_isFirstTransmission)
			{
				int bytes = Encoding.UTF8.GetBytes(_key.AsSpan(), _headerBuffer.AsSpan(11..));
				_headerBuffer[10] = (byte)bytes;
				headerLength++;
				headerLength += bytes;
			}
			using (await Parent._socketSemaphore.WaitGuardAsync().ConfigureAwait(continueOnCapturedContext: false))
			{
				await SendChunkAsync(new ArraySegment<byte>(_headerBuffer, 0, headerLength), cancel).ConfigureAwait(continueOnCapturedContext: false);
				if (_bufferPos > 0)
				{
					await SendChunkAsync(new ArraySegment<byte>(_dataBuffer, 0, _bufferPos), cancel).ConfigureAwait(continueOnCapturedContext: false);
					_bufferPos = 0;
				}
			}
			_isFirstTransmission = false;
		}

		protected abstract ValueTask SendChunkAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);

		protected override void Dispose(bool disposing)
		{
			FlushAsync(finish: true).AsTask().Wait();
			DisposeCore();
		}

		public override async ValueTask DisposeAsync()
		{
			GC.SuppressFinalize(this);
			await FlushAsync(finish: true).ConfigureAwait(continueOnCapturedContext: false);
			DisposeCore();
		}

		private void DisposeCore()
		{
			ArrayPool<byte>.Shared.Return(_dataBuffer);
			ArrayPool<byte>.Shared.Return(_headerBuffer);
		}

		~ChunkedSendStream()
		{
			FlushAsync(finish: true).AsTask().Wait();
		}
	}

	protected enum Opcode : byte
	{
		Transfer
	}

	[Flags]
	protected enum TransferFlags : byte
	{
		None = 0,
		Start = 1,
		Finish = 2,
		HasData = 4
	}

	internal const int BufferSize = 16384;

	internal const int MaxKeySize = 96;

	internal const int MaxHeaderSize = 128;

	protected readonly INetChannel Channel = channel;

	protected readonly ISawmill Sawmill = sawmill;

	protected long OutgoingIdCounter;

	public int MaxChannelCount = int.MaxValue;

	private readonly Dictionary<long, ChannelWriter<ArraySegment<byte>>> _receivingChannels = new Dictionary<long, ChannelWriter<ArraySegment<byte>>>();

	private readonly SemaphoreSlim _socketSemaphore = new SemaphoreSlim(1, 1);

	internal readonly BaseTransferManager Parent = parent;

	protected abstract bool BoundedChannel { get; }

	public abstract Task ServerInit();

	public abstract Task ClientInit(CancellationToken cancel);

	public abstract Stream StartTransfer(TransferStartInfo startInfo);

	private void TransferReceived(string key, ChannelReader<ArraySegment<byte>> reader)
	{
		if (_receivingChannels.Count >= MaxChannelCount)
		{
			Sawmill.Warning($"Disconnecting client {Channel} for breaching max channel count of {_receivingChannels}");
			Channel.Disconnect("Reached max transfer channel count");
		}
		else
		{
			ReceiveStream stream = new ReceiveStream(reader);
			Parent.TransferReceived(key, Channel, stream);
		}
	}

	protected void HandleHeaderReceived(ReadOnlyMemory<byte> data, out TransferFlags flags, out long transferId, out ChannelWriter<ArraySegment<byte>> channel)
	{
		ParseHeader(data.Span, out flags, out transferId, out string key);
		if (!_receivingChannels.TryGetValue(transferId, out channel))
		{
			if ((flags & TransferFlags.Start) == 0)
			{
				throw new ProtocolViolationException($"Received data for unknown transfer {transferId}");
			}
			Sawmill.Verbose($"Starting transfer stream {transferId} with key {key}");
			Channel<ArraySegment<byte>> channel2 = (BoundedChannel ? System.Threading.Channels.Channel.CreateBounded<ArraySegment<byte>>(new BoundedChannelOptions(4)
			{
				SingleReader = true,
				SingleWriter = true
			}) : System.Threading.Channels.Channel.CreateUnbounded<ArraySegment<byte>>(new UnboundedChannelOptions
			{
				SingleReader = true,
				SingleWriter = true
			}));
			channel = channel2.Writer;
			_receivingChannels.Add(transferId, channel);
			TransferReceived(key, channel2.Reader);
		}
	}

	protected void HandlePostData(TransferFlags flags, long transferId, ChannelWriter<ArraySegment<byte>> channel)
	{
		if ((flags & TransferFlags.Finish) != TransferFlags.None)
		{
			Sawmill.Verbose($"Finishing transfer stream {transferId}");
			channel.Complete();
			_receivingChannels.Remove(transferId);
		}
	}

	private static void ParseHeader(ReadOnlySpan<byte> buf, out TransferFlags flags, out long transferId, out string? key)
	{
		flags = (TransferFlags)buf[1];
		transferId = BinaryPrimitives.ReadInt64LittleEndian(buf.Slice(2, 8));
		if ((flags & TransferFlags.Start) != TransferFlags.None)
		{
			byte length = buf[10];
			key = Encoding.UTF8.GetString(buf.Slice(11, length));
		}
		else
		{
			key = null;
		}
	}

	public virtual void Dispose()
	{
		foreach (ChannelWriter<ArraySegment<byte>> value in _receivingChannels.Values)
		{
			value.Complete();
		}
	}
}
