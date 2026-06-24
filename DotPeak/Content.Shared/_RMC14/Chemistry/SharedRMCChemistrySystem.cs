// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.SharedRMCChemistrySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Chemistry;

public abstract class SharedRMCChemistrySystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private RMCReagentSystem _reagents;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private IGameTiming _timing;
  private readonly List<Entity<RMCChemicalDispenserComponent>> _dispensers = new List<Entity<RMCChemicalDispenserComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SolutionComponent, ComponentGetState>(new EntityEventRefHandler<SolutionComponent, ComponentGetState>(this.OnSolutionGetState));
    this.SubscribeLocalEvent<SolutionComponent, ComponentHandleState>(new EntityEventRefHandler<SolutionComponent, ComponentHandleState>(this.OnSolutionHandleState));
    this.SubscribeLocalEvent<DetailedExaminableSolutionComponent, ExaminedEvent>(new EntityEventRefHandler<DetailedExaminableSolutionComponent, ExaminedEvent>(this.OnDetailedSolutionExamined));
    this.SubscribeLocalEvent<RMCChemicalDispenserComponent, MapInitEvent>(new EntityEventRefHandler<RMCChemicalDispenserComponent, MapInitEvent>(this.OnDispenserMapInit));
    this.SubscribeLocalEvent<RMCToggleableSolutionTransferComponent, MapInitEvent>(new EntityEventRefHandler<RMCToggleableSolutionTransferComponent, MapInitEvent>(this.OnToggleableSolutionTransferMapInit));
    this.SubscribeLocalEvent<RMCToggleableSolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCToggleableSolutionTransferComponent, GetVerbsEvent<AlternativeVerb>>(this.OnToggleableSolutionTransferVerbs));
    this.SubscribeLocalEvent<RMCSolutionTransferWhitelistComponent, SolutionTransferAttemptEvent>(new EntityEventRefHandler<RMCSolutionTransferWhitelistComponent, SolutionTransferAttemptEvent>(this.OnTransferWhitelistAttempt));
    this.SubscribeLocalEvent<NoMixingReagentsComponent, ExaminedEvent>(new EntityEventRefHandler<NoMixingReagentsComponent, ExaminedEvent>(this.OnNoMixingReagentsExamined));
    this.SubscribeLocalEvent<NoMixingReagentsComponent, SolutionTransferAttemptEvent>(new EntityEventRefHandler<NoMixingReagentsComponent, SolutionTransferAttemptEvent>(this.OnNoMixingReagentsTransferAttempt));
    this.SubscribeLocalEvent<RMCEmptySolutionComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCEmptySolutionComponent, GetVerbsEvent<AlternativeVerb>>(this.OnEmptySolutionGetVerbs));
    this.Subs.BuiEvents<RMCChemicalDispenserComponent>((object) RMCChemicalDispenserUi.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCChemicalDispenserComponent>) (subs =>
    {
      subs.Event<RMCChemicalDispenserDispenseSettingBuiMsg>(new EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserDispenseSettingBuiMsg>(this.OnChemicalDispenserSettingMsg));
      subs.Event<RMCChemicalDispenserBeakerBuiMsg>(new EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserBeakerBuiMsg>(this.OnChemicalDispenserBeakerSettingMsg));
      subs.Event<RMCChemicalDispenserEjectBeakerBuiMsg>(new EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserEjectBeakerBuiMsg>(this.OnChemicalDispenserEjectBeakerMsg));
      subs.Event<RMCChemicalDispenserDispenseBuiMsg>(new EntityEventRefHandler<RMCChemicalDispenserComponent, RMCChemicalDispenserDispenseBuiMsg>(this.OnChemicalDispenserDispenseMsg));
    }));
  }

  private void OnSolutionGetState(Entity<SolutionComponent> ent, ref ComponentGetState args)
  {
    Solution solution = new Solution(ent.Comp.Solution, this._prototypes);
    args.State = (IComponentState) new SolutionComponentState(solution);
  }

  private void OnSolutionHandleState(Entity<SolutionComponent> ent, ref ComponentHandleState args)
  {
    if (!(args.Current is SolutionComponentState current))
      return;
    ent.Comp.Solution = new Solution(current.Solution, this._prototypes);
  }

  private void OnDetailedSolutionExamined(
    Entity<DetailedExaminableSolutionComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("DetailedExaminableSolutionComponent"))
    {
      args.PushText("It contains:");
      Solution solution;
      if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.Solution, out Entity<SolutionComponent>? _, out solution) || solution.Volume <= FixedPoint2.Zero)
      {
        args.PushText("Nothing.");
      }
      else
      {
        foreach (ReagentQuantity content in solution.Contents)
        {
          ReagentId reagent1 = content.Reagent;
          string str = reagent1.Prototype;
          RMCReagentSystem reagents = this._reagents;
          reagent1 = content.Reagent;
          ProtoId<ReagentPrototype> prototype = (ProtoId<ReagentPrototype>) reagent1.Prototype;
          Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent2;
          ref Content.Shared._RMC14.Chemistry.Reagent.Reagent local = ref reagent2;
          if (reagents.TryIndex(prototype, out local))
            str = reagent2.LocalizedName;
          args.PushText($"{content.Quantity.Float():F2} units of {str}");
        }
        args.PushText($"Total volume: {solution.Volume} / {solution.MaxVolume}.");
      }
      RMCToggleableSolutionTransferComponent comp;
      if (!this.TryComp<RMCToggleableSolutionTransferComponent>(ent.Owner, out comp))
        return;
      string str1;
      switch (comp.Direction)
      {
        case SolutionTransferDirection.Input:
          str1 = "Transfer mode: Drawing";
          break;
        case SolutionTransferDirection.Output:
          str1 = "Transfer mode: Dispensing";
          break;
        default:
          str1 = string.Empty;
          break;
      }
      string text = str1;
      if (string.IsNullOrEmpty(text))
        return;
      args.PushText(text);
    }
  }

  private void OnDispenserMapInit(Entity<RMCChemicalDispenserComponent> ent, ref MapInitEvent args)
  {
    this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.ContainerSlotId);
  }

  private void OnToggleableSolutionTransferMapInit(
    Entity<RMCToggleableSolutionTransferComponent> ent,
    ref MapInitEvent args)
  {
    ent.Comp.Direction = SolutionTransferDirection.Input;
    this.RemCompDeferred<DrainableSolutionComponent>((EntityUid) ent);
    RefillableSolutionComponent solutionComponent = this.EnsureComp<RefillableSolutionComponent>((EntityUid) ent);
    solutionComponent.Solution = ent.Comp.Solution;
    this.Dirty((EntityUid) ent, (IComponent) solutionComponent);
  }

  private void OnToggleableSolutionTransferVerbs(
    Entity<RMCToggleableSolutionTransferComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    bool dispensing = this.HasComp<DrainableSolutionComponent>((EntityUid) ent);
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = dispensing ? "Enable drawing" : "Enable dispensing";
    alternativeVerb.Act = (Action) (() =>
    {
      dispensing = this.HasComp<DrainableSolutionComponent>((EntityUid) ent);
      if (dispensing)
      {
        this.RemCompDeferred<DrainableSolutionComponent>((EntityUid) ent);
        RefillableSolutionComponent solutionComponent = this.EnsureComp<RefillableSolutionComponent>((EntityUid) ent);
        solutionComponent.Solution = ent.Comp.Solution;
        ent.Comp.Direction = SolutionTransferDirection.Input;
        this.Dirty((EntityUid) ent, (IComponent) solutionComponent);
        this._popup.PopupClient("Now drawing", (EntityUid) ent, new EntityUid?(user), PopupType.Medium);
      }
      else
      {
        this.RemCompDeferred<RefillableSolutionComponent>((EntityUid) ent);
        DrainableSolutionComponent solutionComponent = this.EnsureComp<DrainableSolutionComponent>((EntityUid) ent);
        solutionComponent.Solution = ent.Comp.Solution;
        ent.Comp.Direction = SolutionTransferDirection.Output;
        this.Dirty((EntityUid) ent, (IComponent) solutionComponent);
        this._popup.PopupClient("Now dispensing", (EntityUid) ent, new EntityUid?(user), PopupType.Medium);
      }
    });
    verbs.Add(alternativeVerb);
  }

  private void OnTransferWhitelistAttempt(
    Entity<RMCSolutionTransferWhitelistComponent> ent,
    ref SolutionTransferAttemptEvent args)
  {
    if (ent.Owner == args.From)
    {
      if (!this._entityWhitelist.IsWhitelistFail(ent.Comp.SourceWhitelist, args.To))
        return;
      args.Cancel(this.Loc.GetString((string) ent.Comp.Popup));
    }
    else
    {
      if (!this._entityWhitelist.IsWhitelistFail(ent.Comp.TargetWhitelist, args.From))
        return;
      args.Cancel(this.Loc.GetString((string) ent.Comp.Popup));
    }
  }

  private void OnNoMixingReagentsExamined(
    Entity<NoMixingReagentsComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("NoMixingReagentsComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-fuel-examine-cant-mix"));
  }

  private void OnNoMixingReagentsTransferAttempt(
    Entity<NoMixingReagentsComponent> ent,
    ref SolutionTransferAttemptEvent args)
  {
    Solution solution1 = args.FromSolution.Comp.Solution;
    Solution solution2 = args.ToSolution.Comp.Solution;
    if (solution2.Contents.Count > 1)
    {
      args.Cancel(this.Loc.GetString("rmc-fuel-cant-mix"));
    }
    else
    {
      foreach (ReagentQuantity content in solution2.Contents)
      {
        if (solution1.Volume > FixedPoint2.Zero && !solution1.ContainsReagent(content.Reagent))
        {
          args.Cancel(this.Loc.GetString("rmc-fuel-cant-mix"));
          break;
        }
      }
    }
  }

  private void OnEmptySolutionGetVerbs(
    Entity<RMCEmptySolutionComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    Entity<SolutionComponent>? solutionEnt;
    if (!args.CanAccess || !args.CanComplexInteract || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.Solution, out solutionEnt, out Solution _) || solutionEnt.Value.Comp.Solution.Volume <= FixedPoint2.Zero)
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("rmc-empty-solution-verb");
    alternativeVerb.Act = (Action) (() =>
    {
      if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.Solution, out solutionEnt, out Solution _))
        return;
      this._solution.RemoveAllSolution(solutionEnt.Value);
    });
    alternativeVerb.Priority = 1;
    verbs.Add(alternativeVerb);
  }

  private void OnChemicalDispenserSettingMsg(
    Entity<RMCChemicalDispenserComponent> ent,
    ref RMCChemicalDispenserDispenseSettingBuiMsg args)
  {
    if (!((IEnumerable<FixedPoint2>) ent.Comp.Settings).Contains<FixedPoint2>(args.Amount))
      return;
    ent.Comp.DispenseSetting = args.Amount;
    this.Dirty<RMCChemicalDispenserComponent>(ent);
  }

  private void OnChemicalDispenserBeakerSettingMsg(
    Entity<RMCChemicalDispenserComponent> ent,
    ref RMCChemicalDispenserBeakerBuiMsg args)
  {
    ItemSlot itemSlot;
    if (!this._itemSlots.TryGetSlot((EntityUid) ent, ent.Comp.ContainerSlotId, out itemSlot))
      return;
    EntityUid? containedEntity = (EntityUid?) itemSlot.ContainerSlot?.ContainedEntity;
    Entity<SolutionComponent>? soln;
    if (!containedEntity.HasValue || !this._solution.TryGetMixableSolution((Entity<MixableSolutionComponent, SolutionContainerManagerComponent>) containedEntity.GetValueOrDefault(), out soln, out Solution _) || !((IEnumerable<FixedPoint2>) ent.Comp.Settings).Contains<FixedPoint2>(args.Amount))
      return;
    this._solution.SplitSolution(soln.Value, args.Amount);
    this.DispenserUpdated(ent);
  }

  private void OnChemicalDispenserEjectBeakerMsg(
    Entity<RMCChemicalDispenserComponent> ent,
    ref RMCChemicalDispenserEjectBeakerBuiMsg args)
  {
    ItemSlot itemSlot;
    if (!this._itemSlots.TryGetSlot((EntityUid) ent, ent.Comp.ContainerSlotId, out itemSlot))
      return;
    this._itemSlots.TryEjectToHands((EntityUid) ent, itemSlot, new EntityUid?(args.Actor), true);
    this.Dirty<RMCChemicalDispenserComponent>(ent);
  }

  private void OnChemicalDispenserDispenseMsg(
    Entity<RMCChemicalDispenserComponent> ent,
    ref RMCChemicalDispenserDispenseBuiMsg args)
  {
    ItemSlot itemSlot;
    if (!this._itemSlots.TryGetSlot((EntityUid) ent, ent.Comp.ContainerSlotId, out itemSlot))
      return;
    EntityUid? containedEntity = (EntityUid?) itemSlot.ContainerSlot?.ContainedEntity;
    Entity<SolutionComponent>? soln;
    Entity<RMCChemicalStorageComponent> storage;
    if (!containedEntity.HasValue || !this._solution.TryGetMixableSolution((Entity<MixableSolutionComponent, SolutionContainerManagerComponent>) containedEntity.GetValueOrDefault(), out soln, out Solution _) || !((IEnumerable<ProtoId<ReagentPrototype>>) ent.Comp.Reagents).Contains<ProtoId<ReagentPrototype>>(args.Reagent) || !this.TryGetStorage(ent.Comp.Network, out storage))
      return;
    FixedPoint2 fixedPoint2_1 = ent.Comp.DispenseSetting;
    FixedPoint2 availableVolume = soln.Value.Comp.Solution.AvailableVolume;
    if (fixedPoint2_1 > availableVolume)
      fixedPoint2_1 = availableVolume;
    if (fixedPoint2_1 == FixedPoint2.Zero)
      return;
    FixedPoint2 fixedPoint2_2 = ent.Comp.FreeReagents.Contains(args.Reagent) ? FixedPoint2.Zero : ent.Comp.CostPerUnit * fixedPoint2_1;
    if (fixedPoint2_2 > storage.Comp.Energy)
      return;
    this.ChangeStorageEnergy(storage, storage.Comp.Energy - fixedPoint2_2);
    this._solution.TryAddReagent(soln.Value, (string) args.Reagent, ent.Comp.DispenseSetting);
  }

  public bool TryGetStorage(EntProtoId network, out Entity<RMCChemicalStorageComponent> storage)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCChemicalStorageComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCChemicalStorageComponent>();
    EntityUid uid;
    RMCChemicalStorageComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!this.IsClientSide(uid) && comp1.Network == network)
      {
        storage = (Entity<RMCChemicalStorageComponent>) (uid, comp1);
        return true;
      }
    }
    storage = new Entity<RMCChemicalStorageComponent>();
    return false;
  }

  public void ChangeStorageEnergy(Entity<RMCChemicalStorageComponent> storage, FixedPoint2 energy)
  {
    storage.Comp.Energy = FixedPoint2.Clamp(energy, FixedPoint2.Zero, storage.Comp.MaxEnergy);
    this.Dirty<RMCChemicalStorageComponent>(storage);
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCChemicalDispenserComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCChemicalDispenserComponent>();
    EntityUid uid;
    RMCChemicalDispenserComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (!(comp1.Network != storage.Comp.Network))
      {
        comp1.Energy = energy;
        this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  protected virtual void DispenserUpdated(Entity<RMCChemicalDispenserComponent> ent)
  {
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCChemicalStorageComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCChemicalStorageComponent>();
    EntityUid uid1;
    RMCChemicalStorageComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (!(curTime < comp1_1.RechargeAt))
      {
        comp1_1.RechargeAt = curTime + comp1_1.RechargeEvery;
        this.Dirty(uid1, (IComponent) comp1_1);
        TransformComponent transformComponent1 = this.Transform(uid1);
        this._dispensers.Clear();
        Robust.Shared.GameObjects.EntityQueryEnumerator<RMCChemicalDispenserComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCChemicalDispenserComponent>();
        EntityUid uid2;
        RMCChemicalDispenserComponent comp1_2;
        while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
        {
          TransformComponent transformComponent2 = this.Transform(uid2);
          if (comp1_2.Network == comp1_1.Network)
          {
            EntityUid? gridUid1 = transformComponent1.GridUid;
            EntityUid? gridUid2 = transformComponent2.GridUid;
            if ((gridUid1.HasValue == gridUid2.HasValue ? (gridUid1.HasValue ? (gridUid1.GetValueOrDefault() == gridUid2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
              this._dispensers.Add((Entity<RMCChemicalDispenserComponent>) (uid2, comp1_2));
          }
        }
        comp1_1.MaxEnergy = comp1_1.BaseMax + comp1_1.MaxPer * this._dispensers.Count;
        comp1_1.Recharge = comp1_1.BaseRecharge + comp1_1.RechargePer * this._dispensers.Count;
        if (!comp1_1.Updated)
        {
          comp1_1.Updated = true;
          comp1_1.Energy = comp1_1.MaxEnergy;
        }
        else
          comp1_1.Energy = FixedPoint2.Min(comp1_1.MaxEnergy, comp1_1.Energy + comp1_1.Recharge);
        foreach (Entity<RMCChemicalDispenserComponent> dispenser in this._dispensers)
        {
          dispenser.Comp.Energy = comp1_1.Energy;
          dispenser.Comp.MaxEnergy = comp1_1.MaxEnergy;
          this.Dirty<RMCChemicalDispenserComponent>(dispenser);
        }
      }
    }
  }
}
