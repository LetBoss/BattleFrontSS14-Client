// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Profiling.ProfValue
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Robust.Shared.Profiling;

[StructLayout(LayoutKind.Explicit)]
public struct ProfValue
{
  [FieldOffset(0)]
  public ProfValueType Type;
  [FieldOffset(8)]
  public TimeAndAllocSample TimeAllocSample;
  [FieldOffset(8)]
  public int Int32;
  [FieldOffset(8)]
  public long Int64;
}
