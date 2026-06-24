// Decompiled with JetBrains decompiler
// Type: Content.Client.Disposal.PressureBar
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable disable
namespace Content.Client.Disposal;

public sealed class PressureBar : ProgressBar
{
  public bool UpdatePressure(TimeSpan fullTime)
  {
    TimeSpan curTime = IoCManager.Resolve<IGameTiming>().CurTime;
    float pressure = (float) Math.Min(1.0, 1.0 - (fullTime.TotalSeconds - curTime.TotalSeconds) * 0.05000000074505806);
    this.UpdatePressureBar(pressure);
    return (double) pressure >= 1.0;
  }

  private void UpdatePressureBar(float pressure)
  {
    ((Range) this).Value = pressure;
    float num = pressure / ((Range) this).MaxValue;
    float x = (double) num > 0.5 ? MathHelper.Lerp(0.066f, 0.33f, (float) (((double) num - 0.5) / 0.5)) : MathHelper.Lerp(0.0f, 0.066f, num / 0.5f);
    if (this.ForegroundStyleBoxOverride == null)
    {
      StyleBox styleBox;
      this.ForegroundStyleBoxOverride = styleBox = (StyleBox) new StyleBoxFlat();
    }
    ((StyleBoxFlat) this.ForegroundStyleBoxOverride).BackgroundColor = Color.FromHsv(new Vector4(x, 1f, 0.8f, 1f));
  }
}
