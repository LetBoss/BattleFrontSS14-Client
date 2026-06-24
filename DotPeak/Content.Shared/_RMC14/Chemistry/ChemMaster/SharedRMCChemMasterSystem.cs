// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.ChemMaster.SharedRMCChemMasterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry.SmartFridge;
using Content.Shared._RMC14.IconLabel;
using Content.Shared._RMC14.Storage;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Labels.Components;
using Content.Shared.Labels.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.ChemMaster;

public abstract class SharedRMCChemMasterSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private LabelSystem _label;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedRMCIconLabelSystem _rmcIconLabel;
  [Dependency]
  private SharedRMCSmartFridgeSystem _rmcSmartFridge;
  [Dependency]
  private RMCStorageSystem _rmcStorage;
  [Dependency]
  private SharedSolutionContainerSystem _solution;
  [Dependency]
  private SolutionTransferSystem _solutionTransfer;
  [Dependency]
  private SharedStorageSystem _storage;
  private readonly List<EntityUid> _toFill = new List<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCChemMasterComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCChemMasterComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<RMCChemMasterComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCChemMasterComponent, EntInsertedIntoContainerMessage>(this.OnEntInsertedIntoContainer));
    this.SubscribeLocalEvent<RMCChemMasterComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<RMCChemMasterComponent, EntRemovedFromContainerMessage>(this.OnEntRemovedFromContainer));
    this.SubscribeLocalEvent<RMCChemMasterComponent, RMCChemMasterPillBottleTransferDoAfterEvent>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleTransferDoAfterEvent>(this.OnPillBottleBoxTransferDoAfter));
    this.Subs.BuiEvents<RMCChemMasterComponent>((object) RMCChemMasterUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCChemMasterComponent>) (subs =>
    {
      subs.Event<RMCChemMasterPillBottleLabelMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleLabelMsg>(this.OnPillBottleLabelMsg));
      subs.Event<RMCChemMasterPillBottleColorMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleColorMsg>(this.OnPillBottleColorMsg));
      subs.Event<RMCChemMasterPillBottleFillMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleFillMsg>(this.OnPillBottleFillMsg));
      subs.Event<RMCChemMasterPillBottleTransferMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleTransferMsg>(this.OnPillBottleTransferMsg));
      subs.Event<RMCChemMasterPillBottleEjectMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterPillBottleEjectMsg>(this.OnPillBottleEjectMsg));
      subs.Event<RMCChemMasterBeakerEjectMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBeakerEjectMsg>(this.OnBeakerEjectMsg));
      subs.Event<RMCChemMasterBeakerTransferMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBeakerTransferMsg>(this.OnBeakerTransferMsg));
      subs.Event<RMCChemMasterBeakerTransferAllMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBeakerTransferAllMsg>(this.OnBeakerTransferAllMsg));
      subs.Event<RMCChemMasterBufferModeMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBufferModeMsg>(this.OnBufferModeMsg));
      subs.Event<RMCChemMasterBufferTransferMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBufferTransferMsg>(this.OnBufferTransferMsg));
      subs.Event<RMCChemMasterBufferTransferAllMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterBufferTransferAllMsg>(this.OnBufferTransferAllMsg));
      subs.Event<RMCChemMasterSetPillAmountMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterSetPillAmountMsg>(this.OnSetPillAmountMsg));
      subs.Event<RMCChemMasterSetPillTypeMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterSetPillTypeMsg>(this.OnSetPillTypeMsg));
      subs.Event<RMCChemMasterCreatePillsMsg>(new EntityEventRefHandler<RMCChemMasterComponent, RMCChemMasterCreatePillsMsg>(this.OnCreatePillsMsg));
    }));
  }

  private void OnInteractUsing(Entity<RMCChemMasterComponent> ent, ref InteractUsingEvent args)
  {
    RMCPillBottleTransferComponent comp1;
    StorageComponent comp2;
    if (this.TryComp<RMCPillBottleTransferComponent>(args.Used, out comp1) && this.TryComp<StorageComponent>(args.Used, out comp2))
    {
      args.Handled = true;
      Container container = this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.PillBottleContainer);
      int val2 = ent.Comp.MaxPillBottles - container.Count;
      if (val2 <= 0)
        this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-full-pill-bottles"), (EntityUid) ent, new EntityUid?(args.User));
      else if (comp2.StoredItems.Count == 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-pill-bottle-box-empty", ("box", (object) args.Used)), (EntityUid) ent, new EntityUid?(args.User));
      }
      else
      {
        TimeSpan delay = TimeSpan.FromSeconds((double) Math.Min(comp2.StoredItems.Count, val2) * (double) comp1.TimePerBottle);
        RMCChemMasterPillBottleTransferDoAfterEvent @event = new RMCChemMasterPillBottleTransferDoAfterEvent();
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, delay, (DoAfterEvent) @event, new EntityUid?(ent.Owner), new EntityUid?(ent.Owner), new EntityUid?(args.Used))
        {
          BreakOnMove = true,
          NeedHand = true
        }))
          return;
        this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-pill-bottle-box-start", ("box", (object) args.Used), ("target", (object) ent)), args.User, new EntityUid?(args.User));
      }
    }
    else
    {
      if (!this._entityWhitelist.IsWhitelistPass(ent.Comp.PillBottleWhitelist, args.Used))
        return;
      args.Handled = true;
      Container container = this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.PillBottleContainer);
      if (container.Count >= ent.Comp.MaxPillBottles)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-full-pill-bottles"), (EntityUid) ent, new EntityUid?(args.User));
      }
      else
      {
        this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) container);
        this._audio.PlayPredicted(ent.Comp.PillBottleInsertSound, (EntityUid) ent, new EntityUid?(args.User));
      }
    }
  }

  protected virtual void OnEntInsertedIntoContainer(
    Entity<RMCChemMasterComponent> ent,
    ref EntInsertedIntoContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.BufferSolutionId)
      return;
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  protected virtual void OnEntRemovedFromContainer(
    Entity<RMCChemMasterComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.BufferSolutionId)
      return;
    ent.Comp.SelectedBottles.Remove(args.Entity);
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnPillBottleLabelMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterPillBottleLabelMsg args)
  {
    string text = args.Label;
    if (text.Length > ent.Comp.MaxLabelLength)
      text = text.Substring(0, ent.Comp.MaxLabelLength);
    foreach (EntityUid selectedBottle in ent.Comp.SelectedBottles)
    {
      BaseContainer container;
      if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (selectedBottle, (TransformComponent) null), out container) && !(container.Owner != ent.Owner))
      {
        this._label.Label(selectedBottle, text);
        this._rmcIconLabel.Label((Entity<IconLabelComponent>) selectedBottle, (LocId) "rmc-custom-container-label-text", ("customLabel", (object) text));
      }
    }
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnPillBottleColorMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterPillBottleColorMsg args)
  {
    foreach (EntityUid selectedBottle in ent.Comp.SelectedBottles)
    {
      BaseContainer container;
      if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (selectedBottle, (TransformComponent) null), out container) && !(container.Owner != ent.Owner))
        this._appearance.SetData(selectedBottle, (Enum) RMCPillBottleVisuals.Color, (object) args.Color);
    }
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnPillBottleFillMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterPillBottleFillMsg args)
  {
    EntityUid? entity;
    BaseContainer container;
    if (!this.TryGetEntity(args.Bottle, out entity) || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.PillBottleContainer, out container) || !container.ContainedEntities.Contains<EntityUid>(entity.Value))
      return;
    if (args.Fill)
      ent.Comp.SelectedBottles.Add(entity.Value);
    else
      ent.Comp.SelectedBottles.Remove(entity.Value);
    this.Dirty<RMCChemMasterComponent>(ent);
    this.RefreshUIs(ent);
  }

  private void OnPillBottleTransferMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterPillBottleTransferMsg args)
  {
    EntityUid? entity;
    BaseContainer container;
    if (!this.TryGetEntity(args.Bottle, out entity) || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entity.Value, (TransformComponent) null), out container) || container.Owner != ent.Owner || container.ID != ent.Comp.PillBottleContainer)
      return;
    this._rmcSmartFridge.TransferToNearby(ent.Owner.ToCoordinates(), ent.Comp.LinkRange, entity.Value);
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnPillBottleEjectMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterPillBottleEjectMsg args)
  {
    EntityUid? entity;
    BaseContainer container;
    if (!this.TryGetEntity(args.Bottle, out entity) || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entity.Value, (TransformComponent) null), out container) || container.Owner != ent.Owner || container.ID != ent.Comp.PillBottleContainer)
      return;
    if (this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entity.Value, container))
    {
      this._hands.TryPickupAnyHand(args.Actor, entity.Value);
      this._audio.PlayPredicted(ent.Comp.PillBottleEjectSound, (EntityUid) ent, new EntityUid?(args.Actor));
    }
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnBeakerEjectMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterBeakerEjectMsg args)
  {
    ItemSlot slot;
    if (!this.TryGetBeaker(ent, out Entity<FitsInDispenserComponent> _, out slot, out Entity<SolutionComponent> _))
      return;
    this._itemSlots.TryEjectToHands((EntityUid) ent, slot, new EntityUid?(args.Actor), true);
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnBeakerTransferMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterBeakerTransferMsg args)
  {
    Entity<SolutionComponent> solution;
    Entity<SolutionComponent>? entity;
    if (args.Amount < FixedPoint2.Zero || !this.TryGetBeaker(ent, out Entity<FitsInDispenserComponent> _, out ItemSlot _, out solution) || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.BufferSolutionId, out entity))
      return;
    FixedPoint2 quantity1 = solution.Comp.Solution.RemoveReagent(new ReagentQuantity((string) args.Reagent, args.Amount), true);
    FixedPoint2 acceptedQuantity;
    this._solution.TryAddReagent(entity.Value, (string) args.Reagent, quantity1, out acceptedQuantity);
    FixedPoint2 quantity2 = quantity1 - acceptedQuantity;
    if (quantity2 > FixedPoint2.Zero)
      this._solution.TryAddReagent(solution, (string) args.Reagent, quantity2);
    this._solution.UpdateChemicals(entity.Value);
    this._solution.UpdateChemicals(solution);
    this.Dirty<RMCChemMasterComponent>(ent);
    this.RefreshUIs(ent);
  }

  private void OnBeakerTransferAllMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterBeakerTransferAllMsg args)
  {
    Entity<FitsInDispenserComponent> beaker;
    Entity<SolutionComponent> solution;
    Entity<SolutionComponent>? entity;
    if (!this.TryGetBeaker(ent, out beaker, out ItemSlot _, out solution) || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.BufferSolutionId, out entity))
      return;
    this._solutionTransfer.Transfer(new EntityUid?(args.Actor), (EntityUid) beaker, solution, (EntityUid) ent, entity.Value, solution.Comp.Solution.Volume);
    this.Dirty<RMCChemMasterComponent>(ent);
    this.RefreshUIs(ent);
  }

  private void OnBufferModeMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterBufferModeMsg args)
  {
    if (!Enum.IsDefined<RMCChemMasterBufferMode>(args.Mode))
      return;
    ent.Comp.BufferTransferMode = args.Mode;
    this.Dirty<RMCChemMasterComponent>(ent);
    this.RefreshUIs(ent);
  }

  private void OnBufferTransferMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterBufferTransferMsg args)
  {
    Entity<SolutionComponent>? entity;
    if (args.Amount < FixedPoint2.Zero || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.BufferSolutionId, out entity))
      return;
    FixedPoint2 quantity1 = entity.Value.Comp.Solution.RemoveReagent(new ReagentQuantity((string) args.Reagent, args.Amount), true);
    if (ent.Comp.BufferTransferMode == RMCChemMasterBufferMode.ToDisposal)
    {
      this._solution.UpdateChemicals(entity.Value);
      this.Dirty<RMCChemMasterComponent>(ent);
      this.RefreshUIs(ent);
    }
    else
    {
      Entity<SolutionComponent> solution;
      if (!this.TryGetBeaker(ent, out Entity<FitsInDispenserComponent> _, out ItemSlot _, out solution))
        return;
      FixedPoint2 acceptedQuantity;
      this._solution.TryAddReagent(solution, (string) args.Reagent, quantity1, out acceptedQuantity);
      FixedPoint2 quantity2 = quantity1 - acceptedQuantity;
      if (quantity2 > FixedPoint2.Zero)
        this._solution.TryAddReagent(entity.Value, (string) args.Reagent, quantity2);
      this._solution.UpdateChemicals(entity.Value);
      this._solution.UpdateChemicals(solution);
      this.Dirty<RMCChemMasterComponent>(ent);
      this.RefreshUIs(ent);
    }
  }

  private void OnBufferTransferAllMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterBufferTransferAllMsg args)
  {
    Entity<SolutionComponent>? entity;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.BufferSolutionId, out entity))
      return;
    if (ent.Comp.BufferTransferMode == RMCChemMasterBufferMode.ToDisposal)
    {
      this._solution.RemoveAllSolution(entity.Value);
    }
    else
    {
      Entity<FitsInDispenserComponent> beaker;
      Entity<SolutionComponent> solution;
      if (this.TryGetBeaker(ent, out beaker, out ItemSlot _, out solution))
        this._solutionTransfer.Transfer(new EntityUid?(args.Actor), (EntityUid) ent, entity.Value, (EntityUid) beaker, solution, entity.Value.Comp.Solution.Volume);
    }
    this.Dirty<RMCChemMasterComponent>(ent);
    this.RefreshUIs(ent);
  }

  private void OnSetPillAmountMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterSetPillAmountMsg args)
  {
    ent.Comp.PillAmount = Math.Clamp(args.Amount, 1, ent.Comp.MaxPillAmount);
    this.Dirty<RMCChemMasterComponent>(ent);
  }

  private void OnSetPillTypeMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterSetPillTypeMsg args)
  {
    if (args.Type <= 0U || (long) args.Type > (long) ent.Comp.PillTypes)
      return;
    ent.Comp.SelectedType = args.Type;
    this.Dirty<RMCChemMasterComponent>(ent);
    this.RefreshUIs(ent);
  }

  private void OnCreatePillsMsg(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterCreatePillsMsg args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.PillBottleContainer, out container))
      return;
    this._toFill.Clear();
    foreach (EntityUid selectedBottle in ent.Comp.SelectedBottles)
    {
      StorageComponent comp;
      if (container.Contains(selectedBottle) && this.TryComp<StorageComponent>(selectedBottle, out comp))
      {
        if (this._rmcStorage.EstimateFreeColumns((Entity<StorageComponent>) (selectedBottle, comp)) < ent.Comp.PillAmount)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-pills-not-enough-space"), new EntityUid?(args.Actor), PopupType.MediumCaution);
          return;
        }
        this._toFill.Add(selectedBottle);
      }
    }
    Entity<SolutionComponent>? entity1;
    if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.BufferSolutionId, out entity1))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-not-enough-space-solution"), new EntityUid?(args.Actor), PopupType.MediumCaution);
    }
    else
    {
      FixedPoint2 volume = entity1.Value.Comp.Solution.Volume;
      int divider = this._toFill.Count * ent.Comp.PillAmount;
      if (divider == 0)
        return;
      FixedPoint2 fixedPoint2 = volume / (float) divider;
      if (volume <= FixedPoint2.Zero || fixedPoint2 <= FixedPoint2.Zero)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-not-enough-space-solution"), new EntityUid?(args.Actor), PopupType.MediumCaution);
      }
      else
      {
        if (this._net.IsClient)
          return;
        string str = string.Join(", ", entity1.Value.Comp.Solution.Contents.Select<ReagentQuantity, string>((Func<ReagentQuantity, string>) (c => $"{c.Quantity}u {c.Reagent.Prototype}")));
        EntityCoordinates coordinates = this.Transform((EntityUid) ent).Coordinates;
        List<(string, FixedPoint2)> list = entity1.Value.Comp.Solution.Contents.Select<ReagentQuantity, (string, FixedPoint2)>((Func<ReagentQuantity, (string, FixedPoint2)>) (c => (c.Reagent.Prototype, c.Quantity / (float) divider))).ToList<(string, FixedPoint2)>();
        foreach (EntityUid uid in this._toFill)
        {
          string currentLabel = this.CompOrNull<LabelComponent>(uid)?.CurrentLabel;
          for (int index = 0; index < ent.Comp.PillAmount; ++index)
          {
            EntityUid entityUid = this.Spawn((string) ent.Comp.PillProto, coordinates);
            if (!this._storage.Insert(uid, entityUid, out EntityUid? _, new EntityUid?(args.Actor), playSound: false))
            {
              this.QueueDel(new EntityUid?(entityUid));
            }
            else
            {
              PillComponent pillComponent = this.EnsureComp<PillComponent>(entityUid);
              pillComponent.PillType = ent.Comp.SelectedType - 1U;
              this.Dirty(entityUid, (IComponent) pillComponent);
              if (currentLabel != null)
                this._label.Label(entityUid, currentLabel);
              SolutionSpikerComponent comp;
              Entity<SolutionComponent>? entity2;
              if (this.TryComp<SolutionSpikerComponent>(entityUid, out comp) && this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) entityUid, comp.SourceSolution, out entity2))
              {
                foreach ((string prototype, FixedPoint2 quantity1) in list)
                {
                  FixedPoint2 quantity2 = entity1.Value.Comp.Solution.RemoveReagent(prototype, quantity1);
                  this._solution.TryAddReagent(entity2.Value, prototype, quantity2);
                }
                ISharedAdminLogManager adminLog = this._adminLog;
                LogStringHandler logStringHandler = new LogStringHandler(38, 4);
                logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "player", "ToPrettyString(args.Actor)");
                logStringHandler.AppendLiteral(" transferred ");
                logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(entity2.Value.Comp.Solution));
                logStringHandler.AppendLiteral(" to ");
                logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "target", "ToPrettyString(pill)");
                logStringHandler.AppendLiteral(", which now contains ");
                logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(entity2.Value.Comp.Solution));
                ref LogStringHandler local = ref logStringHandler;
                adminLog.Add(LogType.Action, LogImpact.Medium, ref local);
              }
            }
          }
        }
        this._solution.UpdateChemicals(entity1.Value);
        ISharedAdminLogManager adminLog1 = this._adminLog;
        LogStringHandler logStringHandler1 = new LogStringHandler(73, 7);
        logStringHandler1.AppendLiteral("");
        logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.Actor), "user", "ToPrettyString(args.Actor)");
        logStringHandler1.AppendLiteral(" created ");
        logStringHandler1.AppendFormatted<int>(ent.Comp.PillAmount, "pillAmount", "ent.Comp.PillAmount");
        logStringHandler1.AppendLiteral(" ");
        logStringHandler1.AppendFormatted<FixedPoint2>(fixedPoint2, "pillUnits", "perPill");
        logStringHandler1.AppendLiteral("u pills in ");
        logStringHandler1.AppendFormatted<int>(ent.Comp.SelectedBottles.Count, "bottleAmount", "ent.Comp.SelectedBottles.Count");
        logStringHandler1.AppendLiteral(" pill bottles using ");
        logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "chemMaster", "ToPrettyString(ent)");
        logStringHandler1.AppendLiteral(".\r\nSolution: ");
        logStringHandler1.AppendFormatted(str, format: "solution");
        logStringHandler1.AppendLiteral("\r\nPill bottle IDs: ");
        logStringHandler1.AppendFormatted(string.Join<EntityUid>(", ", (IEnumerable<EntityUid>) ent.Comp.SelectedBottles), format: "bottleIds");
        ref LogStringHandler local1 = ref logStringHandler1;
        adminLog1.Add(LogType.RMCChemMaster, ref local1);
        this.Dirty<RMCChemMasterComponent>(ent);
      }
    }
  }

  private void OnPillBottleBoxTransferDoAfter(
    Entity<RMCChemMasterComponent> ent,
    ref RMCChemMasterPillBottleTransferDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? used = args.Used;
    RMCPillBottleTransferComponent comp1;
    StorageComponent comp2;
    if (!used.HasValue || !this.Exists(args.Used) || !this.TryComp<RMCPillBottleTransferComponent>(args.Used, out comp1) || !this.TryComp<StorageComponent>(args.Used, out comp2))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-pill-bottle-box-failed"), args.User, new EntityUid?(args.User));
    }
    else
    {
      args.Handled = true;
      Container container1 = this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.PillBottleContainer);
      int num1 = ent.Comp.MaxPillBottles - container1.Count;
      if (num1 <= 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-full-pill-bottles"), (EntityUid) ent, new EntityUid?(args.User));
      }
      else
      {
        SharedContainerSystem container2 = this._container;
        used = args.Used;
        EntityUid uid = used.Value;
        string id = comp2.Container.ID;
        BaseContainer container3;
        ref BaseContainer local = ref container3;
        if (!container2.TryGetContainer(uid, id, out local))
          return;
        int num2 = 0;
        foreach (EntityUid entityUid in container3.ContainedEntities.ToList<EntityUid>())
        {
          if (num2 < num1)
          {
            if (this.Exists(entityUid) && this._entityWhitelist.IsWhitelistPass(ent.Comp.PillBottleWhitelist, entityUid) && this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, container3))
            {
              if (this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entityUid, (BaseContainer) container1))
                ++num2;
              else if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entityUid, container3))
                this._hands.TryPickupAnyHand(args.User, entityUid);
            }
          }
          else
            break;
        }
        if (num2 > 0)
        {
          this._audio.PlayPredicted(comp1.InsertPillBottleSound, (EntityUid) ent, new EntityUid?(args.User));
          this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-pill-bottle-box-complete", ("count", (object) num2), ("target", (object) ent)), args.User, new EntityUid?(args.User));
        }
        else
          this._popup.PopupClient(this.Loc.GetString("rmc-chem-master-pill-bottle-box-failed"), args.User, new EntityUid?(args.User));
        this.Dirty<RMCChemMasterComponent>(ent);
      }
    }
  }

  private bool TryGetBeaker(
    Entity<RMCChemMasterComponent> chemMaster,
    out Entity<FitsInDispenserComponent> beaker,
    [NotNullWhen(true)] out ItemSlot? slot,
    out Entity<SolutionComponent> solution)
  {
    beaker = new Entity<FitsInDispenserComponent>();
    solution = new Entity<SolutionComponent>();
    if (this._itemSlots.TryGetSlot((EntityUid) chemMaster, chemMaster.Comp.BeakerSlot, out slot))
    {
      EntityUid? containedEntity = (EntityUid?) slot.ContainerSlot?.ContainedEntity;
      if (containedEntity.HasValue)
      {
        EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
        FitsInDispenserComponent comp;
        Entity<SolutionComponent>? entity;
        if (!this.TryComp<FitsInDispenserComponent>(valueOrDefault, out comp) || !this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) valueOrDefault, comp.Solution, out entity))
          return false;
        beaker = (Entity<FitsInDispenserComponent>) (valueOrDefault, comp);
        solution = entity.Value;
        return true;
      }
    }
    return false;
  }

  protected virtual void RefreshUIs(Entity<RMCChemMasterComponent> ent)
  {
  }
}
