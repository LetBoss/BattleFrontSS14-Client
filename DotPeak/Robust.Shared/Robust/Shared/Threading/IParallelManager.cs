// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Threading.IParallelManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using System;
using System.Threading;

#nullable enable
namespace Robust.Shared.Threading;

[NotContentImplementable]
public interface IParallelManager
{
  event Action ParallelCountChanged;

  int ParallelProcessCount { get; }

  void AddAndInvokeParallelCountChanged(Action changed);

  WaitHandle Process(IRobustJob job);

  void ProcessNow(IRobustJob job);

  void ProcessNow(IParallelRobustJob jobs, int amount);

  void ProcessSerialNow(IParallelRobustJob jobs, int amount);

  WaitHandle Process(IParallelRobustJob jobs, int amount);

  void ProcessNow(IParallelBulkRobustJob jobs, int amount);

  void ProcessSerialNow(IParallelBulkRobustJob jobs, int amount);

  WaitHandle Process(IParallelBulkRobustJob jobs, int amount);
}
