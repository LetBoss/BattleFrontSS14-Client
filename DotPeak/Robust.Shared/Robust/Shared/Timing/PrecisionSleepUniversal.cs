// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Timing.PrecisionSleepUniversal
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Threading;

#nullable disable
namespace Robust.Shared.Timing;

internal sealed class PrecisionSleepUniversal : PrecisionSleep
{
  public override void Sleep(TimeSpan time) => Thread.Sleep(time);
}
