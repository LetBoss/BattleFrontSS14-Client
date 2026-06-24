// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Threading.IParallelRobustJob
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Threading;

public interface IParallelRobustJob : IParallelRangeRobustJob
{
  void IParallelRangeRobustJob.ExecuteRange(int startIndex, int endIndex)
  {
    for (int index = startIndex; index < endIndex; ++index)
      this.Execute(index);
  }

  void Execute(int index);
}
