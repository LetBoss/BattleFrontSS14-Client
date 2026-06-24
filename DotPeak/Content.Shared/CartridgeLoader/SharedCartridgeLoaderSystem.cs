// Decompiled with JetBrains decompiler
// Type: Content.Shared.CartridgeLoader.SharedCartridgeLoaderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.CartridgeLoader;

public abstract class SharedCartridgeLoaderSystem : EntitySystem
{
  public const string InstalledContainerId = "program-container";
  [Dependency]
  private ItemSlotsSystem _itemSlotsSystem;
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  private SharedContainerSystem _container;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CartridgeLoaderComponent, ComponentInit>(new ComponentEventHandler<CartridgeLoaderComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CartridgeLoaderComponent, ComponentRemove>(new ComponentEventHandler<CartridgeLoaderComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
    SharedCartridgeLoaderSystem cartridgeLoaderSystem1 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<CartridgeLoaderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<CartridgeLoaderComponent, EntInsertedIntoContainerMessage>((object) cartridgeLoaderSystem1, __vmethodptr(cartridgeLoaderSystem1, OnItemInserted)), (Type[]) null, (Type[]) null);
    SharedCartridgeLoaderSystem cartridgeLoaderSystem2 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<CartridgeLoaderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<CartridgeLoaderComponent, EntRemovedFromContainerMessage>((object) cartridgeLoaderSystem2, __vmethodptr(cartridgeLoaderSystem2, OnItemRemoved)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, CartridgeLoaderComponent loader, ComponentInit args)
  {
    this._itemSlotsSystem.AddItemSlot(uid, "Cartridge-Slot", loader.CartridgeSlot);
  }

  private void OnComponentRemove(
    EntityUid uid,
    CartridgeLoaderComponent loader,
    ComponentRemove args)
  {
    this._itemSlotsSystem.RemoveItemSlot(uid, loader.CartridgeSlot);
    BaseContainer baseContainer;
    if (!this._container.TryGetContainer(uid, "program-container", ref baseContainer, (ContainerManagerComponent) null))
      return;
    this._container.ShutdownContainer(baseContainer);
  }

  protected virtual void OnItemInserted(
    EntityUid uid,
    CartridgeLoaderComponent loader,
    EntInsertedIntoContainerMessage args)
  {
    this.UpdateAppearanceData(uid, loader);
  }

  protected virtual void OnItemRemoved(
    EntityUid uid,
    CartridgeLoaderComponent loader,
    EntRemovedFromContainerMessage args)
  {
    this.UpdateAppearanceData(uid, loader);
  }

  private void UpdateAppearanceData(EntityUid uid, CartridgeLoaderComponent loader)
  {
    this._appearanceSystem.SetData(uid, (Enum) CartridgeLoaderVisuals.CartridgeInserted, (object) loader.CartridgeSlot.HasItem, (AppearanceComponent) null);
  }
}
