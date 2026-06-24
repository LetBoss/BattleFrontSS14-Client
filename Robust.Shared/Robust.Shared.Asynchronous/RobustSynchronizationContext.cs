using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Robust.Shared.Exceptions;

namespace Robust.Shared.Asynchronous;

internal sealed class RobustSynchronizationContext : SynchronizationContext
{
	private record struct Mail(SendOrPostCallback Callback, object? State);

	private readonly IRuntimeLog _runtimeLog;

	private readonly ChannelReader<Mail> _channelReader;

	private readonly ChannelWriter<Mail> _channelWriter;

	public RobustSynchronizationContext(IRuntimeLog runtimeLog)
	{
		_runtimeLog = runtimeLog;
		Channel<Mail> channel = Channel.CreateUnbounded<Mail>(new UnboundedChannelOptions
		{
			SingleReader = true,
			SingleWriter = false,
			AllowSynchronousContinuations = true
		});
		_channelReader = channel.Reader;
		_channelWriter = channel.Writer;
	}

	public override void Send(SendOrPostCallback d, object? state)
	{
		if (SynchronizationContext.Current != this)
		{
			throw new NotImplementedException();
		}
		d(state);
	}

	public override void Post(SendOrPostCallback d, object? state)
	{
		_channelWriter.TryWrite(new Mail(d, state));
	}

	public void ProcessPendingTasks()
	{
		Mail item;
		while (_channelReader.TryRead(out item))
		{
			try
			{
				item.Callback(item.State);
			}
			catch (Exception exception)
			{
				_runtimeLog.LogException(exception, "Async Queued Callback");
			}
		}
	}

	public ValueTask<bool> WaitOnPendingTasks()
	{
		return _channelReader.WaitToReadAsync();
	}
}
