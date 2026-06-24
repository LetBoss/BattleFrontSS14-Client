// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.PilotedClothingSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Inventory.Events;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Storage;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class PilotedClothingSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedMoverController _moverController;
  [Dependency]
  private EntityWhitelistSystem _whitelist;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PilotedClothingComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<PilotedClothingComponent, EntInsertedIntoContainerMessage>((object) this, __methodptr(OnEntInserted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PilotedClothingComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<PilotedClothingComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnEntRemoved)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PilotedClothingComponent, GotEquippedEvent>(new EntityEventRefHandler<PilotedClothingComponent, GotEquippedEvent>((object) this, __methodptr(OnEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PilotedClothingComponent, GotUnequippedEvent>(new EntityEventRefHandler<PilotedClothingComponent, GotUnequippedEvent>((object) this, __methodptr(OnUnequipped)), (Type[]) null, (Type[]) null);
  }

  private void OnEntInserted(
    Entity<PilotedClothingComponent> entity,
    ref EntInsertedIntoContainerMessage args)
  {
    StorageComponent storageComponent;
    if (!this.TryComp<StorageComponent>(Entity<PilotedClothingComponent>.op_Implicit(entity), ref storageComponent) || ((ContainerModifiedMessage) args).Container != storageComponent.Container || this._whitelist.IsWhitelistFail(entity.Comp.PilotWhitelist, ((ContainerModifiedMessage) args).Entity))
      return;
    entity.Comp.Pilot = new EntityUid?(((ContainerModifiedMessage) args).Entity);
    this.Dirty<PilotedClothingComponent>(entity, (MetaDataComponent) null);
    this.StartPiloting(entity);
  }

  private void OnEntRemoved(
    Entity<PilotedClothingComponent> entity,
    ref EntRemovedFromContainerMessage args)
  {
    EntityUid entity1 = ((ContainerModifiedMessage) args).Entity;
    EntityUid? pilot = entity.Comp.Pilot;
    if ((pilot.HasValue ? (EntityUid.op_Inequality(entity1, pilot.GetValueOrDefault()) ? 1 : 0) : 1) != 0)
      return;
    this.StopPiloting(entity);
    entity.Comp.Pilot = new EntityUid?();
    this.Dirty<PilotedClothingComponent>(entity, (MetaDataComponent) null);
  }

  private void OnEquipped(Entity<PilotedClothingComponent> entity, ref GotEquippedEvent args)
  {
    ClothingComponent clothingComponent;
    if (!this.TryComp<ClothingComponent>(Entity<PilotedClothingComponent>.op_Implicit(entity), ref clothingComponent) || (clothingComponent.Slots & args.SlotFlags) == 0)
      return;
    entity.Comp.Wearer = new EntityUid?(args.Equipee);
    this.Dirty<PilotedClothingComponent>(entity, (MetaDataComponent) null);
    this.StartPiloting(entity);
  }

  private void OnUnequipped(Entity<PilotedClothingComponent> entity, ref GotUnequippedEvent args)
  {
    this.StopPiloting(entity);
    entity.Comp.Wearer = new EntityUid?();
    this.Dirty<PilotedClothingComponent>(entity, (MetaDataComponent) null);
  }

  private bool StartPiloting(Entity<PilotedClothingComponent> entity)
  {
    if (!entity.Comp.Pilot.HasValue || !entity.Comp.Wearer.HasValue || !this._timing.IsFirstTimePredicted)
      return false;
    EntityUid entityUid1 = entity.Comp.Pilot.Value;
    EntityUid entityUid2 = entity.Comp.Wearer.Value;
    this.EnsureComp<PilotedByClothingComponent>(entityUid2);
    if (entity.Comp.RelayMovement)
      this._moverController.SetRelay(entityUid1, entityUid2);
    StartedPilotingClothingEvent pilotingClothingEvent = new StartedPilotingClothingEvent(Entity<PilotedClothingComponent>.op_Implicit(entity), entityUid2);
    this.RaiseLocalEvent<StartedPilotingClothingEvent>(entityUid1, ref pilotingClothingEvent, false);
    StartingBeingPilotedByClothing pilotedByClothing = new StartingBeingPilotedByClothing(Entity<PilotedClothingComponent>.op_Implicit(entity), entityUid1);
    this.RaiseLocalEvent<StartingBeingPilotedByClothing>(entityUid2, ref pilotedByClothing, false);
    return true;
  }

  private bool StopPiloting(Entity<PilotedClothingComponent> entity)
  {
    if (!entity.Comp.Pilot.HasValue || !entity.Comp.Wearer.HasValue)
      return false;
    EntityUid Pilot = entity.Comp.Pilot.Value;
    this.RemCompDeferred<RelayInputMoverComponent>(Pilot);
    EntityUid Wearer = entity.Comp.Wearer.Value;
    this.RemCompDeferred<MovementRelayTargetComponent>(Wearer);
    this.RemCompDeferred<PilotedByClothingComponent>(Wearer);
    StoppedPilotingClothingEvent pilotingClothingEvent = new StoppedPilotingClothingEvent(Entity<PilotedClothingComponent>.op_Implicit(entity), Wearer);
    this.RaiseLocalEvent<StoppedPilotingClothingEvent>(Pilot, ref pilotingClothingEvent, false);
    StoppedBeingPilotedByClothing pilotedByClothing = new StoppedBeingPilotedByClothing(Entity<PilotedClothingComponent>.op_Implicit(entity), Pilot);
    this.RaiseLocalEvent<StoppedBeingPilotedByClothing>(Wearer, ref pilotedByClothing, false);
    return true;
  }
}
