// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayRecordingStats
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable disable
namespace Robust.Shared.Replays;

public record struct ReplayRecordingStats(
  TimeSpan Time,
  uint Ticks,
  long Size,
  long UncompressedSize)
;
