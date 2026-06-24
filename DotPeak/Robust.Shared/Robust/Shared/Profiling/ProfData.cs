// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Profiling.ProfData
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable disable
namespace Robust.Shared.Profiling;

public static class ProfData
{
  public static ProfValue Int32(int int32)
  {
    return new ProfValue()
    {
      Type = ProfValueType.Int32,
      Int32 = int32
    };
  }

  public static ProfValue Int64(long int64)
  {
    return new ProfValue()
    {
      Type = ProfValueType.Int64,
      Int64 = int64
    };
  }

  public static ProfValue TimeAlloc(in ProfSampler sampler)
  {
    return new ProfValue()
    {
      Type = ProfValueType.TimeAllocSample,
      TimeAllocSample = new TimeAndAllocSample()
      {
        Alloc = sampler.ElapsedAlloc,
        Time = (float) sampler.Elapsed.TotalSeconds
      }
    };
  }
}
