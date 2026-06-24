// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Temperature.SharedRMCTemperatureSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Temperature;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared._RMC14.Temperature;

public abstract class SharedRMCTemperatureSystem : EntitySystem
{
  public virtual float GetTemperature(EntityUid entity) => 0.0f;

  public virtual void ForceChangeTemperature(EntityUid entity, float temperature)
  {
  }

  public virtual bool TryGetCurrentTemperature(EntityUid uid, out float temperature)
  {
    temperature = TemperatureHelpers.CelsiusToKelvin(37f);
    return true;
  }
}
