// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.SlimPoweredLightSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Light.Components;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Light.EntitySystems;

public sealed class SlimPoweredLightSystem : EntitySystem
{
  [Dependency]
  private SharedPowerReceiverSystem _receiver;
  [Dependency]
  private SharedPointLightSystem _lights;
  private bool _setting;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<SlimPoweredLightComponent, AttemptPointLightToggleEvent>(new EntityEventRefHandler<SlimPoweredLightComponent, AttemptPointLightToggleEvent>(this.OnLightAttempt));
    this.SubscribeLocalEvent<SlimPoweredLightComponent, PowerChangedEvent>(new EntityEventRefHandler<SlimPoweredLightComponent, PowerChangedEvent>(this.OnLightPowerChanged));
  }

  private void OnLightAttempt(
    Entity<SlimPoweredLightComponent> ent,
    ref AttemptPointLightToggleEvent args)
  {
    if (this._setting || !args.Enabled || this._receiver.IsPowered((Entity<SharedApcPowerReceiverComponent>) ent.Owner))
      return;
    args.Cancelled = true;
  }

  private void OnLightPowerChanged(
    Entity<SlimPoweredLightComponent> ent,
    ref PowerChangedEvent args)
  {
    if (args.Powered)
    {
      if (!ent.Comp.Enabled)
        return;
    }
    else if (!ent.Comp.Enabled)
      return;
    SharedPointLightComponent component;
    if (!this._lights.TryGetLight(ent.Owner, out component))
      return;
    bool enabled = ent.Comp.Enabled && args.Powered;
    this._setting = true;
    this._lights.SetEnabled(ent.Owner, enabled, component);
    this._setting = false;
  }

  public void SetEnabled(Entity<SlimPoweredLightComponent?> entity, bool enabled)
  {
    if (!this.Resolve<SlimPoweredLightComponent>(entity.Owner, ref entity.Comp, false) || entity.Comp.Enabled == enabled)
      return;
    entity.Comp.Enabled = enabled;
    this.Dirty<SlimPoweredLightComponent>(entity);
    this._lights.SetEnabled(entity.Owner, enabled);
  }
}
