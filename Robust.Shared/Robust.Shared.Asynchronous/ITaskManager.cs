using System;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Asynchronous;

[NotContentImplementable]
public interface ITaskManager
{
	void Initialize();

	void ProcessPendingTasks();

	void RunOnMainThread(Action callback);

	void BlockWaitOnTask(Task task);
}
