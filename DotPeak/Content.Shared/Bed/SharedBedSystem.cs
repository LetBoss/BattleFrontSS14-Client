// Decompiled with JetBrains decompiler
// Type: Content.Shared.Bed.SharedBedSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Bed.Components;
using Content.Shared.Bed.Sleep;
using Content.Shared.Body.Events;
using Content.Shared.Body.Systems;
using Content.Shared.Buckle.Components;
using Content.Shared.Emag.Systems;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Bed;

public abstract class SharedBedSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private ActionContainerSystem _actConts;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private EmagSystem _emag;
  [Dependency]
  private SharedMetabolizerSystem _metabolizer;
  [Dependency]
  private SharedPowerReceiverSystem _powerReceiver;
  [Dependency]
  private SleepingSystem _sleepingSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HealOnBuckleComponent, MapInitEvent>(new EntityEventRefHandler<HealOnBuckleComponent, MapInitEvent>((object) this, __methodptr(OnHealMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HealOnBuckleComponent, StrappedEvent>(new EntityEventRefHandler<HealOnBuckleComponent, StrappedEvent>((object) this, __methodptr(OnStrapped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HealOnBuckleComponent, UnstrappedEvent>(new EntityEventRefHandler<HealOnBuckleComponent, UnstrappedEvent>((object) this, __methodptr(OnUnstrapped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StasisBedComponent, StrappedEvent>(new EntityEventRefHandler<StasisBedComponent, StrappedEvent>((object) this, __methodptr(OnStasisStrapped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StasisBedComponent, UnstrappedEvent>(new EntityEventRefHandler<StasisBedComponent, UnstrappedEvent>((object) this, __methodptr(OnStasisUnstrapped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StasisBedComponent, GotEmaggedEvent>(new EntityEventRefHandler<StasisBedComponent, GotEmaggedEvent>((object) this, __methodptr(OnStasisEmagged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StasisBedComponent, PowerChangedEvent>(new EntityEventRefHandler<StasisBedComponent, PowerChangedEvent>((object) this, __methodptr(OnPowerChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<StasisBedBuckledComponent, GetMetabolicMultiplierEvent>(new EntityEventRefHandler<StasisBedBuckledComponent, GetMetabolicMultiplierEvent>((object) this, __methodptr(OnStasisGetMetabolicMultiplier)), (Type[]) null, (Type[]) null);
  }

  private void OnHealMapInit(Entity<HealOnBuckleComponent> ent, ref MapInitEvent args)
  {
    this._actConts.EnsureAction(ent.Owner, ref ent.Comp.SleepAction, EntProtoId.op_Implicit(SleepingSystem.SleepActionId));
    this.Dirty<HealOnBuckleComponent>(ent, (MetaDataComponent) null);
  }

  private void OnStrapped(Entity<HealOnBuckleComponent> bed, ref StrappedEvent args)
  {
    this.EnsureComp<HealOnBuckleHealingComponent>(Entity<HealOnBuckleComponent>.op_Implicit(bed));
    bed.Comp.NextHealTime = this.Timing.CurTime + TimeSpan.FromSeconds((double) bed.Comp.HealTime);
    this._actionsSystem.AddAction(Entity<BuckleComponent>.op_Implicit(args.Buckle), ref bed.Comp.SleepAction, EntProtoId.op_Implicit(SleepingSystem.SleepActionId), Entity<HealOnBuckleComponent>.op_Implicit(bed));
    this.Dirty<HealOnBuckleComponent>(bed, (MetaDataComponent) null);
  }

  private void OnUnstrapped(Entity<HealOnBuckleComponent> bed, ref UnstrappedEvent args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(args.Buckle.Owner);
    EntityUid? nullable = bed.Comp.SleepAction;
    Entity<ActionComponent>? action = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
    SleepingSystem sleepingSystem = this._sleepingSystem;
    Entity<SleepingComponent> ent = Entity<SleepingComponent>.op_Implicit(args.Buckle.Owner);
    nullable = new EntityUid?();
    EntityUid? user = nullable;
    sleepingSystem.TryWaking(ent, user: user);
    this.RemComp<HealOnBuckleHealingComponent>(Entity<HealOnBuckleComponent>.op_Implicit(bed));
  }

  private void OnStasisStrapped(Entity<StasisBedComponent> ent, ref StrappedEvent args)
  {
    this.EnsureComp<StasisBedBuckledComponent>(Entity<BuckleComponent>.op_Implicit(args.Buckle));
    this._metabolizer.UpdateMetabolicMultiplier(Entity<BuckleComponent>.op_Implicit(args.Buckle));
  }

  private void OnStasisUnstrapped(Entity<StasisBedComponent> ent, ref UnstrappedEvent args)
  {
    this.RemComp<StasisBedBuckledComponent>(Entity<StasisBedComponent>.op_Implicit(ent));
    this._metabolizer.UpdateMetabolicMultiplier(Entity<BuckleComponent>.op_Implicit(args.Buckle));
  }

  private void OnStasisEmagged(Entity<StasisBedComponent> ent, ref GotEmaggedEvent args)
  {
    if (!this._emag.CompareFlag(args.Type, EmagType.Interaction) || this._emag.CheckFlag(Entity<StasisBedComponent>.op_Implicit(ent), EmagType.Interaction))
      return;
    ent.Comp.Multiplier = 1f / ent.Comp.Multiplier;
    this.UpdateMetabolisms(Entity<StrapComponent>.op_Implicit(ent.Owner));
    this.Dirty<StasisBedComponent>(ent, (MetaDataComponent) null);
    args.Handled = true;
  }

  private void OnPowerChanged(Entity<StasisBedComponent> ent, ref PowerChangedEvent args)
  {
    this.UpdateMetabolisms(Entity<StrapComponent>.op_Implicit(ent.Owner));
  }

  private void OnStasisGetMetabolicMultiplier(
    Entity<StasisBedBuckledComponent> ent,
    ref GetMetabolicMultiplierEvent args)
  {
    BuckleComponent buckleComponent;
    if (!this.TryComp<BuckleComponent>(Entity<StasisBedBuckledComponent>.op_Implicit(ent), ref buckleComponent))
      return;
    EntityUid? buckledTo = buckleComponent.BuckledTo;
    if (!buckledTo.HasValue)
      return;
    EntityUid valueOrDefault = buckledTo.GetValueOrDefault();
    StasisBedComponent stasisBedComponent;
    if (!this.TryComp<StasisBedComponent>(valueOrDefault, ref stasisBedComponent) || !this._powerReceiver.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(valueOrDefault)))
      return;
    args.Multiplier *= stasisBedComponent.Multiplier;
  }

  protected void UpdateMetabolisms(Entity<StrapComponent?> ent)
  {
    if (!this.Resolve<StrapComponent>(Entity<StrapComponent>.op_Implicit(ent), ref ent.Comp, false))
      return;
    foreach (EntityUid buckledEntity in ent.Comp.BuckledEntities)
      this._metabolizer.UpdateMetabolicMultiplier(buckledEntity);
  }
}
