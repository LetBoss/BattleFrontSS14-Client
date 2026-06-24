// Decompiled with JetBrains decompiler
// Type: Content.Shared.Eye.Blinding.Systems.BlindfoldSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.Eye.Blinding.Systems;

public sealed class BlindfoldSystem : EntitySystem
{
  [Dependency]
  private BlindableSystem _blindableSystem;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<BlindfoldComponent, GotEquippedEvent>(new EntityEventRefHandler<BlindfoldComponent, GotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<BlindfoldComponent, GotUnequippedEvent>(new EntityEventRefHandler<BlindfoldComponent, GotUnequippedEvent>(this.OnUnequipped));
    this.SubscribeLocalEvent<BlindfoldComponent, InventoryRelayedEvent<CanSeeAttemptEvent>>(new EntityEventRefHandler<BlindfoldComponent, InventoryRelayedEvent<CanSeeAttemptEvent>>(this.OnBlindfoldTrySee));
  }

  private void OnBlindfoldTrySee(
    Entity<BlindfoldComponent> blindfold,
    ref InventoryRelayedEvent<CanSeeAttemptEvent> args)
  {
    args.Args.Cancel();
  }

  private void OnEquipped(Entity<BlindfoldComponent> blindfold, ref GotEquippedEvent args)
  {
    this._blindableSystem.UpdateIsBlind((Entity<BlindableComponent>) args.Equipee);
  }

  private void OnUnequipped(Entity<BlindfoldComponent> blindfold, ref GotUnequippedEvent args)
  {
    this._blindableSystem.UpdateIsBlind((Entity<BlindableComponent>) args.Equipee);
  }
}
