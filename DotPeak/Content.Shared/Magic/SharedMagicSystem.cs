// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.SharedMagicSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Lock;
using Content.Shared.Magic.Components;
using Content.Shared.Magic.Events;
using Content.Shared.Maps;
using Content.Shared.Mind;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Speech.Muting;
using Content.Shared.Storage;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Spawners;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Magic;

public abstract class SharedMagicSystem : EntitySystem
{
  [Dependency]
  private ISerializationManager _seriMan;
  [Dependency]
  private IMapManager _mapManager;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedGunSystem _gunSystem;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private SharedDoorSystem _door;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private LockSystem _lock;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private TurfSystem _turf;
  private static readonly ProtoId<TagPrototype> InvalidForGlobalSpawnSpellTag = (ProtoId<TagPrototype>) "InvalidForGlobalSpawnSpell";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<MagicComponent, BeforeCastSpellEvent>(new EntityEventRefHandler<MagicComponent, BeforeCastSpellEvent>(this.OnBeforeCastSpell));
    this.SubscribeLocalEvent<InstantSpawnSpellEvent>(new EntityEventHandler<InstantSpawnSpellEvent>(this.OnInstantSpawn));
    this.SubscribeLocalEvent<TeleportSpellEvent>(new EntityEventHandler<TeleportSpellEvent>(this.OnTeleportSpell));
    this.SubscribeLocalEvent<WorldSpawnSpellEvent>(new EntityEventHandler<WorldSpawnSpellEvent>(this.OnWorldSpawn));
    this.SubscribeLocalEvent<ProjectileSpellEvent>(new EntityEventHandler<ProjectileSpellEvent>(this.OnProjectileSpell));
    this.SubscribeLocalEvent<ChangeComponentsSpellEvent>(new EntityEventHandler<ChangeComponentsSpellEvent>(this.OnChangeComponentsSpell));
    this.SubscribeLocalEvent<SmiteSpellEvent>(new EntityEventHandler<SmiteSpellEvent>(this.OnSmiteSpell));
    this.SubscribeLocalEvent<KnockSpellEvent>(new EntityEventHandler<KnockSpellEvent>(this.OnKnockSpell));
    this.SubscribeLocalEvent<ChargeSpellEvent>(new EntityEventHandler<ChargeSpellEvent>(this.OnChargeSpell));
    this.SubscribeLocalEvent<RandomGlobalSpawnSpellEvent>(new EntityEventHandler<RandomGlobalSpawnSpellEvent>(this.OnRandomGlobalSpawnSpell));
    this.SubscribeLocalEvent<MindSwapSpellEvent>(new EntityEventHandler<MindSwapSpellEvent>(this.OnMindSwapSpell));
    this.SubscribeLocalEvent<VoidApplauseSpellEvent>(new EntityEventHandler<VoidApplauseSpellEvent>(this.OnVoidApplause));
  }

  private void OnBeforeCastSpell(Entity<MagicComponent> ent, ref BeforeCastSpellEvent args)
  {
    MagicComponent comp = ent.Comp;
    bool flag = true;
    if (comp.RequiresClothes)
    {
      InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) args.Performer, SlotFlags.HEAD | SlotFlags.OUTERCLOTHING);
      ContainerSlot container;
      while (slotEnumerator.MoveNext(out container))
      {
        EntityUid? containedEntity = container.ContainedEntity;
        flag = containedEntity.HasValue && this.HasComp<WizardClothesComponent>(containedEntity.GetValueOrDefault());
        if (!flag)
          break;
      }
    }
    if (comp.RequiresSpeech && this.HasComp<MutedComponent>(args.Performer))
      flag = false;
    if (flag)
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("spell-requirements-failed"), args.Performer, new EntityUid?(args.Performer));
  }

  private bool PassesSpellPrerequisites(EntityUid spell, EntityUid performer)
  {
    BeforeCastSpellEvent args = new BeforeCastSpellEvent(performer);
    this.RaiseLocalEvent<BeforeCastSpellEvent>(spell, ref args);
    return !args.Cancelled;
  }

  private void OnInstantSpawn(InstantSpawnSpellEvent args)
  {
    if (args.Handled || !this.PassesSpellPrerequisites((EntityUid) args.Action, args.Performer))
      return;
    foreach (EntityCoordinates instantSpawnPosition in this.GetInstantSpawnPositions(this.Transform(args.Performer), args.PosData))
    {
      string prototype = (string) args.Prototype;
      EntityCoordinates position = instantSpawnPosition;
      EntityUid performer = args.Performer;
      bool collideWithCaster = args.PreventCollideWithCaster;
      float? lifetime = new float?();
      int num = collideWithCaster ? 1 : 0;
      this.SpawnSpellHelper(prototype, position, performer, lifetime, num != 0);
    }
    args.Handled = true;
  }

  private List<EntityCoordinates> GetInstantSpawnPositions(
    TransformComponent casterXform,
    MagicInstantSpawnData data)
  {
    switch (data)
    {
      case TargetCasterPos _:
        return new List<EntityCoordinates>(1)
        {
          casterXform.Coordinates
        };
      case TargetInFrontSingle _:
        EntityCoordinates coordinates1 = casterXform.Coordinates;
        ref EntityCoordinates local1 = ref coordinates1;
        Angle localRotation1 = casterXform.LocalRotation;
        Vector2 position1 = Vector2Helpers.Normalized(((Angle) ref localRotation1).ToWorldVec());
        EntityCoordinates coordinates2 = local1.Offset(position1);
        MapGridComponent comp1;
        if (!this.TryComp<MapGridComponent>(casterXform.GridUid, out comp1))
          return new List<EntityCoordinates>();
        TileRef? tile1;
        if (!this._turf.TryGetTileRef(coordinates2, out tile1))
          return new List<EntityCoordinates>();
        Vector2i gridIndices1 = tile1.Value.GridIndices;
        return new List<EntityCoordinates>(1)
        {
          this._mapSystem.GridTileToLocal(casterXform.GridUid.Value, comp1, gridIndices1)
        };
      case TargetInFront _:
        EntityCoordinates coordinates3 = casterXform.Coordinates;
        ref EntityCoordinates local2 = ref coordinates3;
        Angle localRotation2 = casterXform.LocalRotation;
        Vector2 position2 = Vector2Helpers.Normalized(((Angle) ref localRotation2).ToWorldVec());
        EntityCoordinates coordinates4 = local2.Offset(position2);
        MapGridComponent comp2;
        if (!this.TryComp<MapGridComponent>(casterXform.GridUid, out comp2))
          return new List<EntityCoordinates>();
        TileRef? tile2;
        if (!this._turf.TryGetTileRef(coordinates4, out tile2))
          return new List<EntityCoordinates>();
        Vector2i gridIndices2 = tile2.Value.GridIndices;
        SharedMapSystem mapSystem1 = this._mapSystem;
        EntityUid? gridUid = casterXform.GridUid;
        EntityUid uid1 = gridUid.Value;
        MapGridComponent grid1 = comp2;
        Vector2i gridTile1 = gridIndices2;
        EntityCoordinates local3 = mapSystem1.GridTileToLocal(uid1, grid1, gridTile1);
        Angle localRotation3 = casterXform.LocalRotation;
        switch ((int) ((Angle) ref localRotation3).GetCardinalDir())
        {
          case 0:
          case 4:
            SharedMapSystem mapSystem2 = this._mapSystem;
            gridUid = casterXform.GridUid;
            EntityUid uid2 = gridUid.Value;
            MapGridComponent grid2 = comp2;
            Vector2i gridTile2 = Vector2i.op_Addition(gridIndices2, Vector2i.op_Implicit((1, 0)));
            EntityCoordinates local4 = mapSystem2.GridTileToLocal(uid2, grid2, gridTile2);
            SharedMapSystem mapSystem3 = this._mapSystem;
            gridUid = casterXform.GridUid;
            EntityUid uid3 = gridUid.Value;
            MapGridComponent grid3 = comp2;
            Vector2i gridTile3 = Vector2i.op_Addition(gridIndices2, Vector2i.op_Implicit((-1, 0)));
            EntityCoordinates local5 = mapSystem3.GridTileToLocal(uid3, grid3, gridTile3);
            return new List<EntityCoordinates>(3)
            {
              local3,
              local4,
              local5
            };
          case 2:
          case 6:
            SharedMapSystem mapSystem4 = this._mapSystem;
            gridUid = casterXform.GridUid;
            EntityUid uid4 = gridUid.Value;
            MapGridComponent grid4 = comp2;
            Vector2i gridTile4 = Vector2i.op_Addition(gridIndices2, Vector2i.op_Implicit((0, 1)));
            EntityCoordinates local6 = mapSystem4.GridTileToLocal(uid4, grid4, gridTile4);
            SharedMapSystem mapSystem5 = this._mapSystem;
            gridUid = casterXform.GridUid;
            EntityUid uid5 = gridUid.Value;
            MapGridComponent grid5 = comp2;
            Vector2i gridTile5 = Vector2i.op_Addition(gridIndices2, Vector2i.op_Implicit((0, -1)));
            EntityCoordinates local7 = mapSystem5.GridTileToLocal(uid5, grid5, gridTile5);
            return new List<EntityCoordinates>(3)
            {
              local3,
              local6,
              local7
            };
          default:
            return new List<EntityCoordinates>();
        }
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private void OnWorldSpawn(WorldSpawnSpellEvent args)
  {
    if (args.Handled || !this.PassesSpellPrerequisites((EntityUid) args.Action, args.Performer))
      return;
    EntityCoordinates target = args.Target;
    this.WorldSpawnSpellHelper(args.Prototypes, target, args.Performer, args.Lifetime, args.Offset);
    args.Handled = true;
  }

  private void WorldSpawnSpellHelper(
    List<EntitySpawnEntry> entityEntries,
    EntityCoordinates entityCoords,
    EntityUid performer,
    float? lifetime,
    Vector2 offsetVector2)
  {
    List<string> spawns = EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) entityEntries, this._random);
    EntityCoordinates position = entityCoords;
    foreach (string proto in spawns)
    {
      this.SpawnSpellHelper(proto, position, performer, lifetime);
      position = position.Offset(offsetVector2);
    }
  }

  private void OnProjectileSpell(ProjectileSpellEvent ev)
  {
    if (ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer) || !this._net.IsServer)
      return;
    ev.Handled = true;
    EntityCoordinates coordinates = this.Transform(ev.Performer).Coordinates;
    EntityCoordinates target = ev.Target;
    Vector2 mapLinearVelocity = this._physics.GetMapLinearVelocity(ev.Performer);
    MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(coordinates);
    this._gunSystem.ShootProjectile(this.Spawn((string) ev.Prototype, mapCoordinates, rotation: new Angle()), this._transform.ToMapCoordinates(target).Position - mapCoordinates.Position, mapLinearVelocity, new EntityUid?(ev.Performer), new EntityUid?(ev.Performer));
  }

  private void OnChangeComponentsSpell(ChangeComponentsSpellEvent ev)
  {
    if (ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer))
      return;
    ev.Handled = true;
    this.RemoveComponents(ev.Target, ev.ToRemove);
    this.AddComponents(ev.Target, ev.ToAdd);
  }

  private void OnTeleportSpell(TeleportSpellEvent args)
  {
    if (args.Handled || !this.PassesSpellPrerequisites((EntityUid) args.Action, args.Performer))
      return;
    TransformComponent xform = this.Transform(args.Performer);
    if (xform.MapID != this._transform.GetMapId(args.Target) || !this._interaction.InRangeUnobstructed(args.Performer, args.Target, 1000f, CollisionGroup.Opaque, popup: true))
      return;
    this._transform.SetCoordinates(args.Performer, args.Target);
    this._transform.AttachToGridOrMap(args.Performer, xform);
    args.Handled = true;
  }

  public virtual void OnVoidApplause(VoidApplauseSpellEvent ev)
  {
    if (ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer))
      return;
    ev.Handled = true;
    this._transform.SwapPositions((Entity<TransformComponent>) ev.Performer, (Entity<TransformComponent>) ev.Target);
  }

  private void SpawnSpellHelper(
    string? proto,
    EntityCoordinates position,
    EntityUid performer,
    float? lifetime = null,
    bool preventCollide = false)
  {
    if (!this._net.IsServer)
      return;
    EntityUid uid = this.Spawn(proto, position.SnapToGrid((IEntityManager) this.EntityManager, this._mapManager));
    if (lifetime.HasValue)
      this.EnsureComp<TimedDespawnComponent>(uid).Lifetime = lifetime.Value;
    if (!preventCollide)
      return;
    this.EnsureComp<PreventCollideComponent>(uid).Uid = performer;
  }

  private void AddComponents(EntityUid target, ComponentRegistry comps)
  {
    foreach ((string str, EntityPrototype.ComponentRegistryEntry componentRegistryEntry) in (Dictionary<string, EntityPrototype.ComponentRegistryEntry>) comps)
    {
      if (!this.HasComp(target, componentRegistryEntry.Component.GetType()))
      {
        object component = (object) (Component) this.Factory.GetComponent(str);
        this._seriMan.CopyTo((object) componentRegistryEntry.Component, ref component);
        this.AddComp<Component>(target, (Component) component);
      }
    }
  }

  private void RemoveComponents(EntityUid target, HashSet<string> comps)
  {
    foreach (string comp in comps)
    {
      ComponentRegistration registration;
      if (this.Factory.TryGetRegistration(comp, out registration))
        this.RemComp(target, registration.Type);
    }
  }

  private void OnSmiteSpell(SmiteSpellEvent ev)
  {
    if (ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer))
      return;
    ev.Handled = true;
    Vector2 impulse = (this._transform.GetMapCoordinates(ev.Target, this.Transform(ev.Target)).Position - this._transform.GetMapCoordinates(ev.Performer, this.Transform(ev.Performer)).Position) * 10000f;
    this._physics.ApplyLinearImpulse(ev.Target, impulse);
    BodyComponent comp;
    if (!this.TryComp<BodyComponent>(ev.Target, out comp))
      return;
    this._body.GibBody(ev.Target, true, comp, splatCone: new Angle());
  }

  private void OnKnockSpell(KnockSpellEvent args)
  {
    if (args.Handled || !this.PassesSpellPrerequisites((EntityUid) args.Action, args.Performer))
      return;
    args.Handled = true;
    TransformComponent xform = this.Transform(args.Performer);
    foreach (EntityUid entityUid in this._lookup.GetEntitiesInRange(this._transform.GetMapCoordinates(args.Performer, xform), args.Range, LookupFlags.Dynamic | LookupFlags.Static))
    {
      if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) args.Performer, (Entity<TransformComponent>) entityUid, 0.0f, CollisionGroup.Opaque))
      {
        DoorBoltComponent comp1;
        if (this.TryComp<DoorBoltComponent>(entityUid, out comp1) && comp1.BoltsDown)
          this._door.SetBoltsDown((Entity<DoorBoltComponent>) (entityUid, comp1), false, predicted: true);
        DoorComponent comp2;
        if (this.TryComp<DoorComponent>(entityUid, out comp2) && comp2.State != DoorState.Open)
          this._door.StartOpening(entityUid);
        LockComponent comp3;
        if (this.TryComp<LockComponent>(entityUid, out comp3) && comp3.Locked)
          this._lock.Unlock(entityUid, new EntityUid?(args.Performer), comp3);
      }
    }
  }

  private void OnChargeSpell(ChargeSpellEvent ev)
  {
    HandsComponent comp1;
    if (ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer) || !this.TryComp<HandsComponent>(ev.Performer, out comp1))
      return;
    EntityUid? uid = new EntityUid?();
    foreach (EntityUid entityUid in this._hands.EnumerateHeld((Entity<HandsComponent>) (ev.Performer, comp1)))
    {
      if (this._tag.HasTag(entityUid, (ProtoId<TagPrototype>) ev.WandTag))
        uid = new EntityUid?(entityUid);
    }
    ev.Handled = true;
    BasicEntityAmmoProviderComponent comp2;
    if (!uid.HasValue || !this.TryComp<BasicEntityAmmoProviderComponent>(uid, out comp2) || !comp2.Count.HasValue)
      return;
    this._gunSystem.UpdateBasicEntityAmmoCount(uid.Value, comp2.Count.Value + ev.Charge, comp2);
  }

  protected virtual void OnRandomGlobalSpawnSpell(RandomGlobalSpawnSpellEvent ev)
  {
    if (!this._net.IsServer || ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer))
      return;
    List<EntitySpawnEntry> spawns = ev.Spawns;
    if (spawns == null)
      return;
    ev.Handled = true;
    foreach (Entity<MindComponent> aliveHuman in this._mind.GetAliveHumans())
    {
      EntityUid? ownedEntity = aliveHuman.Comp.OwnedEntity;
      if (ownedEntity.HasValue)
      {
        ownedEntity = aliveHuman.Comp.OwnedEntity;
        EntityUid entityUid = ownedEntity.Value;
        if (!this._tag.HasTag(entityUid, SharedMagicSystem.InvalidForGlobalSpawnSpellTag))
        {
          MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(entityUid);
          foreach (string spawn in EntitySpawnCollection.GetSpawns((IEnumerable<EntitySpawnEntry>) spawns, this._random))
          {
            EntityUid entity = this.Spawn(spawn, mapCoordinates, rotation: new Angle());
            this._hands.PickupOrDrop(new EntityUid?(entityUid), entity);
          }
        }
      }
    }
    this._audio.PlayGlobal(ev.Sound, ev.Performer);
  }

  private void OnMindSwapSpell(MindSwapSpellEvent ev)
  {
    if (ev.Handled || !this.PassesSpellPrerequisites((EntityUid) ev.Action, ev.Performer))
      return;
    ev.Handled = true;
    EntityUid mindId1;
    if (!this._mind.TryGetMind(ev.Performer, out mindId1, out MindComponent _))
      return;
    EntityUid mindId2;
    int num = this._mind.TryGetMind(ev.Target, out mindId2, out MindComponent _) ? 1 : 0;
    this._mind.TransferTo(mindId1, new EntityUid?(ev.Target));
    if (num != 0)
      this._mind.TransferTo(mindId2, new EntityUid?(ev.Performer));
    this._stun.TryParalyze(ev.Target, ev.TargetStunDuration, true);
    this._stun.TryParalyze(ev.Performer, ev.PerformerStunDuration, true);
  }
}
