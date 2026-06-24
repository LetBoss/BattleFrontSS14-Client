using System.Threading;
using System.Threading.Tasks;

namespace Robust.Shared.Utility;

internal static class WaitHandleHelpers
{
	public static Task WaitOneAsync(WaitHandle handle)
	{
		TaskCompletionSource tcs = new TaskCompletionSource();
		RegisteredWaitHandle rwh = ThreadPool.RegisterWaitForSingleObject(handle, delegate
		{
			tcs.TrySetResult();
		}, null, -1, executeOnlyOnce: true);
		Task task = tcs.Task;
		task.ContinueWith((Task _) => rwh.Unregister(null));
		return task;
	}
}
