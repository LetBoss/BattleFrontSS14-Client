// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.SlotBasedConnectedContainerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.Components;
using Content.Shared.Inventory;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared.Containers;

public sealed class SlotBasedConnectedContainerSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _containers;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Dependency]
  private InventorySystem _inventory;

  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SlotBasedConnectedContainerComponent, GetConnectedContainerEvent>(new EntityEventRefHandler<SlotBasedConnectedContainerComponent, GetConnectedContainerEvent>((object) this, __methodptr(OnGettingConnectedContainer)), (Type[]) null, (Type[]) null);
  }

  public bool TryGetConnectedContainer(EntityUid uid, [NotNullWhen(true)] out EntityUid? slotEntity)
  {
    SlotBasedConnectedContainerComponent containerComponent;
    if (this.TryComp<SlotBasedConnectedContainerComponent>(uid, ref containerComponent))
      return this.TryGetConnectedContainer(uid, containerComponent.TargetSlot, containerComponent.ContainerWhitelist, out slotEntity);
    slotEntity = new EntityUid?();
    return false;
  }

  private void OnGettingConnectedContainer(
    Entity<SlotBasedConnectedContainerComponent> ent,
    ref GetConnectedContainerEvent args)
  {
    EntityUid? slotEntity;
    if (!this.TryGetConnectedContainer(Entity<SlotBasedConnectedContainerComponent>.op_Implicit(ent), ent.Comp.TargetSlot, ent.Comp.ContainerWhitelist, out slotEntity))
      return;
    args.ContainerEntity = slotEntity;
  }

  private bool TryGetConnectedContainer(
    EntityUid uid,
    SlotFlags slotFlag,
    EntityWhitelist? providerWhitelist,
    [NotNullWhen(true)] out EntityUid? slotEntity)
  {
    slotEntity = new EntityUid?();
    BaseContainer baseContainer;
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (!this._containers.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((uid, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer) || !this._inventory.TryGetContainerSlotEnumerator(Entity<InventoryComponent>.op_Implicit(baseContainer.Owner), out containerSlotEnumerator, slotFlag))
      return false;
    EntityUid uid1;
    while (containerSlotEnumerator.NextItem(out uid1))
    {
      if (!this._whitelistSystem.IsWhitelistFailOrNull(providerWhitelist, uid1))
      {
        slotEntity = new EntityUid?(uid1);
        return true;
      }
    }
    return false;
  }
}
