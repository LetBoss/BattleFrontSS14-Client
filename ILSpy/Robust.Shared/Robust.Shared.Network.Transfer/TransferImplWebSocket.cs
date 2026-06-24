using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Robust.Shared.Log;

namespace Robust.Shared.Network.Transfer;

internal abstract class TransferImplWebSocket(ISawmill sawmill, BaseTransferManager parent, INetChannel channel) : BaseTransferImpl(sawmill, parent, channel)
{
	private sealed class SendStream : ChunkedSendStream
	{
		public SendStream(TransferImplWebSocket parent, long id, string key)
			: base(parent, id, key)
		{
		}

		protected override async ValueTask SendChunkAsync(ArraySegment<byte> buffer, CancellationToken cancel)
		{
			WebSocket? webSocket = ((TransferImplWebSocket)Parent).WebSocket;
			BaseTransferManager.SentDataMetrics.Inc((double)buffer.Count);
			await webSocket.SendAsync(buffer, WebSocketMessageType.Binary, endOfMessage: true, cancel).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	internal const string KeyHeaderName = "RT-Key";

	internal const string UserIdHeaderName = "RT-UserId";

	internal const int RandomKeyBytes = 32;

	private readonly byte[] _headerBuffer = ArrayPool<byte>.Shared.Rent(128);

	private readonly CancellationTokenSource _readCancel = new CancellationTokenSource();

	public WebSocket? WebSocket;

	protected override bool BoundedChannel => true;

	public override Stream StartTransfer(TransferStartInfo startInfo)
	{
		if (WebSocket == null)
		{
			throw new InvalidOperationException("Player not connected yet");
		}
		long id = Interlocked.Increment(ref OutgoingIdCounter);
		return new SendStream(this, id, startInfo.MessageKey);
	}

	protected async void ReadThread()
	{
		_ = 1;
		try
		{
			CancellationToken cancel = _readCancel.Token;
			while (!cancel.IsCancellationRequested)
			{
				ValueWebSocketReceiveResult valueWebSocketReceiveResult = await WebSocket.ReceiveAsync(_headerBuffer.AsMemory(), cancel).ConfigureAwait(continueOnCapturedContext: false);
				BaseTransferManager.ReceivedDataMetrics.Inc((double)valueWebSocketReceiveResult.Count);
				if (!valueWebSocketReceiveResult.EndOfMessage)
				{
					throw new ProtocolViolationException("Header did not fit in one receive");
				}
				if (valueWebSocketReceiveResult.MessageType != WebSocketMessageType.Binary)
				{
					throw new ProtocolViolationException("Data must be binary!");
				}
				Memory<byte> memory = _headerBuffer.AsMemory(0, valueWebSocketReceiveResult.Count);
				HandleHeaderReceived(memory, out TransferFlags flags, out long transferId, out ChannelWriter<ArraySegment<byte>> channel);
				if ((flags & TransferFlags.HasData) != TransferFlags.None)
				{
					await ReceiveTransferData(WebSocket, channel, cancel).ConfigureAwait(continueOnCapturedContext: false);
				}
				HandlePostData(flags, transferId, channel);
				channel = null;
			}
		}
		catch (Exception value)
		{
			Sawmill.Error($"Error reading transfer socket: {value}");
			Channel.Disconnect("Error in transfer socket");
		}
	}

	private static async ValueTask ReceiveTransferData(WebSocket ws, ChannelWriter<ArraySegment<byte>> channel, CancellationToken cancel)
	{
		while (!cancel.IsCancellationRequested)
		{
			byte[] buf = ArrayPool<byte>.Shared.Rent(16384);
			ValueWebSocketReceiveResult result = await ws.ReceiveAsync(buf.AsMemory(), cancel).ConfigureAwait(continueOnCapturedContext: false);
			BaseTransferManager.ReceivedDataMetrics.Inc((double)result.Count);
			if (result.MessageType != WebSocketMessageType.Binary)
			{
				throw new ProtocolViolationException("Data must be binary!");
			}
			await channel.WriteAsync(new ArraySegment<byte>(buf, 0, result.Count), cancel).ConfigureAwait(continueOnCapturedContext: false);
			if (result.EndOfMessage)
			{
				break;
			}
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		WebSocket?.Dispose();
		_readCancel.Cancel();
		ArrayPool<byte>.Shared.Return(_headerBuffer);
	}
}
