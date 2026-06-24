// Decompiled with JetBrains decompiler
// Type: Content.Shared.PneumaticCannon.SharedPneumaticCannonSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.PneumaticCannon;

public abstract class SharedPneumaticCannonSystem : EntitySystem
{
  [Dependency]
  protected SharedContainerSystem Container;
  [Dependency]
  protected SharedPopupSystem Popup;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PneumaticCannonComponent, AttemptShootEvent>(new ComponentEventRefHandler<PneumaticCannonComponent, AttemptShootEvent>(this.OnAttemptShoot));
  }

  private void OnAttemptShoot(
    EntityUid uid,
    PneumaticCannonComponent component,
    ref AttemptShootEvent args)
  {
    if ((double) component.GasUsage == 0.0)
      return;
    args.ThrowItems = component.ThrowItems;
    BaseContainer container;
    if (this.Container.TryGetContainer(uid, "gas_tank", out container) && container is ContainerSlot containerSlot && containerSlot.ContainedEntity.HasValue)
      return;
    args.Cancelled = true;
  }
}
