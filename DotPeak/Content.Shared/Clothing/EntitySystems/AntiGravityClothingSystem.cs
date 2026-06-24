// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.AntiGravityClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class AntiGravityClothingSystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AntiGravityClothingComponent, InventoryRelayedEvent<IsWeightlessEvent>>(new EntityEventRefHandler<AntiGravityClothingComponent, InventoryRelayedEvent<IsWeightlessEvent>>((object) this, __methodptr(OnIsWeightless)), (Type[]) null, (Type[]) null);
  }

  private void OnIsWeightless(
    Entity<AntiGravityClothingComponent> ent,
    ref InventoryRelayedEvent<IsWeightlessEvent> args)
  {
    if (args.Args.Handled)
      return;
    args.Args.Handled = true;
    args.Args.IsWeightless = true;
  }
}
