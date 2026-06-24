// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Asynchronous.TaskManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Exceptions;
using Robust.Shared.IoC;
using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Asynchronous;

internal sealed class TaskManager : ITaskManager
{
  private RobustSynchronizationContext _mainThreadContext;
  [Dependency]
  private readonly IRuntimeLog _runtimeLog;
  private static readonly SendOrPostCallback _runCallback = (SendOrPostCallback) (o =>
  {
    Action action = (Action) o;
    if (action == null)
      return;
    action();
  });

  public void Initialize()
  {
    this._mainThreadContext = new RobustSynchronizationContext(this._runtimeLog);
    this.ResetSynchronizationContext();
  }

  public void ResetSynchronizationContext()
  {
    SynchronizationContext.SetSynchronizationContext((SynchronizationContext) this._mainThreadContext);
  }

  public void ProcessPendingTasks() => this._mainThreadContext.ProcessPendingTasks();

  public void RunOnMainThread(Action callback)
  {
    this._mainThreadContext.Post(TaskManager._runCallback, (object) callback);
  }

  public void BlockWaitOnTask(Task task)
  {
    while (true)
    {
      Task<bool> task1 = this._mainThreadContext.WaitOnPendingTasks().AsTask();
      if (Task.WaitAny(task, (Task) task1) != 0)
        this._mainThreadContext.ProcessPendingTasks();
      else
        break;
    }
  }
}
