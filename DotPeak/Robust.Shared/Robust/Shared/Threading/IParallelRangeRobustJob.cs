// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Threading.IParallelRangeRobustJob
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Threading;

public interface IParallelRangeRobustJob
{
  int MinimumBatchParallel => 2;

  int BatchSize => 1;

  void ExecuteRange(int startIndex, int endIndex);
}
