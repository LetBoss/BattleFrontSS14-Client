// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Evolution.XenoEvolutionSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Xenonids.Announce;
using Content.Shared._RMC14.Xenonids.Egg;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Climbing.Components;
using Content.Shared.Climbing.Systems;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.FixedPoint;
using Content.Shared.GameTicking;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Jittering;
using Content.Shared.Mind;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.Prototypes;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Events;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Evolution;

public sealed class XenoEvolutionSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _action;
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private ClimbSystem _climb;
  [Dependency]
  private IConfigurationManager _config;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private SharedGameTicker _gameTicker;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private SharedXenoAnnounceSystem _xenoAnnounce;
  [Dependency]
  private SharedXenoHiveSystem _xenoHive;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedXenoWeedsSystem _xenoWeeds;
  [Dependency]
  private IMapManager _map;
  private TimeSpan _evolutionPointsRequireOvipositorAfter;
  private TimeSpan _evolutionAccumulatePointsBefore;
  private TimeSpan _evolveSameCasteCooldown;
  private TimeSpan _earlyEvoBoostBefore;
  private readonly HashSet<EntityUid> _climbable = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _doors = new HashSet<EntityUid>();
  private readonly HashSet<EntityUid> _intersecting = new HashSet<EntityUid>();
  private Robust.Shared.GameObjects.EntityQuery<MobStateComponent> _mobStateQuery;

  public override void Initialize()
  {
    this._mobStateQuery = this.GetEntityQuery<MobStateComponent>();
    this.SubscribeLocalEvent<XenoDevolveComponent, XenoOpenDevolveActionEvent>(new EntityEventRefHandler<XenoDevolveComponent, XenoOpenDevolveActionEvent>(this.OnXenoOpenDevolveAction));
    this.SubscribeLocalEvent<XenoEvolutionComponent, MapInitEvent>(new EntityEventRefHandler<XenoEvolutionComponent, MapInitEvent>(this.OnXenoEvolveMapInit));
    this.SubscribeLocalEvent<XenoEvolutionComponent, XenoOpenEvolutionsActionEvent>(new EntityEventRefHandler<XenoEvolutionComponent, XenoOpenEvolutionsActionEvent>(this.OnXenoEvolveAction));
    this.SubscribeLocalEvent<XenoEvolutionComponent, XenoEvolutionDoAfterEvent>(new EntityEventRefHandler<XenoEvolutionComponent, XenoEvolutionDoAfterEvent>(this.OnXenoEvolveDoAfter));
    this.SubscribeLocalEvent<XenoEvolutionComponent, NewXenoEvolvedEvent>(new EntityEventRefHandler<XenoEvolutionComponent, NewXenoEvolvedEvent>(this.OnXenoEvolutionNewEvolved));
    this.SubscribeLocalEvent<XenoEvolutionComponent, XenoDevolvedEvent>(new EntityEventRefHandler<XenoEvolutionComponent, XenoDevolvedEvent>(this.OnXenoEvolutionDevolved));
    this.SubscribeLocalEvent<XenoNewlyEvolvedComponent, PreventCollideEvent>(new EntityEventRefHandler<XenoNewlyEvolvedComponent, PreventCollideEvent>(this.OnNewlyEvolvedPreventCollide));
    this.SubscribeLocalEvent<XenoEvolutionGranterComponent, NewXenoEvolvedEvent>(new EntityEventRefHandler<XenoEvolutionGranterComponent, NewXenoEvolvedEvent>(this.OnGranterEvolved));
    this.SubscribeLocalEvent<XenoOvipositorChangedEvent>(new EntityEventRefHandler<XenoOvipositorChangedEvent>(this.OnOvipositorChanged));
    this.Subs.BuiEvents<XenoEvolutionComponent>((object) XenoEvolutionUIKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoEvolutionComponent>) (subs =>
    {
      subs.Event<XenoEvolveBuiMsg>(new EntityEventRefHandler<XenoEvolutionComponent, XenoEvolveBuiMsg>(this.OnXenoEvolveBui));
      subs.Event<XenoStrainBuiMsg>(new EntityEventRefHandler<XenoEvolutionComponent, XenoStrainBuiMsg>(this.OnXenoStrainBui));
    }));
    this.Subs.BuiEvents<XenoDevolveComponent>((object) XenoDevolveUIKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<XenoDevolveComponent>) (subs => subs.Event<XenoDevolveBuiMsg>(new EntityEventRefHandler<XenoDevolveComponent, XenoDevolveBuiMsg>(this.OnXenoDevolveBui))));
    this.Subs.CVar<int>(this._config, RMCCVars.RMCEvolutionPointsRequireOvipositorMinutes, (Action<int>) (v => this._evolutionPointsRequireOvipositorAfter = TimeSpan.FromMinutes((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCEvolutionPointsAccumulateBeforeMinutes, (Action<int>) (v => this._evolutionAccumulatePointsBefore = TimeSpan.FromMinutes((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCXenoEvolveSameCasteCooldownSeconds, (Action<int>) (v => this._evolveSameCasteCooldown = TimeSpan.FromSeconds((long) v)), true);
    this.Subs.CVar<int>(this._config, RMCCVars.RMCXenoEarlyEvoPointBoostBeforeMinutes, (Action<int>) (v => this._earlyEvoBoostBefore = TimeSpan.FromMinutes((long) v)), true);
  }

  private void OnXenoOpenDevolveAction(
    Entity<XenoDevolveComponent> xeno,
    ref XenoOpenDevolveActionEvent args)
  {
    if (args.Handled || !this.DamagedCheckPopup((EntityUid) xeno))
      return;
    args.Handled = true;
    this._ui.OpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoDevolveUIKey.Key, new EntityUid?((EntityUid) xeno));
  }

  private void OnXenoEvolveMapInit(Entity<XenoEvolutionComponent> ent, ref MapInitEvent args)
  {
    this._action.AddAction((EntityUid) ent, ref ent.Comp.Action, (string) ent.Comp.ActionId);
  }

  private void OnXenoEvolveAction(
    Entity<XenoEvolutionComponent> xeno,
    ref XenoOpenEvolutionsActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this._ui.OpenUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoEvolutionUIKey.Key, new EntityUid?((EntityUid) xeno));
    XenoEvolveBuiState state = new XenoEvolveBuiState(this.LackingOvipositor());
    this._ui.SetUiState((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoEvolutionUIKey.Key, (BoundUserInterfaceState) state);
  }

  private void OnXenoEvolveBui(Entity<XenoEvolutionComponent> xeno, ref XenoEvolveBuiMsg args)
  {
    EntityUid actor = args.Actor;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoEvolutionUIKey.Key, new EntityUid?(actor));
    if (this._net.IsClient)
      return;
    if (!this.CanEvolvePopup(xeno, args.Choice))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} sent an invalid evolution choice: {args.Choice}.");
    }
    else
    {
      if (!this.DamagedCheckPopup((EntityUid) xeno, false))
        return;
      TimeSpan curTime = this._timing.CurTime;
      EntityPrototype prototype;
      if (this._prototypes.TryIndex(args.Choice, out prototype) && prototype.HasComponent<XenoEvolutionGranterComponent>(this._compFactory))
      {
        Entity<HiveComponent>? hive = this._xenoHive.GetHive((Entity<HiveMemberComponent>) xeno.Owner);
        if (hive.HasValue)
        {
          Entity<HiveComponent> valueOrDefault1 = hive.GetValueOrDefault();
          TimeSpan? lastQueenDeath = valueOrDefault1.Comp.LastQueenDeath;
          if (lastQueenDeath.HasValue)
          {
            TimeSpan valueOrDefault2 = lastQueenDeath.GetValueOrDefault();
            if (curTime < valueOrDefault2 + valueOrDefault1.Comp.NewQueenCooldown)
            {
              TimeSpan timeSpan = valueOrDefault2 + valueOrDefault1.Comp.NewQueenCooldown - curTime;
              string message = this.Loc.GetString("rmc-xeno-evolution-cant-evolve-recent-queen-death-minutes", ("minutes", (object) timeSpan.Minutes), ("seconds", (object) timeSpan.Seconds));
              if (timeSpan.Minutes == 0)
                message = this.Loc.GetString("rmc-xeno-evolution-cant-evolve-recent-queen-death-seconds", ("seconds", (object) timeSpan.Seconds));
              this._popup.PopupEntity(message, (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
              return;
            }
          }
        }
      }
      XenoEvolutionDoAfterEvent @event = new XenoEvolutionDoAfterEvent(args.Choice);
      DoAfterArgs args1 = new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.EvolutionDelay, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
      {
        BreakOnRest = false
      };
      if (xeno.Comp.EvolutionDelay > TimeSpan.Zero)
        this._popup.PopupClient(this.Loc.GetString("cm-xeno-evolution-start"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
      if (!this._doAfter.TryStartDoAfter(args1))
        return;
      this._jitter.DoJitter((EntityUid) xeno, xeno.Comp.EvolutionDelay, true, 80f, 8f, true);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-evolution-start-others", (nameof (xeno), (object) xeno)), (EntityUid) xeno, Filter.PvsExcept((EntityUid) xeno), true, PopupType.Medium);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-evolution-start-self"), (EntityUid) xeno, (EntityUid) xeno, PopupType.Medium);
    }
  }

  private void OnXenoStrainBui(Entity<XenoEvolutionComponent> xeno, ref XenoStrainBuiMsg args)
  {
    EntityUid actor = args.Actor;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoEvolutionUIKey.Key, new EntityUid?(actor));
    if (this._net.IsClient)
      return;
    if (!xeno.Comp.Strains.Contains(args.Choice))
    {
      this.Log.Warning($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} sent an invalid strain choice: {args.Choice}.");
    }
    else
    {
      if (!this.ContainedCheckPopup((EntityUid) xeno) || !this.DamagedCheckPopup((EntityUid) xeno, false))
        return;
      EntityUid entityUid = this.TransferXeno((EntityUid) xeno, args.Choice);
      NewXenoEvolvedEvent args1 = new NewXenoEvolvedEvent(xeno, entityUid, false);
      this.RaiseLocalEvent<NewXenoEvolvedEvent>(entityUid, ref args1, true);
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(22, 2);
      logStringHandler.AppendLiteral("Xenonid ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), "ToPrettyString(xeno)");
      logStringHandler.AppendLiteral(" chose strain ");
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "ToPrettyString(newXeno)");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCEvolve, ref local);
      this.Del(new EntityUid?(xeno.Owner));
      AfterNewXenoEvolvedEvent args2 = new AfterNewXenoEvolvedEvent();
      this.RaiseLocalEvent<AfterNewXenoEvolvedEvent>(entityUid, ref args2);
    }
  }

  private void OnXenoDevolveBui(Entity<XenoDevolveComponent> xeno, ref XenoDevolveBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) xeno.Owner, (Enum) XenoEvolutionUIKey.Key, new EntityUid?((EntityUid) xeno));
    this.TryDevolve(xeno, args.Choice);
  }

  private void OnXenoEvolveDoAfter(
    Entity<XenoEvolutionComponent> xeno,
    ref XenoEvolutionDoAfterEvent args)
  {
    if (this._net.IsClient || args.Handled || args.Cancelled || !this._mind.TryGetMind((EntityUid) xeno, out EntityUid _, out MindComponent _) || !this.CanEvolvePopup(xeno, args.Choice))
      return;
    args.Handled = true;
    EntityUid entityUid = this.TransferXeno((EntityUid) xeno, args.Choice);
    NewXenoEvolvedEvent args1 = new NewXenoEvolvedEvent(xeno, entityUid, true);
    this.RaiseLocalEvent<NewXenoEvolvedEvent>(entityUid, ref args1, true);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(22, 2);
    logStringHandler.AppendLiteral("Xenonid ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), "ToPrettyString(xeno)");
    logStringHandler.AppendLiteral(" evolved into ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "ToPrettyString(newXeno)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCEvolve, ref local);
    this.Del(new EntityUid?(xeno.Owner));
    this._popup.PopupEntity(this.Loc.GetString("cm-xeno-evolution-end"), entityUid, entityUid);
    AfterNewXenoEvolvedEvent args2 = new AfterNewXenoEvolvedEvent();
    this.RaiseLocalEvent<AfterNewXenoEvolvedEvent>(entityUid, ref args2);
  }

  private void OnXenoEvolutionNewEvolved(
    Entity<XenoEvolutionComponent> xeno,
    ref NewXenoEvolvedEvent args)
  {
    this.TransferPoints((Entity<XenoEvolutionComponent>) ((EntityUid) args.OldXeno, (XenoEvolutionComponent) args.OldXeno), xeno, args.SubtractPoints);
    this._jitter.DoJitter((EntityUid) xeno, xeno.Comp.EvolutionJitterDuration, true, 80f, 8f, true);
  }

  private void OnXenoEvolutionDevolved(
    Entity<XenoEvolutionComponent> xeno,
    ref XenoDevolvedEvent args)
  {
    this.TransferPoints((Entity<XenoEvolutionComponent>) args.OldXeno, (Entity<XenoEvolutionComponent>) ((EntityUid) xeno, (XenoEvolutionComponent) xeno), false);
  }

  private void TransferPoints(
    Entity<XenoEvolutionComponent?> old,
    Entity<XenoEvolutionComponent> xeno,
    bool subtract)
  {
    if (!this.Resolve<XenoEvolutionComponent>((EntityUid) old, ref old.Comp, false))
      return;
    xeno.Comp.Points = subtract ? FixedPoint2.Max((FixedPoint2) 0, old.Comp.Points - old.Comp.Max) : old.Comp.Points;
    this.Dirty<XenoEvolutionComponent>(xeno);
  }

  private void OnNewlyEvolvedPreventCollide(
    Entity<XenoNewlyEvolvedComponent> ent,
    ref PreventCollideEvent args)
  {
    if (!ent.Comp.StopCollide.Contains(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  private void OnGranterEvolved(
    Entity<XenoEvolutionGranterComponent> ent,
    ref NewXenoEvolvedEvent args)
  {
    this._xenoAnnounce.AnnounceSameHive((Entity<HiveMemberComponent>) ent.Owner, this.Loc.GetString("rmc-new-queen"));
  }

  private void OnOvipositorChanged(ref XenoOvipositorChangedEvent ev)
  {
    if (this._net.IsClient)
      return;
    Robust.Shared.GameObjects.EntityQueryEnumerator<ActorComponent, XenoEvolutionComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ActorComponent, XenoEvolutionComponent>();
    XenoEvolveBuiState state = new XenoEvolveBuiState(this.LackingOvipositor());
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out ActorComponent _, out XenoEvolutionComponent _))
      this._ui.SetUiState((Entity<UserInterfaceComponent>) uid, (Enum) XenoEvolutionUIKey.Key, (BoundUserInterfaceState) state);
  }

  private bool ContainedCheckPopup(EntityUid xeno, bool doPopup = true)
  {
    if (!this._container.IsEntityInContainer(xeno))
      return true;
    if (doPopup)
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-evolution-failed-bad-location"), xeno, xeno, PopupType.MediumCaution);
    return false;
  }

  private bool DamagedCheckPopup(EntityUid xeno, bool predicted = true, bool doPopup = true)
  {
    DamageableComponent comp;
    if (!this.TryComp<DamageableComponent>(xeno, out comp) || comp.TotalDamage <= 1)
      return true;
    if (predicted)
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-evolution-cant-evolve-damaged"), xeno, new EntityUid?(xeno), PopupType.MediumCaution);
    else
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-evolution-cant-evolve-damaged"), xeno, xeno, PopupType.MediumCaution);
    return false;
  }

  private bool CanEvolvePopup(
    Entity<XenoEvolutionComponent> xeno,
    EntProtoId newXeno,
    bool doPopup = true)
  {
    if (!xeno.Comp.EvolvesTo.Contains(newXeno) && !xeno.Comp.EvolvesToWithoutPoints.Contains(newXeno))
      return false;
    EntityPrototype prototype;
    if (!this._prototypes.TryIndex(newXeno, out prototype))
      return true;
    if (!this.ContainedCheckPopup((EntityUid) xeno, doPopup))
      return false;
    XenoEvolutionCappedComponent capped;
    if (prototype.TryGetComponent<XenoEvolutionCappedComponent>(out capped, this._compFactory) && this.HasLiving<XenoEvolutionCappedComponent>(capped.Max, (Predicate<Entity<XenoEvolutionCappedComponent>>) (e => e.Comp.Id == capped.Id)))
    {
      if (doPopup)
        this._popup.PopupEntity(this.Loc.GetString("cm-xeno-evolution-failed-already-have", ("prototype", (object) prototype.Name)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
      return false;
    }
    if (!xeno.Comp.CanEvolveWithoutGranter && !this.HasLiving<XenoEvolutionGranterComponent>(1))
    {
      if (doPopup)
        this._popup.PopupEntity(this.Loc.GetString("cm-xeno-evolution-failed-hive-shaken"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
      return false;
    }
    RestrictEvolveOffWeedsComponent comp1;
    EntityUid? nullable;
    if (this.TryComp<RestrictEvolveOffWeedsComponent>(xeno.Owner, out comp1))
    {
      EntityCoordinates grid = this._transform.GetMoverCoordinates((EntityUid) xeno).SnapToGrid((IEntityManager) this.EntityManager, this._map);
      nullable = this._transform.GetGrid(grid);
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault = nullable.GetValueOrDefault();
        MapGridComponent comp2;
        if (this.TryComp<MapGridComponent>(valueOrDefault, out comp2))
        {
          if (!this._xenoWeeds.IsOnWeeds((Entity<MapGridComponent>) (valueOrDefault, comp2), grid) && comp1.RestrictTime > this._gameTicker.RoundDuration())
          {
            this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-evolution-failed-early-weeds"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
            return false;
          }
          goto label_20;
        }
      }
      return false;
    }
label_20:
    XenoComponent component;
    prototype.TryGetComponent<XenoComponent>(out component, this._compFactory);
    if (component != null && component.UnlockAt > this._gameTicker.RoundDuration())
    {
      if (doPopup)
        this._popup.PopupEntity(this.Loc.GetString("cm-xeno-evolution-failed-cannot-support"), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
      return false;
    }
    if (component != null && !component.BypassTierCount)
    {
      Entity<HiveComponent>? hive = this._xenoHive.GetHive((Entity<HiveMemberComponent>) xeno.Owner);
      if (hive.HasValue)
      {
        Entity<HiveComponent> valueOrDefault = hive.GetValueOrDefault();
        FixedPoint2 fixedPoint2;
        if (this._xenoHive.TryGetTierLimit((Entity<HiveComponent>) ((EntityUid) valueOrDefault, valueOrDefault.Comp), component.Tier, out fixedPoint2))
        {
          int num1 = 0;
          double num2 = Math.Min(Math.Sqrt((double) (valueOrDefault.Comp.BurrowedLarva * valueOrDefault.Comp.BurrowedLarvaSlotFactor)), (double) valueOrDefault.Comp.BurrowedLarva);
          Robust.Shared.GameObjects.EntityQueryEnumerator<XenoComponent, HiveMemberComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoComponent, HiveMemberComponent>();
          Dictionary<EntProtoId, int> dictionary = valueOrDefault.Comp.FreeSlots.ToDictionary<EntProtoId, int>();
          EntityUid uid;
          XenoComponent comp1_1;
          HiveMemberComponent comp2;
          while (entityQueryEnumerator.MoveNext(out uid, out comp1_1, out comp2))
          {
            if (!this._mobState.IsDead(uid))
            {
              nullable = comp2.Hive;
              EntityUid owner = valueOrDefault.Owner;
              if ((nullable.HasValue ? (nullable.GetValueOrDefault() != owner ? 1 : 0) : 1) == 0 && comp1_1.CountedInSlots)
              {
                ++num2;
                if (comp1_1.Tier >= component.Tier)
                {
                  if (dictionary.ContainsKey((EntProtoId) comp1_1.Role.Id) && dictionary[(EntProtoId) comp1_1.Role.Id] > 0)
                    --dictionary[(EntProtoId) comp1_1.Role.Id];
                  else
                    ++num1;
                }
              }
            }
          }
          if (num2 != 0.0 && (FixedPoint2) ((float) num1 / (float) num2) >= fixedPoint2 && (!dictionary.ContainsKey(newXeno) || dictionary[newXeno] <= 0))
          {
            if (doPopup)
              this._popup.PopupEntity(this.Loc.GetString("cm-xeno-evolution-failed-hive-full", ("tier", (object) component.Tier)), (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
            return false;
          }
        }
      }
    }
    XenoRecentlyDevolvedComponent comp3;
    TimeSpan timeSpan1;
    if (!this.TryComp<XenoRecentlyDevolvedComponent>((EntityUid) xeno, out comp3) || !comp3.Recent.TryGetValue(newXeno, out timeSpan1) || !(timeSpan1 + this._evolveSameCasteCooldown > this._timing.CurTime))
      return true;
    TimeSpan timeSpan2 = timeSpan1 + this._evolveSameCasteCooldown - this._timing.CurTime;
    string message = this.Loc.GetString("rmc-xeno-evolution-cant-evolve-caste-cooldown", ("minutes", (object) timeSpan2.Minutes), ("seconds", (object) timeSpan2.Seconds));
    if (doPopup)
      this._popup.PopupEntity(message, (EntityUid) xeno, (EntityUid) xeno, PopupType.MediumCaution);
    return false;
  }

  private bool CanEvolveAny(Entity<XenoEvolutionComponent> xeno)
  {
    if (xeno.Comp.Points >= xeno.Comp.Max && xeno.Comp.EvolvesTo.Count > 0)
      return true;
    foreach (EntProtoId evolvesToWithoutPoint in xeno.Comp.EvolvesToWithoutPoints)
    {
      if (this.CanEvolvePopup(xeno, evolvesToWithoutPoint, false))
        return true;
    }
    return false;
  }

  public int GetLiving<T>(Predicate<Entity<T>>? predicate = null) where T : IComponent
  {
    int living = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<T> entityQueryEnumerator = this.EntityQueryEnumerator<T>();
    EntityUid uid;
    T comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      MobStateComponent component;
      if ((!this._mobStateQuery.TryComp(uid, out component) || !this._mobState.IsDead(uid, component)) && (predicate == null || predicate((Entity<T>) (uid, comp1))))
        ++living;
    }
    return living;
  }

  public bool HasLiving<T>(int count, Predicate<Entity<T>>? predicate = null) where T : IComponent
  {
    if (count <= 0)
      return true;
    int num = 0;
    Robust.Shared.GameObjects.EntityQueryEnumerator<T> entityQueryEnumerator = this.EntityQueryEnumerator<T>();
    EntityUid uid;
    T comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      MobStateComponent component;
      if ((!this._mobStateQuery.TryComp(uid, out component) || !this._mobState.IsDead(uid, component)) && (predicate == null || predicate((Entity<T>) (uid, comp1))))
      {
        ++num;
        if (num >= count)
          return true;
      }
    }
    return false;
  }

  public FixedPoint2 AddPointsCapped(Entity<XenoEvolutionComponent?> evolution, FixedPoint2 points)
  {
    if (!this.Resolve<XenoEvolutionComponent>((EntityUid) evolution, ref evolution.Comp, false))
      return FixedPoint2.Zero;
    FixedPoint2 points1 = evolution.Comp.Points;
    evolution.Comp.Points += FixedPoint2.Min(evolution.Comp.Max, points);
    this.Dirty<XenoEvolutionComponent>(evolution);
    return evolution.Comp.Points - points1;
  }

  public void SetPoints(Entity<XenoEvolutionComponent> evolution, FixedPoint2 points)
  {
    evolution.Comp.Points = points;
    this.Dirty<XenoEvolutionComponent>(evolution);
  }

  public bool NeedsOvipositor()
  {
    return this._gameTicker.RoundDuration() > this._evolutionPointsRequireOvipositorAfter;
  }

  public bool HasOvipositor()
  {
    return this.HasLiving<XenoEvolutionGranterComponent>(1, (Predicate<Entity<XenoEvolutionGranterComponent>>) (e => this.HasComp<XenoAttachedOvipositorComponent>((EntityUid) e)));
  }

  public bool LackingOvipositor() => this.NeedsOvipositor() && !this.HasOvipositor();

  private EntityUid TransferXeno(EntityUid xeno, EntProtoId proto)
  {
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(xeno);
    EntityUid entityUid = this.Spawn((string) proto, moverCoordinates);
    this._xenoHive.SetSameHive((Entity<HiveMemberComponent>) xeno, (Entity<HiveMemberComponent>) entityUid);
    EntityUid mindId;
    if (this._mind.TryGetMind(xeno, out mindId, out MindComponent _))
    {
      this._mind.TransferTo(mindId, new EntityUid?(entityUid));
      this._mind.UnVisit(mindId);
    }
    foreach (EntityUid entity in this._hands.EnumerateHeld((Entity<HandsComponent>) xeno))
      this._hands.TryDrop((Entity<HandsComponent>) xeno, entity);
    XenoNewlyEvolvedComponent evolvedComponent = this.EnsureComp<XenoNewlyEvolvedComponent>(entityUid);
    this._doors.Clear();
    this._entityLookup.GetEntitiesIntersecting(xeno, this._doors);
    foreach (EntityUid door in this._doors)
    {
      if (this.HasComp<DoorComponent>(door) || this.HasComp<AirlockComponent>(door))
        evolvedComponent.StopCollide.Add(door);
    }
    XenoRecentlyDevolvedComponent devolvedComponent = this.EnsureComp<XenoRecentlyDevolvedComponent>(entityUid);
    XenoRecentlyDevolvedComponent comp;
    if (this.TryComp<XenoRecentlyDevolvedComponent>(xeno, out comp))
    {
      foreach ((EntProtoId key, TimeSpan timeSpan) in comp.Recent)
        devolvedComponent.Recent[key] = timeSpan;
    }
    string id = this.Prototype(xeno)?.ID;
    if (id != null)
      devolvedComponent.Recent[(EntProtoId) id] = this._timing.CurTime;
    return entityUid;
  }

  private void TryDevolve(Entity<XenoDevolveComponent> xeno, EntProtoId to, bool damagedCheck = true)
  {
    if (damagedCheck && !this.DamagedCheckPopup((EntityUid) xeno))
      return;
    EntityUid? nullable = this.Devolve(xeno, to);
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    if (!this._net.IsServer)
      return;
    this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-evolution-devolve", (nameof (xeno), (object) valueOrDefault)), valueOrDefault, valueOrDefault, PopupType.LargeCaution);
  }

  public EntityUid? Devolve(Entity<XenoDevolveComponent> xeno, EntProtoId to)
  {
    if (this._net.IsClient || !((IEnumerable<EntProtoId>) xeno.Comp.DevolvesTo).Contains<EntProtoId>(to))
      return new EntityUid?();
    EntityUid entityUid = this.TransferXeno((EntityUid) xeno, to);
    XenoDevolvedEvent args1 = new XenoDevolvedEvent((EntityUid) xeno, entityUid);
    this.RaiseLocalEvent<XenoDevolvedEvent>(entityUid, ref args1, true);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(23, 2);
    logStringHandler.AppendLiteral("Xenonid ");
    logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) xeno)), "ToPrettyString(xeno)");
    logStringHandler.AppendLiteral(" devolved into ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) entityUid), "ToPrettyString(newXeno)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCDevolve, ref local);
    this.Del(new EntityUid?(xeno.Owner));
    AfterNewXenoEvolvedEvent args2 = new AfterNewXenoEvolvedEvent();
    this.RaiseLocalEvent<AfterNewXenoEvolvedEvent>(entityUid, ref args2);
    return new EntityUid?(entityUid);
  }

  public override void Update(float frameTime)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoNewlyEvolvedComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<XenoNewlyEvolvedComponent>();
    EntityUid uid1;
    XenoNewlyEvolvedComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      if (comp1_1.TriedClimb)
      {
        this._intersecting.Clear();
        this._entityLookup.GetEntitiesIntersecting(uid1, this._intersecting);
        for (int index = comp1_1.StopCollide.Count - 1; index >= 0; --index)
        {
          if (!this._intersecting.Contains(comp1_1.StopCollide[index]))
            comp1_1.StopCollide.RemoveAt(index);
        }
        if (comp1_1.StopCollide.Count == 0)
          this.RemCompDeferred<XenoNewlyEvolvedComponent>(uid1);
      }
      else
      {
        comp1_1.TriedClimb = true;
        ClimbingComponent comp;
        if (this.TryComp<ClimbingComponent>(uid1, out comp))
        {
          this._climbable.Clear();
          this._entityLookup.GetEntitiesIntersecting(uid1, this._climbable);
          foreach (EntityUid entityUid in this._climbable)
          {
            if (this.HasComp<ClimbableComponent>(entityUid))
            {
              this._climb.ForciblySetClimbing(uid1, entityUid);
              this.Dirty(uid1, (IComponent) comp);
              break;
            }
          }
        }
      }
    }
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan timeSpan = this._gameTicker.RoundDuration();
    bool flag1 = this.NeedsOvipositor();
    bool flag2 = flag1 ? this.HasOvipositor() : this.HasLiving<XenoEvolutionGranterComponent>(1);
    if (flag1)
    {
      Robust.Shared.GameObjects.EntityQueryEnumerator<XenoEvolutionGranterComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<XenoEvolutionGranterComponent>();
      EntityUid uid2;
      XenoEvolutionGranterComponent comp1_2;
      while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
      {
        if (!comp1_2.GotOvipositorPopup)
        {
          comp1_2.GotOvipositorPopup = true;
          this.Dirty(uid2, (IComponent) comp1_2);
          this._popup.PopupEntity("It is time to settle down and let your children grow.", uid2, uid2, PopupType.LargeCaution);
          this._xenoHive.AnnounceNeedsOvipositorToSameHive((Entity<HiveMemberComponent>) uid2);
        }
      }
    }
    FixedPoint2 zero = FixedPoint2.Zero;
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvolutionBonusComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<EvolutionBonusComponent>();
    EvolutionBonusComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out comp1_3))
      zero += comp1_3.Amount;
    FixedPoint2? nullable = new FixedPoint2?();
    Robust.Shared.GameObjects.EntityQueryEnumerator<EvolutionOverrideComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<EvolutionOverrideComponent>();
    EvolutionOverrideComponent comp1_4;
    while (entityQueryEnumerator4.MoveNext(out comp1_4))
      nullable = new FixedPoint2?(comp1_4.Amount);
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoEvolutionComponent> entityQueryEnumerator5 = this.EntityQueryEnumerator<XenoEvolutionComponent>();
    EntityUid uid3;
    XenoEvolutionComponent comp1_5;
    while (entityQueryEnumerator5.MoveNext(out uid3, out comp1_5))
    {
      if (!(comp1_5.Max == FixedPoint2.Zero) && !(curTime < comp1_5.LastPointsAt + TimeSpan.FromSeconds(1L)))
      {
        comp1_5.LastPointsAt = curTime;
        this.Dirty(uid3, (IComponent) comp1_5);
        if (!comp1_5.GotPopup && this.CanEvolveAny((Entity<XenoEvolutionComponent>) (uid3, comp1_5)))
        {
          comp1_5.GotPopup = true;
          this.Dirty(uid3, (IComponent) comp1_5);
          this._popup.PopupEntity(this.Loc.GetString("cm-xeno-evolution-ready"), uid3, uid3, PopupType.Large);
          this._audio.PlayEntity(comp1_5.EvolutionReadySound, uid3, uid3);
        }
        else
        {
          FixedPoint2 fixedPoint2_1 = this._earlyEvoBoostBefore > this._gameTicker.RoundDuration() ? comp1_5.EarlyPointsPerSecond : comp1_5.PointsPerSecond;
          FixedPoint2 fixedPoint2_2 = nullable ?? fixedPoint2_1 + zero;
          if (comp1_5.Points < comp1_5.Max || timeSpan < this._evolutionAccumulatePointsBefore)
          {
            if (!flag1 || !comp1_5.RequiresGranter || flag2)
              this.SetPoints((Entity<XenoEvolutionComponent>) (uid3, comp1_5), comp1_5.Points + fixedPoint2_2);
          }
          else if (comp1_5.Points > comp1_5.Max)
            this.SetPoints((Entity<XenoEvolutionComponent>) (uid3, comp1_5), FixedPoint2.Max(comp1_5.Points - fixedPoint2_2, comp1_5.Max));
        }
      }
    }
  }
}
