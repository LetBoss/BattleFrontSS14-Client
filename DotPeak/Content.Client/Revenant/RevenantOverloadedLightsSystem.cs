// Decompiled with JetBrains decompiler
// Type: Content.Client.Revenant.RevenantOverloadedLightsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Revenant.Components;
using Content.Shared.Revenant.EntitySystems;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Revenant;

public sealed class RevenantOverloadedLightsSystem : SharedRevenantOverloadedLightsSystem
{
  [Dependency]
  private SharedPointLightSystem _lights;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevenantOverloadedLightsComponent, ComponentStartup>(new ComponentEventHandler<RevenantOverloadedLightsComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevenantOverloadedLightsComponent, ComponentShutdown>(new ComponentEventHandler<RevenantOverloadedLightsComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<RevenantOverloadedLightsComponent, PointLightComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RevenantOverloadedLightsComponent, PointLightComponent>();
    EntityUid entityUid;
    RevenantOverloadedLightsComponent overloadedLightsComponent;
    PointLightComponent pointLightComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref overloadedLightsComponent, ref pointLightComponent))
      this._lights.SetEnergy(entityUid, 2f * Math.Abs((float) Math.Sin(Math.PI / 4.0 * (double) overloadedLightsComponent.Accumulator)), (SharedPointLightComponent) pointLightComponent);
  }

  private void OnStartup(
    EntityUid uid,
    RevenantOverloadedLightsComponent component,
    ComponentStartup args)
  {
    SharedPointLightComponent pointLightComponent = this._lights.EnsureLight(uid);
    component.OriginalEnergy = new float?(pointLightComponent.Energy);
    component.OriginalEnabled = pointLightComponent.Enabled;
    this._lights.SetEnabled(uid, component.OriginalEnabled, pointLightComponent, (MetaDataComponent) null);
    this.Dirty(uid, (IComponent) pointLightComponent, (MetaDataComponent) null);
  }

  private void OnShutdown(
    EntityUid uid,
    RevenantOverloadedLightsComponent component,
    ComponentShutdown args)
  {
    SharedPointLightComponent pointLightComponent;
    if (!this._lights.TryGetLight(uid, ref pointLightComponent))
      return;
    if (!component.OriginalEnergy.HasValue)
    {
      this.RemComp(uid, (IComponent) pointLightComponent);
    }
    else
    {
      this._lights.SetEnergy(uid, component.OriginalEnergy.Value, pointLightComponent);
      this._lights.SetEnabled(uid, component.OriginalEnabled, pointLightComponent, (MetaDataComponent) null);
      this.Dirty(uid, (IComponent) pointLightComponent, (MetaDataComponent) null);
    }
  }

  protected override void OnZap(
    Entity<RevenantOverloadedLightsComponent> component)
  {
  }
}
