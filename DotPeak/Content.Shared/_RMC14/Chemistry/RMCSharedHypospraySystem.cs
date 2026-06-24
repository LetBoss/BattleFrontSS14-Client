// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.RMCSharedHypospraySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Medical.Refill;
using Content.Shared.Administration.Logs;
using Content.Shared.Chemistry;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Forensics;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mobs.Components;
using Content.Shared.Popups;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._RMC14.Chemistry;

public abstract class RMCSharedHypospraySystem : EntitySystem
{
  [Dependency]
  protected ISharedAdminLogManager _adminLog;
  [Dependency]
  protected SharedAppearanceSystem _appearance;
  [Dependency]
  protected SharedAudioSystem _audio;
  [Dependency]
  protected SharedContainerSystem _container;
  [Dependency]
  protected SharedDoAfterSystem _doAfter;
  [Dependency]
  protected SharedInteractionSystem _interaction;
  [Dependency]
  protected HypospraySystem _hypospray;
  [Dependency]
  protected IPrototypeManager _prototype;
  [Dependency]
  protected ReactiveSystem _reactive;
  [Dependency]
  protected SharedSolutionContainerSystem _solution;
  [Dependency]
  protected INetManager _net;
  [Dependency]
  protected SharedPopupSystem _popup;
  [Dependency]
  protected SkillsSystem _skills;
  [Dependency]
  protected ItemSlotsSystem _slots;
  [Dependency]
  protected EntityWhitelistSystem _whitelist;
  [Dependency]
  protected UseDelaySystem _useDelay;
  [Dependency]
  protected SolutionTransferSystem _transfer;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCHyposprayComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<RMCHyposprayComponent, GetVerbsEvent<AlternativeVerb>>(this.AddSetTransferVerbs));
    this.SubscribeLocalEvent<RMCHyposprayComponent, ComponentStartup>(new EntityEventRefHandler<RMCHyposprayComponent, ComponentStartup>(this.OnStartup));
    this.SubscribeLocalEvent<RMCHyposprayComponent, EntInsertedIntoContainerMessage>(new EntityEventRefHandler<RMCHyposprayComponent, EntInsertedIntoContainerMessage>(this.OnInsert));
    this.SubscribeLocalEvent<RMCHyposprayComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<RMCHyposprayComponent, EntRemovedFromContainerMessage>(this.OnRemove));
    this.SubscribeLocalEvent<RMCHyposprayComponent, AfterInteractEvent>(new EntityEventRefHandler<RMCHyposprayComponent, AfterInteractEvent>(this.OnInteractAfter));
    this.SubscribeLocalEvent<RMCHyposprayComponent, UseInHandEvent>(new EntityEventRefHandler<RMCHyposprayComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<RMCHyposprayComponent, ExaminedEvent>(new EntityEventRefHandler<RMCHyposprayComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<RMCHyposprayComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCHyposprayComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<RMCHyposprayComponent, TacticalReloadHyposprayDoAfterEvent>(new EntityEventRefHandler<RMCHyposprayComponent, TacticalReloadHyposprayDoAfterEvent>(this.OnTacticalReload));
    this.SubscribeLocalEvent<RMCHyposprayComponent, HyposprayDoAfterEvent>(new EntityEventRefHandler<RMCHyposprayComponent, HyposprayDoAfterEvent>(this.OnHypoInject));
    this.SubscribeLocalEvent<RMCHyposprayComponent, RefilledSolutionEvent>(new EntityEventRefHandler<RMCHyposprayComponent, RefilledSolutionEvent>(this.OnRefilled));
    this.SubscribeLocalEvent<HyposprayComponent, HyposprayDoAfterEvent>(new EntityEventRefHandler<HyposprayComponent, HyposprayDoAfterEvent>(this.OnHyposprayDoAfter));
  }

  private void OnExamine(Entity<RMCHyposprayComponent> ent, ref ExaminedEvent args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.SlotId, out container) || container.ContainedEntities.Count == 0)
      return;
    EntityUid containedEntity = container.ContainedEntities[0];
    args.PushText(this.Loc.GetString("rmc-hypospray-loaded", ("vial", (object) containedEntity)));
  }

  private void AddSetTransferVerbs(
    Entity<RMCHyposprayComponent> entity,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    EntityUid user = args.User;
    (EntityUid _, RMCHyposprayComponent _) = entity;
    int num = 0;
    foreach (FixedPoint2 transferAmount in entity.Comp.TransferAmounts)
    {
      FixedPoint2 amount = transferAmount;
      AlternativeVerb alternativeVerb1 = new AlternativeVerb();
      alternativeVerb1.Text = this.Loc.GetString("comp-solution-transfer-verb-amount", ("amount", (object) amount));
      alternativeVerb1.Category = VerbCategory.SetTransferAmount;
      alternativeVerb1.Act = (Action) (() =>
      {
        component.TransferAmount = amount;
        this._popup.PopupClient(this.Loc.GetString("comp-solution-transfer-set-amount", ("amount", (object) amount)), user, new EntityUid?(user));
        this.Dirty<RMCHyposprayComponent>(entity);
      });
      alternativeVerb1.Priority = num;
      AlternativeVerb alternativeVerb2 = alternativeVerb1;
      --num;
      args.Verbs.Add(alternativeVerb2);
    }
  }

  private void OnStartup(Entity<RMCHyposprayComponent> ent, ref ComponentStartup args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnInsert(Entity<RMCHyposprayComponent> ent, ref EntInsertedIntoContainerMessage args)
  {
    if (!ent.Comp.Initialized || args.Container.ID != ent.Comp.SlotId)
      return;
    this.UpdateAppearance(ent);
  }

  private void OnRemove(Entity<RMCHyposprayComponent> ent, ref EntRemovedFromContainerMessage args)
  {
    if (args.Container.ID != ent.Comp.SlotId)
      return;
    this.UpdateAppearance(ent);
  }

  private void OnUseInHand(Entity<RMCHyposprayComponent> ent, ref UseInHandEvent args)
  {
    if (args.Handled)
      return;
    int index = Array.IndexOf<FixedPoint2>(ent.Comp.TransferAmounts, ent.Comp.TransferAmount) + 1;
    if (index >= ent.Comp.TransferAmounts.Length)
      index = 0;
    ent.Comp.TransferAmount = ent.Comp.TransferAmounts[index];
    this._popup.PopupClient(this.Loc.GetString("rmc-hypospray-amount-change", ("amount", (object) ent.Comp.TransferAmount)), args.User, new EntityUid?(args.User));
    this.Dirty<RMCHyposprayComponent>(ent);
    args.Handled = true;
  }

  private void OnInteractAfter(Entity<RMCHyposprayComponent> ent, ref AfterInteractEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? target = args.Target;
    BaseContainer container;
    ItemSlotsComponent comp1;
    if (!target.HasValue || !args.CanReach || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.SlotId, out container) || !this.TryComp<ItemSlotsComponent>((EntityUid) ent, out comp1))
      return;
    ItemSlotsSystem slots = this._slots;
    EntityUid uid = (EntityUid) ent;
    target = args.Target;
    EntityUid usedUid = target.Value;
    EntityUid? user1 = new EntityUid?(args.User);
    ItemSlot slot = comp1.Slots[ent.Comp.SlotId];
    if (slots.CanInsert(uid, usedUid, user1, slot, true))
    {
      args.Handled = true;
      if (!this._skills.HasSkills((Entity<SkillsComponent>) args.User, ent.Comp.TacticalSkills))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-hypospray-fail-tacreload"), args.Used, new EntityUid?(args.User));
      }
      else
      {
        if (container.ContainedEntities.Count == 0)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-hypospray-load-tacreload", ("hypo", (object) ent)), args.Used, new EntityUid?(args.User));
        }
        else
        {
          if (!this._slots.TryEjectToHands((EntityUid) ent, comp1.Slots[ent.Comp.SlotId], new EntityUid?(args.User), true))
            return;
          this._popup.PopupClient(this.Loc.GetString("rmc-hypospray-swap-tacreload"), args.Used, new EntityUid?(args.User));
        }
        this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.TacticalReloadTime, (DoAfterEvent) new TacticalReloadHyposprayDoAfterEvent(), new EntityUid?((EntityUid) ent), args.Target, new EntityUid?((EntityUid) ent))
        {
          BreakOnMove = true,
          BreakOnWeightlessMove = false,
          BreakOnDamage = true,
          NeedHand = ent.Comp.NeedHand,
          BreakOnHandChange = ent.Comp.BreakOnHandChange,
          MovementThreshold = ent.Comp.MovementThreshold
        });
      }
    }
    else if (container.ContainedEntities.Count == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-hypospray-no-vial"), (EntityUid) ent, new EntityUid?(args.User));
      args.Handled = true;
    }
    else
    {
      target = args.Target;
      if (!this.HasComp<InjectableSolutionComponent>(target.Value))
        return;
      if (ent.Comp.OnlyAffectsMobs)
      {
        target = args.Target;
        if (!this.HasComp<MobStateComponent>(target.Value))
          return;
      }
      args.Handled = true;
      UseDelayComponent comp2;
      if (this.TryComp<UseDelayComponent>((EntityUid) ent, out comp2) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) ent, comp2)))
        return;
      AttemptHyposprayUseEvent args1;
      ref AttemptHyposprayUseEvent local = ref args1;
      EntityUid user2 = args.User;
      target = args.Target;
      EntityUid Target = target.Value;
      TimeSpan zero = TimeSpan.Zero;
      local = new AttemptHyposprayUseEvent(user2, Target, zero);
      this.RaiseLocalEvent<AttemptHyposprayUseEvent>((EntityUid) ent, ref args1);
      HyposprayDoAfterEvent @event = new HyposprayDoAfterEvent();
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, args1.DoAfter, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), args.Target, new EntityUid?((EntityUid) ent))
      {
        BreakOnMove = true,
        BreakOnHandChange = true,
        NeedHand = true,
        LagCompensated = true
      });
    }
  }

  protected virtual void OnInteractUsing(
    Entity<RMCHyposprayComponent> ent,
    ref InteractUsingEvent args)
  {
    BaseContainer container;
    ItemSlotsComponent comp1;
    if (args.Handled || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.SlotId, out container) || !this.TryComp<ItemSlotsComponent>((EntityUid) ent, out comp1) || this._slots.CanInsert((EntityUid) ent, args.Used, new EntityUid?(args.User), comp1.Slots[ent.Comp.SlotId], true))
      return;
    if (container.ContainedEntities.Count == 0)
    {
      this._popup.PopupEntity(this.Loc.GetString("rmc-hypospray-no-vial"), (EntityUid) ent, args.User);
    }
    else
    {
      EntityUid containedEntity = container.ContainedEntities[0];
      Entity<SolutionComponent>? soln1;
      Entity<SolutionComponent>? soln2;
      SolutionTransferComponent comp2;
      if (!this._solution.TryGetRefillableSolution((Entity<RefillableSolutionComponent, SolutionContainerManagerComponent>) containedEntity, out soln1, out Solution _) || !this._solution.TryGetDrainableSolution((Entity<DrainableSolutionComponent, SolutionContainerManagerComponent>) args.Used, out soln2, out Solution _) || !this.TryComp<SolutionTransferComponent>(args.Used, out comp2))
        return;
      args.Handled = true;
      FixedPoint2 fixedPoint2 = this._transfer.Transfer(new EntityUid?(args.User), args.Used, soln2.Value, containedEntity, soln1.Value, comp2.TransferAmount);
      if (fixedPoint2 > 0)
        this._popup.PopupClient(this.Loc.GetString("comp-solution-transfer-transfer-solution", ("amount", (object) fixedPoint2), ("target", (object) containedEntity)), (EntityUid) ent, new EntityUid?(args.User));
      this.Dirty<SolutionComponent>(soln2.Value);
      this.Dirty<SolutionComponent>(soln1.Value);
      this.UpdateAppearance(ent);
    }
  }

  private void OnHyposprayDoAfter(Entity<HyposprayComponent> ent, ref HyposprayDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    args.Handled = true;
    this._hypospray.TryDoInject(ent, valueOrDefault, args.User, false);
  }

  private void OnRefilled(Entity<RMCHyposprayComponent> ent, ref RefilledSolutionEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnHypoInject(Entity<RMCHyposprayComponent> ent, ref HyposprayDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    args.Handled = true;
    string str = (string) null;
    if (valueOrDefault == args.User)
      str = "hypospray-component-inject-self-message";
    BaseContainer container;
    if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.SlotId, out container) || container.ContainedEntities.Count == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-hypospray-no-vial"), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      Entity<SolutionComponent>? entity;
      Solution solution1;
      if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) container.ContainedEntities[0], ent.Comp.VialName, out entity, out solution1) || solution1.Volume == 0)
      {
        this._popup.PopupClient(this.Loc.GetString("hypospray-component-empty-message"), valueOrDefault, new EntityUid?(args.User));
      }
      else
      {
        Entity<SolutionComponent>? soln;
        Solution solution2;
        if (!this._solution.TryGetInjectableSolution((Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>) valueOrDefault, out soln, out solution2))
        {
          SharedPopupSystem popup = this._popup;
          ILocalizationManager loc = this.Loc;
          EntityUid uid1 = valueOrDefault;
          EntityManager entityManager = this.EntityManager;
          nullable = new EntityUid?();
          EntityUid? viewer = nullable;
          (string, object) valueTuple = ("target", (object) Identity.Entity(uid1, (IEntityManager) entityManager, viewer));
          string message = loc.GetString("hypospray-cant-inject", valueTuple);
          EntityUid uid2 = valueOrDefault;
          EntityUid? recipient = new EntityUid?(args.User);
          popup.PopupClient(message, uid2, recipient);
        }
        else
        {
          this._popup.PopupClient(this.Loc.GetString(str ?? "hypospray-component-inject-other-message", ("other", (object) valueOrDefault)), valueOrDefault, new EntityUid?(args.User));
          if (valueOrDefault != args.User)
            this._popup.PopupEntity(this.Loc.GetString("hypospray-component-feel-prick-message"), valueOrDefault, valueOrDefault);
          this._audio.PlayPredicted(ent.Comp.InjectSound, (EntityUid) ent, new EntityUid?(args.User));
          UseDelayComponent comp;
          if (this.TryComp<UseDelayComponent>((EntityUid) ent, out comp))
            this._useDelay.TryResetDelay((Entity<UseDelayComponent>) ((EntityUid) ent, comp));
          FixedPoint2 quantity = FixedPoint2.Min(ent.Comp.TransferAmount, solution2.AvailableVolume);
          if (quantity <= 0)
          {
            this._popup.PopupClient(this.Loc.GetString("hypospray-component-transfer-already-full-message", ("owner", (object) valueOrDefault)), valueOrDefault, new EntityUid?(args.User));
          }
          else
          {
            Solution solution3 = this._solution.SplitSolution(entity.Value, quantity);
            if (!solution2.CanAddSolution(solution3))
              return;
            this._reactive.DoEntityReaction(valueOrDefault, solution3, ReactionMethod.Injection);
            this._solution.TryAddSolution(soln.Value, solution3);
            TransferDnaEvent args1 = new TransferDnaEvent()
            {
              Donor = valueOrDefault,
              Recipient = (EntityUid) ent
            };
            this.RaiseLocalEvent<TransferDnaEvent>(valueOrDefault, ref args1);
            ISharedAdminLogManager adminLog = this._adminLog;
            LogStringHandler logStringHandler = new LogStringHandler(36, 4);
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
            logStringHandler.AppendLiteral(" injected ");
            logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) valueOrDefault), "target", "ToPrettyString(target)");
            logStringHandler.AppendLiteral(" with a solution ");
            logStringHandler.AppendFormatted(SharedSolutionContainerSystem.ToPrettyString(solution3), format: "removedSolution");
            logStringHandler.AppendLiteral(" using a ");
            logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "using", "ToPrettyString(ent)");
            ref LogStringHandler local = ref logStringHandler;
            adminLog.Add(LogType.ForceFeed, ref local);
            this.UpdateAppearance(ent);
          }
        }
      }
    }
  }

  private void OnTacticalReload(
    Entity<RMCHyposprayComponent> ent,
    ref TacticalReloadHyposprayDoAfterEvent args)
  {
    ItemSlotsComponent comp;
    if (args.Cancelled || args.Handled || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.SlotId, out BaseContainer _) || !this.TryComp<ItemSlotsComponent>((EntityUid) ent, out comp))
      return;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    ItemSlotsSystem slots = this._slots;
    Entity<ItemSlotsComponent> ent1 = (Entity<ItemSlotsComponent>) ((EntityUid) ent, comp);
    nullable = args.Target;
    EntityUid entityUid = nullable.Value;
    nullable = new EntityUid?();
    EntityUid? user = nullable;
    slots.TryInsertEmpty(ent1, entityUid, user);
  }

  protected void UpdateAppearance(Entity<RMCHyposprayComponent> ent)
  {
    AppearanceComponent comp;
    BaseContainer container;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp) || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.SlotId, out container))
      return;
    int count = container.ContainedEntities.Count;
    this._appearance.SetData((EntityUid) ent, (Enum) VialVisuals.Occupied, (object) (count != 0), comp);
    if (!this.HasComp<SolutionContainerVisualsComponent>((EntityUid) ent))
      return;
    Solution solution;
    if (count == 0)
    {
      solution = new Solution();
    }
    else
    {
      Entity<SolutionComponent>? entity;
      if (!this._solution.TryGetSolution((Entity<SolutionContainerManagerComponent>) container.ContainedEntities[0], ent.Comp.VialName, out entity))
        return;
      solution = entity.Value.Comp.Solution;
    }
    this._appearance.SetData((EntityUid) ent, (Enum) SolutionContainerVisuals.FillFraction, (object) solution.FillFraction, comp);
    this._appearance.SetData((EntityUid) ent, (Enum) SolutionContainerVisuals.Color, (object) solution.GetColor(this._prototype), comp);
    this._appearance.SetData((EntityUid) ent, (Enum) SolutionContainerVisuals.SolutionName, (object) ent.Comp.SolutionName, comp);
    ReagentId? primaryReagentId = solution.GetPrimaryReagentId();
    if (primaryReagentId.HasValue)
    {
      ReagentId valueOrDefault = primaryReagentId.GetValueOrDefault();
      this._appearance.SetData((EntityUid) ent, (Enum) SolutionContainerVisuals.BaseOverride, (object) valueOrDefault.ToString(), comp);
    }
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
  }

  public bool DoAfter(Entity<HyposprayComponent> entity, EntityUid target, EntityUid user)
  {
    UseDelayComponent comp;
    if (!this._hypospray.EligibleEntity(target, (HyposprayComponent) entity) || this.TryComp<UseDelayComponent>((EntityUid) entity, out comp) && this._useDelay.IsDelayed((Entity<UseDelayComponent>) ((EntityUid) entity, comp)))
      return false;
    AttemptHyposprayUseEvent args = new AttemptHyposprayUseEvent(user, target, TimeSpan.Zero);
    this.RaiseLocalEvent<AttemptHyposprayUseEvent>((EntityUid) entity, ref args);
    HyposprayDoAfterEvent @event = new HyposprayDoAfterEvent();
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, args.DoAfter, (DoAfterEvent) @event, new EntityUid?((EntityUid) entity), new EntityUid?(target), new EntityUid?((EntityUid) entity))
    {
      BreakOnMove = true,
      BreakOnHandChange = true,
      NeedHand = true
    });
    return true;
  }
}
