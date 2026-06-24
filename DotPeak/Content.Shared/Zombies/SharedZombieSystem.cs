// Decompiled with JetBrains decompiler
// Type: Content.Shared.Zombies.SharedZombieSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Armor;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Content.Shared.NameModifier.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using System;

#nullable enable
namespace Content.Shared.Zombies;

public abstract class SharedZombieSystem : EntitySystem
{
  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ZombieComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<ZombieComponent, RefreshMovementSpeedModifiersEvent>(this.OnRefreshSpeed));
    this.SubscribeLocalEvent<ZombieComponent, RefreshNameModifiersEvent>(new EntityEventRefHandler<ZombieComponent, RefreshNameModifiersEvent>(this.OnRefreshNameModifiers));
    this.SubscribeLocalEvent<ZombificationResistanceComponent, ArmorExamineEvent>(new EntityEventRefHandler<ZombificationResistanceComponent, ArmorExamineEvent>(this.OnArmorExamine));
    this.SubscribeLocalEvent<ZombificationResistanceComponent, InventoryRelayedEvent<ZombificationResistanceQueryEvent>>(new EntityEventRefHandler<ZombificationResistanceComponent, InventoryRelayedEvent<ZombificationResistanceQueryEvent>>(this.OnResistanceQuery));
  }

  private void OnResistanceQuery(
    Entity<ZombificationResistanceComponent> ent,
    ref InventoryRelayedEvent<ZombificationResistanceQueryEvent> query)
  {
    query.Args.TotalCoefficient *= ent.Comp.ZombificationResistanceCoefficient;
  }

  private void OnArmorExamine(
    Entity<ZombificationResistanceComponent> ent,
    ref ArmorExamineEvent args)
  {
    float num = MathF.Round((float) ((1.0 - (double) ent.Comp.ZombificationResistanceCoefficient) * 100.0), 1);
    if ((double) num == 0.0)
      return;
    args.Msg.PushNewline();
    args.Msg.AddMarkupOrThrow(this.Loc.GetString((string) ent.Comp.Examine, ("value", (object) num)));
  }

  private void OnRefreshSpeed(
    EntityUid uid,
    ZombieComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    float movementSpeedDebuff = component.ZombieMovementSpeedDebuff;
    args.ModifySpeed(movementSpeedDebuff, movementSpeedDebuff);
  }

  private void OnRefreshNameModifiers(
    Entity<ZombieComponent> entity,
    ref RefreshNameModifiersEvent args)
  {
    args.AddModifier((LocId) "zombie-name-prefix");
  }
}
