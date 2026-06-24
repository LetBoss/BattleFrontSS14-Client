// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Fruit.SharedXenoFruitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Aura;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.ResinHole;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Fruit.Events;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Pheromones;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Administration.Logs;
using Content.Shared.Buckle.Components;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Systems;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Fruit;

public sealed class SharedXenoFruitSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private IMapManager _map;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private ISharedAdminLogManager _adminLogs;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private MobThresholdSystem _mobThreshold;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeed;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private XenoShieldSystem _xenoShield;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedXenoPheromonesSystem _xenoPhero;
  [Dependency]
  private SharedXenoWeedsSystem _xenoWeeds;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private SharedAuraSystem _aura;
  [Dependency]
  private IComponentFactory _componentFactory;
  private static readonly ProtoId<DamageTypePrototype> FruitPlantDamageType = (ProtoId<DamageTypePrototype>) "Blunt";
  private static readonly ProtoId<TagPrototype> AirlockTag = (ProtoId<TagPrototype>) "Airlock";
  private static readonly ProtoId<TagPrototype> StructureTag = (ProtoId<TagPrototype>) "Structure";
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;

  public override void Initialize()
  {
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    this.SubscribeLocalEvent<XenoFruitPlanterComponent, XenoFruitChooseActionEvent>(new EntityEventRefHandler<XenoFruitPlanterComponent, XenoFruitChooseActionEvent>(this.OnXenoFruitChooseAction));
    this.SubscribeLocalEvent<XenoFruitChooseActionComponent, XenoFruitChosenEvent>(new EntityEventRefHandler<XenoFruitChooseActionComponent, XenoFruitChosenEvent>(this.OnActionFruitChosen));
    this.SubscribeLocalEvent<XenoFruitComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitComponent, ExaminedEvent>(this.OnXenoFruitExamined));
    this.SubscribeLocalEvent<XenoFruitComponent, ActivateInWorldEvent>(new EntityEventRefHandler<XenoFruitComponent, ActivateInWorldEvent>(this.OnXenoFruitActivateInWorld));
    this.SubscribeLocalEvent<XenoFruitComponent, AfterInteractEvent>(new EntityEventRefHandler<XenoFruitComponent, AfterInteractEvent>(this.OnXenoFruitAfterInteract));
    this.SubscribeLocalEvent<XenoFruitComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<XenoFruitComponent, GetVerbsEvent<ActivationVerb>>(this.OnXenoFruitGetVerbs));
    this.SubscribeLocalEvent<XenoFruitComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<XenoFruitComponent, GetVerbsEvent<AlternativeVerb>>(this.OnXenoFruitGetAltVerbs));
    this.SubscribeLocalEvent<XenoFruitPlanterComponent, XenoFruitPlantActionEvent>(new EntityEventRefHandler<XenoFruitPlanterComponent, XenoFruitPlantActionEvent>(this.OnXenoFruitPlantAction));
    this.SubscribeLocalEvent<XenoFruitComponent, XenoFruitHarvestDoAfterEvent>(new EntityEventRefHandler<XenoFruitComponent, XenoFruitHarvestDoAfterEvent>(this.OnXenoFruitHarvestDoAfter));
    this.SubscribeLocalEvent<XenoFruitComponent, XenoFruitConsumeDoAfterEvent>(new EntityEventRefHandler<XenoFruitComponent, XenoFruitConsumeDoAfterEvent>(this.OnXenoFruitConsumeDoAfter));
    this.SubscribeLocalEvent<XenoFruitHealComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitHealComponent, ExaminedEvent>(this.OnXenoHealFruitExamined));
    this.SubscribeLocalEvent<XenoFruitRegenComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitRegenComponent, ExaminedEvent>(this.OnXenoRegenFruitExamined));
    this.SubscribeLocalEvent<XenoFruitShieldComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitShieldComponent, ExaminedEvent>(this.OnXenoShieldFruitExamined));
    this.SubscribeLocalEvent<XenoFruitHasteComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitHasteComponent, ExaminedEvent>(this.OnXenoHasteFruitExamined));
    this.SubscribeLocalEvent<XenoFruitSpeedComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitSpeedComponent, ExaminedEvent>(this.OnXenoSpeedFruitExamined));
    this.SubscribeLocalEvent<XenoFruitPlasmaComponent, ExaminedEvent>(new ComponentEventHandler<XenoFruitPlasmaComponent, ExaminedEvent>(this.OnXenoPlasmaFruitExamined));
    this.SubscribeLocalEvent<GardenerShieldComponent, RemovedShieldEvent>(new EntityEventRefHandler<GardenerShieldComponent, RemovedShieldEvent>(this.OnShieldRemove));
    this.SubscribeLocalEvent<XenoFruitEffectRegenComponent, XenoFruitEffectRegenEvent>(new EntityEventRefHandler<XenoFruitEffectRegenComponent, XenoFruitEffectRegenEvent>(this.OnXenoFruitEffectRegen));
    this.SubscribeLocalEvent<XenoFruitEffectPlasmaComponent, XenoFruitEffectPlasmaEvent>(new EntityEventRefHandler<XenoFruitEffectPlasmaComponent, XenoFruitEffectPlasmaEvent>(this.OnXenoFruitEffectPlasma));
    this.SubscribeLocalEvent<XenoFruitEffectSpeedComponent, RefreshMovementSpeedModifiersEvent>(new EntityEventRefHandler<XenoFruitEffectSpeedComponent, RefreshMovementSpeedModifiersEvent>(this.OnXenoFruitSpeedRefresh));
    this.SubscribeLocalEvent<XenoFruitEffectSpeedComponent, ComponentShutdown>(new EntityEventRefHandler<XenoFruitEffectSpeedComponent, ComponentShutdown>(this.OnXenoFruitEffectSpeedShutdown));
    this.SubscribeLocalEvent<XenoFruitEffectHasteComponent, MeleeHitEvent>(new EntityEventRefHandler<XenoFruitEffectHasteComponent, MeleeHitEvent>(this.OnXenoFruitEffectHasteHit));
    this.SubscribeLocalEvent<XenoFruitEffectHasteComponent, ComponentShutdown>(new EntityEventRefHandler<XenoFruitEffectHasteComponent, ComponentShutdown>(this.OnXenoFruitEffectHasteShutdown));
    this.SubscribeLocalEvent<XenoFruitComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<XenoFruitComponent, AfterAutoHandleStateEvent>(this.OnXenoFruitAfterState));
    this.SubscribeLocalEvent<XenoFruitComponent, DestructionEventArgs>(new EntityEventRefHandler<XenoFruitComponent, DestructionEventArgs>(this.OnXenoFruitDestruction));
    this.SubscribeLocalEvent<XenoFruitComponent, ComponentShutdown>(new EntityEventRefHandler<XenoFruitComponent, ComponentShutdown>(this.OnXenoFruitShutdown));
    this.SubscribeLocalEvent<XenoFruitComponent, EntityTerminatingEvent>(new EntityEventRefHandler<XenoFruitComponent, EntityTerminatingEvent>(this.OnXenoFruitTerminating));
    this.Subs.BuiEvents<XenoFruitPlanterComponent>((object) XenoFruitChooseUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoFruitPlanterComponent>) (subs => subs.Event<XenoFruitChooseBuiMsg>(new EntityEventRefHandler<XenoFruitPlanterComponent, XenoFruitChooseBuiMsg>(this.OnXenoFruitChooseBui))));
  }

  private void OnXenoFruitChooseAction(
    Entity<XenoFruitPlanterComponent> xeno,
    ref XenoFruitChooseActionEvent args)
  {
    args.Handled = true;
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoFruitChooseUI.Key, (EntityUid) xeno);
  }

  private void OnXenoFruitChooseBui(
    Entity<XenoFruitPlanterComponent> xeno,
    ref XenoFruitChooseBuiMsg args)
  {
    if (!xeno.Comp.CanPlant.Contains(args.FruitId))
      return;
    xeno.Comp.FruitChoice = new EntProtoId?(args.FruitId);
    this.Dirty<XenoFruitPlanterComponent>(xeno);
    XenoFruitChosenEvent args1 = new XenoFruitChosenEvent(args.FruitId);
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions((EntityUid) xeno))
      this.RaiseLocalEvent<XenoFruitChosenEvent>(entityUid, ref args1);
    XenoFruitPlanterVisualsChangedEvent args2 = new XenoFruitPlanterVisualsChangedEvent(new EntProtoId<XenoFruitComponent>((string) args.FruitId));
    this.RaiseLocalEvent<XenoFruitPlanterVisualsChangedEvent>((EntityUid) xeno, ref args2);
  }

  private void OnActionFruitChosen(
    Entity<XenoFruitChooseActionComponent> xeno,
    ref XenoFruitChosenEvent args)
  {
    Entity<ActionComponent>? action = this._actions.GetAction(new Entity<ActionComponent>?((Entity<ActionComponent>) xeno.Owner));
    if (action.HasValue)
    {
      Entity<ActionComponent> valueOrDefault = action.GetValueOrDefault();
      Robust.Shared.Prototypes.EntityPrototype prototype;
      if (this._prototype.TryIndex(args.Choice, out prototype))
        this._actions.SetIcon(valueOrDefault.AsNullable(), (SpriteSpecifier) new SpriteSpecifier.Rsi(new ResPath("_RMC14/Structures/Xenos/xeno_fruit.rsi"), this.GetFruitSprite(prototype)));
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-choose", ("fruit", (object) args.Choice)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
  }

  private void OnXenoFruitExamined(EntityUid uid, XenoFruitComponent fruit, ExaminedEvent args)
  {
    string str;
    switch (fruit.State)
    {
      case XenoFruitState.Item:
        str = "rmc-xeno-fruit-examine-grown";
        break;
      case XenoFruitState.Growing:
        str = "rmc-xeno-fruit-examine-growing";
        break;
      case XenoFruitState.Grown:
        str = "rmc-xeno-fruit-examine-grown";
        break;
      case XenoFruitState.Eaten:
        str = "rmc-xeno-fruit-examine-spent";
        break;
      default:
        str = "rmc-xeno-fruit-examine-grown";
        break;
    }
    string messageId = str;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-examine-base", ("growthStatus", (object) this.Loc.GetString(messageId))));
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-consume-examine"), -10);
  }

  private void OnXenoHealFruitExamined(
    EntityUid uid,
    XenoFruitHealComponent fruit,
    ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-instant-heal", ("amount", (object) fruit.HealAmount)), -12);
  }

  private void OnXenoRegenFruitExamined(
    EntityUid uid,
    XenoFruitRegenComponent fruit,
    ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-regen-heal", ("amount", (object) fruit.RegenPerTick), ("time", (object) ((double) fruit.TickCount * fruit.TickPeriod).TotalSeconds)), -12);
  }

  private void OnXenoShieldFruitExamined(
    EntityUid uid,
    XenoFruitShieldComponent fruit,
    ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-shield", ("percent", (object) (fruit.ShieldRatio * 100)), ("max", (object) fruit.ShieldAmount), ("duration", (object) fruit.Duration.TotalSeconds), ("decay", (object) fruit.ShieldDecay)), -12);
  }

  private void OnXenoHasteFruitExamined(
    EntityUid uid,
    XenoFruitHasteComponent fruit,
    ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-cooldown", ("amount", (object) (fruit.ReductionPerSlash * 100)), ("max", (object) (fruit.ReductionMax * 100)), ("time", (object) fruit.Duration.TotalSeconds)), -12);
  }

  private void OnXenoSpeedFruitExamined(
    EntityUid uid,
    XenoFruitSpeedComponent fruit,
    ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-speed", ("amount", (object) fruit.SpeedModifier), ("time", (object) fruit.Duration.TotalSeconds)), -12);
  }

  private void OnXenoPlasmaFruitExamined(
    EntityUid uid,
    XenoFruitPlasmaComponent fruit,
    ExaminedEvent args)
  {
    if (!this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.PushMarkup(this.Loc.GetString("rmc-xeno-fruit-regen-plasma", ("amount", (object) fruit.RegenPerTick), ("time", (object) ((double) fruit.TickCount * fruit.TickPeriod).TotalSeconds)), -12);
  }

  private void OnXenoFruitActivateInWorld(
    Entity<XenoFruitComponent> fruit,
    ref ActivateInWorldEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    EntityUid user = args.User;
    if (fruit.Comp.State != XenoFruitState.Item)
      this.TryHarvest(fruit, user);
    else
      this.TryConsume(fruit, user);
  }

  private void OnXenoFruitAfterInteract(
    Entity<XenoFruitComponent> fruit,
    ref AfterInteractEvent args)
  {
    if (!args.CanReach)
      return;
    EntityUid? target = args.Target;
    if (!target.HasValue)
      return;
    EntityUid valueOrDefault = target.GetValueOrDefault();
    if (!this.HasComp<MobStateComponent>(valueOrDefault))
      return;
    if (args.User == valueOrDefault)
      this.TryConsume(fruit, args.User);
    else
      this.TryFeed(fruit, args.User, valueOrDefault);
  }

  private void OnXenoFruitGetVerbs(
    EntityUid fruit,
    XenoFruitComponent comp,
    GetVerbsEvent<ActivationVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || comp.State == XenoFruitState.Item)
      return;
    ActivationVerb activationVerb1 = new ActivationVerb();
    activationVerb1.Act = (Action) (() => this.TryHarvest((Entity<XenoFruitComponent>) (fruit, comp), args.User));
    activationVerb1.Text = this.Loc.GetString("rmc-xeno-fruit-verb-harvest");
    activationVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/pickup.svg.192dpi.png"));
    ActivationVerb activationVerb2 = activationVerb1;
    args.Verbs.Add(activationVerb2);
  }

  private void OnXenoFruitGetAltVerbs(
    EntityUid fruit,
    XenoFruitComponent comp,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryConsume((Entity<XenoFruitComponent>) (fruit, comp), args.User));
    alternativeVerb1.Text = this.Loc.GetString("rmc-xeno-fruit-verb-consume");
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/cutlery.svg.192dpi.png"));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private bool CanPlantOnTilePopup(
    Entity<XenoFruitPlanterComponent> xeno,
    EntityCoordinates target,
    bool checkWeeds,
    out string popup)
  {
    popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed");
    EntityUid? grid = this._transform.GetGrid(target);
    if (grid.HasValue)
    {
      EntityUid valueOrDefault = grid.GetValueOrDefault();
      MapGridComponent comp;
      if (this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      {
        target = target.SnapToGrid((IEntityManager) this.EntityManager, this._map);
        if (checkWeeds && !this._xenoWeeds.IsOnWeeds((Entity<MapGridComponent>) (valueOrDefault, comp), target))
        {
          popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed-weeds");
          return false;
        }
        if (this._xenoWeeds.IsOnWeeds((Entity<MapGridComponent>) (valueOrDefault, comp), target, true))
        {
          popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed-node");
          return false;
        }
        Entity<XenoWeedsComponent>? weedsOnFloor = this._xenoWeeds.GetWeedsOnFloor((Entity<MapGridComponent>) (valueOrDefault, comp), target);
        if (checkWeeds && weedsOnFloor.HasValue && !this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) weedsOnFloor.Value.Owner))
        {
          popup = this.Loc.GetString("rmc-xeno-fruit-wrong-hive");
          return false;
        }
        Vector2i tile = this._mapSystem.CoordinatesToTile(valueOrDefault, comp, target);
        AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(valueOrDefault, comp, tile);
        EntityUid? uid;
        while (entitiesEnumerator.MoveNext(out uid))
        {
          if (this.HasComp<XenoFruitComponent>(uid))
          {
            popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed-fruit");
            return false;
          }
          if (this.HasComp<XenoResinHoleComponent>(uid))
          {
            popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed-resin-hole");
            return false;
          }
          if (!this.HasComp<StrapComponent>(uid) && !this.HasComp<XenoEggComponent>(uid) && !this.HasComp<XenoConstructComponent>(uid))
          {
            if (!this._tags.HasAnyTag(uid.Value, SharedXenoFruitSystem.StructureTag, SharedXenoFruitSystem.AirlockTag))
              continue;
          }
          popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed");
          return false;
        }
        if (!this._turf.IsTileBlocked(valueOrDefault, tile, CollisionGroup.FlyingMobMask | CollisionGroup.MidImpassable, comp))
          return true;
        popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed");
        return false;
      }
    }
    popup = this.Loc.GetString("rmc-xeno-fruit-plant-failed");
    return false;
  }

  private void OnXenoFruitPlantAction(
    Entity<XenoFruitPlanterComponent> xeno,
    ref XenoFruitPlantActionEvent args)
  {
    if (args.Handled)
      return;
    EntProtoId? fruitChoice = xeno.Comp.FruitChoice;
    if (!fruitChoice.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-plant-failed-select"), xeno.Owner, new EntityUid?(xeno.Owner), PopupType.SmallCaution);
    }
    else
    {
      EntityCoordinates grid = this._transform.GetMoverCoordinates((EntityUid) xeno).SnapToGrid((IEntityManager) this.EntityManager, this._map);
      if (!grid.IsValid((IEntityManager) this.EntityManager))
        return;
      string popup;
      if (!this.CanPlantOnTilePopup(xeno, grid, args.CheckWeeds, out popup))
      {
        this._popup.PopupClient(popup, grid, new EntityUid?(xeno.Owner), PopupType.SmallCaution);
      }
      else
      {
        if (!this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, args.PlasmaCost))
          return;
        DamageSpecifier damage = new DamageSpecifier()
        {
          DamageDict = {
            [(string) SharedXenoFruitSystem.FruitPlantDamageType] = args.HealthCost
          }
        };
        DamageableComponent comp1;
        if (this.TryComp<DamageableComponent>((EntityUid) xeno, out comp1))
          this._damageable.AddDamage(xeno.Owner, comp1, damage);
        args.Handled = true;
        this._audio.PlayPredicted(xeno.Comp.PlantSound, grid, new EntityUid?((EntityUid) xeno));
        bool flag = false;
        if (this._net.IsServer)
        {
          fruitChoice = xeno.Comp.FruitChoice;
          EntityUid entityUid1 = this.Spawn(fruitChoice.HasValue ? (string) fruitChoice.GetValueOrDefault() : (string) null, grid);
          XenoFruitComponent xenoFruitComponent = this.EnsureComp<XenoFruitComponent>(entityUid1);
          TransformComponent xform = this.Transform(entityUid1);
          this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) entityUid1);
          this._transform.SetCoordinates(entityUid1, grid);
          this._transform.SetLocalRotation(entityUid1, Angle.op_Implicit(0.0f));
          this.SetFruitState((Entity<XenoFruitComponent>) (entityUid1, xenoFruitComponent), XenoFruitState.Growing);
          this._transform.AnchorEntity(entityUid1, xform);
          if (!xeno.Comp.PlantedFruit.Contains(entityUid1))
            xeno.Comp.PlantedFruit.Add(entityUid1);
          XenoWeedsComponent comp2;
          if (this.TryComp<XenoWeedsComponent>(this._xenoWeeds.GetWeedsOnFloor(grid), out comp2))
            xenoFruitComponent.GrowTime *= (double) comp2.FruitGrowthMultiplier;
          xenoFruitComponent.Planter = new EntityUid?(xeno.Owner);
          if (xeno.Comp.PlantedFruit.Count > xeno.Comp.MaxFruitAllowed)
          {
            flag = true;
            EntityUid entityUid2 = xeno.Comp.PlantedFruit[0];
            xeno.Comp.PlantedFruit.Remove(entityUid2);
            this.QueueDel(new EntityUid?(entityUid2));
          }
          ISharedAdminLogManager adminLogs = this._adminLogs;
          LogStringHandler logStringHandler = new LogStringHandler(18, 3);
          logStringHandler.AppendLiteral("Xeno ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) xeno.Owner), nameof (xeno), "ToPrettyString(xeno.Owner)");
          logStringHandler.AppendLiteral(" planted ");
          logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid1), "entity", "ToPrettyString(entity)");
          logStringHandler.AppendLiteral(" at ");
          logStringHandler.AppendFormatted<EntityCoordinates>(grid, "coordinates");
          ref LogStringHandler local = ref logStringHandler;
          adminLogs.Add(LogType.RMCXenoFruitPlant, ref local);
        }
        this._popup.PopupPredicted(flag ? this.Loc.GetString("rmc-xeno-fruit-plant-limit-exceeded") : this.Loc.GetString("rmc-xeno-fruit-plant-success-self"), this.Loc.GetString("rmc-xeno-fruit-plant-success-others", (nameof (xeno), (object) xeno)), xeno.Owner, new EntityUid?(xeno.Owner));
        this.UpdateFruitCount(xeno);
      }
    }
  }

  public void GardenerFruitActionMessage(Entity<XenoFruitComponent> fruit, LocId message)
  {
    if (!fruit.Comp.Planter.HasValue || this._net.IsClient)
      return;
    this._popup.PopupEntity(this.Loc.GetString((string) message), fruit.Comp.Planter.Value, fruit.Comp.Planter.Value, PopupType.SmallCaution);
  }

  private bool TryHarvest(Entity<XenoFruitComponent> fruit, EntityUid user)
  {
    if (fruit.Comp.State == XenoFruitState.Item || !this.HasComp<HandsComponent>(user))
      return false;
    if (!this.HasComp<MarineComponent>(user) && !this._hive.FromSameHive((Entity<HiveMemberComponent>) fruit.Owner, (Entity<HiveMemberComponent>) user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-wrong-hive"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (this.HasComp<XenoComponent>(user) && !this.HasComp<XenoFruitPlanterComponent>(user) && fruit.Comp.State == XenoFruitState.Growing)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-pick-failed-not-mature", (nameof (fruit), (object) fruit)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    XenoFruitPlanterComponent comp;
    float num = this.TryComp<XenoFruitPlanterComponent>(user, out comp) ? comp.FruitPickingMultiplier : 1f;
    XenoFruitHarvestDoAfterEvent @event = new XenoFruitHarvestDoAfterEvent();
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, fruit.Comp.HarvestDelay * (double) num, (DoAfterEvent) @event, new EntityUid?((EntityUid) fruit), new EntityUid?(fruit.Owner))
    {
      NeedHand = true,
      BreakOnMove = true,
      RequireCanInteract = true,
      BlockDuplicate = true,
      DuplicateCondition = DuplicateConditions.SameEvent
    }))
    {
      if (this.HasComp<XenoComponent>(user))
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-failed-xeno"), user, new EntityUid?(user), PopupType.SmallCaution);
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-failed-marine"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (this.HasComp<XenoComponent>(user))
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-start-xeno", (nameof (fruit), (object) fruit)), user, new EntityUid?(user));
    else
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-start-marine", (nameof (fruit), (object) fruit)), user, new EntityUid?(user));
    return true;
  }

  private void OnXenoFruitHarvestDoAfter(
    Entity<XenoFruitComponent> fruit,
    ref XenoFruitHarvestDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    this._audio.PlayPredicted(fruit.Comp.HarvestSound, (EntityUid) fruit, new EntityUid?(args.User));
    if (fruit.Comp.State == XenoFruitState.Growing)
    {
      if (this.HasComp<XenoComponent>(args.User))
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-failed-not-mature-xeno", (nameof (fruit), (object) fruit)), args.User, new EntityUid?(args.User), PopupType.MediumCaution);
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-failed-not-mature-marine", (nameof (fruit), (object) fruit)), args.User, new EntityUid?(args.User), PopupType.MediumCaution);
      if (this._net.IsClient || this.TerminatingOrDeleted((EntityUid) fruit) || this.EntityManager.IsQueuedForDeletion((EntityUid) fruit))
        return;
      this.QueueDel(new EntityUid?((EntityUid) fruit));
    }
    else
    {
      if (this.HasComp<XenoComponent>(args.User))
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-success-xeno", (nameof (fruit), (object) fruit)), args.User, new EntityUid?(args.User));
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-harvest-success-marine", (nameof (fruit), (object) fruit)), args.User, new EntityUid?(args.User));
      TransformComponent xform = this.Transform((EntityUid) fruit);
      this._transform.Unanchor((EntityUid) fruit, xform);
      this.SetFruitState(fruit, XenoFruitState.Item);
      this._hands.TryPickup(args.User, (EntityUid) fruit);
      this.RemCompDeferred<AuraComponent>((EntityUid) fruit);
      EntityUid user = args.User;
      EntityUid? planter = fruit.Comp.Planter;
      if ((planter.HasValue ? (user != planter.GetValueOrDefault() ? 1 : 0) : 1) == 0)
        return;
      this.GardenerFruitActionMessage(fruit, (LocId) "rmc-xeno-fruit-picked");
    }
  }

  private bool TryConsume(Entity<XenoFruitComponent> fruit, EntityUid user)
  {
    if (!this.HasComp<XenoComponent>(user) || fruit.Comp.State == XenoFruitState.Eaten)
      return false;
    if (fruit.Comp.State == XenoFruitState.Growing)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-pick-failed-not-mature", (nameof (fruit), (object) fruit)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) fruit.Owner, (Entity<HiveMemberComponent>) user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-wrong-hive"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (this.HasComp<XenoFruitSpeedComponent>((EntityUid) fruit) && this.HasComp<XenoFruitEffectSpeedComponent>(user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-effect-already", (nameof (fruit), (object) fruit)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>(user, out comp))
      return false;
    if (!fruit.Comp.CanConsumeAtFull && comp.TotalDamage == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-pick-failed-health-full"), user, new EntityUid?(user));
      return false;
    }
    XenoFruitConsumeDoAfterEvent @event = new XenoFruitConsumeDoAfterEvent();
    DoAfterArgs args = new DoAfterArgs((IEntityManager) this.EntityManager, user, fruit.Comp.ConsumeDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) fruit), new EntityUid?(user), new EntityUid?((EntityUid) fruit))
    {
      NeedHand = true,
      BreakOnMove = true,
      RequireCanInteract = true
    };
    string recipientMessage = this.Loc.GetString("rmc-xeno-fruit-eat-fail-self", (nameof (fruit), (object) fruit));
    string othersMessage = this.Loc.GetString("rmc-xeno-fruit-eat-fail-others", (nameof (fruit), (object) fruit), ("xeno", (object) user));
    if (!this._doAfter.TryStartDoAfter(args))
    {
      this._popup.PopupPredicted(recipientMessage, othersMessage, user, new EntityUid?(user));
      return false;
    }
    this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-fruit-eat-start-self", (nameof (fruit), (object) fruit)), this.Loc.GetString("rmc-xeno-fruit-eat-start-others", (nameof (fruit), (object) fruit), ("xeno", (object) user)), user, new EntityUid?(user));
    return true;
  }

  private bool TryFeed(Entity<XenoFruitComponent> fruit, EntityUid user, EntityUid target)
  {
    if (!this.HasComp<XenoComponent>(user) || fruit.Comp.State == XenoFruitState.Eaten)
      return false;
    if (!this.HasComp<XenoComponent>(target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-feed-refuse", (nameof (target), (object) target), (nameof (fruit), (object) fruit)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (this._mobState.IsDead(target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-feed-dead", (nameof (target), (object) target), (nameof (fruit), (object) fruit)), user, new EntityUid?(user));
      return false;
    }
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) fruit.Owner, (Entity<HiveMemberComponent>) user))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-wrong-hive"), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (!this._hive.FromSameHive((Entity<HiveMemberComponent>) user, (Entity<HiveMemberComponent>) target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-feed-wrong-hive", (nameof (target), (object) target)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (this.HasComp<XenoFruitSpeedComponent>((EntityUid) fruit) && this.HasComp<XenoFruitEffectSpeedComponent>(target))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-effect-already-feed", ("xeno", (object) target), (nameof (fruit), (object) fruit)), user, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    DamageableComponent comp1;
    if (!this.TryComp<DamageableComponent>(target, out comp1))
      return false;
    if (!fruit.Comp.CanConsumeAtFull && comp1.TotalDamage == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-pick-failed-health-full-target"), user, new EntityUid?(user));
      return false;
    }
    XenoFruitPlanterComponent comp2;
    float num = this.TryComp<XenoFruitPlanterComponent>(user, out comp2) ? comp2.FruitFeedingMultiplier : 1f;
    XenoFruitConsumeDoAfterEvent @event = new XenoFruitConsumeDoAfterEvent();
    DoAfterArgs args = new DoAfterArgs((IEntityManager) this.EntityManager, user, fruit.Comp.ConsumeDelay * (double) num, (DoAfterEvent) @event, new EntityUid?((EntityUid) fruit), new EntityUid?(target), new EntityUid?((EntityUid) fruit))
    {
      NeedHand = true,
      BreakOnMove = true,
      BreakOnHandChange = true,
      RequireCanInteract = true,
      TargetEffect = (EntProtoId?) "RMCEffectHealBusy"
    };
    string recipientMessage1 = this.Loc.GetString("rmc-xeno-fruit-feed-fail-self", (nameof (target), (object) target), (nameof (fruit), (object) fruit));
    string message1 = this.Loc.GetString("rmc-xeno-fruit-feed-fail-target", (nameof (user), (object) user), (nameof (fruit), (object) fruit));
    string othersMessage1 = this.Loc.GetString("rmc-xeno-fruit-feed-fail-others", (nameof (user), (object) user), (nameof (target), (object) target), (nameof (fruit), (object) fruit));
    if (!this._doAfter.TryStartDoAfter(args))
    {
      this._popup.PopupClient(message1, target, new EntityUid?(target), PopupType.MediumCaution);
      this._popup.PopupPredicted(recipientMessage1, othersMessage1, user, new EntityUid?(user));
      return false;
    }
    string recipientMessage2 = this.Loc.GetString("rmc-xeno-fruit-feed-start-self", (nameof (target), (object) target), (nameof (fruit), (object) fruit));
    string message2 = this.Loc.GetString("rmc-xeno-fruit-feed-start-target", (nameof (user), (object) user), (nameof (fruit), (object) fruit));
    string othersMessage2 = this.Loc.GetString("rmc-xeno-fruit-feed-start-others", (nameof (user), (object) user), (nameof (target), (object) target), (nameof (fruit), (object) fruit));
    this._popup.PopupClient(message2, target, new EntityUid?(target), PopupType.MediumCaution);
    this._popup.PopupPredicted(recipientMessage2, othersMessage2, user, new EntityUid?(user));
    return true;
  }

  private void OnXenoFruitConsumeDoAfter(
    Entity<XenoFruitComponent> fruit,
    ref XenoFruitConsumeDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    EntityUid? nullable = args.Target;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    EntityUid user1 = args.User;
    if (this.TerminatingOrDeleted((EntityUid) fruit) || this.EntityManager.IsQueuedForDeletion((EntityUid) fruit) || fruit.Comp.State == XenoFruitState.Eaten)
    {
      if (!(user1 == valueOrDefault))
        return;
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-pick-failed-no-longer", (nameof (fruit), (object) fruit)), user1, new EntityUid?(user1), PopupType.SmallCaution);
    }
    else
    {
      this.ApplyFruitEffects(fruit, valueOrDefault);
      this._popup.PopupClient(this.Loc.GetString((string) fruit.Comp.Popup), valueOrDefault, new EntityUid?(valueOrDefault), PopupType.Medium);
      EntityUid user2 = args.User;
      nullable = fruit.Comp.Planter;
      if ((nullable.HasValue ? (user2 != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
      {
        nullable = args.Target;
        EntityUid? planter = fruit.Comp.Planter;
        if ((nullable.HasValue == planter.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != planter.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
          this.GardenerFruitActionMessage(fruit, (LocId) "rmc-xeno-fruit-consumed");
      }
      this.SetFruitState(fruit, XenoFruitState.Eaten);
      this.RemCompDeferred<AuraComponent>((EntityUid) fruit);
      if (!this._net.IsServer)
        return;
      this.EnsureComp<TimedDespawnComponent>((EntityUid) fruit).Lifetime = fruit.Comp.SpentDespawnTime;
    }
  }

  private void ApplyFruitEffects(Entity<XenoFruitComponent> fruit, EntityUid target)
  {
    XenoFruitHealComponent comp1;
    if (this.TryComp<XenoFruitHealComponent>((EntityUid) fruit, out comp1))
      this._xeno.HealDamage((Entity<DamageableComponent>) (target, (DamageableComponent) null), comp1.HealAmount);
    XenoFruitRegenComponent comp2;
    if (this.TryComp<XenoFruitRegenComponent>((EntityUid) fruit, out comp2))
      this.ApplyFruitRegen((Entity<XenoFruitRegenComponent>) (fruit.Owner, comp2), target);
    XenoFruitPlasmaComponent comp3;
    if (this.TryComp<XenoFruitPlasmaComponent>((EntityUid) fruit, out comp3))
      this.ApplyFruitPlasma((Entity<XenoFruitPlasmaComponent>) (fruit.Owner, comp3), target);
    XenoFruitShieldComponent comp4;
    if (this.TryComp<XenoFruitShieldComponent>((EntityUid) fruit, out comp4))
      this.ApplyFruitShield((Entity<XenoFruitShieldComponent>) (fruit.Owner, comp4), target);
    XenoFruitSpeedComponent comp5;
    if (this.TryComp<XenoFruitSpeedComponent>((EntityUid) fruit, out comp5))
      this.ApplyFruitSpeed((Entity<XenoFruitSpeedComponent>) (fruit.Owner, comp5), target);
    XenoFruitHasteComponent comp6;
    if (!this.TryComp<XenoFruitHasteComponent>((EntityUid) fruit, out comp6))
      return;
    this.ApplyFruitHaste((Entity<XenoFruitHasteComponent>) (fruit.Owner, comp6), target);
  }

  private void ApplyFruitRegen(Entity<XenoFruitRegenComponent> fruit, EntityUid target)
  {
    XenoFruitEffectRegenComponent effectRegenComponent = this.EnsureComp<XenoFruitEffectRegenComponent>(target);
    effectRegenComponent.TickPeriod = fruit.Comp.TickPeriod;
    effectRegenComponent.TicksLeft = new int?(fruit.Comp.TickCount);
    effectRegenComponent.RegenPerTick = fruit.Comp.RegenPerTick;
  }

  private void OnXenoFruitEffectRegen(
    Entity<XenoFruitEffectRegenComponent> xeno,
    ref XenoFruitEffectRegenEvent ev)
  {
    this._xeno.HealDamage((Entity<DamageableComponent>) (xeno.Owner, (DamageableComponent) null), xeno.Comp.RegenPerTick);
  }

  private void ApplyFruitPlasma(Entity<XenoFruitPlasmaComponent> fruit, EntityUid target)
  {
    XenoFruitEffectPlasmaComponent effectPlasmaComponent = this.EnsureComp<XenoFruitEffectPlasmaComponent>(target);
    effectPlasmaComponent.TickPeriod = fruit.Comp.TickPeriod;
    effectPlasmaComponent.TicksLeft = new int?(fruit.Comp.TickCount);
    effectPlasmaComponent.RegenPerTick = fruit.Comp.RegenPerTick;
  }

  private void OnXenoFruitEffectPlasma(
    Entity<XenoFruitEffectPlasmaComponent> xeno,
    ref XenoFruitEffectPlasmaEvent ev)
  {
    this._xenoPlasma.RegenPlasma((Entity<XenoPlasmaComponent>) (xeno.Owner, (XenoPlasmaComponent) null), xeno.Comp.RegenPerTick);
  }

  private void ApplyFruitSpeed(Entity<XenoFruitSpeedComponent> fruit, EntityUid target)
  {
    XenoFruitEffectSpeedComponent effectSpeedComponent = this.EnsureComp<XenoFruitEffectSpeedComponent>(target);
    effectSpeedComponent.Duration = fruit.Comp.Duration;
    effectSpeedComponent.SpeedModifier = fruit.Comp.SpeedModifier;
    this._movementSpeed.RefreshMovementSpeedModifiers(target);
  }

  private void OnXenoFruitEffectSpeedShutdown(
    Entity<XenoFruitEffectSpeedComponent> xeno,
    ref ComponentShutdown ev)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-effect-end"), xeno.Owner, new EntityUid?(xeno.Owner), PopupType.MediumCaution);
    this._movementSpeed.RefreshMovementSpeedModifiers((EntityUid) xeno);
  }

  private void OnXenoFruitSpeedRefresh(
    Entity<XenoFruitEffectSpeedComponent> xeno,
    ref RefreshMovementSpeedModifiersEvent args)
  {
    XenoFruitEffectSpeedComponent comp1 = xeno.Comp;
    MovementSpeedModifierComponent comp2;
    if (!this.TryComp<MovementSpeedModifierComponent>((EntityUid) xeno, out comp2))
      return;
    float num1 = comp2.BaseWalkSpeed + comp1.SpeedModifier.Float();
    float num2 = comp2.BaseSprintSpeed + comp1.SpeedModifier.Float();
    args.ModifySpeed(num1 / comp2.BaseWalkSpeed, num2 / comp2.BaseSprintSpeed);
  }

  private void ApplyFruitShield(Entity<XenoFruitShieldComponent> fruit, EntityUid target)
  {
    XenoFruitShieldComponent comp = fruit.Comp;
    FixedPoint2 fixedPoint2 = this._mobThreshold.GetThresholdForState(target, MobState.Dead) * comp.ShieldRatio;
    FixedPoint2 amount = fixedPoint2 < comp.ShieldAmount ? fixedPoint2 : comp.ShieldAmount;
    this._xenoShield.ApplyShield(target, XenoShieldSystem.ShieldType.Gardener, amount, new TimeSpan?(comp.Duration), comp.ShieldDecay.Double(), true, amount.Double());
    this.EnsureComp<GardenerShieldComponent>(target);
  }

  public void OnShieldRemove(Entity<GardenerShieldComponent> ent, ref RemovedShieldEvent args)
  {
    if (args.Type == XenoShieldSystem.ShieldType.Gardener)
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-defensive-shield-end"), (EntityUid) ent, (EntityUid) ent, PopupType.MediumCaution);
    this.RemCompDeferred<GardenerShieldComponent>((EntityUid) ent);
  }

  public void ApplyFruitHaste(Entity<XenoFruitHasteComponent> fruit, EntityUid target)
  {
    XenoFruitEffectHasteComponent comp;
    bool flag = this.EnsureComp<XenoFruitEffectHasteComponent>(target, out comp);
    comp.Duration = fruit.Comp.Duration;
    comp.ReductionMax = fruit.Comp.ReductionMax;
    comp.ReductionPerSlash = fruit.Comp.ReductionPerSlash;
    comp.ReductionCurrent = flag ? comp.ReductionCurrent : (FixedPoint2) 0;
    comp.EndAt = new TimeSpan?();
  }

  private void RefreshUseDelays(EntityUid user, FixedPoint2 amount)
  {
    foreach ((EntityUid entityUid, ActionComponent _) in this._actions.GetActions(user))
    {
      ActionReducedUseDelayComponent comp;
      if (this.TryComp<ActionReducedUseDelayComponent>(entityUid, out comp))
      {
        ActionReducedUseDelayEvent args = new ActionReducedUseDelayEvent(amount);
        this.RaiseLocalEvent<ActionReducedUseDelayEvent>(entityUid, args);
        this.Dirty(entityUid, (IComponent) comp);
      }
    }
  }

  private void OnXenoFruitEffectHasteHit(
    Entity<XenoFruitEffectHasteComponent> xeno,
    ref MeleeHitEvent args)
  {
    if (args.Handled || !args.IsHit || args.HitEntities.Count == 0)
      return;
    bool flag = false;
    foreach (EntityUid hitEntity in (IEnumerable<EntityUid>) args.HitEntities)
    {
      MobStateComponent component;
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, hitEntity) && this._mobStateQuery.TryComp(hitEntity, out component) && component.CurrentState != MobState.Dead)
      {
        flag = true;
        break;
      }
    }
    if (!flag || xeno.Comp.ReductionCurrent >= xeno.Comp.ReductionMax)
      return;
    xeno.Comp.ReductionCurrent += xeno.Comp.ReductionPerSlash;
    this.RefreshUseDelays(xeno.Owner, xeno.Comp.ReductionCurrent);
  }

  private void OnXenoFruitEffectHasteShutdown(
    Entity<XenoFruitEffectHasteComponent> xeno,
    ref ComponentShutdown ev)
  {
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-fruit-effect-end"), xeno.Owner, new EntityUid?(xeno.Owner), PopupType.MediumCaution);
    this.RefreshUseDelays(xeno.Owner, (FixedPoint2) 0);
  }

  public bool TrySpeedupGrowth(Entity<XenoFruitComponent> fruit, TimeSpan amount)
  {
    XenoFruitComponent comp = fruit.Comp;
    if (!comp.GrowAt.HasValue || comp.State != XenoFruitState.Growing)
      return false;
    XenoFruitComponent xenoFruitComponent = comp;
    TimeSpan? growAt = comp.GrowAt;
    TimeSpan timeSpan = amount;
    TimeSpan? nullable = growAt.HasValue ? new TimeSpan?(growAt.GetValueOrDefault() - timeSpan) : new TimeSpan?();
    xenoFruitComponent.GrowAt = nullable;
    return true;
  }

  private void XenoFruitRemoved(Entity<XenoFruitComponent> fruit)
  {
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityUid? planter = fruit.Comp.Planter;
    if (!planter.HasValue)
      return;
    EntityUid valueOrDefault = planter.GetValueOrDefault();
    XenoFruitPlanterComponent comp;
    if (!this.TryComp<XenoFruitPlanterComponent>(valueOrDefault, out comp) || !comp.PlantedFruit.Contains(fruit.Owner))
      return;
    comp.PlantedFruit.Remove(fruit.Owner);
    this.UpdateFruitCount((Entity<XenoFruitPlanterComponent>) (valueOrDefault, comp));
  }

  private void OnXenoFruitDestruction(
    Entity<XenoFruitComponent> fruit,
    ref DestructionEventArgs args)
  {
    this.GardenerFruitActionMessage(fruit, (LocId) "rmc-xeno-fruit-destroyed");
    this.XenoFruitRemoved(fruit);
  }

  private void OnXenoFruitShutdown(Entity<XenoFruitComponent> fruit, ref ComponentShutdown args)
  {
    this.XenoFruitRemoved(fruit);
  }

  private void OnXenoFruitTerminating(
    Entity<XenoFruitComponent> fruit,
    ref EntityTerminatingEvent args)
  {
    this.XenoFruitRemoved(fruit);
  }

  private void OnXenoFruitAfterState(
    Entity<XenoFruitComponent> fruit,
    ref AfterAutoHandleStateEvent args)
  {
    XenoFruitStateChangedEvent args1 = new XenoFruitStateChangedEvent();
    this.RaiseLocalEvent<XenoFruitStateChangedEvent>((EntityUid) fruit, ref args1);
  }

  private void SetFruitState(Entity<XenoFruitComponent> fruit, XenoFruitState state)
  {
    fruit.Comp.State = state;
    this.Dirty<XenoFruitComponent>(fruit);
    if (this.HasComp<XenoPheromonesObjectComponent>((EntityUid) fruit))
    {
      switch (state)
      {
        case XenoFruitState.Item:
          this._xenoPhero.DeactivatePheromones((Entity<XenoPheromonesComponent>) fruit.Owner);
          break;
        case XenoFruitState.Grown:
          this._xenoPhero.TryActivatePheromonesObject((Entity<XenoPheromonesObjectComponent>) fruit.Owner);
          break;
      }
    }
    XenoFruitStateChangedEvent args = new XenoFruitStateChangedEvent();
    this.RaiseLocalEvent<XenoFruitStateChangedEvent>((EntityUid) fruit, ref args);
  }

  private void UpdateFruitCount(Entity<XenoFruitPlanterComponent> xeno)
  {
    XenoFruitChooseBuiState state = new XenoFruitChooseBuiState(xeno.Comp.PlantedFruit.Count, xeno.Comp.MaxFruitAllowed);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoFruitChooseUI.Key, (BoundUserInterfaceState) state);
  }

  public string GetFruitSprite(Robust.Shared.Prototypes.EntityPrototype ent)
  {
    XenoFruitComponent component;
    return !ent.TryGetComponent<XenoFruitComponent>(out component, this._componentFactory) ? "fruit_lesser_spent" : component.GrownState;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFruitComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoFruitComponent>();
    EntityUid uid1;
    XenoFruitComponent comp1_1;
    TimeSpan valueOrDefault;
    TimeSpan? nullable;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (comp1_1.State == XenoFruitState.Growing)
      {
        XenoFruitComponent xenoFruitComponent = comp1_1;
        valueOrDefault = xenoFruitComponent.GrowAt.GetValueOrDefault();
        if (!xenoFruitComponent.GrowAt.HasValue)
        {
          TimeSpan timeSpan = curTime + comp1_1.GrowTime;
          xenoFruitComponent.GrowAt = new TimeSpan?(timeSpan);
        }
        TimeSpan timeSpan1 = curTime;
        nullable = comp1_1.GrowAt;
        if ((nullable.HasValue ? (timeSpan1 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        {
          this.SetFruitState((Entity<XenoFruitComponent>) (uid1, comp1_1), XenoFruitState.Grown);
          SharedAuraSystem aura = this._aura;
          EntityUid ent = uid1;
          Color outlineColor = comp1_1.OutlineColor;
          nullable = new TimeSpan?();
          TimeSpan? duration = nullable;
          aura.GiveAura(ent, outlineColor, duration);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFruitEffectSpeedComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<XenoFruitEffectSpeedComponent>();
    EntityUid uid2;
    XenoFruitEffectSpeedComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      XenoFruitEffectSpeedComponent effectSpeedComponent = comp1_2;
      valueOrDefault = effectSpeedComponent.EndAt.GetValueOrDefault();
      if (!effectSpeedComponent.EndAt.HasValue)
      {
        TimeSpan timeSpan = curTime + comp1_2.Duration;
        effectSpeedComponent.EndAt = new TimeSpan?(timeSpan);
      }
      TimeSpan timeSpan2 = curTime;
      nullable = comp1_2.EndAt;
      if ((nullable.HasValue ? (timeSpan2 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        this.RemCompDeferred<XenoFruitEffectSpeedComponent>(uid2);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFruitEffectHasteComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<XenoFruitEffectHasteComponent>();
    EntityUid uid3;
    XenoFruitEffectHasteComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid3, out comp1_3))
    {
      XenoFruitEffectHasteComponent effectHasteComponent = comp1_3;
      valueOrDefault = effectHasteComponent.EndAt.GetValueOrDefault();
      if (!effectHasteComponent.EndAt.HasValue)
      {
        TimeSpan timeSpan = curTime + comp1_3.Duration;
        effectHasteComponent.EndAt = new TimeSpan?(timeSpan);
      }
      TimeSpan timeSpan3 = curTime;
      nullable = comp1_3.EndAt;
      if ((nullable.HasValue ? (timeSpan3 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
        this.RemCompDeferred<XenoFruitEffectHasteComponent>(uid3);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFruitEffectRegenComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<XenoFruitEffectRegenComponent>();
    EntityUid uid4;
    XenoFruitEffectRegenComponent comp1_4;
    int? ticksLeft;
    while (entityQueryEnumerator4.MoveNext(out uid4, out comp1_4))
    {
      XenoFruitEffectRegenComponent effectRegenComponent1 = comp1_4;
      valueOrDefault = effectRegenComponent1.NextTickAt.GetValueOrDefault();
      if (!effectRegenComponent1.NextTickAt.HasValue)
      {
        TimeSpan timeSpan = curTime + comp1_4.TickPeriod;
        effectRegenComponent1.NextTickAt = new TimeSpan?(timeSpan);
      }
      TimeSpan timeSpan4 = curTime;
      nullable = comp1_4.NextTickAt;
      if ((nullable.HasValue ? (timeSpan4 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      {
        ticksLeft = comp1_4.TicksLeft;
        int num = 0;
        if (ticksLeft.GetValueOrDefault() <= num & ticksLeft.HasValue)
        {
          this.RemCompDeferred<XenoFruitEffectRegenComponent>(uid4);
        }
        else
        {
          XenoFruitEffectRegenEvent args = new XenoFruitEffectRegenEvent();
          this.RaiseLocalEvent<XenoFruitEffectRegenEvent>(uid4, args);
          XenoFruitEffectRegenComponent effectRegenComponent2 = comp1_4;
          ticksLeft = effectRegenComponent2.TicksLeft;
          effectRegenComponent2.TicksLeft = ticksLeft.HasValue ? new int?(ticksLeft.GetValueOrDefault() - 1) : new int?();
          comp1_4.NextTickAt = new TimeSpan?(curTime + comp1_4.TickPeriod);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoFruitEffectPlasmaComponent> entityQueryEnumerator5 = this.EntityQueryEnumerator<XenoFruitEffectPlasmaComponent>();
    EntityUid uid5;
    XenoFruitEffectPlasmaComponent comp1_5;
    while (entityQueryEnumerator5.MoveNext(out uid5, out comp1_5))
    {
      XenoFruitEffectPlasmaComponent effectPlasmaComponent1 = comp1_5;
      valueOrDefault = effectPlasmaComponent1.NextTickAt.GetValueOrDefault();
      if (!effectPlasmaComponent1.NextTickAt.HasValue)
      {
        TimeSpan timeSpan = curTime + comp1_5.TickPeriod;
        effectPlasmaComponent1.NextTickAt = new TimeSpan?(timeSpan);
      }
      TimeSpan timeSpan5 = curTime;
      nullable = comp1_5.NextTickAt;
      if ((nullable.HasValue ? (timeSpan5 < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
      {
        ticksLeft = comp1_5.TicksLeft;
        int num = 0;
        if (ticksLeft.GetValueOrDefault() <= num & ticksLeft.HasValue)
        {
          this.RemCompDeferred<XenoFruitEffectPlasmaComponent>(uid5);
        }
        else
        {
          XenoFruitEffectPlasmaEvent args = new XenoFruitEffectPlasmaEvent();
          this.RaiseLocalEvent<XenoFruitEffectPlasmaEvent>(uid5, args);
          XenoFruitEffectPlasmaComponent effectPlasmaComponent2 = comp1_5;
          ticksLeft = effectPlasmaComponent2.TicksLeft;
          effectPlasmaComponent2.TicksLeft = ticksLeft.HasValue ? new int?(ticksLeft.GetValueOrDefault() - 1) : new int?();
          comp1_5.NextTickAt = new TimeSpan?(curTime + comp1_5.TickPeriod);
        }
      }
    }
  }
}
