// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.RechargeCycleAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Interaction;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class RechargeCycleAmmoSystem : EntitySystem
{
  [Dependency]
  private SharedGunSystem _gun;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<RechargeCycleAmmoComponent, ActivateInWorldEvent>(new ComponentEventHandler<RechargeCycleAmmoComponent, ActivateInWorldEvent>(this.OnRechargeCycled));
  }

  private void OnRechargeCycled(
    EntityUid uid,
    RechargeCycleAmmoComponent component,
    ActivateInWorldEvent args)
  {
    BasicEntityAmmoProviderComponent comp;
    if (!args.Complex || !this.TryComp<BasicEntityAmmoProviderComponent>(uid, out comp) || args.Handled)
      return;
    int? count = comp.Count;
    int? capacity = comp.Capacity;
    if (count.GetValueOrDefault() >= capacity.GetValueOrDefault() & count.HasValue & capacity.HasValue || !comp.Count.HasValue)
      return;
    this._gun.UpdateBasicEntityAmmoCount(uid, comp.Count.Value + 1, comp);
    this.Dirty(uid, (IComponent) comp);
    args.Handled = true;
  }
}
