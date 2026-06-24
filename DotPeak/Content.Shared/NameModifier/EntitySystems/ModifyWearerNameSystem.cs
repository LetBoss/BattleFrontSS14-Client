// Decompiled with JetBrains decompiler
// Type: Content.Shared.NameModifier.EntitySystems.ModifyWearerNameSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing;
using Content.Shared.Inventory;
using Content.Shared.NameModifier.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.NameModifier.EntitySystems;

public sealed class ModifyWearerNameSystem : EntitySystem
{
  [Dependency]
  private NameModifierSystem _nameMod;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ModifyWearerNameComponent, InventoryRelayedEvent<RefreshNameModifiersEvent>>(new EntityEventRefHandler<ModifyWearerNameComponent, InventoryRelayedEvent<RefreshNameModifiersEvent>>(this.OnRefreshNameModifiers));
    this.SubscribeLocalEvent<ModifyWearerNameComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<ModifyWearerNameComponent, ClothingGotEquippedEvent>(this.OnGotEquipped));
    this.SubscribeLocalEvent<ModifyWearerNameComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<ModifyWearerNameComponent, ClothingGotUnequippedEvent>(this.OnGotUnequipped));
  }

  private void OnGotEquipped(
    Entity<ModifyWearerNameComponent> entity,
    ref ClothingGotEquippedEvent args)
  {
    this._nameMod.RefreshNameModifiers((Entity<NameModifierComponent>) args.Wearer);
  }

  private void OnGotUnequipped(
    Entity<ModifyWearerNameComponent> entity,
    ref ClothingGotUnequippedEvent args)
  {
    this._nameMod.RefreshNameModifiers((Entity<NameModifierComponent>) args.Wearer);
  }

  private void OnRefreshNameModifiers(
    Entity<ModifyWearerNameComponent> entity,
    ref InventoryRelayedEvent<RefreshNameModifiersEvent> args)
  {
    args.Args.AddModifier(entity.Comp.LocId, entity.Comp.Priority);
  }
}
