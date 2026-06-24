// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ninja.Systems.EnergyKatanaSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory.Events;
using Content.Shared.Ninja.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Ninja.Systems;

public sealed class EnergyKatanaSystem : EntitySystem
{
  [Dependency]
  private SharedSpaceNinjaSystem _ninja;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<EnergyKatanaComponent, GotEquippedEvent>(new EntityEventRefHandler<EnergyKatanaComponent, GotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<EnergyKatanaComponent, CheckDashEvent>(new EntityEventRefHandler<EnergyKatanaComponent, CheckDashEvent>(this.OnCheckDash));
  }

  private void OnEquipped(Entity<EnergyKatanaComponent> ent, ref GotEquippedEvent args)
  {
    this._ninja.BindKatana((Entity<SpaceNinjaComponent>) args.Equipee, (EntityUid) ent);
  }

  private void OnCheckDash(Entity<EnergyKatanaComponent> ent, ref CheckDashEvent args)
  {
    if (this._ninja.IsNinja(new EntityUid?(args.User)))
      return;
    args.Cancelled = true;
  }
}
