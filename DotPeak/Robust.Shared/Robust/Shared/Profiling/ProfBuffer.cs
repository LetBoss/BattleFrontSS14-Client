// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Profiling.ProfBuffer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Profiling;

public struct ProfBuffer
{
  public long LogWriteOffset;
  public long IndexWriteOffset;
  public ProfLog[] LogBuffer;
  public ProfIndex[] IndexBuffer;

  public readonly ProfBuffer Snapshot()
  {
    ProfBuffer profBuffer = this;
    profBuffer.LogBuffer = (ProfLog[]) profBuffer.LogBuffer.Clone();
    profBuffer.IndexBuffer = (ProfIndex[]) profBuffer.IndexBuffer.Clone();
    return profBuffer;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly ref ProfLog Log(long idx)
  {
    return ref this.LogBuffer[idx & (long) this.LogBuffer.Length - 1L];
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public readonly ref ProfIndex Index(long idx)
  {
    return ref this.IndexBuffer[idx & (long) this.IndexBuffer.Length - 1L];
  }
}
