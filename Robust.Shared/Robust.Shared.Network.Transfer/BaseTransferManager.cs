using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using Prometheus;
using Robust.Shared.Asynchronous;
using Robust.Shared.Collections;
using Robust.Shared.Log;

namespace Robust.Shared.Network.Transfer;

internal abstract class BaseTransferManager
{
	protected sealed class RegisteredKey
	{
		public Action<TransferReceivedEvent>? Callback;
	}

	private sealed class NetChannelClosedException : Exception
	{
		public NetChannelClosedException(string message)
			: base(message)
		{
		}
	}

	internal static readonly Counter SentDataMetrics = Metrics.CreateCounter("robust_transfer_sent_bytes", "Number of bytes sent via the transfer system", (CounterConfiguration)null);

	internal static readonly Counter ReceivedDataMetrics = Metrics.CreateCounter("robust_transfer_received_bytes", "Number of bytes received via the transfer system", (CounterConfiguration)null);

	private readonly NetMessageAccept _side;

	private readonly ITaskManager _taskManager;

	protected readonly Dictionary<string, RegisteredKey> RegisteredKeys = new Dictionary<string, RegisteredKey>();

	protected readonly ISawmill Sawmill;

	private readonly Lock _waitingSendChannelLock = new Lock();

	private readonly Dictionary<INetChannel, TaskCompletionSource> _waitingSendChannels = new Dictionary<INetChannel, TaskCompletionSource>();

	private ValueList<(INetChannel, TaskCompletionSource)> _sendChannelQueue;

	private protected BaseTransferManager(ILogManager logManager, NetMessageAccept side, ITaskManager taskManager)
	{
		_side = side;
		_taskManager = taskManager;
		Sawmill = logManager.GetSawmill("net.transfer");
	}

	public void RegisterTransferMessage(string key, Action<TransferReceivedEvent>? rxCallback = null, NetMessageAccept accept = NetMessageAccept.Both)
	{
		if ((accept & ~NetMessageAccept.Both) != NetMessageAccept.None)
		{
			throw new ArgumentException("Invalid accept given: must be client, server, or both");
		}
		bool exists;
		ref RegisteredKey valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(RegisteredKeys, key, out exists);
		if (exists)
		{
			throw new InvalidOperationException("Key '" + key + "' was already registered!");
		}
		valueRefOrAddDefault = new RegisteredKey();
		if ((int)(accept & _side) > 0)
		{
			valueRefOrAddDefault.Callback = rxCallback;
		}
	}

	internal void TransferReceived(string key, INetChannel channel, Stream stream)
	{
		if (!RegisteredKeys.TryGetValue(key, out RegisteredKey registered))
		{
			throw new Exception("Unknown key: " + key);
		}
		if (registered.Callback == null)
		{
			throw new Exception("Key is send-only: " + key);
		}
		_taskManager.RunOnMainThread(delegate
		{
			registered.Callback(new TransferReceivedEvent(key, channel, stream));
		});
	}

	public void FrameUpdate()
	{
		using (_waitingSendChannelLock.EnterScope())
		{
			foreach (var (netChannel2, item) in _waitingSendChannels)
			{
				if (!netChannel2.IsConnected || SendCheck(netChannel2))
				{
					_sendChannelQueue.Add((netChannel2, item));
				}
			}
			foreach (var item3 in _sendChannelQueue)
			{
				INetChannel item2 = item3.Item1;
				_waitingSendChannels.Remove(item2);
			}
		}
		foreach (var (netChannel3, taskCompletionSource2) in _sendChannelQueue)
		{
			if (!netChannel3.IsConnected)
			{
				taskCompletionSource2.TrySetException(new NetChannelClosedException("Channel closed"));
			}
			else
			{
				taskCompletionSource2.TrySetResult();
			}
		}
		_sendChannelQueue.Clear();
	}

	public async ValueTask WaitToSend(INetChannel channel)
	{
		if (SendCheck(channel))
		{
			return;
		}
		TaskCompletionSource taskCompletionSource;
		using (_waitingSendChannelLock.EnterScope())
		{
			bool exists;
			ref TaskCompletionSource valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(_waitingSendChannels, channel, out exists);
			if (valueRefOrAddDefault == null)
			{
				valueRefOrAddDefault = new TaskCompletionSource();
			}
			taskCompletionSource = valueRefOrAddDefault;
		}
		await taskCompletionSource.Task;
	}

	private static bool SendCheck(INetChannel channel)
	{
		return channel.CanSendImmediately((NetDeliveryMethod)67, 16);
	}
}
