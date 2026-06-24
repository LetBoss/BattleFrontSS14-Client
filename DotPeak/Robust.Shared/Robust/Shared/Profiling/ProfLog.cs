// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Profiling.ProfLog
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Runtime.InteropServices;

#nullable disable
namespace Robust.Shared.Profiling;

[StructLayout(LayoutKind.Explicit)]
public struct ProfLog
{
  [FieldOffset(0)]
  public ProfLogType Type;
  [FieldOffset(8)]
  public ProfLogValue Value;
  [FieldOffset(8)]
  public ProfLogGroupEnd GroupEnd;
}
