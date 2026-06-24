// Decompiled with JetBrains decompiler
// Type: Content.Client.Cargo.Systems.ClientPriceGunSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Systems;
using Content.Shared.Timing;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Cargo.Systems;

public sealed class ClientPriceGunSystem : SharedPriceGunSystem
{
  [Dependency]
  private UseDelaySystem _useDelay;

  protected override bool GetPriceOrBounty(
    Entity<PriceGunComponent> entity,
    EntityUid target,
    EntityUid user)
  {
    UseDelayComponent useDelayComponent;
    return this.TryComp<UseDelayComponent>(Entity<PriceGunComponent>.op_Implicit(entity), ref useDelayComponent) && !this._useDelay.IsDelayed(Entity<UseDelayComponent>.op_Implicit((Entity<PriceGunComponent>.op_Implicit(entity), useDelayComponent)));
  }
}
