// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Power.SharedRMCPowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Sprite;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.Access.Components;
using Content.Shared.Damage;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Content.Shared.PowerCell;
using Content.Shared.Toggleable;
using Content.Shared.Tools;
using Content.Shared.Tools.Systems;
using Content.Shared.UserInterface;
using Content.Shared.Weapons.Melee;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Power;

public abstract class SharedRMCPowerSystem : EntitySystem
{
  private static readonly bool DisableApcChannelButtons = true;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private MetaDataSystem _metaData;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPointLightSystem _pointLight;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedPowerReceiverSystem _powerReceiver;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SharedRMCSpriteSystem _sprite;
  [Dependency]
  private SharedToolSystem _tool;
  [Dependency]
  private SharedTransformSystem _transform;
  protected readonly HashSet<EntityUid> ToUpdate = new HashSet<EntityUid>();
  private readonly Dictionary<MapId, List<EntityUid>> _reactorPoweredLights = new Dictionary<MapId, List<EntityUid>>();
  private readonly HashSet<MapId> _reactorsUpdated = new HashSet<MapId>();
  private bool _recalculate;
  private Robust.Shared.GameObjects.EntityQuery<RMCApcComponent> _apcQuery;
  private Robust.Shared.GameObjects.EntityQuery<AppearanceComponent> _appearanceQuery;
  private Robust.Shared.GameObjects.EntityQuery<RMCAreaPowerComponent> _areaPowerQuery;
  private Robust.Shared.GameObjects.EntityQuery<AreaComponent> _areaQuery;
  private Robust.Shared.GameObjects.EntityQuery<RMCPowerReceiverComponent> _powerReceiverQuery;

  public override void Initialize()
  {
    this._apcQuery = this.GetEntityQuery<RMCApcComponent>();
    this._appearanceQuery = this.GetEntityQuery<AppearanceComponent>();
    this._areaPowerQuery = this.GetEntityQuery<RMCAreaPowerComponent>();
    this._areaQuery = this.GetEntityQuery<AreaComponent>();
    this._powerReceiverQuery = this.GetEntityQuery<RMCPowerReceiverComponent>();
    this.SubscribeLocalEvent<RMCApcComponent, ComponentStartup>(new EntityEventRefHandler<RMCApcComponent, ComponentStartup>(this.OnApcStartup));
    this.SubscribeLocalEvent<RMCApcComponent, MapInitEvent>(new EntityEventRefHandler<RMCApcComponent, MapInitEvent>(this.OnApcUpdate<MapInitEvent>));
    this.SubscribeLocalEvent<RMCApcComponent, EntParentChangedMessage>(new EntityEventRefHandler<RMCApcComponent, EntParentChangedMessage>(this.OnApcUpdate<EntParentChangedMessage>));
    this.SubscribeLocalEvent<RMCApcComponent, ComponentRemove>(new EntityEventRefHandler<RMCApcComponent, ComponentRemove>(this.OnApcRemove<ComponentRemove>));
    this.SubscribeLocalEvent<RMCApcComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCApcComponent, EntityTerminatingEvent>(this.OnApcRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<RMCApcComponent, BreakageEventArgs>(new EntityEventRefHandler<RMCApcComponent, BreakageEventArgs>(this.OnApcBreakage));
    this.SubscribeLocalEvent<RMCApcComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCApcComponent, InteractUsingEvent>(this.OnApcInteractUsing));
    this.SubscribeLocalEvent<RMCApcComponent, InteractHandEvent>(new EntityEventRefHandler<RMCApcComponent, InteractHandEvent>(this.OnApcInteractHand));
    this.SubscribeLocalEvent<RMCApcComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<RMCApcComponent, ActivatableUIOpenAttemptEvent>(this.OnApcActivatableUIOpenAttempt));
    this.SubscribeLocalEvent<RMCApcComponent, ExaminedEvent>(new EntityEventRefHandler<RMCApcComponent, ExaminedEvent>(this.OnApcExamined));
    this.SubscribeLocalEvent<RMCPowerReceiverComponent, MapInitEvent>(new EntityEventRefHandler<RMCPowerReceiverComponent, MapInitEvent>(this.OnReceiverMapInit));
    this.SubscribeLocalEvent<RMCPowerReceiverComponent, EntParentChangedMessage>(new EntityEventRefHandler<RMCPowerReceiverComponent, EntParentChangedMessage>(this.OnReceiverUpdate<EntParentChangedMessage>));
    this.SubscribeLocalEvent<RMCPowerReceiverComponent, ComponentRemove>(new EntityEventRefHandler<RMCPowerReceiverComponent, ComponentRemove>(this.OnReceiverRemove<ComponentRemove>));
    this.SubscribeLocalEvent<RMCPowerReceiverComponent, EntityTerminatingEvent>(new EntityEventRefHandler<RMCPowerReceiverComponent, EntityTerminatingEvent>(this.OnReceiverRemove<EntityTerminatingEvent>));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, MapInitEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, MapInitEvent>(this.OnFusionReactorMapInit));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, InteractUsingEvent>(this.OnFusionReactorInteractUsing));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorCellDoAfterEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorCellDoAfterEvent>(this.OnFusionReactorCellDoAfter));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorRemoveCellDoAfterEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorRemoveCellDoAfterEvent>(this.OnFusionReactorRemoveCellDoAfter));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorRepairDoAfterEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorRepairDoAfterEvent>(this.OnFusionReactorRepairWeldingDoAfter));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, InteractHandEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, InteractHandEvent>(this.OnFusionReactorInteractHand));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, RMCFusionReactorDestroyDoAfterEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, RMCFusionReactorDestroyDoAfterEvent>(this.OnFusionReactorDestroyDoAfter));
    this.SubscribeLocalEvent<RMCFusionReactorComponent, ExaminedEvent>(new EntityEventRefHandler<RMCFusionReactorComponent, ExaminedEvent>(this.OnFusionReactorExamined));
    this.SubscribeLocalEvent<RMCReactorPoweredLightComponent, MapInitEvent>(new EntityEventRefHandler<RMCReactorPoweredLightComponent, MapInitEvent>(this.OnReactorPoweredLightMapInit));
    this.Subs.BuiEvents<RMCApcComponent>((object) RMCApcUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCApcComponent>) (subs =>
    {
      subs.Event<RMCApcSetChannelBuiMsg>(new EntityEventRefHandler<RMCApcComponent, RMCApcSetChannelBuiMsg>(this.OnApcSetChannelBuiMsg));
      subs.Event<RMCApcCoverBuiMsg>(new EntityEventRefHandler<RMCApcComponent, RMCApcCoverBuiMsg>(this.OnApcCover));
    }));
  }

  private void OnApcStartup(Entity<RMCApcComponent> ent, ref ComponentStartup args)
  {
    this.OffsetApc(ent);
  }

  private void OnApcUpdate<T>(Entity<RMCApcComponent> ent, ref T args)
  {
    MetaDataComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp) || comp.EntityLifeStage < EntityLifeStage.MapInitialized)
      return;
    this.ToUpdate.Add((EntityUid) ent);
    if (this._net.IsClient || this.TerminatingOrDeleted((EntityUid) ent))
      return;
    Robust.Shared.Prototypes.EntityPrototype areaPrototype;
    if (this._area.TryGetArea((EntityUid) ent, out Entity<AreaComponent>? _, out areaPrototype))
      this._metaData.SetEntityName((EntityUid) ent, areaPrototype.Name + " APC");
    this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.CellContainerSlot);
    EntProtoId<PowerCellComponent>? startingCell = ent.Comp.StartingCell;
    if (startingCell.HasValue)
      this.TrySpawnInContainer((string) startingCell.GetValueOrDefault(), (EntityUid) ent, ent.Comp.CellContainerSlot, out EntityUid? _);
    this.OffsetApc(ent);
  }

  private void OnApcRemove<T>(Entity<RMCApcComponent> ent, ref T args)
  {
    RMCAreaPowerComponent component;
    if (this.TerminatingOrDeleted(ent.Comp.Area) || !this._areaPowerQuery.TryComp(ent.Comp.Area, out component))
      return;
    component.Apcs.Remove((EntityUid) ent);
    this.Dirty(ent.Comp.Area.Value, (IComponent) component);
  }

  private void OnApcBreakage(Entity<RMCApcComponent> ent, ref BreakageEventArgs args)
  {
    ent.Comp.State = RMCApcState.WiresExposed;
    ent.Comp.Broken = true;
    this.Dirty<RMCApcComponent>(ent);
    this._appearance.SetData((EntityUid) ent, (Enum) RMCApcVisualsLayers.Layer, (object) RMCApcState.WiresExposed);
  }

  private void OnApcInteractUsing(Entity<RMCApcComponent> ent, ref InteractUsingEvent args)
  {
    EntityUid user = args.User;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) user, ent.Comp.Skill, ent.Comp.SkillLevel))
    {
      this._popup.PopupClient($"You don't know how to use the {this.Name((EntityUid) ent)}'s interface.", (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      EntityUid used = args.Used;
      if (this._tool.HasQuality(used, (string) ent.Comp.CrowbarTool))
      {
        switch (ent.Comp.State)
        {
          case RMCApcState.Working:
          case RMCApcState.WiresExposed:
            if (ent.Comp.CoverLockedButton)
            {
              this._popup.PopupClient("The cover is locked and cannot be opened.", user, new EntityUid?(user), PopupType.MediumCaution);
              return;
            }
            BaseContainer container;
            ent.Comp.State = !this._container.TryGetContainer((EntityUid) ent, ent.Comp.CellContainerSlot, out container) || container.ContainedEntities.Count <= 0 ? RMCApcState.CoverOpenNoBattery : RMCApcState.CoverOpenBattery;
            this.Dirty<RMCApcComponent>(ent);
            this._appearance.SetData((EntityUid) ent, (Enum) RMCApcVisualsLayers.Layer, (object) ent.Comp.State);
            break;
          case RMCApcState.CoverOpenBattery:
          case RMCApcState.CoverOpenNoBattery:
            ent.Comp.State = RMCApcState.Working;
            this.Dirty<RMCApcComponent>(ent);
            this._appearance.SetData((EntityUid) ent, (Enum) RMCApcVisualsLayers.Layer, (object) ent.Comp.State);
            break;
        }
      }
      if (this.HasComp<PowerCellComponent>(used) && ent.Comp.State == RMCApcState.CoverOpenNoBattery)
      {
        ContainerSlot targetContainer = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.CellContainerSlot);
        this._hands.TryDropIntoContainer((Entity<HandsComponent>) user, used, (BaseContainer) targetContainer);
        if (targetContainer.ContainedEntities.Count <= 0)
          return;
        ent.Comp.State = RMCApcState.CoverOpenBattery;
        this.Dirty<RMCApcComponent>(ent);
        this._appearance.SetData((EntityUid) ent, (Enum) RMCApcVisualsLayers.Layer, (object) ent.Comp.State);
        this.ToUpdate.Add((EntityUid) ent);
      }
      else
      {
        if (this.TryComp<AccessComponent>(used, out AccessComponent _))
        {
          ent.Comp.Locked = !ent.Comp.Locked;
          this.Dirty<RMCApcComponent>(ent);
        }
        if (!this._tool.HasQuality(used, (string) ent.Comp.RepairTool))
          return;
        RMCApcComponent comp1 = ent.Comp;
        RMCApcState rmcApcState;
        switch (ent.Comp.State)
        {
          case RMCApcState.Working:
            rmcApcState = RMCApcState.WiresExposed;
            break;
          case RMCApcState.WiresExposed:
            rmcApcState = RMCApcState.Working;
            break;
          default:
            rmcApcState = ent.Comp.State;
            break;
        }
        comp1.State = rmcApcState;
        ent.Comp.Broken = false;
        this.Dirty<RMCApcComponent>(ent);
        this._appearance.SetData((EntityUid) ent, (Enum) RMCApcVisualsLayers.Layer, (object) ent.Comp.State);
        DamageableComponent comp2;
        if (!this.TryComp<DamageableComponent>((EntityUid) ent, out comp2))
          return;
        this._damageable.SetAllDamage((EntityUid) ent, comp2, FixedPoint2.Zero);
      }
    }
  }

  private void OnApcInteractHand(Entity<RMCApcComponent> ent, ref InteractHandEvent args)
  {
    BaseContainer container;
    if (ent.Comp.State != RMCApcState.CoverOpenBattery || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.CellContainerSlot, out container))
      return;
    foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) container.ContainedEntities)
    {
      if (this._container.Remove((Entity<TransformComponent, MetaDataComponent>) containedEntity, container))
      {
        this._hands.TryPickupAnyHand(args.User, containedEntity);
        ent.Comp.State = RMCApcState.CoverOpenNoBattery;
        ent.Comp.ChargePercentage = 0.0f;
        this.Dirty<RMCApcComponent>(ent);
        this._appearance.SetData((EntityUid) ent, (Enum) RMCApcVisualsLayers.Layer, (object) ent.Comp.State);
        this.ToUpdate.Add((EntityUid) ent);
        break;
      }
    }
  }

  private void OnApcActivatableUIOpenAttempt(
    Entity<RMCApcComponent> ent,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    if (!this._skills.HasSkill((Entity<SkillsComponent>) args.User, ent.Comp.Skill, ent.Comp.SkillLevel))
    {
      args.Cancel();
      this._popup.PopupClient($"You don't know how to use the {this.Name((EntityUid) ent)}'s interface.", (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      if (ent.Comp.State == RMCApcState.Working)
        return;
      args.Cancel();
    }
  }

  private void OnApcExamined(Entity<RMCApcComponent> ent, ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("RMCApcComponent"))
    {
      string str;
      switch (ent.Comp.State)
      {
        case RMCApcState.Working:
          str = "Use:\n- An [color=cyan]engineering ID[/color] to lock or unlock the interface.\n- A [color=cyan]crowbar[/color] to open the cover.\n- A [color=cyan]screwdriver[/color] to expose the wires.";
          break;
        case RMCApcState.WiresExposed:
          str = "Use a [color=cyan]screwdriver[/color] to unexpose the wires or a [color=cyan]crowbar[/color] to open the cover!";
          break;
        case RMCApcState.CoverOpenBattery:
          str = "Use an [color=cyan]empty hand[/color] to remove the battery or a [color=cyan]crowbar[/color] to close the cover!";
          break;
        case RMCApcState.CoverOpenNoBattery:
          str = "Use a [color=cyan]battery[/color] to put in a battery!";
          break;
        default:
          str = (string) null;
          break;
      }
      string markup = str;
      if (markup == null)
        return;
      args.PushMarkup(markup);
    }
  }

  protected virtual void OnReceiverMapInit(
    Entity<RMCPowerReceiverComponent> ent,
    ref MapInitEvent args)
  {
    this.OnReceiverUpdate<MapInitEvent>(ent, ref args);
  }

  private void OnReceiverUpdate<T>(Entity<RMCPowerReceiverComponent> ent, ref T args)
  {
    this.ToUpdate.Add((EntityUid) ent);
  }

  private void OnReceiverRemove<T>(Entity<RMCPowerReceiverComponent> ent, ref T args)
  {
    Entity<RMCAreaPowerComponent> areaPower;
    if (!this.TryGetPowerArea((EntityUid) ent, out areaPower) || this.TerminatingOrDeleted((EntityUid) areaPower))
      return;
    this.GetAreaReceivers(areaPower, ent.Comp.Channel).Remove((EntityUid) ent);
  }

  private void OnFusionReactorMapInit(Entity<RMCFusionReactorComponent> ent, ref MapInitEvent args)
  {
    this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.CellContainerSlot);
    EntProtoId<RMCFusionCellComponent>? startingCell = ent.Comp.StartingCell;
    if (startingCell.HasValue)
      this.TrySpawnInContainer((string) startingCell.GetValueOrDefault(), (EntityUid) ent, ent.Comp.CellContainerSlot, out EntityUid? _);
    if (ent.Comp.RandomizeDamage)
    {
      double num = this._random.NextDouble();
      ent.Comp.State = num >= 0.5 ? (num >= 0.85 ? RMCFusionReactorState.Wrench : RMCFusionReactorState.Wire) : RMCFusionReactorState.Weld;
      this.Dirty<RMCFusionReactorComponent>(ent);
    }
    this.UpdateAppearance(ent);
    this.ReactorUpdated(ent);
  }

  private void OnFusionReactorInteractUsing(
    Entity<RMCFusionReactorComponent> ent,
    ref InteractUsingEvent args)
  {
    EntityUid user1 = args.User;
    EntityUid used1 = args.Used;
    args.Handled = true;
    ContainerSlot containerSlot = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.CellContainerSlot);
    if (this.HasComp<RMCFusionCellComponent>(used1))
    {
      EntityUid? nullable = containerSlot.ContainedEntity;
      if (nullable.HasValue)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-insert-already-has-cell", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user1), PopupType.SmallCaution);
      }
      else
      {
        RMCFusionReactorCellDoAfterEvent cellDoAfterEvent = new RMCFusionReactorCellDoAfterEvent();
        TimeSpan timeSpan = ent.Comp.CellDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user1, ent.Comp.Skill);
        EntityManager entityManager = this.EntityManager;
        EntityUid user2 = user1;
        TimeSpan delay = timeSpan;
        RMCFusionReactorCellDoAfterEvent @event = cellDoAfterEvent;
        EntityUid? eventTarget = new EntityUid?((EntityUid) ent);
        nullable = new EntityUid?(used1);
        EntityUid? target = new EntityUid?();
        EntityUid? used2 = nullable;
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user2, delay, (DoAfterEvent) @event, eventTarget, target, used2)
        {
          BreakOnMove = true,
          DuplicateCondition = DuplicateConditions.SameEvent
        }))
          return;
        this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-insert-start-self", ("cell", (object) used1), ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user1));
      }
    }
    else if (this._tool.HasQuality(used1, (string) ent.Comp.CrowbarQuality))
    {
      EntityUid? nullable = containerSlot.ContainedEntity;
      if (!nullable.HasValue)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-remove-none", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user1), PopupType.SmallCaution);
      }
      else
      {
        RMCFusionReactorRemoveCellDoAfterEvent cellDoAfterEvent = new RMCFusionReactorRemoveCellDoAfterEvent();
        TimeSpan timeSpan = ent.Comp.CellDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) user1, ent.Comp.Skill);
        EntityManager entityManager = this.EntityManager;
        EntityUid user3 = user1;
        TimeSpan delay = timeSpan;
        RMCFusionReactorRemoveCellDoAfterEvent @event = cellDoAfterEvent;
        EntityUid? eventTarget = new EntityUid?((EntityUid) ent);
        nullable = new EntityUid?(used1);
        EntityUid? target = new EntityUid?();
        EntityUid? used3 = nullable;
        if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user3, delay, (DoAfterEvent) @event, eventTarget, target, used3)
        {
          BreakOnMove = true,
          DuplicateCondition = DuplicateConditions.SameEvent
        }))
          return;
        ILocalizationManager loc = this.Loc;
        nullable = containerSlot.ContainedEntity;
        (string, object) valueTuple1 = ("cell", (object) nullable.Value);
        (string, object) valueTuple2 = ("reactor", (object) ent);
        this._popup.PopupClient(loc.GetString("rmc-fusion-reactor-remove-start-self", valueTuple1, valueTuple2), (EntityUid) ent, new EntityUid?(user1));
      }
    }
    else if (this._tool.HasQuality(used1, (string) ent.Comp.WeldingQuality))
      this.TryRepair(ent, user1, used1, RMCFusionReactorState.Weld);
    else if (this._tool.HasQuality(used1, (string) ent.Comp.CuttingQuality))
      this.TryRepair(ent, user1, used1, RMCFusionReactorState.Wire);
    else if (this._tool.HasQuality(used1, (string) ent.Comp.WrenchQuality))
    {
      this.TryRepair(ent, user1, used1, RMCFusionReactorState.Wrench);
    }
    else
    {
      RMCDeviceBreakerComponent comp;
      if (!this.TryComp<RMCDeviceBreakerComponent>(used1, out comp) || ent.Comp.State == RMCFusionReactorState.Weld)
        return;
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp.DoAfterTime, (DoAfterEvent) new RMCDeviceBreakerDoAfterEvent(), new EntityUid?(args.Used), new EntityUid?(args.Target), new EntityUid?(args.Used))
      {
        BreakOnMove = true,
        RequireCanInteract = true,
        BreakOnHandChange = true,
        DuplicateCondition = DuplicateConditions.SameTool
      });
    }
  }

  private void OnFusionReactorCellDoAfter(
    Entity<RMCFusionReactorComponent> ent,
    ref RMCFusionReactorCellDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    args.Handled = true;
    EntityUid user = args.User;
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.CellContainerSlot);
    if (!this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) valueOrDefault, (BaseContainer) container))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-insert-fail-self", ("cell", (object) valueOrDefault), ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-insert-finish-self", ("cell", (object) valueOrDefault), ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user));
      this.UpdateAppearance(ent);
    }
  }

  private void OnFusionReactorRemoveCellDoAfter(
    Entity<RMCFusionReactorComponent> ent,
    ref RMCFusionReactorRemoveCellDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    EntityUid user = args.User;
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) ent, ent.Comp.CellContainerSlot);
    EntityUid? containedEntity = container.ContainedEntity;
    if (containedEntity.HasValue)
    {
      EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
      if (this._container.Remove((Entity<TransformComponent, MetaDataComponent>) valueOrDefault, (BaseContainer) container))
        this._hands.TryPickupAnyHand(user, valueOrDefault);
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-remove-finish-self", ("cell", (object) valueOrDefault), ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user));
      this.UpdateAppearance(ent);
    }
    else
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-remove-none", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
  }

  private void OnFusionReactorRepairWeldingDoAfter(
    Entity<RMCFusionReactorComponent> ent,
    ref RMCFusionReactorRepairDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    if (ent.Comp.State != args.State)
      return;
    RMCFusionReactorComponent comp = ent.Comp;
    RMCFusionReactorState fusionReactorState;
    switch (args.State)
    {
      case RMCFusionReactorState.Wrench:
        fusionReactorState = RMCFusionReactorState.Working;
        break;
      case RMCFusionReactorState.Wire:
        fusionReactorState = RMCFusionReactorState.Wrench;
        break;
      case RMCFusionReactorState.Weld:
        fusionReactorState = RMCFusionReactorState.Wire;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    comp.State = fusionReactorState;
    this.Dirty<RMCFusionReactorComponent>(ent);
    this.UpdateAppearance(ent);
    this.ReactorUpdated(ent);
  }

  private void OnFusionReactorInteractHand(
    Entity<RMCFusionReactorComponent> ent,
    ref InteractHandEvent args)
  {
    EntityUid user = args.User;
    if (!this.HasComp<XenoComponent>(user) || !this.HasComp<MeleeWeaponComponent>(user))
      return;
    if (ent.Comp.State == RMCFusionReactorState.Weld)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-already-destroyed", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user));
    }
    else
    {
      RMCFusionReactorDestroyDoAfterEvent @event = new RMCFusionReactorDestroyDoAfterEvent();
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, ent.Comp.DestroyDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent
      });
    }
  }

  private void OnFusionReactorDestroyDoAfter(
    Entity<RMCFusionReactorComponent> ent,
    ref RMCFusionReactorDestroyDoAfterEvent args)
  {
    EntityUid user = args.User;
    if (args.Cancelled || args.Handled)
      return;
    if (ent.Comp.State == RMCFusionReactorState.Weld)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-already-destroyed", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user));
    }
    else
    {
      args.Handled = true;
      this.DestroyReactor(ent, new EntityUid?(args.User));
      if (ent.Comp.State == RMCFusionReactorState.Weld)
        return;
      args.Repeat = true;
    }
  }

  public void DestroyReactor(Entity<RMCFusionReactorComponent> ent, EntityUid? user)
  {
    RMCFusionReactorComponent comp = ent.Comp;
    RMCFusionReactorState fusionReactorState;
    switch (ent.Comp.State)
    {
      case RMCFusionReactorState.Working:
        fusionReactorState = RMCFusionReactorState.Wrench;
        break;
      case RMCFusionReactorState.Wrench:
        fusionReactorState = RMCFusionReactorState.Wire;
        break;
      case RMCFusionReactorState.Wire:
        fusionReactorState = RMCFusionReactorState.Weld;
        break;
      default:
        throw new ArgumentOutOfRangeException();
    }
    comp.State = fusionReactorState;
    this.Dirty<RMCFusionReactorComponent>(ent);
    this.UpdateAppearance(ent);
    this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-destroyed", ("reactor", (object) ent)), (EntityUid) ent, user, PopupType.SmallCaution);
    this.ReactorUpdated(ent);
  }

  private void OnFusionReactorExamined(
    Entity<RMCFusionReactorComponent> ent,
    ref ExaminedEvent args)
  {
    if (this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("RMCFusionReactorComponent"))
    {
      if (ent.Comp.State != RMCFusionReactorState.Working)
      {
        string str1;
        switch (ent.Comp.State)
        {
          case RMCFusionReactorState.Wrench:
            str1 = "a [color=cyan]Wrench[/color]";
            break;
          case RMCFusionReactorState.Wire:
            str1 = "[color=cyan]Wirecutters[/color]";
            break;
          case RMCFusionReactorState.Weld:
            str1 = "a [color=cyan]Welder[/color]";
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        string str2 = str1;
        args.PushMarkup($"Use {str2} to repair it!");
      }
      BaseContainer container;
      if (this._container.TryGetContainer((EntityUid) ent, ent.Comp.CellContainerSlot, out container) && container.ContainedEntities.Count != 0)
        return;
      args.PushMarkup("It needs a [color=cyan]fuel cell[/color]!");
    }
  }

  private void OnReactorPoweredLightMapInit(
    Entity<RMCReactorPoweredLightComponent> ent,
    ref MapInitEvent args)
  {
    TransformComponent comp;
    if (!this.TryComp((EntityUid) ent, out comp))
      return;
    this._reactorPoweredLights.GetOrNew<MapId, List<EntityUid>>(comp.MapID).Add((EntityUid) ent);
  }

  private void OnApcSetChannelBuiMsg(Entity<RMCApcComponent> ent, ref RMCApcSetChannelBuiMsg args)
  {
    if (SharedRMCPowerSystem.DisableApcChannelButtons)
      return;
    int channel = (int) args.Channel;
    if (args.Channel < RMCPowerChannel.Equipment || channel >= ent.Comp.Channels.Length)
      return;
    ent.Comp.Channels[channel].Button = args.State;
    this.Dirty<RMCApcComponent>(ent);
  }

  private void OnApcCover(Entity<RMCApcComponent> ent, ref RMCApcCoverBuiMsg args)
  {
    if (ent.Comp.State != RMCApcState.Working || ent.Comp.Locked)
      return;
    ent.Comp.CoverLockedButton = !ent.Comp.CoverLockedButton;
    this.Dirty<RMCApcComponent>(ent);
  }

  private void UpdateAppearance(Entity<RMCFusionReactorComponent> ent)
  {
    switch (ent.Comp.State)
    {
      case RMCFusionReactorState.Wrench:
        this._appearance.SetData((EntityUid) ent, (Enum) RMCFusionReactorLayers.Layer, (object) RMCFusionReactorVisuals.Wrench);
        break;
      case RMCFusionReactorState.Wire:
        this._appearance.SetData((EntityUid) ent, (Enum) RMCFusionReactorLayers.Layer, (object) RMCFusionReactorVisuals.Wire);
        break;
      case RMCFusionReactorState.Weld:
        this._appearance.SetData((EntityUid) ent, (Enum) RMCFusionReactorLayers.Layer, (object) RMCFusionReactorVisuals.Weld);
        break;
      default:
        BaseContainer container;
        if (!this._container.TryGetContainer((EntityUid) ent, ent.Comp.CellContainerSlot, out container) || container.ContainedEntities.Count == 0)
        {
          this._appearance.SetData((EntityUid) ent, (Enum) RMCFusionReactorLayers.Layer, (object) RMCFusionReactorVisuals.Empty);
          break;
        }
        this._appearance.SetData((EntityUid) ent, (Enum) RMCFusionReactorLayers.Layer, (object) RMCFusionReactorVisuals.Hundred);
        break;
    }
  }

  private void TryRepair(
    Entity<RMCFusionReactorComponent> ent,
    EntityUid user,
    EntityUid used,
    RMCFusionReactorState state)
  {
    if (ent.Comp.State == RMCFusionReactorState.Working)
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-repair-not-needed", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    else if (ent.Comp.State != state)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-repair-different-tool", ("reactor", (object) ent)), (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    }
    else
    {
      ProtoId<ToolQualityPrototype> protoId;
      switch (state)
      {
        case RMCFusionReactorState.Wrench:
          protoId = ent.Comp.WrenchQuality;
          break;
        case RMCFusionReactorState.Wire:
          protoId = ent.Comp.CuttingQuality;
          break;
        case RMCFusionReactorState.Weld:
          protoId = ent.Comp.WeldingQuality;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (state), (object) state, (string) null);
      }
      ProtoId<ToolQualityPrototype> toolQualityNeeded = protoId;
      if (!this._tool.UseTool(used, user, new EntityUid?((EntityUid) ent), (float) ent.Comp.RepairDelay.TotalSeconds, (string) toolQualityNeeded, (DoAfterEvent) new RMCFusionReactorRepairDoAfterEvent(state), ent.Comp.WeldingCost, duplicateCondition: DuplicateConditions.SameTool))
        return;
      this._popup.PopupClient(this.Loc.GetString("rmc-fusion-reactor-repair-start-self", ("reactor", (object) ent), ("tool", (object) used)), (EntityUid) ent, new EntityUid?(user));
    }
  }

  private bool TryGetPowerArea(EntityUid ent, out Entity<RMCAreaPowerComponent> areaPower)
  {
    areaPower = new Entity<RMCAreaPowerComponent>();
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea(ent, out area, out Robust.Shared.Prototypes.EntityPrototype _))
      return false;
    RMCAreaPowerComponent areaPowerComponent = this.EnsureComp<RMCAreaPowerComponent>((EntityUid) area.Value);
    areaPower = (Entity<RMCAreaPowerComponent>) ((EntityUid) area.Value, areaPowerComponent);
    return true;
  }

  private int GetNewPowerLoad(Entity<RMCPowerReceiverComponent> receiver)
  {
    switch (receiver.Comp.Mode)
    {
      case RMCPowerMode.Off:
        return 0;
      case RMCPowerMode.Idle:
        return receiver.Comp.IdleLoad;
      case RMCPowerMode.Active:
        return receiver.Comp.ActiveLoad;
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  protected HashSet<EntityUid> GetAreaReceivers(
    Entity<RMCAreaPowerComponent> area,
    RMCPowerChannel channel)
  {
    switch (channel)
    {
      case RMCPowerChannel.Equipment:
        return area.Comp.EquipmentReceivers;
      case RMCPowerChannel.Lighting:
        return area.Comp.LightingReceivers;
      case RMCPowerChannel.Environment:
        return area.Comp.EnvironmentReceivers;
      default:
        throw new ArgumentOutOfRangeException(nameof (channel), (object) channel, (string) null);
    }
  }

  protected void UpdateApcChannel(
    Entity<RMCApcComponent> apc,
    Entity<RMCAreaPowerComponent> area,
    RMCPowerChannel channel,
    bool on)
  {
    ref RMCApcChannel local = ref apc.Comp.Channels[(int) channel];
    if (local.On == on)
      return;
    if (local.Button == RMCApcButtonState.Auto || local.Button == RMCApcButtonState.On & on || local.Button == RMCApcButtonState.Off && !on)
      local.On = on;
    this.PowerUpdated(area, channel, on);
  }

  protected virtual void PowerUpdated(
    Entity<RMCAreaPowerComponent> area,
    RMCPowerChannel channel,
    bool on)
  {
  }

  public bool IsAreaPowered(Entity<RMCAreaPowerComponent?> area, RMCPowerChannel channel)
  {
    if (!this._areaPowerQuery.Resolve((EntityUid) area, ref area.Comp, false))
      return false;
    AreaComponent component1;
    if (this._areaQuery.TryComp((EntityUid) area, out component1) && component1.AlwaysPowered)
      return true;
    foreach (EntityUid apc in area.Comp.Apcs)
    {
      RMCApcComponent component2;
      if (this._apcQuery.TryComp(apc, out component2) && component2.Channels[(int) channel].On)
        return true;
    }
    return false;
  }

  public abstract bool IsPowered(EntityUid ent);

  private bool AnyReactorsOn(MapId map)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCFusionReactorComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCFusionReactorComponent, TransformComponent>();
    RMCFusionReactorComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out comp1, out comp2))
    {
      if (comp1.State == RMCFusionReactorState.Working && comp2.MapID == map)
        return true;
    }
    return false;
  }

  private void ReactorUpdated(Entity<RMCFusionReactorComponent> ent)
  {
    this._reactorsUpdated.Add(this._transform.GetMapId((Entity<TransformComponent>) ent.Owner));
  }

  protected void UpdateReceiverPower(EntityUid receiver, ref PowerChangedEvent ev)
  {
    SharedApcPowerReceiverComponent component1 = (SharedApcPowerReceiverComponent) null;
    if (!this._powerReceiver.ResolveApc(receiver, ref component1) || component1.Powered == ev.Powered || !component1.NeedsPower)
      return;
    component1.Powered = ev.Powered;
    this.Dirty(receiver, (IComponent) component1);
    this.RaiseLocalEvent<PowerChangedEvent>(receiver, ref ev);
    AppearanceComponent component2;
    if (!this._appearanceQuery.TryComp(receiver, out component2))
      return;
    this._appearance.SetData(receiver, (Enum) PowerDeviceVisuals.Powered, (object) ev.Powered, component2);
  }

  public void RecalculatePower() => this._recalculate = true;

  private void OffsetApc(Entity<RMCApcComponent> ent)
  {
    SpriteSetRenderOrderComponent renderOrderComponent = this.EnsureComp<SpriteSetRenderOrderComponent>((EntityUid) ent);
    Angle localRotation = this.Transform((EntityUid) ent).LocalRotation;
    switch ((int) ((Angle) ref localRotation).GetDir())
    {
      case 0:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(0.45f, -0.32f));
        break;
      case 2:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(0.7f, -1.45f));
        break;
      case 4:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(-0.5f, -1.5f));
        break;
      case 6:
        this._sprite.SetOffset((EntityUid) ent, new Vector2(-0.7f, -0.4f));
        break;
    }
    this.Dirty((EntityUid) ent, (IComponent) renderOrderComponent);
  }

  public override void Update(float frameTime)
  {
    if (this._recalculate)
    {
      this._recalculate = false;
      Robust.Shared.GameObjects.EntityQueryEnumerator<RMCApcComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RMCApcComponent>();
      EntityUid uid1;
      while (entityQueryEnumerator1.MoveNext(out uid1, out RMCApcComponent _))
        this.ToUpdate.Add(uid1);
      Robust.Shared.GameObjects.EntityQueryEnumerator<RMCPowerReceiverComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCPowerReceiverComponent>();
      EntityUid uid2;
      while (entityQueryEnumerator2.MoveNext(out uid2, out RMCPowerReceiverComponent _))
        this.ToUpdate.Add(uid2);
      Robust.Shared.GameObjects.EntityQueryEnumerator<RMCFusionReactorComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<RMCFusionReactorComponent>();
      EntityUid uid3;
      while (entityQueryEnumerator3.MoveNext(out uid3, out RMCFusionReactorComponent _))
        this._reactorsUpdated.Add(this.Transform(uid3).MapID);
      Robust.Shared.GameObjects.EntityQueryEnumerator<RMCReactorPoweredLightComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<RMCReactorPoweredLightComponent>();
      EntityUid uid4;
      while (entityQueryEnumerator4.MoveNext(out uid4, out RMCReactorPoweredLightComponent _))
        this._reactorPoweredLights.GetOrNew<MapId, List<EntityUid>>(this.Transform(uid4).MapID).Add(uid4);
    }
    if (this._net.IsClient)
    {
      this.ToUpdate.Clear();
      this._reactorPoweredLights.Clear();
      this._reactorsUpdated.Clear();
    }
    else
    {
      try
      {
        using (HashSet<MapId>.Enumerator enumerator = this._reactorsUpdated.GetEnumerator())
        {
label_21:
          while (enumerator.MoveNext())
          {
            MapId current = enumerator.Current;
            bool enabled = this.AnyReactorsOn(current);
            Robust.Shared.GameObjects.EntityQueryEnumerator<RMCReactorPoweredLightComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCReactorPoweredLightComponent, TransformComponent>();
            while (true)
            {
              EntityUid uid;
              TransformComponent comp2;
              do
              {
                if (!entityQueryEnumerator.MoveNext(out uid, out RMCReactorPoweredLightComponent _, out comp2))
                  goto label_21;
              }
              while (!(comp2.MapID == current));
              this._appearance.SetData(uid, (Enum) ToggleableVisuals.Enabled, (object) enabled);
              this._pointLight.SetEnabled(uid, enabled);
            }
          }
        }
      }
      finally
      {
        this._reactorsUpdated.Clear();
      }
      try
      {
        foreach (EntityUid entityUid in this.ToUpdate)
        {
          if (!this.TerminatingOrDeleted(entityUid))
          {
            RMCApcComponent component1;
            RMCAreaPowerComponent component2;
            if (this._apcQuery.TryComp(entityUid, out component1) && this._areaPowerQuery.TryComp(component1.Area, out component2))
            {
              component2.Apcs.Remove(entityUid);
              this.Dirty(entityUid, (IComponent) component1);
            }
            RMCPowerReceiverComponent component3;
            RMCAreaPowerComponent component4;
            if (this._powerReceiverQuery.TryComp(entityUid, out component3) && this._areaPowerQuery.TryComp(component3.Area, out component4))
            {
              this.GetAreaReceivers((Entity<RMCAreaPowerComponent>) (component3.Area.Value, component4), component3.Channel).Remove(entityUid);
              component4.Load[(int) component3.Channel] -= component3.LastLoad;
              this.Dirty(entityUid, (IComponent) component3);
            }
            Entity<RMCAreaPowerComponent> areaPower;
            if (this.TryGetPowerArea(entityUid, out areaPower))
            {
              if (component1 != null)
              {
                if (areaPower.Comp.Apcs.Add(entityUid))
                  this.Dirty<RMCAreaPowerComponent>(areaPower);
                component1.Area = new EntityUid?((EntityUid) areaPower);
                this.Dirty(entityUid, (IComponent) component1);
              }
              if (component3 != null)
              {
                component3.Area = new EntityUid?((EntityUid) areaPower);
                this.Dirty(entityUid, (IComponent) component3);
                PowerChangedEvent ev = new PowerChangedEvent(this.IsAreaPowered((Entity<RMCAreaPowerComponent>) ((EntityUid) areaPower, (RMCAreaPowerComponent) areaPower), component3.Channel), 0.0f);
                this.UpdateReceiverPower(entityUid, ref ev);
                if (this.GetAreaReceivers(areaPower, component3.Channel).Add(entityUid))
                {
                  component3.LastLoad = this.GetNewPowerLoad((Entity<RMCPowerReceiverComponent>) (entityUid, component3));
                  areaPower.Comp.Load[(int) component3.Channel] += component3.LastLoad;
                  this.Dirty<RMCAreaPowerComponent>(areaPower);
                }
              }
            }
          }
        }
      }
      finally
      {
        this.ToUpdate.Clear();
      }
    }
  }
}
