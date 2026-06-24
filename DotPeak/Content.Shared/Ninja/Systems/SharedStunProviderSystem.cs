// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.SharedStunProviderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public abstract class SharedStunProviderSystem : EntitySystem
{
  public void SetBattery(Entity<StunProviderComponent?> ent, EntityUid? battery)
  {
    if (!this.Resolve<StunProviderComponent>((EntityUid) ent, ref ent.Comp))
      return;
    EntityUid? batteryUid = ent.Comp.BatteryUid;
    EntityUid? nullable = battery;
    if ((batteryUid.HasValue == nullable.HasValue ? (batteryUid.HasValue ? (batteryUid.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
      return;
    ent.Comp.BatteryUid = battery;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }
}
