// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Systems.RadiationPulseSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radiation.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

#nullable enable
namespace Content.Shared.Radiation.Systems;

public sealed class RadiationPulseSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RadiationPulseComponent, ComponentStartup>(new ComponentEventHandler<RadiationPulseComponent, ComponentStartup>(this.OnStartup));
  }

  private void OnStartup(EntityUid uid, RadiationPulseComponent component, ComponentStartup args)
  {
    component.StartTime = this._timing.RealTime;
    TimedDespawnComponent comp1;
    if (this.TryComp<TimedDespawnComponent>(uid, out comp1))
      component.VisualDuration = comp1.Lifetime;
    RadiationSourceComponent comp2;
    if (!this.TryComp<RadiationSourceComponent>(uid, out comp2))
      return;
    component.VisualRange = comp2.Intensity / comp2.Slope;
  }
}
