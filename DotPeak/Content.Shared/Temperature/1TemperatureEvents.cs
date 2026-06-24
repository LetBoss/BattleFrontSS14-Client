// Decompiled with JetBrains decompiler
// Type: Content.Shared.Temperature.OnTemperatureChangeEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Temperature;

public sealed class OnTemperatureChangeEvent : EntityEventArgs
{
  public readonly float CurrentTemperature;
  public readonly float LastTemperature;
  public readonly float TemperatureDelta;

  public OnTemperatureChangeEvent(float current, float last, float delta)
  {
    this.CurrentTemperature = current;
    this.LastTemperature = last;
    this.TemperatureDelta = delta;
  }
}
