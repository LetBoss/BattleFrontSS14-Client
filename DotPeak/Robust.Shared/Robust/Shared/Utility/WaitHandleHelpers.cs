// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.WaitHandleHelpers
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Utility;

internal static class WaitHandleHelpers
{
  public static Task WaitOneAsync(WaitHandle handle)
  {
    TaskCompletionSource tcs = new TaskCompletionSource();
    RegisteredWaitHandle rwh = ThreadPool.RegisterWaitForSingleObject(handle, (WaitOrTimerCallback) ((_param1, _param2) => tcs.TrySetResult()), (object) null, -1, true);
    Task task = tcs.Task;
    task.ContinueWith<bool>((Func<Task, bool>) (_ => rwh.Unregister((WaitHandle) null)));
    return task;
  }
}
