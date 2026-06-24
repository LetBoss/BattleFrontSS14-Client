// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Utility.ResizableMemoryRegionMetrics
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Diagnostics.Metrics;

#nullable enable
namespace Robust.Shared.Utility;

internal static class ResizableMemoryRegionMetrics
{
  public static readonly Meter Meter = new Meter("Robust.ResizableMemoryRegion");
  public const string GaugeName = "used_bytes";
}
