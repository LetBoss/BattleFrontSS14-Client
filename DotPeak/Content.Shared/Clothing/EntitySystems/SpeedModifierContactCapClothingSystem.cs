// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.SpeedModifierContactCapClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Inventory;
using Content.Shared.Movement.Events;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class SpeedModifierContactCapClothingSystem : EntitySystem
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpeedModifierContactCapClothingComponent, InventoryRelayedEvent<GetSpeedModifierContactCapEvent>>(new EntityEventRefHandler<SpeedModifierContactCapClothingComponent, InventoryRelayedEvent<GetSpeedModifierContactCapEvent>>((object) this, __methodptr(OnGetMaxSlow)), (Type[]) null, (Type[]) null);
  }

  private void OnGetMaxSlow(
    Entity<SpeedModifierContactCapClothingComponent> ent,
    ref InventoryRelayedEvent<GetSpeedModifierContactCapEvent> args)
  {
    args.Args.SetIfMax(ent.Comp.MaxContactSprintSlowdown, ent.Comp.MaxContactWalkSlowdown);
  }
}
