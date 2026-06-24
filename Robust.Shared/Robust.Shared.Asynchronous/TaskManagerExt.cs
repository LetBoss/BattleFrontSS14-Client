using System;
using System.Threading.Tasks;

namespace Robust.Shared.Asynchronous;

internal static class TaskManagerExt
{
	public static Task TaskOnMainThread(this ITaskManager taskManager, Action callback)
	{
		TaskCompletionSource tcs = new TaskCompletionSource();
		taskManager.RunOnMainThread(delegate
		{
			try
			{
				callback();
				tcs.SetResult();
			}
			catch (Exception exception)
			{
				tcs.TrySetException(exception);
			}
		});
		return tcs.Task;
	}
}
