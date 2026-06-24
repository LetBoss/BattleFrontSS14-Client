// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Asynchronous.RobustSynchronizationContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Exceptions;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Asynchronous;

internal sealed class RobustSynchronizationContext : SynchronizationContext
{
  private readonly IRuntimeLog _runtimeLog;
  private readonly ChannelReader<RobustSynchronizationContext.Mail> _channelReader;
  private readonly ChannelWriter<RobustSynchronizationContext.Mail> _channelWriter;

  public RobustSynchronizationContext(IRuntimeLog runtimeLog)
  {
    this._runtimeLog = runtimeLog;
    UnboundedChannelOptions options = new UnboundedChannelOptions();
    options.SingleReader = true;
    options.SingleWriter = false;
    options.AllowSynchronousContinuations = true;
    Channel<RobustSynchronizationContext.Mail> unbounded = Channel.CreateUnbounded<RobustSynchronizationContext.Mail>(options);
    this._channelReader = unbounded.Reader;
    this._channelWriter = unbounded.Writer;
  }

  public override void Send(SendOrPostCallback d, object? state)
  {
    if (SynchronizationContext.Current != this)
      throw new NotImplementedException();
    d(state);
  }

  public override void Post(SendOrPostCallback d, object? state)
  {
    this._channelWriter.TryWrite(new RobustSynchronizationContext.Mail(d, state));
  }

  public void ProcessPendingTasks()
  {
    RobustSynchronizationContext.Mail mail;
    while (this._channelReader.TryRead(out mail))
    {
      try
      {
        mail.Callback(mail.State);
      }
      catch (Exception ex)
      {
        this._runtimeLog.LogException(ex, "Async Queued Callback");
      }
    }
  }

  public ValueTask<bool> WaitOnPendingTasks() => this._channelReader.WaitToReadAsync();

  private record struct Mail(SendOrPostCallback Callback, object? State);
}
