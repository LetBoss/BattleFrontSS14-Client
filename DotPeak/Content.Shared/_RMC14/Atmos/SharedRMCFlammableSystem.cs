// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Atmos.SharedRMCFlammableSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Armor;
using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.Chemistry.Reagent;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Explosion;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.OnCollide;
using Content.Shared._RMC14.Water;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Alert;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Directions;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Maps;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Atmos;

public abstract class SharedRMCFlammableSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private EntityWhitelistSystem _entityWhitelist;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private IMapManager _map;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedOnCollideSystem _onCollide;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototype;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private RMCReagentSystem _reagents;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private CMArmorSystem _armor;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  private static readonly ProtoId<AlertPrototype> FireAlert = (ProtoId<AlertPrototype>) "Fire";
  private static readonly ProtoId<ReagentPrototype> WaterReagent = (ProtoId<ReagentPrototype>) "Water";
  private static readonly ProtoId<TagPrototype> StructureTag = (ProtoId<TagPrototype>) "Structure";
  private static readonly ProtoId<TagPrototype> WallTag = (ProtoId<TagPrototype>) "Wall";
  private static readonly ProtoId<DamageTypePrototype> HeatDamage = (ProtoId<DamageTypePrototype>) "Heat";
  private Robust.Shared.GameObjects.EntityQuery<BlockTileFireComponent> _blockTileFireQuery;
  private Robust.Shared.GameObjects.EntityQuery<DoorComponent> _doorQuery;
  private Robust.Shared.GameObjects.EntityQuery<FlammableComponent> _flammableQuery;
  private Robust.Shared.GameObjects.EntityQuery<RMCIgniteOnCollideComponent> _igniteOnCollideQuery;
  private Robust.Shared.GameObjects.EntityQuery<ProjectileComponent> _projectileQuery;
  private Robust.Shared.GameObjects.EntityQuery<TileFireComponent> _tileFireQuery;
  private Robust.Shared.GameObjects.EntityQuery<InventoryComponent> _inventoryQuery;

  public override void Initialize()
  {
    this._blockTileFireQuery = this.GetEntityQuery<BlockTileFireComponent>();
    this._doorQuery = this.GetEntityQuery<DoorComponent>();
    this._flammableQuery = this.GetEntityQuery<FlammableComponent>();
    this._igniteOnCollideQuery = this.GetEntityQuery<RMCIgniteOnCollideComponent>();
    this._projectileQuery = this.GetEntityQuery<ProjectileComponent>();
    this._tileFireQuery = this.GetEntityQuery<TileFireComponent>();
    this._inventoryQuery = this.GetEntityQuery<InventoryComponent>();
    this.SubscribeLocalEvent<IgniteOnProjectileHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<IgniteOnProjectileHitComponent, ProjectileHitEvent>(this.OnIgniteOnProjectileHit));
    this.SubscribeLocalEvent<TileFireComponent, MapInitEvent>(new EntityEventRefHandler<TileFireComponent, MapInitEvent>(this.OnTileFireMapInit));
    this.SubscribeLocalEvent<TileFireComponent, VaporHitEvent>(new EntityEventRefHandler<TileFireComponent, VaporHitEvent>(this.OnTileFireVaporHit));
    this.SubscribeLocalEvent<TileFireComponent, InteractHandEvent>(new EntityEventRefHandler<TileFireComponent, InteractHandEvent>(this.OnTileFireInteractHand), new Type[1]
    {
      typeof (InteractionPopupSystem)
    });
    this.SubscribeLocalEvent<TileFireComponent, PreventCollideEvent>(new EntityEventRefHandler<TileFireComponent, PreventCollideEvent>(this.OnTileFirePreventCollide));
    this.SubscribeLocalEvent<CraftsIntoMolotovComponent, ExaminedEvent>(new EntityEventRefHandler<CraftsIntoMolotovComponent, ExaminedEvent>(this.OnCraftsIntoMolotovExamined));
    this.SubscribeLocalEvent<CraftsIntoMolotovComponent, InteractUsingEvent>(new EntityEventRefHandler<CraftsIntoMolotovComponent, InteractUsingEvent>(this.OnCraftsIntoMolotovInteractUsing));
    this.SubscribeLocalEvent<CraftsIntoMolotovComponent, CraftMolotovDoAfterEvent>(new EntityEventRefHandler<CraftsIntoMolotovComponent, CraftMolotovDoAfterEvent>(this.OnCraftsIntoMolotovDoAfter));
    this.SubscribeLocalEvent<TileFireOnTriggerComponent, RMCTriggerEvent>(new EntityEventRefHandler<TileFireOnTriggerComponent, RMCTriggerEvent>(this.OnTileFireTriggered));
    this.SubscribeLocalEvent<TileFireOnTriggerComponent, CMExplosiveTriggeredEvent>(new EntityEventRefHandler<TileFireOnTriggerComponent, CMExplosiveTriggeredEvent>(this.OnTileFireOnTriggerExplosive));
    this.SubscribeLocalEvent<DirectionalTileFireOnTriggerComponent, RMCTriggerEvent>(new EntityEventRefHandler<DirectionalTileFireOnTriggerComponent, RMCTriggerEvent>(this.OnDirectionTileFireTriggered));
    this.SubscribeLocalEvent<RMCIgniteOnCollideComponent, StartCollideEvent>(new EntityEventRefHandler<RMCIgniteOnCollideComponent, StartCollideEvent>(this.OnIgniteCollide));
    this.SubscribeLocalEvent<RMCIgniteOnCollideComponent, DamageCollideEvent>(new EntityEventRefHandler<RMCIgniteOnCollideComponent, DamageCollideEvent>(this.OnIgniteDamageCollide));
    this.SubscribeLocalEvent<SteppingOnFireComponent, CMGetArmorEvent>(new EntityEventRefHandler<SteppingOnFireComponent, CMGetArmorEvent>(this.OnSteppingOnFireGetArmor));
    this.SubscribeLocalEvent<SteppingOnFireComponent, ComponentRemove>(new EntityEventRefHandler<SteppingOnFireComponent, ComponentRemove>(this.OnSteppingOnFireRemoved));
    this.SubscribeLocalEvent<CanBeFirePattedComponent, InteractHandEvent>(new EntityEventRefHandler<CanBeFirePattedComponent, InteractHandEvent>(this.OnCanBeFirePattedInteractHand), new Type[1]
    {
      typeof (InteractionPopupSystem)
    });
    this.SubscribeLocalEvent<FlammableComponent, IgnitedEvent>(new EntityEventRefHandler<FlammableComponent, IgnitedEvent>(this.OnFlammableIgnite));
    this.SubscribeLocalEvent<FlammableComponent, RMCExtinguishedEvent>(new EntityEventRefHandler<FlammableComponent, RMCExtinguishedEvent>(this.OnFlammableExtinguished));
    this.SubscribeLocalEvent<PlasmaFrenzyComponent, IgnitedEvent>(new EntityEventRefHandler<PlasmaFrenzyComponent, IgnitedEvent>(this.OnPlasmaFrenzyIgnite));
    this.SubscribeLocalEvent<RMCImmuneToIgnitionComponent, GetIgnitionImmunityEvent>(new EntityEventRefHandler<RMCImmuneToIgnitionComponent, GetIgnitionImmunityEvent>(this.OnGetIgnitionImmunity));
    this.SubscribeLocalEvent<RMCImmuneToIgnitionComponent, InventoryRelayedEvent<GetIgnitionImmunityEvent>>(new EntityEventRefHandler<RMCImmuneToIgnitionComponent, InventoryRelayedEvent<GetIgnitionImmunityEvent>>(this.OnGetIgnitionEquipmentImmunity));
    this.SubscribeLocalEvent<RMCImmuneToIgnitionComponent, ExaminedEvent>(new EntityEventRefHandler<RMCImmuneToIgnitionComponent, ExaminedEvent>(this.OnIgnitionImmunityExamined));
    this.SubscribeLocalEvent<RMCImmuneToFireTileDamageComponent, RMCGetFireImmunityEvent>(new EntityEventRefHandler<RMCImmuneToFireTileDamageComponent, RMCGetFireImmunityEvent>(this.OnImmuneToTileFireGet));
    this.SubscribeLocalEvent<RMCImmuneToFireTileDamageComponent, ExaminedEvent>(new EntityEventRefHandler<RMCImmuneToFireTileDamageComponent, ExaminedEvent>(this.OnImmuneToTileFireExamined));
    this.SubscribeLocalEvent<RMCFireArmorDebuffModifierComponent, ExaminedEvent>(new EntityEventRefHandler<RMCFireArmorDebuffModifierComponent, ExaminedEvent>(this.OnFireArmorDebuffModifierExamined));
  }

  private void OnIgniteOnProjectileHit(
    Entity<IgniteOnProjectileHitComponent> ent,
    ref ProjectileHitEvent args)
  {
    if (!this.CanBeIgnited(args.Target, (EntityUid) ent, ent.Comp.Intensity))
      return;
    this.ChangeBurnColor(args.Target, ent.Comp.BurnColor);
    this.Ignite((Entity<FlammableComponent>) args.Target, ent.Comp.Intensity, ent.Comp.Duration, new int?(ent.Comp.Duration), false);
  }

  private void OnTileFireMapInit(Entity<TileFireComponent> ent, ref MapInitEvent args)
  {
    ent.Comp.SpawnedAt = this._timing.CurTime;
    this.Dirty<TileFireComponent>(ent);
  }

  private void OnTileFireVaporHit(Entity<TileFireComponent> ent, ref VaporHitEvent args)
  {
    if (this._net.IsClient)
      return;
    bool flag = false;
    foreach (string container in args.Solution.Comp.Containers)
    {
      Solution solution;
      if (this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) args.Solution.Owner, container, out Entity<SolutionComponent>? _, out solution) && solution.ContainsPrototype((string) SharedRMCFlammableSystem.WaterReagent))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return;
    if (ent.Comp.ExtinguishInstantly)
    {
      this.QueueDel(new EntityUid?((EntityUid) ent));
    }
    else
    {
      ent.Comp.Duration -= TimeSpan.FromSeconds((long) args.Power);
      this.Dirty<TileFireComponent>(ent);
    }
  }

  private void OnTileFireInteractHand(Entity<TileFireComponent> ent, ref InteractHandEvent args)
  {
    EntityUid user = args.User;
    TileFirePatterComponent comp;
    if (!this.TryComp<TileFirePatterComponent>(user, out comp))
      return;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < comp.Last + comp.Cooldown)
      return;
    comp.Last = curTime;
    this.Dirty(user, (IComponent) comp);
    ent.Comp.Duration -= comp.RemoveDuration * (double) ent.Comp.PatExtinguishMultiplier;
    this.Dirty<TileFireComponent>(ent);
    this._rmcMelee.DoLunge(user, (EntityUid) ent);
    this._audio.PlayPredicted(comp.Sound, user, new EntityUid?(user), new AudioParams?(AudioParams.Default.WithVolume(-8f).WithVariation(new float?(0.05f))));
  }

  private void OnTileFirePreventCollide(Entity<TileFireComponent> ent, ref PreventCollideEvent args)
  {
    if (args.Cancelled || !this._projectileQuery.HasComp(args.OtherEntity) && !this._tileFireQuery.HasComp(args.OtherEntity))
      return;
    args.Cancelled = true;
  }

  private void OnCraftsIntoMolotovExamined(
    Entity<CraftsIntoMolotovComponent> ent,
    ref ExaminedEvent args)
  {
    if (!this.CanCraftMolotovPopup(ent, args.Examiner, false, out FixedPoint2 _))
      return;
    using (args.PushGroup("CraftsIntoMolotovComponent"))
      args.PushMarkup("[color=cyan]You can turn this into a molotov with a piece of paper![/color]");
  }

  private void OnCraftsIntoMolotovInteractUsing(
    Entity<CraftsIntoMolotovComponent> ent,
    ref InteractUsingEvent args)
  {
    if (!this.HasComp<PaperComponent>(args.Used) || !this.CanCraftMolotovPopup(ent, args.User, true, out FixedPoint2 _))
      return;
    CraftMolotovDoAfterEvent @event = new CraftMolotovDoAfterEvent();
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, ent.Comp.Delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(args.Used))
    {
      BreakOnMove = true
    });
  }

  private void OnCraftsIntoMolotovDoAfter(
    Entity<CraftsIntoMolotovComponent> ent,
    ref CraftMolotovDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled)
      return;
    args.Handled = true;
    FixedPoint2 intensity;
    if (!this.HasComp<PaperComponent>(args.Used) || !this.CanCraftMolotovPopup(ent, args.User, true, out intensity) || this._net.IsClient)
      return;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent);
    EntityUid entityUid = this.Spawn((string) ent.Comp.Spawns, moverCoordinates);
    TileFireOnTriggerComponent triggerComponent = this.EnsureComp<TileFireOnTriggerComponent>(entityUid);
    triggerComponent.Duration = new int?(intensity.Int());
    this.Dirty(entityUid, (IComponent) triggerComponent);
    this.Del(new EntityUid?((EntityUid) ent));
    this.Del(args.Used);
    this._hands.TryPickupAnyHand(args.User, entityUid);
  }

  private void OnTileFireTriggered(Entity<TileFireOnTriggerComponent> ent, ref RMCTriggerEvent args)
  {
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) ent);
    this._audio.PlayPvs(ent.Comp.Sound, moverCoordinates);
    EntityCoordinates grid = moverCoordinates.SnapToGrid((IEntityManager) this.EntityManager, this._map);
    this.SpawnFireDiamond(ent.Comp.Spawn, grid, ent.Comp.Range, ent.Comp.Intensity, ent.Comp.Duration);
    this.QueueDel(new EntityUid?((EntityUid) ent));
  }

  private void OnDirectionTileFireTriggered(
    Entity<DirectionalTileFireOnTriggerComponent> ent,
    ref RMCTriggerEvent args)
  {
    (EntityCoordinates Coords, Angle worldRot) coordinateRotation = this._transform.GetMoverCoordinateRotation((EntityUid) ent, this.Transform((EntityUid) ent));
    EntityCoordinates entityCoordinates = coordinateRotation.Coords.SnapToGrid((IEntityManager) this.EntityManager, this._map);
    DirectionalTileFireOnTriggerComponent comp = ent.Comp;
    Angle angle = DirectionExtensions.ToAngle(ent.Comp.Direction);
    // ISSUE: explicit reference operation
    angle = Angle.FromDegrees(((Angle) ref angle).Degrees + ((Angle) @coordinateRotation.worldRot).Degrees);
    Direction dir = ((Angle) ref angle).GetDir();
    comp.Direction = dir;
    this.Dirty<DirectionalTileFireOnTriggerComponent>(ent);
    if (ent.Comp.Rebounded)
      entityCoordinates = entityCoordinates.Offset(ent.Comp.Direction);
    this._audio.PlayPvs(ent.Comp.Sound, coordinateRotation.Coords);
    this.SpawnFireCone(ent, entityCoordinates, ent.Comp.Intensity, ent.Comp.Duration);
    this.QueueDel(new EntityUid?((EntityUid) ent));
  }

  private void OnTileFireOnTriggerExplosive(
    Entity<TileFireOnTriggerComponent> ent,
    ref CMExplosiveTriggeredEvent args)
  {
    EntityCoordinates grid = this._transform.GetMoverCoordinates((EntityUid) ent).SnapToGrid((IEntityManager) this.EntityManager, this._map);
    this.SpawnFireDiamond(ent.Comp.Spawn, grid, ent.Comp.Range, ent.Comp.Intensity, ent.Comp.Duration);
  }

  private void OnIgniteCollide(Entity<RMCIgniteOnCollideComponent> ent, ref StartCollideEvent args)
  {
    this.TryIgnite(ent, args.OtherEntity, false);
  }

  private void OnIgniteDamageCollide(
    Entity<RMCIgniteOnCollideComponent> ent,
    ref DamageCollideEvent args)
  {
    if (!this.CanBeIgnited(args.Target, (EntityUid) ent, ent.Comp.Intensity, true))
      return;
    this.Ignite((Entity<FlammableComponent>) args.Target, ent.Comp.Intensity, ent.Comp.Duration, ent.Comp.MaxStacks);
  }

  private void OnSteppingOnFireRemoved(
    Entity<SteppingOnFireComponent> ent,
    ref ComponentRemove args)
  {
    this._armor.UpdateArmorValue((Entity<CMArmorComponent>) ((EntityUid) ent, (CMArmorComponent) null));
  }

  private void OnSteppingOnFireGetArmor(
    Entity<SteppingOnFireComponent> ent,
    ref CMGetArmorEvent args)
  {
    args.ArmorModifier *= ent.Comp.ArmorMultiplier;
  }

  private void OnCanBeFirePattedInteractHand(
    Entity<CanBeFirePattedComponent> ent,
    ref InteractHandEvent args)
  {
    EntityUid user = args.User;
    FirePatterComponent comp1;
    FlammableComponent comp2;
    if (args.Target != ent.Owner || user == args.Target || !this.TryComp<FirePatterComponent>(user, out comp1) || this._entityWhitelist.IsBlacklistPass(comp1.Blacklist, (EntityUid) ent) || !this.TryComp<FlammableComponent>((EntityUid) ent, out comp2) || !comp2.OnFire)
      return;
    args.Handled = true;
    TimeSpan curTime = this._timing.CurTime;
    if (curTime < comp1.LastPat + comp1.Cooldown)
      return;
    comp1.LastPat = curTime;
    this.Dirty(user, (IComponent) comp1);
    this.Pat((Entity<FlammableComponent>) ent.Owner, comp1.Stacks);
    this._audio.PlayPredicted(comp1.Sound, user, new EntityUid?(user));
    this._popup.PopupClient($"You try to put out the fire on {this.Name((EntityUid) ent)}!", (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
    this._popup.PopupEntity(this.Name(user) + " tries to put out the fire on you!", (EntityUid) ent, (EntityUid) ent, PopupType.SmallCaution);
    Filter filter = Filter.PvsExcept((EntityUid) ent).RemoveWhereAttachedEntity((Predicate<EntityUid>) (e => e == user || e == ent.Owner));
    this._popup.PopupEntity($"{this.Name(user)} tries to put out the fire on {this.Name((EntityUid) ent)}!", (EntityUid) ent, filter, true);
  }

  private void OnFlammableIgnite(Entity<FlammableComponent> ent, ref IgnitedEvent args)
  {
    this.EnsureComp<OnFireComponent>((EntityUid) ent);
  }

  private void OnFlammableExtinguished(
    Entity<FlammableComponent> ent,
    ref RMCExtinguishedEvent args)
  {
    this.RemCompDeferred<OnFireComponent>((EntityUid) ent);
    this.RemCompDeferred<RMCFireBypassActiveComponent>((EntityUid) ent);
  }

  public void UpdateFireAlert(EntityUid ent)
  {
    ShowFireAlertEvent args = new ShowFireAlertEvent();
    this.RaiseLocalEvent<ShowFireAlertEvent>(ent, ref args);
    if (args.Show)
      this._alerts.ShowAlert(ent, SharedRMCFlammableSystem.FireAlert);
    else
      this._alerts.ClearAlert(ent, SharedRMCFlammableSystem.FireAlert);
  }

  public bool IsOnFire(Entity<FlammableComponent?> ent)
  {
    return this.Resolve<FlammableComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.OnFire;
  }

  public virtual bool Ignite(
    Entity<FlammableComponent?> flammable,
    int intensity,
    int duration,
    int? maxStacks,
    bool igniteDamage = true)
  {
    return false;
  }

  public virtual void Extinguish(Entity<FlammableComponent?> flammable)
  {
  }

  public virtual void Pat(Entity<FlammableComponent?> flammable, int stacks)
  {
  }

  public virtual void AdjustStacks(Entity<FlammableComponent?> flammable, int stacks)
  {
  }

  public virtual void DoStopDropRollAnimation(EntityUid uid)
  {
  }

  private void SpawnFireChain(
    EntProtoId spawn,
    EntityUid chain,
    EntityCoordinates coordinates,
    int? intensity,
    int? duration)
  {
    EntityUid uid = this.Spawn((string) spawn, coordinates);
    if (intensity.HasValue || duration.HasValue)
    {
      RMCIgniteOnCollideComponent collideComponent = this.EnsureComp<RMCIgniteOnCollideComponent>(uid);
      if (intensity.HasValue)
        collideComponent.Intensity = intensity.Value;
      if (duration.HasValue)
        collideComponent.Duration = duration.Value;
      this.Dirty(uid, (IComponent) collideComponent);
    }
    DamageOnCollideComponent collideComponent1 = this.EnsureComp<DamageOnCollideComponent>(uid);
    this._onCollide.SetChain((Entity<DamageOnCollideComponent>) (uid, collideComponent1), chain);
  }

  private void SpawnFires(
    EntProtoId spawn,
    EntityCoordinates coordinates,
    int range,
    EntityUid chain,
    int? intensity,
    int? duration,
    HashSet<EntityCoordinates>? spawned = null)
  {
    if (this._net.IsClient)
      return;
    if (spawned == null)
      spawned = new HashSet<EntityCoordinates>();
    foreach (Direction cardinalDirection in this._rmcMap.CardinalDirections)
    {
      EntityCoordinates target = coordinates.Offset(cardinalDirection);
      if (spawned.Add(target))
      {
        bool cont;
        int nextRange = this.SpawnFire(target, spawn, chain, range, intensity, duration, out cont);
        if (!(nextRange == 0 | cont))
          Timer.Spawn(TimeSpan.FromMilliseconds(50L), (Action) (() =>
          {
            try
            {
              this.SpawnFires(spawn, target, nextRange, chain, intensity, duration, spawned);
            }
            catch (Exception ex)
            {
              this.Log.Error($"Error occurred spawning fires:\n{ex}");
            }
          }));
      }
    }
  }

  public void SpawnFireDiamond(
    EntProtoId spawn,
    EntityCoordinates center,
    int range,
    int? intensity = null,
    int? duration = null)
  {
    Entity<CollideChainComponent> chain = this._onCollide.SpawnChain();
    this.SpawnFire(center, spawn, (EntityUid) chain, range, intensity, duration, out bool _);
    this.SpawnFires(spawn, center, range, (EntityUid) chain, intensity, duration);
  }

  public void SpawnFireLines(
    EntProtoId spawn,
    EntityCoordinates center,
    int cardinalRange,
    int ordinalRange,
    int? intensity = null,
    int? duration = null)
  {
    Entity<CollideChainComponent> chain = this._onCollide.SpawnChain();
    HashSet<EntityCoordinates> entityCoordinatesSet = new HashSet<EntityCoordinates>();
    foreach (Direction allDirection in DirectionExtensions.AllDirections)
    {
      int range = this._rmcMap.CardinalDirections.Contains(allDirection) ? cardinalRange : ordinalRange;
      EntityCoordinates entityCoordinates = center.Offset(allDirection);
      while (range > 0)
      {
        if (entityCoordinatesSet.Add(entityCoordinates))
        {
          bool cont;
          range = this.SpawnFire(entityCoordinates, spawn, (EntityUid) chain, range, intensity, duration, out cont);
          entityCoordinates = entityCoordinates.Offset(allDirection);
          if (cont)
            break;
        }
      }
    }
  }

  public int SpawnFire(
    EntityCoordinates target,
    EntProtoId spawn,
    EntityUid chain,
    int range,
    int? intensity,
    int? duration,
    out bool cont)
  {
    cont = false;
    ContentTileDefinition def;
    if (!this._rmcMap.TryGetTileDef(target, out def) || def.ID == "Space")
    {
      cont = true;
      return range;
    }
    Entity<TileFireComponent> ent;
    if (this._rmcMap.HasAnchoredEntityEnumerator<TileFireComponent>(target, out ent, facing: (DirectionFlag) 0))
    {
      EntProtoId entProtoId = spawn;
      EntProtoId<TileFireComponent>? id = ent.Comp.Id;
      EntProtoId? nullable = id.HasValue ? new EntProtoId?((EntProtoId) id.GetValueOrDefault()) : new EntProtoId?();
      if ((nullable.HasValue ? (entProtoId == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
      {
        cont = true;
        return range;
      }
      this.QueueDel(new EntityUid?((EntityUid) ent));
    }
    int num = range - 1;
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(target, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
    {
      if (this._blockTileFireQuery.HasComp(uid))
      {
        num = 0;
        break;
      }
      if (this._tag.HasAnyTag(uid, SharedRMCFlammableSystem.StructureTag, SharedRMCFlammableSystem.WallTag) && !this._doorQuery.HasComp(uid))
      {
        num = 0;
        break;
      }
    }
    this.SpawnFireChain(spawn, chain, target, intensity, duration);
    return num;
  }

  private void SpawnFireCone(
    Entity<DirectionalTileFireOnTriggerComponent> ent,
    EntityCoordinates center,
    int? intensity = null,
    int? duration = null)
  {
    Entity<CollideChainComponent> chain = this._onCollide.SpawnChain();
    if (this._net.IsClient)
      return;
    ent.Comp.DiagonalRange = (int) Math.Floor((double) ent.Comp.Range / 2.0);
    this.Dirty<DirectionalTileFireOnTriggerComponent>(ent);
    bool initialShot = !ent.Comp.InitialSpread;
    EntityCoordinates entityCoordinates1 = center;
    HashSet<EntityCoordinates> entityCoordinatesSet = new HashSet<EntityCoordinates>();
    while (ent.Comp.Range > 0)
    {
      foreach (EntityCoordinates entityCoordinates2 in this.AddTarget(ent, entityCoordinates1, initialShot))
        entityCoordinatesSet.Add(entityCoordinates2);
      initialShot = false;
      RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(entityCoordinates1, facing: (DirectionFlag) 0);
      EntityUid uid;
      while (entitiesEnumerator.MoveNext(out uid))
      {
        if (this._tag.HasAnyTag(uid, SharedRMCFlammableSystem.WallTag) && !this._doorQuery.HasComp(uid))
        {
          ent.Comp.Range = 0;
          break;
        }
      }
      entityCoordinates1 = this.ChangeTarget(entityCoordinates1, ent.Comp.Direction);
      --ent.Comp.Range;
      --ent.Comp.DiagonalRange;
    }
    foreach (EntityCoordinates entityCoordinates3 in entityCoordinatesSet)
    {
      if (this.CheckViableTile(ent, entityCoordinates3))
        this.SpawnFireChain(ent.Comp.Spawn, (EntityUid) chain, entityCoordinates3, intensity, duration);
    }
  }

  private EntityCoordinates ChangeTarget(EntityCoordinates target, Direction direction)
  {
    return target.Offset(direction);
  }

  private HashSet<EntityCoordinates> AddTarget(
    Entity<DirectionalTileFireOnTriggerComponent> ent,
    EntityCoordinates target,
    bool initialShot)
  {
    HashSet<EntityCoordinates> entityCoordinatesSet = new HashSet<EntityCoordinates>()
    {
      target
    };
    int width = ent.Comp.Width;
    int num = ent.Comp.Width + 1;
    Angle angle1 = DirectionExtensions.ToAngle(ent.Comp.Direction);
    double degrees = ((Angle) ref angle1).Degrees;
    EntityCoordinates target1 = target;
    EntityCoordinates entityCoordinates1 = target;
    EntityCoordinates entityCoordinates2 = target;
    Angle angle2;
    for (; width > 0; --width)
    {
      if ((int) degrees % 90 != 0)
      {
        for (; num > 0 && ent.Comp.DiagonalRange > 0; --num)
        {
          target1 = this.ChangeTarget(target1, ent.Comp.Direction);
          EntityCoordinates target2 = entityCoordinates1;
          angle2 = Angle.FromDegrees(degrees - degrees % 90.0);
          Direction dir1 = ((Angle) ref angle2).GetDir();
          entityCoordinates1 = this.ChangeTarget(target2, dir1);
          EntityCoordinates target3 = entityCoordinates2;
          angle2 = Angle.FromDegrees(degrees + degrees % 90.0);
          Direction dir2 = ((Angle) ref angle2).GetDir();
          entityCoordinates2 = this.ChangeTarget(target3, dir2);
          entityCoordinatesSet.Add(entityCoordinates1);
          entityCoordinatesSet.Add(entityCoordinates2);
          entityCoordinatesSet.Add(target1);
        }
      }
      else if (!initialShot)
      {
        EntityCoordinates target4 = entityCoordinates1;
        angle2 = Angle.FromDegrees(degrees - 90.0);
        Direction dir3 = ((Angle) ref angle2).GetDir();
        entityCoordinates1 = this.ChangeTarget(target4, dir3);
        EntityCoordinates target5 = entityCoordinates2;
        angle2 = Angle.FromDegrees(degrees + 90.0);
        Direction dir4 = ((Angle) ref angle2).GetDir();
        entityCoordinates2 = this.ChangeTarget(target5, dir4);
        entityCoordinatesSet.Add(entityCoordinates1);
        entityCoordinatesSet.Add(entityCoordinates2);
      }
    }
    return entityCoordinatesSet;
  }

  private bool CheckViableTile(
    Entity<DirectionalTileFireOnTriggerComponent> ent,
    EntityCoordinates target)
  {
    ContentTileDefinition def;
    if (!this._rmcMap.TryGetTileDef(target, out def) || def.ID == "Space")
      return false;
    Entity<TileFireComponent> ent1;
    if (this._rmcMap.HasAnchoredEntityEnumerator<TileFireComponent>(target, out ent1, facing: (DirectionFlag) 0))
      this.QueueDel(new EntityUid?((EntityUid) ent1));
    return true;
  }

  private bool CanCraftMolotovPopup(
    Entity<CraftsIntoMolotovComponent> ent,
    EntityUid user,
    bool popup,
    out FixedPoint2 intensity)
  {
    intensity = new FixedPoint2();
    Solution solution;
    if (!this._solutionContainer.TryGetSolution((Entity<SolutionContainerManagerComponent>) ent.Owner, ent.Comp.SolutionId, out Entity<SolutionComponent>? _, out solution) || solution.Volume <= FixedPoint2.Zero)
    {
      if (popup)
        this._popup.PopupClient($"The {this.Name((EntityUid) ent)} is empty...", (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    intensity = FixedPoint2.Zero;
    foreach (ReagentQuantity reagentQuantity in solution)
    {
      Content.Shared._RMC14.Chemistry.Reagent.Reagent reagent;
      if (this._reagents.TryIndex((ProtoId<ReagentPrototype>) reagentQuantity.Reagent.Prototype, out reagent))
        intensity += reagent.IntensityMod * reagentQuantity.Quantity;
    }
    if (intensity < ent.Comp.MinIntensity)
    {
      if (popup)
        this._popup.PopupClient($"There's not enough flammable liquid in the {this.Name((EntityUid) ent)}!", (EntityUid) ent, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    intensity = FixedPoint2.Min(intensity, ent.Comp.MaxIntensity);
    return true;
  }

  private void OnPlasmaFrenzyIgnite(Entity<PlasmaFrenzyComponent> ent, ref IgnitedEvent args)
  {
    XenoPlasmaComponent comp;
    if (!this.TryComp<XenoPlasmaComponent>((EntityUid) ent, out comp))
      return;
    if (comp.Plasma < comp.MaxPlasma && this._net.IsServer)
    {
      this._emote.TryEmoteWithChat((EntityUid) ent, ent.Comp.RoarEmote);
      this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-plasma-frenzy-fire"), (EntityUid) ent, (EntityUid) ent, PopupType.SmallCaution);
    }
    this._plasma.SetPlasma((Entity<XenoPlasmaComponent>) ((EntityUid) ent, comp), (FixedPoint2) comp.MaxPlasma);
  }

  private void OnGetIgnitionEquipmentImmunity(
    Entity<RMCImmuneToIgnitionComponent> ent,
    ref InventoryRelayedEvent<GetIgnitionImmunityEvent> args)
  {
    this.OnGetIgnitionImmunity(ent, ref args.Args);
  }

  private void OnGetIgnitionImmunity(
    Entity<RMCImmuneToIgnitionComponent> ent,
    ref GetIgnitionImmunityEvent args)
  {
    if (ent.Comp.IntensityResistance < args.Intensity || !ent.Comp.ImmuneToDirectHits && args.DirectHit)
      return;
    args.Ignite = false;
  }

  private void OnIgnitionImmunityExamined(
    Entity<RMCImmuneToIgnitionComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("RMCImmuneToIgnitionComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-immune-to-ignition-examine", (nameof (ent), (object) ent), ("direct", (object) ent.Comp.ImmuneToDirectHits)));
  }

  private void OnImmuneToTileFireGet(
    Entity<RMCImmuneToFireTileDamageComponent> ent,
    ref RMCGetFireImmunityEvent args)
  {
    if (!args.Fire.HasValue)
    {
      args.Immune = true;
    }
    else
    {
      if (this._entityWhitelist.IsWhitelistPass(ent.Comp.BypassWhitelist, args.Fire.Value))
        return;
      args.Immune = true;
    }
  }

  private void OnImmuneToTileFireExamined(
    Entity<RMCImmuneToFireTileDamageComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("RMCImmuneToFireTileDamageComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-immune-to-fire-tile-damage-examine", (nameof (ent), (object) ent)));
  }

  private void OnFireArmorDebuffModifierExamined(
    Entity<RMCFireArmorDebuffModifierComponent> ent,
    ref ExaminedEvent args)
  {
    using (args.PushGroup("RMCFireArmorDebuffModifierComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-fire-armor-debuff-modifier-examine", (nameof (ent), (object) ent), ("percentage", (object) $"{(float) (((double) ent.Comp.DebuffModifier - 1.0) * 100.0):F0}")));
  }

  public void SetIntensityDuration(
    Entity<RMCIgniteOnCollideComponent?, DamageOnCollideComponent?> ent,
    int? intensity,
    int? duration)
  {
    this.Resolve<RMCIgniteOnCollideComponent, DamageOnCollideComponent>((EntityUid) ent, ref ent.Comp1, ref ent.Comp2, false);
    if (ent.Comp1 != null)
    {
      if (intensity.HasValue)
        ent.Comp1.Intensity = intensity.Value;
      if (duration.HasValue)
        ent.Comp1.Duration = duration.Value;
      this.Dirty((EntityUid) ent, (IComponent) ent.Comp1);
    }
    if (ent.Comp2 == null)
      return;
    if (duration.HasValue)
      ent.Comp2.Damage.DamageDict[(string) SharedRMCFlammableSystem.HeatDamage] = (FixedPoint2) duration.Value;
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp2);
  }

  private void TryIgnite(
    Entity<RMCIgniteOnCollideComponent> ent,
    EntityUid other,
    bool checkIgnited)
  {
    this.EnsureComp<SteppingOnFireComponent>(other);
    Entity<FlammableComponent> entity = new Entity<FlammableComponent>(other, (FlammableComponent) null);
    if (!this.Resolve<FlammableComponent>((EntityUid) entity, ref entity.Comp, false))
      return;
    bool flag = this.IsOnFire(entity);
    if (checkIgnited & flag || !this.CanBeIgnited(other, (EntityUid) ent, ent.Comp.Intensity))
      return;
    RMCGetFireImmunityEvent args = new RMCGetFireImmunityEvent(new EntityUid?((EntityUid) ent));
    this.RaiseLocalEvent<RMCGetFireImmunityEvent>(other, ref args);
    if (!args.Ignite || !this.Ignite(entity, ent.Comp.Intensity, ent.Comp.Duration, ent.Comp.MaxStacks))
      return;
    this.ChangeBurnColor((EntityUid) entity, ent.Comp.BurnColor);
    if (!this.CanFireBypassImmunity((EntityUid) ent, other))
      this.RemCompDeferred<RMCFireBypassActiveComponent>(other);
    else
      this.EnsureComp<RMCFireBypassActiveComponent>(other);
    if (flag || !this.IsOnFire(entity) || !this.CanFireBypassImmunity((EntityUid) ent, other))
      return;
    this._damageable.TryChangeDamage(new EntityUid?((EntityUid) entity), entity.Comp.Damage * (float) ent.Comp.Intensity, true);
  }

  private void ApplyTileEffect(
    Entity<SteppingOnFireComponent> ent,
    RMCIgniteOnCollideComponent ignite,
    EntityUid fireEntity)
  {
    TimeSpan curTime = this._timing.CurTime;
    DamageSpecifier tileDamage = ignite.TileDamage;
    if (tileDamage == null)
      return;
    SteppingOnFireComponent comp1 = ent.Comp;
    EntityUid owner = ent.Owner;
    if (ignite.ArmorMultiplier < comp1.ArmorMultiplier && this._entityWhitelist.IsWhitelistPassOrNull(ignite.ArmorWhitelist, owner))
    {
      comp1.ArmorMultiplier = ignite.ArmorMultiplier;
      RMCFireArmorDebuffModifierComponent comp2;
      if (this.TryComp<RMCFireArmorDebuffModifierComponent>(owner, out comp2))
        comp1.ArmorMultiplier *= (double) comp2.DebuffModifier;
      this._armor.UpdateArmorValue((Entity<CMArmorComponent>) (owner, (CMArmorComponent) null));
    }
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates(owner);
    EntityCoordinates? lastPosition = comp1.LastPosition;
    float distance;
    if (lastPosition.HasValue && lastPosition.GetValueOrDefault().TryDistance((IEntityManager) this.EntityManager, this._transform, moverCoordinates, out distance))
    {
      comp1.Distance += distance;
      if ((double) comp1.Distance >= 1.0)
      {
        comp1.Distance = 0.0f;
        if (this.CanFireBypassImmunity(fireEntity, owner))
          this._damageable.TryChangeDamage(new EntityUid?(owner), tileDamage * (float) ignite.Intensity, true);
      }
    }
    FlammableComponent component1;
    if (!this._flammableQuery.TryComp(ent.Owner, out component1))
      return;
    if (this.CanBeIgnited(owner, fireEntity, ignite.Intensity))
    {
      this.Ignite((Entity<FlammableComponent>) (owner, component1), ignite.Intensity, ignite.Duration, ignite.MaxStacks);
      if (!this.CanFireBypassImmunity(fireEntity, owner))
        this.RemCompDeferred<RMCFireBypassActiveComponent>(owner);
      else
        this.EnsureComp<RMCFireBypassActiveComponent>(owner);
    }
    else if (this.CanFireBypassImmunity(fireEntity, owner))
    {
      GetFireProtectionEvent args = new GetFireProtectionEvent();
      this.RaiseLocalEvent<GetFireProtectionEvent>(owner, ref args);
      InventoryComponent component2;
      if (this._inventoryQuery.TryComp(owner, out component2))
        this._inventory.RelayEvent<GetFireProtectionEvent>((Entity<InventoryComponent>) (owner, component2), ref args);
      if (ent.Comp.UpdateAt <= curTime)
      {
        this._damageable.TryChangeDamage(new EntityUid?(owner), (float) ignite.Intensity / 5f * component1.Damage * args.Multiplier, true, false);
        ent.Comp.UpdateAt = curTime + ent.Comp.UpdateTime;
      }
    }
    comp1.LastPosition = new EntityCoordinates?(moverCoordinates);
    this.Dirty<SteppingOnFireComponent>(ent);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<TileFireComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<TileFireComponent>();
    EntityUid uid1;
    TileFireComponent comp1_1;
    while (entityQueryEnumerator1.MoveNext(out uid1, out comp1_1))
    {
      TimeSpan timeSpan = comp1_1.SpawnedAt + comp1_1.Duration - curTime;
      if (timeSpan <= TimeSpan.Zero)
        this.QueueDel(new EntityUid?(uid1));
      else if (curTime < comp1_1.SpawnedAt + comp1_1.BigFireDuration)
        this._appearance.SetData(uid1, (Enum) TileFireLayers.Base, (object) TileFireVisuals.Four);
      else if (timeSpan < TimeSpan.FromSeconds(9L))
        this._appearance.SetData(uid1, (Enum) TileFireLayers.Base, (object) TileFireVisuals.One);
      else if (timeSpan < TimeSpan.FromSeconds(25L))
        this._appearance.SetData(uid1, (Enum) TileFireLayers.Base, (object) TileFireVisuals.Two);
      else
        this._appearance.SetData(uid1, (Enum) TileFireLayers.Base, (object) TileFireVisuals.Three);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCIgniteOnCollideComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RMCIgniteOnCollideComponent>();
    EntityUid uid2;
    RMCIgniteOnCollideComponent comp1_2;
    while (entityQueryEnumerator2.MoveNext(out uid2, out comp1_2))
    {
      foreach (EntityUid other in this._physics.GetEntitiesIntersectingBody(uid2, (int) comp1_2.Collision))
        this.TryIgnite((Entity<RMCIgniteOnCollideComponent>) (uid2, comp1_2), other, true);
      RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(uid2, facing: (DirectionFlag) 0);
      EntityUid uid3;
      while (entitiesEnumerator.MoveNext(out uid3))
        this.TryIgnite((Entity<RMCIgniteOnCollideComponent>) (uid2, comp1_2), uid3, true);
      if (!comp1_2.InitDamaged)
      {
        comp1_2.InitDamaged = true;
        this.Dirty(uid2, (IComponent) comp1_2);
        this.RemCompDeferred<DamageOnCollideComponent>(uid2);
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<ExtinguishFireComponent> entityQueryEnumerator3 = this.EntityQueryEnumerator<ExtinguishFireComponent>();
    EntityUid uid4;
    ExtinguishFireComponent comp1_3;
    while (entityQueryEnumerator3.MoveNext(out uid4, out comp1_3))
    {
      if (!comp1_3.Extinguished)
      {
        comp1_3.Extinguished = true;
        this.Dirty(uid4, (IComponent) comp1_3);
        foreach (EntityUid entityUid in this._physics.GetEntitiesIntersectingBody(uid4, (int) comp1_3.Collision))
        {
          FlammableComponent component;
          if (this._flammableQuery.TryComp(entityUid, out component))
          {
            ExtinguishFireAttemptEvent args = new ExtinguishFireAttemptEvent(uid4, entityUid);
            this.RaiseLocalEvent<ExtinguishFireAttemptEvent>(uid4, ref args);
            if (!args.Cancelled)
              this.Extinguish((Entity<FlammableComponent>) (entityUid, component));
          }
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<SprayExtinguishTileFireComponent> entityQueryEnumerator4 = this.EntityQueryEnumerator<SprayExtinguishTileFireComponent>();
label_36:
    EntityUid uid5;
    SprayExtinguishTileFireComponent comp1_4;
    while (entityQueryEnumerator4.MoveNext(out uid5, out comp1_4))
    {
      if (!comp1_4.Extinguished)
      {
        comp1_4.Extinguished = true;
        this.Dirty(uid5, (IComponent) comp1_4);
        RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(uid5, facing: (DirectionFlag) 0);
        while (true)
        {
          EntityUid uid6;
          TileFireComponent component;
          do
          {
            if (!entitiesEnumerator.MoveNext(out uid6))
              goto label_36;
          }
          while (!this._tileFireQuery.TryComp(uid6, out component));
          component.Duration -= comp1_4.ExtinguishAmount * (double) component.SprayExtinguishMultiplier;
          this.Dirty(uid6, (IComponent) component);
        }
      }
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<SteppingOnFireComponent, PhysicsComponent> entityQueryEnumerator5 = this.EntityQueryEnumerator<SteppingOnFireComponent, PhysicsComponent>();
    EntityUid uid7;
    SteppingOnFireComponent comp1_5;
    PhysicsComponent comp2_1;
    while (entityQueryEnumerator5.MoveNext(out uid7, out comp1_5, out comp2_1))
    {
      comp1_5.ArmorMultiplier = 1.0;
      this.Dirty(uid7, (IComponent) comp1_5);
      bool flag = false;
      foreach (EntityUid contactingEntity in this._physics.GetContactingEntities(uid7, comp2_1, true))
      {
        RMCIgniteOnCollideComponent component;
        if (this._igniteOnCollideQuery.TryComp(contactingEntity, out component))
        {
          this.ApplyTileEffect((Entity<SteppingOnFireComponent>) (uid7, comp1_5), component, contactingEntity);
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        HashSet<Entity<RMCIgniteOnCollideComponent>> entitiesInRange = this._entityLookup.GetEntitiesInRange<RMCIgniteOnCollideComponent>(this.Transform(uid7).Coordinates, 0.35f);
        if (entitiesInRange.Count != 0)
        {
          Entity<RMCIgniteOnCollideComponent> entity = entitiesInRange.First<Entity<RMCIgniteOnCollideComponent>>();
          this.ApplyTileEffect((Entity<SteppingOnFireComponent>) (uid7, comp1_5), entity.Comp, entity.Owner);
          flag = true;
        }
      }
      if (!flag)
        this.RemCompDeferred<SteppingOnFireComponent>(uid7);
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<OnFireComponent, FlammableComponent> entityQueryEnumerator6 = this.EntityQueryEnumerator<OnFireComponent, FlammableComponent>();
    EntityUid uid8;
    FlammableComponent comp2_2;
    while (entityQueryEnumerator6.MoveNext(out uid8, out OnFireComponent _, out comp2_2))
    {
      if (this._rmcMap.HasAnchoredEntityEnumerator<RMCWaterComponent>(this._transform.GetMoverCoordinates(uid8), facing: (DirectionFlag) 0))
        this.Extinguish((Entity<FlammableComponent>) (uid8, comp2_2));
    }
  }

  private bool CanFireBypassImmunity(EntityUid fireEntity, EntityUid targetEntity)
  {
    if (this.HasComp<RMCFireImmunityBypassComponent>(fireEntity))
      return true;
    RMCGetFireImmunityEvent args = new RMCGetFireImmunityEvent(new EntityUid?(fireEntity));
    this.RaiseLocalEvent<RMCGetFireImmunityEvent>(targetEntity, ref args);
    return !args.Immune;
  }

  public bool CanBeIgnited(EntityUid target, EntityUid fireSource, int intensity, bool directHit = false)
  {
    GetIgnitionImmunityEvent ignitionImmunityEvent = new GetIgnitionImmunityEvent(intensity, directHit);
    this.RaiseLocalEvent<GetIgnitionImmunityEvent>(target, ref ignitionImmunityEvent);
    this.RaiseLocalEvent<GetIgnitionImmunityEvent>(ref ignitionImmunityEvent);
    InventoryComponent component;
    if (this._inventoryQuery.TryComp(target, out component))
      this._inventory.RelayEvent<GetIgnitionImmunityEvent>((Entity<InventoryComponent>) (target, component), ref ignitionImmunityEvent);
    return ignitionImmunityEvent.Ignite;
  }

  public void ChangeBurnColor(EntityUid target, Color color)
  {
    RMCFireColorComponent comp;
    if (!this.TryComp<RMCFireColorComponent>(target, out comp))
      return;
    comp.Color = color;
    this.Dirty(target, (IComponent) comp);
  }
}
