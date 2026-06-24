// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Construction.PubgBlueprintSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._CIV14merka.Fob;
using Content.Shared._CIV14merka.Pvo;
using Content.Shared._CIV14merka.Teams;
using Content.Shared._PUBG.Structures;
using Content.Shared._RMC14.Construction;
using Content.Shared._RMC14.Construction.Prototypes;
using Content.Shared._RMC14.Entrenching;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Input;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Stacks;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared._PUBG.Construction;

public sealed class PubgBlueprintSystem : EntitySystem
{
  [Dependency]
  private readonly INetManager _net;
  [Dependency]
  private readonly IPrototypeManager _prototype;
  [Dependency]
  private readonly IComponentFactory _compFactory;
  [Dependency]
  private readonly SharedTransformSystem _transform;
  [Dependency]
  private readonly SharedStackSystem _stack;
  [Dependency]
  private readonly SharedPopupSystem _popup;
  [Dependency]
  private readonly SharedDoAfterSystem _doAfter;
  [Dependency]
  private readonly RMCConstructionSystem _construction;
  [Dependency]
  private readonly SharedPhysicsSystem _physics;
  [Dependency]
  private readonly SharedHandsSystem _hands;
  [Dependency]
  private readonly SharedAudioSystem _audio;
  private static readonly SoundSpecifier BuildSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Weapons/rubberhammer.ogg");
  private const int MaxBlueprintsPerPlayer = 20;
  private string? _pvoCompName;
  private string? _bunkerCompName;
  private const float BunkerHalfExtent = 2.1f;
  private const float BuildClearanceTiles = 4f;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<PubgPlaceBlueprintEvent>(new EntityEventRefHandler<PubgPlaceBlueprintEvent>(this.OnPlaceBlueprint));
    this.SubscribeLocalEvent<PubgBlueprintComponent, InteractUsingEvent>(new EntityEventRefHandler<PubgBlueprintComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<PubgBlueprintComponent, PubgBlueprintBuildDoAfterEvent>(new EntityEventRefHandler<PubgBlueprintComponent, PubgBlueprintBuildDoAfterEvent>(this.OnBuildDoAfter));
    this.SubscribeLocalEvent<PubgBlueprintComponent, DamageChangedEvent>(new EntityEventRefHandler<PubgBlueprintComponent, DamageChangedEvent>(this.OnDamageChanged));
    this.SubscribeLocalEvent<PubgBlueprintComponent, ExaminedEvent>(new EntityEventRefHandler<PubgBlueprintComponent, ExaminedEvent>(this.OnExamine));
    CommandBinds.Builder.Bind(ContentKeyFunctions.MouseMiddle, (InputCmdHandler) new PointerInputCmdHandler(new PointerInputCmdDelegate(this.OnMiddleClick))).Register<PubgBlueprintSystem>();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<PubgBlueprintSystem>();
  }

  private void OnPlaceBlueprint(ref PubgPlaceBlueprintEvent args)
  {
    RMCConstructionPrototype prototype;
    if (this._net.IsClient || !this._prototype.TryIndex<RMCConstructionPrototype>(args.Recipe, out prototype))
      return;
    if (prototype.RequiresFobZone && !this.IsInFriendlyFobZone(args.User, args.Coordinates))
    {
      this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-need-fob-zone"), args.User, args.User, PopupType.SmallCaution);
    }
    else
    {
      MapCoordinates mapCoordinates = this._transform.ToMapCoordinates(args.Coordinates);
      if (this.BuildsPvo(prototype) && this.TooCloseToBunker(mapCoordinates))
        this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-pvo-no-bunker"), args.User, args.User, PopupType.SmallCaution);
      else if (this.BuildsBunker(prototype) && this.TooCloseToPvo(mapCoordinates))
        this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-bunker-no-pvo"), args.User, args.User, PopupType.SmallCaution);
      else if (this.BuildsBunker(prototype) && this.TooCloseToBunker(mapCoordinates, 2.1f))
      {
        this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-bunker-no-bunker"), args.User, args.User, PopupType.SmallCaution);
      }
      else
      {
        int num = 0;
        Robust.Shared.GameObjects.EntityQueryEnumerator<PubgBlueprintComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PubgBlueprintComponent>();
        EntityUid uid1;
        PubgBlueprintComponent comp1;
        while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
        {
          EntityUid? placer = comp1.Placer;
          uid1 = args.User;
          if ((placer.HasValue ? (placer.GetValueOrDefault() == uid1 ? 1 : 0) : 0) != 0)
            ++num;
        }
        if (num >= 20)
        {
          this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-limit", ("max", (object) 20)), args.User, args.User, PopupType.SmallCaution);
        }
        else
        {
          EntityCoordinates grid = args.Coordinates.SnapToGrid((IEntityManager) this.EntityManager);
          EntityUid uid2 = this.SpawnAtPosition((string) prototype.Prototype, grid);
          if (!prototype.NoRotate)
            this._transform.SetLocalRotation(uid2, DirectionExtensions.ToAngle(args.Direction));
          PubgBlueprintComponent blueprintComponent = this.AddComp<PubgBlueprintComponent>(uid2);
          blueprintComponent.Recipe = args.Recipe;
          double a = Math.Max(prototype.DoAfterTime.TotalSeconds, prototype.DoAfterTimeMin.TotalSeconds);
          blueprintComponent.PointsRequired = Math.Max(1, (int) Math.Ceiling(a));
          blueprintComponent.FillMaterial = prototype.FillMaterial;
          blueprintComponent.MaterialCost = prototype.MaterialCost.GetValueOrDefault();
          blueprintComponent.Direction = args.Direction;
          blueprintComponent.NoRotate = prototype.NoRotate;
          blueprintComponent.Placer = new EntityUid?(args.User);
          this.Dirty(uid2, (IComponent) blueprintComponent);
          this.EnsureComp<RMCConstructionGhostComponent>(uid2);
          PhysicsComponent comp;
          if (!this.TryComp<PhysicsComponent>(uid2, out comp))
            return;
          this._physics.SetCanCollide(uid2, false, body: comp);
        }
      }
    }
  }

  private void OnInteractUsing(Entity<PubgBlueprintComponent> ent, ref InteractUsingEvent args)
  {
    if (args.Handled)
      return;
    EntityUid used = args.Used;
    bool flag1 = this.HasComp<PubgBlueprintHammerComponent>(used);
    bool flag2 = !ent.Comp.Filled && this.HasComp<PubgBlueprintFillerComponent>(used);
    if (!flag1 && !flag2)
      return;
    args.Handled = true;
    if (this._net.IsClient)
      return;
    if (flag1)
      this.TryStartBuild(ent, args.User);
    else
      this.TryFill(ent, args.User, used);
  }

  private void TryFill(Entity<PubgBlueprintComponent> ent, EntityUid user, EntityUid used)
  {
    StackComponent comp;
    if (ent.Comp.Filled || !this.TryComp<StackComponent>(used, out comp))
      return;
    ProtoId<StackPrototype>? fillMaterial = ent.Comp.FillMaterial;
    if (fillMaterial.HasValue)
    {
      ProtoId<StackPrototype> valueOrDefault = fillMaterial.GetValueOrDefault();
      if (comp.StackTypeId != valueOrDefault.Id)
      {
        this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-wrong-material"), (EntityUid) ent, user, PopupType.SmallCaution);
        return;
      }
    }
    int amount = Math.Max(1, ent.Comp.MaterialCost);
    if (this._stack.GetCount(used, comp) < amount)
    {
      this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-more-material", ("count", (object) amount)), (EntityUid) ent, user, PopupType.SmallCaution);
    }
    else
    {
      if (!this._stack.Use(used, amount, comp))
        return;
      ent.Comp.Filled = true;
      this.Dirty<PubgBlueprintComponent>(ent);
      this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-filled"), (EntityUid) ent, user, PopupType.Medium);
    }
  }

  private void TryStartBuild(Entity<PubgBlueprintComponent> ent, EntityUid user)
  {
    if (!ent.Comp.Filled)
      this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-needs-fill"), (EntityUid) ent, user, PopupType.SmallCaution);
    else
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, user, TimeSpan.FromSeconds(1L), (DoAfterEvent) new PubgBlueprintBuildDoAfterEvent(), new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent))
      {
        NeedHand = true,
        BreakOnMove = true,
        MovementThreshold = 0.5f,
        BreakOnDamage = false,
        DuplicateCondition = DuplicateConditions.SameTarget | DuplicateConditions.SameEvent
      });
  }

  private void OnBuildDoAfter(
    Entity<PubgBlueprintComponent> ent,
    ref PubgBlueprintBuildDoAfterEvent args)
  {
    if (args.Cancelled || this._net.IsClient || this.Deleted((EntityUid) ent) || ent.Comp.Completed)
      return;
    ++ent.Comp.Points;
    this.Dirty<PubgBlueprintComponent>(ent);
    this._audio.PlayPvs(PubgBlueprintSystem.BuildSound, (EntityUid) ent, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.15f))));
    if (ent.Comp.Points >= ent.Comp.PointsRequired)
      this.FinishBlueprint(ent, args.User);
    else
      args.Repeat = true;
  }

  private void FinishBlueprint(Entity<PubgBlueprintComponent> ent, EntityUid user)
  {
    if (this._net.IsClient || ent.Comp.Completed)
      return;
    ent.Comp.Completed = true;
    EntityCoordinates coordinates = this.Transform((EntityUid) ent).Coordinates;
    Direction direction = ent.Comp.Direction;
    this.QueueDel(new EntityUid?((EntityUid) ent));
    RMCConstructionPrototype prototype;
    if (!this._prototype.TryIndex<RMCConstructionPrototype>(ent.Comp.Recipe, out prototype))
      return;
    EntityUid entityUid = this.SpawnAtPosition((string) prototype.Prototype, coordinates);
    if (!prototype.NoRotate)
      this._transform.SetLocalRotation(entityUid, DirectionExtensions.ToAngle(direction));
    if (!this.HasComp<BarricadeComponent>(entityUid))
      this._construction.MakeConstructionImmuneToCollision(entityUid, user);
    RMCConstructionBuiltEvent message = new RMCConstructionBuiltEvent(entityUid, user);
    this.RaiseLocalEvent<RMCConstructionBuiltEvent>(ref message);
  }

  private void OnDamageChanged(Entity<PubgBlueprintComponent> ent, ref DamageChangedEvent args)
  {
    if (this._net.IsClient || ent.Comp.Completed || (double) args.Damageable.TotalDamage.Float() < (double) ent.Comp.BreakDamage)
      return;
    ent.Comp.Completed = true;
    this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-destroyed"), (EntityUid) ent, PopupType.MediumCaution);
    this.QueueDel(new EntityUid?((EntityUid) ent));
  }

  private void OnExamine(Entity<PubgBlueprintComponent> ent, ref ExaminedEvent args)
  {
    if (ent.Comp.Filled)
      args.PushMarkup(this.Loc.GetString("pubg-blueprint-examine-filled"));
    else
      args.PushMarkup(this.Loc.GetString("pubg-blueprint-examine-need-material", ("count", (object) Math.Max(1, ent.Comp.MaterialCost))));
    args.PushMarkup(this.Loc.GetString("pubg-blueprint-examine-points", ("points", (object) ent.Comp.Points), ("required", (object) ent.Comp.PointsRequired)));
  }

  private bool OnMiddleClick(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
  {
    EntityUid? attachedEntity = (EntityUid?) session?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return false;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    PubgBlueprintComponent comp;
    if (!this.TryComp<PubgBlueprintComponent>(uid, out comp) || comp.Completed || !this.HoldingHammer(valueOrDefault))
      return false;
    if (this._net.IsServer)
    {
      comp.Completed = true;
      this._popup.PopupEntity(this.Loc.GetString("pubg-blueprint-removed"), uid, valueOrDefault);
      this.QueueDel(new EntityUid?(uid));
    }
    return true;
  }

  private bool IsInFriendlyFobZone(EntityUid user, EntityCoordinates coords)
  {
    MapCoordinates mapCoordinates1 = this._transform.ToMapCoordinates(coords);
    if (mapCoordinates1.MapId == MapId.Nullspace)
      return false;
    CivTeamMemberComponent comp;
    string sideId = this.TryComp<CivTeamMemberComponent>(user, out comp) ? comp.SideId : (string) null;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CivFobComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivFobComponent, TransformComponent>();
    EntityUid uid;
    CivFobComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out TransformComponent _))
    {
      if (!this.HasComp<PubgBlueprintComponent>(uid) && (string.IsNullOrWhiteSpace(sideId) || string.IsNullOrWhiteSpace(comp1.SideId) || string.Equals(comp1.SideId, sideId, StringComparison.OrdinalIgnoreCase)))
      {
        MapCoordinates mapCoordinates2 = this._transform.GetMapCoordinates(uid);
        if (!(mapCoordinates2.MapId != mapCoordinates1.MapId) && (double) (mapCoordinates2.Position - mapCoordinates1.Position).Length() <= (double) comp1.BuildZoneRange)
          return true;
      }
    }
    return false;
  }

  private bool BuildsPvo(RMCConstructionPrototype recipe)
  {
    if (this._pvoCompName == null)
      this._pvoCompName = this._compFactory.GetComponentName(typeof (CivPvoComponent));
    EntityPrototype prototype;
    return this._prototype.TryIndex(recipe.Prototype, out prototype) && prototype.Components.ContainsKey(this._pvoCompName);
  }

  private bool BuildsBunker(RMCConstructionPrototype recipe)
  {
    if (this._bunkerCompName == null)
      this._bunkerCompName = this._compFactory.GetComponentName(typeof (PubgBunkerComponent));
    EntityPrototype prototype;
    return this._prototype.TryIndex(recipe.Prototype, out prototype) && prototype.Components.ContainsKey(this._bunkerCompName);
  }

  private bool TooCloseToBunker(MapCoordinates placeMap, float placerHalfExtent = 0.0f)
  {
    if (placeMap.MapId == MapId.Nullspace)
      return false;
    float num = (float) (2.0999999046325684 + (double) placerHalfExtent + 4.0);
    Robust.Shared.GameObjects.EntityQueryEnumerator<PubgBunkerComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<PubgBunkerComponent, TransformComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out PubgBunkerComponent _, out TransformComponent _))
    {
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(uid);
      if (!(mapCoordinates.MapId != placeMap.MapId) && (double) (mapCoordinates.Position - placeMap.Position).Length() <= (double) num)
        return true;
    }
    return false;
  }

  private bool TooCloseToPvo(MapCoordinates placeMap)
  {
    if (placeMap.MapId == MapId.Nullspace)
      return false;
    float num = 6.1f;
    Robust.Shared.GameObjects.EntityQueryEnumerator<CivPvoComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CivPvoComponent, TransformComponent>();
    EntityUid uid;
    while (entityQueryEnumerator.MoveNext(out uid, out CivPvoComponent _, out TransformComponent _))
    {
      MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(uid);
      if (!(mapCoordinates.MapId != placeMap.MapId) && (double) (mapCoordinates.Position - placeMap.Position).Length() <= (double) num)
        return true;
    }
    return false;
  }

  private bool HoldingHammer(EntityUid user)
  {
    if (!this.HasComp<HandsComponent>(user))
      return false;
    foreach (EntityUid uid in this._hands.EnumerateHeld((Entity<HandsComponent>) user))
    {
      if (this.HasComp<PubgBlueprintHammerComponent>(uid))
        return true;
    }
    return false;
  }
}
