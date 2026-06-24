// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Asynchronous.TaskManagerExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Asynchronous;

internal static class TaskManagerExt
{
  public static Task TaskOnMainThread(this ITaskManager taskManager, Action callback)
  {
    TaskCompletionSource tcs = new TaskCompletionSource();
    taskManager.RunOnMainThread((Action) (() =>
    {
      try
      {
        callback();
        tcs.SetResult();
      }
      catch (Exception ex)
      {
        tcs.TrySetException(ex);
      }
    }));
    return tcs.Task;
  }
}
