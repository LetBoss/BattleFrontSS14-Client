using System;
using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.Exceptions;
using Robust.Shared.IoC;

namespace Robust.Shared.Asynchronous;

internal sealed class TaskManager : ITaskManager
{
	private RobustSynchronizationContext _mainThreadContext;

	[Dependency]
	private readonly IRuntimeLog _runtimeLog;

	private static readonly SendOrPostCallback _runCallback = delegate(object? o)
	{
		((Action)o)?.Invoke();
	};

	public void Initialize()
	{
		_mainThreadContext = new RobustSynchronizationContext(_runtimeLog);
		ResetSynchronizationContext();
	}

	public void ResetSynchronizationContext()
	{
		SynchronizationContext.SetSynchronizationContext(_mainThreadContext);
	}

	public void ProcessPendingTasks()
	{
		_mainThreadContext.ProcessPendingTasks();
	}

	public void RunOnMainThread(Action callback)
	{
		_mainThreadContext.Post(_runCallback, callback);
	}

	public void BlockWaitOnTask(Task task)
	{
		while (true)
		{
			Task<bool> task2 = _mainThreadContext.WaitOnPendingTasks().AsTask();
			if (Task.WaitAny(task, task2) == 0)
			{
				break;
			}
			_mainThreadContext.ProcessPendingTasks();
		}
	}
}
