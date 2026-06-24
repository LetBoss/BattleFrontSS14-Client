using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Robust.Shared.Log;
using Robust.Shared.Network.Messages.Transfer;

namespace Robust.Shared.Network.Transfer;

internal sealed class TransferImplLidgren(ISawmill sawmill, INetChannel channel, BaseTransferManager transferManager, INetManager netManager) : BaseTransferImpl(sawmill, transferManager, channel)
{
	private sealed class SendStream : ChunkedSendStream
	{
		private readonly INetChannel _channel;

		public SendStream(INetChannel channel, TransferImplLidgren parent, long id, string key)
			: base(parent, id, key)
		{
			_channel = channel;
		}

		protected override async ValueTask SendChunkAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
		{
			if (!_channel.IsConnected)
			{
				throw new InvalidOperationException("Channel is disconnected");
			}
			BaseTransferManager.SentDataMetrics.Inc((double)buffer.Count);
			await Parent.Parent.WaitToSend(_channel);
			MsgTransferData message = new MsgTransferData
			{
				Data = buffer
			};
			_channel.SendMessage(message);
		}
	}

	private TaskCompletionSource? _serverInitTcs;

	private (TransferFlags Flags, long TransferId, ChannelWriter<ArraySegment<byte>> Channel)? _parsedHeader;

	protected override bool BoundedChannel => false;

	public override Task ServerInit()
	{
		MsgTransferInit message = new MsgTransferInit();
		netManager.ServerSendMessage(message, Channel);
		_serverInitTcs = new TaskCompletionSource();
		return _serverInitTcs.Task;
	}

	public override Task ClientInit(CancellationToken cancel)
	{
		MsgTransferAckInit message = new MsgTransferAckInit();
		netManager.ClientSendMessage(message);
		return Task.CompletedTask;
	}

	public override Stream StartTransfer(TransferStartInfo startInfo)
	{
		long id = Interlocked.Increment(ref OutgoingIdCounter);
		return new SendStream(Channel, this, id, startInfo.MessageKey);
	}

	public void ReceiveInitAck()
	{
		_serverInitTcs?.TrySetResult();
	}

	public void ReceiveData(MsgTransferData data)
	{
		BaseTransferManager.ReceivedDataMetrics.Inc((double)data.Data.Count);
		if (!_parsedHeader.HasValue)
		{
			HandleHeaderReceived(data.Data, out TransferFlags flags, out long transferId, out ChannelWriter<ArraySegment<byte>> channel);
			ArrayPool<byte>.Shared.Return(data.Data.Array);
			if ((flags & TransferFlags.HasData) == 0)
			{
				HandlePostData(flags, transferId, channel);
			}
			else
			{
				_parsedHeader = (flags, transferId, channel);
			}
		}
		else
		{
			(TransferFlags Flags, long TransferId, ChannelWriter<ArraySegment<byte>> Channel) value = _parsedHeader.Value;
			TransferFlags item = value.Flags;
			long item2 = value.TransferId;
			ChannelWriter<ArraySegment<byte>> item3 = value.Channel;
			_parsedHeader = null;
			item3.WriteAsync(data.Data).AsTask().Wait();
			HandlePostData(item, item2, item3);
		}
	}
}
