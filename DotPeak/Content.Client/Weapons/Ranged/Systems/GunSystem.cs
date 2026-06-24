// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Ranged.Systems.GunSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._CIV14merka.Particles;
using Content.Client._PUBG.Vision;
using Content.Client._RMC14.ItemPickup;
using Content.Client._RMC14.Movement;
using Content.Client._RMC14.Weapons.Ranged.Prediction;
using Content.Client.Animations;
using Content.Client.Gameplay;
using Content.Client.IoC;
using Content.Client.Items;
using Content.Client.Resources;
using Content.Client.Stack;
using Content.Client.Weapons.Ranged.Components;
using Content.Client.Weapons.Ranged.ItemStatus;
using Content.Shared._CIV14merka.Particles;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared.CombatMode;
using Content.Shared.Rounding;
using Content.Shared.Stacks;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Animations;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Ranged.Systems;

public sealed class GunSystem : SharedGunSystem
{
  [Dependency]
  private StackSystem _stack;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IStateManager _state;
  [Dependency]
  private AnimationPlayerSystem _animPlayer;
  [Dependency]
  private InputSystem _inputSystem;
  [Dependency]
  private SharedMapSystem _maps;
  [Dependency]
  private SharedTransformSystem _xform;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private PubgFocusViewSystem _pubgFocusView;
  [Dependency]
  private CivLocalParticleSystem _civParticles;
  [Dependency]
  private ItemPickupSystem _itemPickup;
  [Dependency]
  private GunPredictionSystem _gunPrediction;
  [Dependency]
  private RMCLagCompensationSystem _rmcLagCompensation;
  [Dependency]
  private CMGunSystem _cmGun;
  public static readonly EntProtoId HitscanProto = EntProtoId.op_Implicit("HitscanEffect");
  private bool _spreadOverlay;

  private void OnAmmoCounterCollect(
    EntityUid uid,
    AmmoCounterComponent component,
    ItemStatusCollectMessage args)
  {
    this.RefreshControl(uid, component);
    if (component.Control == null)
      return;
    args.Controls.Add(component.Control);
  }

  private void RefreshControl(EntityUid uid, AmmoCounterComponent? component = null)
  {
    if (!this.Resolve<AmmoCounterComponent>(uid, ref component, false))
      return;
    component.Control?.Orphan();
    component.Control = (Control) null;
    GunSystem.AmmoCounterControlEvent counterControlEvent1 = new GunSystem.AmmoCounterControlEvent();
    this.RaiseLocalEvent<GunSystem.AmmoCounterControlEvent>(uid, counterControlEvent1, false);
    GunSystem.AmmoCounterControlEvent counterControlEvent2 = counterControlEvent1;
    if (counterControlEvent2.Control == null)
      counterControlEvent2.Control = (Control) new GunSystem.DefaultStatusControl();
    component.Control = counterControlEvent1.Control;
    this.UpdateAmmoCount(uid, component);
  }

  private void UpdateAmmoCount(
    EntityUid uid,
    AmmoCounterComponent component,
    int artificialIncrease = 0)
  {
    if (component.Control == null)
      return;
    GunSystem.UpdateAmmoCounterEvent ammoCounterEvent = new GunSystem.UpdateAmmoCounterEvent()
    {
      Control = component.Control,
      ArtificialIncrease = artificialIncrease
    };
    this.RaiseLocalEvent<GunSystem.UpdateAmmoCounterEvent>(uid, ammoCounterEvent, false);
  }

  public override void UpdateAmmoCount(EntityUid uid, bool prediction = true, int artificialIncrease = 0)
  {
    AmmoCounterComponent component;
    if (prediction && !this.Timing.IsFirstTimePredicted || !this.TryComp<AmmoCounterComponent>(uid, ref component))
      return;
    this.UpdateAmmoCount(uid, component, artificialIncrease);
  }

  protected override void InitializeBallistic()
  {
    base.InitializeBallistic();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnBallisticAmmoCount)), (Type[]) null, (Type[]) null);
  }

  private void OnBallisticAmmoCount(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    GunSystem.UpdateAmmoCounterEvent args)
  {
    if (!(args.Control is GunSystem.DefaultStatusControl control))
      return;
    control.Update(this.GetBallisticShots(component) + args.ArtificialIncrease, component.Capacity);
  }

  protected override void Cycle(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    MapCoordinates coordinates)
  {
    if (!this.Timing.IsFirstTimePredicted)
      return;
    EntityUid? nullable = new EntityUid?();
    if (component.Entities.Count > 0)
    {
      List<EntityUid> entities = component.Entities;
      EntityUid uid1 = entities[entities.Count - 1];
      component.Entities.RemoveAt(component.Entities.Count - 1);
      this.Containers.Remove(Entity<TransformComponent, MetaDataComponent>.op_Implicit(uid1), (BaseContainer) component.Container, true, false, new EntityCoordinates?(), new Angle?());
      this.EnsureShootable(uid1);
    }
    else if (component.UnspawnedCount > 0)
    {
      --component.UnspawnedCount;
      ref EntityUid? local = ref nullable;
      EntProtoId? proto = component.Proto;
      EntityUid entityUid = this.Spawn(proto.HasValue ? EntProtoId.op_Implicit(proto.GetValueOrDefault()) : (string) null, coordinates, (ComponentRegistry) null, new Angle());
      local = new EntityUid?(entityUid);
      this._stack.SetCount(nullable.Value, 1, (StackComponent) null);
      this.EnsureShootable(nullable.Value);
    }
    if (nullable.HasValue && this.IsClientSide(nullable.Value, (MetaDataComponent) null))
      this.Del(new EntityUid?(nullable.Value));
    GunCycledEvent gunCycledEvent = new GunCycledEvent();
    this.RaiseLocalEvent<GunCycledEvent>(uid, ref gunCycledEvent, false);
  }

  protected override void InitializeBasicEntity()
  {
    base.InitializeBasicEntity();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<BasicEntityAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnBasicEntityAmmoCount)), (Type[]) null, (Type[]) null);
  }

  private void OnBasicEntityAmmoCount(
    EntityUid uid,
    BasicEntityAmmoProviderComponent component,
    GunSystem.UpdateAmmoCounterEvent args)
  {
    if (!(args.Control is GunSystem.DefaultStatusControl control) || !component.Count.HasValue || !component.Capacity.HasValue)
      return;
    control.Update(component.Count.Value, component.Capacity.Value);
  }

  protected override void InitializeBattery()
  {
    base.InitializeBattery();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>((object) this, __methodptr(OnControl)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnAmmoCountUpdate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>((object) this, __methodptr(OnControl)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnAmmoCountUpdate)), (Type[]) null, (Type[]) null);
  }

  private void OnAmmoCountUpdate(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    GunSystem.UpdateAmmoCounterEvent args)
  {
    if (!(args.Control is GunSystem.BoxesStatusControl control))
      return;
    control.Update(component.Shots, component.Capacity);
  }

  private void OnControl(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    GunSystem.AmmoCounterControlEvent args)
  {
    args.Control = (Control) new GunSystem.BoxesStatusControl();
  }

  protected override void InitializeChamberMagazine()
  {
    base.InitializeChamberMagazine();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>((object) this, __methodptr(OnChamberMagazineCounter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnChamberMagazineAmmoUpdate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<ChamberMagazineAmmoProviderComponent, AppearanceChangeEvent>((object) this, __methodptr(OnChamberMagazineAppearance)), (Type[]) null, (Type[]) null);
  }

  private void OnChamberMagazineAppearance(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    ref AppearanceChangeEvent args)
  {
    int num;
    bool flag;
    if (args.Sprite == null || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) GunVisualLayers.Base, ref num, false) || !this.Appearance.TryGetData<bool>(uid, (Enum) AmmoVisuals.BoltClosed, ref flag, (AppearanceComponent) null))
      return;
    if (flag)
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit("base"));
    else
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), num, RSI.StateId.op_Implicit("bolt-open"));
  }

  protected override void OnMagazineSlotChange(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    ContainerModifiedMessage args)
  {
    base.OnMagazineSlotChange(uid, component, args);
    if ("gun_chamber" != args.Container.ID || !(args is EntRemovedFromContainerMessage containerMessage) || !this.IsClientSide(((ContainerModifiedMessage) containerMessage).Entity, (MetaDataComponent) null))
      return;
    this.QueueDel(new EntityUid?(args.Entity));
  }

  private void OnChamberMagazineCounter(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    GunSystem.AmmoCounterControlEvent args)
  {
    args.Control = (Control) new GunSystem.ChamberMagazineStatusControl();
  }

  private void OnChamberMagazineAmmoUpdate(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    GunSystem.UpdateAmmoCounterEvent args)
  {
    if (!(args.Control is GunSystem.ChamberMagazineStatusControl control))
      return;
    EntityUid? chamberEntity = this.GetChamberEntity(uid);
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    GetAmmoCountEvent getAmmoCountEvent = new GetAmmoCountEvent();
    if (magazineEntity.HasValue)
      this.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref getAmmoCountEvent, false);
    control.Update(chamberEntity.HasValue, magazineEntity.HasValue, getAmmoCountEvent.Count, getAmmoCountEvent.Capacity);
  }

  public bool SpreadOverlay
  {
    get => this._spreadOverlay;
    set
    {
      if (this._spreadOverlay == value)
        return;
      this._spreadOverlay = value;
      IOverlayManager ioverlayManager = IoCManager.Resolve<IOverlayManager>();
      if (this._spreadOverlay)
        ioverlayManager.AddOverlay((Overlay) new GunSpreadOverlay((IEntityManager) this.EntityManager, this._eyeManager, this.Timing, this._inputManager, this._player, this, this.TransformSystem));
      else
        ioverlayManager.RemoveOverlay<GunSpreadOverlay>();
    }
  }

  public override void Initialize()
  {
    base.Initialize();
    this.UpdatesOutsidePrediction = true;
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AmmoCounterComponent, ItemStatusCollectMessage>(new ComponentEventHandler<AmmoCounterComponent, ItemStatusCollectMessage>((object) this, __methodptr(OnAmmoCounterCollect)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AmmoCounterComponent, UpdateClientAmmoEvent>(new ComponentEventRefHandler<AmmoCounterComponent, UpdateClientAmmoEvent>((object) this, __methodptr(OnUpdateClientAmmo)), (Type[]) null, (Type[]) null);
    this.SubscribeAllEvent<MuzzleFlashEvent>(new EntityEventHandler<MuzzleFlashEvent>(this.OnMuzzleFlash), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<SharedGunSystem.HitscanEvent>(new EntityEventHandler<SharedGunSystem.HitscanEvent>(this.OnHitscan), (Type[]) null, (Type[]) null);
    this.InitializeMagazineVisuals();
    this.InitializeSpentAmmo();
  }

  private void OnUpdateClientAmmo(
    EntityUid uid,
    AmmoCounterComponent ammoComp,
    ref UpdateClientAmmoEvent args)
  {
    this.UpdateAmmoCount(uid, ammoComp, args.AritifialIncrease);
  }

  private void OnMuzzleFlash(MuzzleFlashEvent args)
  {
    EntityUid entity = this.GetEntity(args.Uid);
    this.CreateEffect(entity, args, new EntityUid?(entity), ((ISharedPlayerManager) this._player).LocalEntity, args.Offset, args.OriginOffset);
  }

  private void OnHitscan(SharedGunSystem.HitscanEvent ev)
  {
    foreach ((NetCoordinates coordinates, Angle angle, SpriteSpecifier Sprite, float Distance) sprite1 in ev.Sprites)
    {
      if (sprite1.Sprite is SpriteSpecifier.Rsi sprite2)
      {
        EntityCoordinates coordinates = this.GetCoordinates(sprite1.coordinates);
        TransformComponent transformComponent1;
        if (this.TryComp(coordinates.EntityId, ref transformComponent1))
        {
          EntityUid entityUid = this.Spawn(EntProtoId.op_Implicit(GunSystem.HitscanProto), coordinates);
          SpriteComponent spriteComponent = this.Comp<SpriteComponent>(entityUid);
          TransformComponent transformComponent2 = this.Transform(entityUid);
          Angle angle = Angle.op_Subtraction(Angle.op_Addition(sprite1.angle, this._xform.GetWorldRotation(transformComponent1)), this._xform.GetWorldRotation(transformComponent2));
          this._xform.SetLocalRotationNoLerp(entityUid, Angle.op_Addition(transformComponent2.LocalRotation, angle), transformComponent2);
          spriteComponent[(object) EffectLayers.Unshaded].AutoAnimated = false;
          this._sprite.LayerSetSprite(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) EffectLayers.Unshaded, (SpriteSpecifier) sprite2);
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), (Enum) EffectLayers.Unshaded, RSI.StateId.op_Implicit(sprite2.RsiState));
          this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), new Vector2(sprite1.Distance, 1f));
          spriteComponent[(object) EffectLayers.Unshaded].Visible = true;
          Animation animation = new Animation()
          {
            Length = TimeSpan.FromSeconds(0.47999998927116394),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) EffectLayers.Unshaded,
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(sprite2.RsiState), 0.0f)
                }
              }
            }
          };
          this._animPlayer.Play(entityUid, animation, "hitscan-effect");
        }
      }
    }
  }

  public virtual void Update(float frameTime)
  {
    if (!this.Timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    CombatModeComponent combatModeComponent;
    if (!localEntity.HasValue || !this.TryComp<CombatModeComponent>(localEntity, ref combatModeComponent) || !combatModeComponent.IsInCombatMode)
      return;
    EntityUid entityUid = localEntity.Value;
    EntityUid gunEntity;
    GunComponent gunComp;
    if (!this.TryGetGun(entityUid, out gunEntity, out gunComp))
      return;
    if (this._inputSystem.CmdStates.GetState(gunComp.UseKey ? EngineKeyFunctions.Use : EngineKeyFunctions.UseSecondary) != 1 && !gunComp.BurstActivated)
    {
      if (gunComp.ShotCounter == 0)
        return;
      this.RaisePredictiveEvent<RequestStopShootEvent>(new RequestStopShootEvent()
      {
        Gun = this.GetNetEntity(gunEntity, (MetaDataComponent) null)
      });
    }
    else
    {
      if (gunComp.NextFire > this.Timing.CurTime)
        return;
      MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
      MapCoordinates coordinates = this._pubgFocusView.AdjustMapCoordinates(gunEntity, map);
      if (MapId.op_Equality(coordinates.MapId, MapId.Nullspace))
      {
        if (gunComp.ShotCounter == 0)
          return;
        this.RaisePredictiveEvent<RequestStopShootEvent>(new RequestStopShootEvent()
        {
          Gun = this.GetNetEntity(gunEntity, (MetaDataComponent) null)
        });
      }
      else
      {
        EntityCoordinates entityCoordinates = !this.HasComp<VehicleTurretComponent>(gunEntity) ? this.TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(this.HasComp<GunUseGunOriginComponent>(gunEntity) ? gunEntity : entityUid), coordinates) : this.TransformSystem.ToCoordinates(coordinates);
        NetEntity? target = new NetEntity?();
        if (this._state.CurrentState is GameplayStateBase currentState)
          target = this.GetNetEntity(currentState.GetClickedEntity(coordinates), (MetaDataComponent) null);
        ICommonSession localSession = ((ISharedPlayerManager) this._player).LocalSession;
        if (localSession == null || this._itemPickup.RecentItemPickUp)
          return;
        this.Log.Debug($"Sending shoot request tick {this.Timing.CurTick} / {this.Timing.CurTime}");
        List<EntityUid> source1 = this._gunPrediction.ShootRequested(this.GetNetEntity(gunEntity, (MetaDataComponent) null), this.GetNetCoordinates(entityCoordinates, (MetaDataComponent) null), target, (List<int>) null, localSession);
        this.RaisePredictiveEvent<RequestShootEvent>(new RequestShootEvent()
        {
          Target = target,
          Coordinates = this.GetNetCoordinates(entityCoordinates, (MetaDataComponent) null),
          Gun = this.GetNetEntity(gunEntity, (MetaDataComponent) null),
          Shot = source1 != null ? source1.Select<EntityUid, int>((Func<EntityUid, int>) (e => e.Id)).ToList<int>() : (List<int>) null,
          LastRealTick = this._rmcLagCompensation.GetLastRealTick(new NetUserId?())
        });
        EntityUid offHand;
        if (!this._cmGun.TryGetAkimboOffHand(entityUid, Entity<GunComponent>.op_Implicit((gunEntity, gunComp)), out offHand))
          return;
        List<EntityUid> source2 = this._gunPrediction.ShootRequested(this.GetNetEntity(offHand, (MetaDataComponent) null), this.GetNetCoordinates(entityCoordinates, (MetaDataComponent) null), target, (List<int>) null, localSession);
        this.RaisePredictiveEvent<RequestShootEvent>(new RequestShootEvent()
        {
          Target = target,
          Coordinates = this.GetNetCoordinates(entityCoordinates, (MetaDataComponent) null),
          Gun = this.GetNetEntity(offHand, (MetaDataComponent) null),
          Shot = source2 != null ? source2.Select<EntityUid, int>((Func<EntityUid, int>) (e => e.Id)).ToList<int>() : (List<int>) null,
          LastRealTick = this._rmcLagCompensation.GetLastRealTick(new NetUserId?())
        });
      }
    }
  }

  protected override void Popup(string message, EntityUid? uid, EntityUid? user)
  {
    if (!uid.HasValue || !user.HasValue || !this.Timing.IsFirstTimePredicted)
      return;
    this.PopupSystem.PopupEntity(message, uid.Value, user.Value);
  }

  protected override void CreateEffect(
    EntityUid gunUid,
    MuzzleFlashEvent message,
    EntityUid? tracked = null,
    EntityUid? player = null,
    Vector2 offset = default (Vector2),
    Vector2 originOffset = default (Vector2))
  {
    if (!this.Timing.IsFirstTimePredicted)
      return;
    if (EntityUid.op_Equality(gunUid, EntityUid.Invalid))
    {
      this.Log.Debug($"Invalid Entity sent MuzzleFlashEvent (proto: {message.Prototype}, gun: {this.ToPrettyString(Entity<MetaDataComponent>.op_Implicit(gunUid))})");
    }
    else
    {
      TransformComponent transformComponent = this.Transform(gunUid);
      EntityUid? gridUid = transformComponent.GridUid;
      MapGridComponent mapGridComponent;
      EntityCoordinates entityCoordinates;
      if (this.TryComp<MapGridComponent>(gridUid, ref mapGridComponent))
      {
        // ISSUE: explicit constructor call
        ((EntityCoordinates) ref entityCoordinates).\u002Ector(gridUid.Value, this._maps.LocalToGrid(gridUid.Value, mapGridComponent, transformComponent.Coordinates));
      }
      else
      {
        EntityUid? mapUid = transformComponent.MapUid;
        if (!mapUid.HasValue)
          return;
        ref EntityCoordinates local = ref entityCoordinates;
        mapUid = transformComponent.MapUid;
        EntityUid entityUid = mapUid.Value;
        Vector2 worldPosition = this.TransformSystem.GetWorldPosition(transformComponent);
        // ISSUE: explicit constructor call
        ((EntityCoordinates) ref local).\u002Ector(entityUid, worldPosition);
      }
      EntityUid entityUid1 = this.Spawn(message.Prototype, entityCoordinates);
      this.TransformSystem.SetWorldRotationNoLerp(Entity<TransformComponent>.op_Implicit(entityUid1), message.Angle);
      if (tracked.HasValue)
      {
        TrackUserComponent trackUserComponent = this.EnsureComp<TrackUserComponent>(entityUid1);
        trackUserComponent.User = tracked;
        trackUserComponent.Offset = offset;
        trackUserComponent.OriginOffset = new Vector2?(originOffset);
      }
      float num = 0.4f;
      TimedDespawnComponent despawnComponent;
      if (this.TryComp<TimedDespawnComponent>(gunUid, ref despawnComponent))
        num = despawnComponent.Lifetime;
      Animation animation1 = new Animation();
      animation1.Length = TimeSpan.FromSeconds((double) num);
      List<AnimationTrack> animationTracks1 = animation1.AnimationTracks;
      AnimationTrackComponentProperty componentProperty1 = new AnimationTrackComponentProperty();
      componentProperty1.ComponentType = typeof (SpriteComponent);
      componentProperty1.Property = "Color";
      ((AnimationTrackProperty) componentProperty1).InterpolationMode = (AnimationInterpolationMode) 0;
      List<AnimationTrackProperty.KeyFrame> keyFrames1 = ((AnimationTrackProperty) componentProperty1).KeyFrames;
      Color white1 = Color.White;
      AnimationTrackProperty.KeyFrame keyFrame1 = new AnimationTrackProperty.KeyFrame((object) ((Color) ref white1).WithAlpha(1f), 0.0f, (Func<float, float>) null);
      keyFrames1.Add(keyFrame1);
      List<AnimationTrackProperty.KeyFrame> keyFrames2 = ((AnimationTrackProperty) componentProperty1).KeyFrames;
      Color white2 = Color.White;
      AnimationTrackProperty.KeyFrame keyFrame2 = new AnimationTrackProperty.KeyFrame((object) ((Color) ref white2).WithAlpha(0.0f), num, (Func<float, float>) null);
      keyFrames2.Add(keyFrame2);
      animationTracks1.Add((AnimationTrack) componentProperty1);
      Animation animation2 = animation1;
      this._animPlayer.Play(entityUid1, animation2, "muzzle-flash");
      PointLightComponent component;
      if (!this.TryComp<PointLightComponent>(entityUid1, ref component))
      {
        component = this.Factory.GetComponent<PointLightComponent>();
        ((Component) component).NetSyncEnabled = false;
        this.AddComp<PointLightComponent>(entityUid1, component, false);
      }
      this.Lights.SetEnabled(entityUid1, true, (SharedPointLightComponent) component, (MetaDataComponent) null);
      this.Lights.SetRadius(entityUid1, 2f, (SharedPointLightComponent) component, (MetaDataComponent) null);
      this.Lights.SetColor(entityUid1, Color.FromHex((ReadOnlySpan<char>) "#cc8e2b", new Color?()), (SharedPointLightComponent) component);
      this.Lights.SetEnergy(entityUid1, 5f, (SharedPointLightComponent) component);
      Animation animation3 = new Animation();
      animation3.Length = TimeSpan.FromSeconds((double) num);
      List<AnimationTrack> animationTracks2 = animation3.AnimationTracks;
      AnimationTrackComponentProperty componentProperty2 = new AnimationTrackComponentProperty();
      componentProperty2.ComponentType = typeof (PointLightComponent);
      componentProperty2.Property = "Energy";
      ((AnimationTrackProperty) componentProperty2).InterpolationMode = (AnimationInterpolationMode) 0;
      ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) 5f, 0.0f, (Func<float, float>) null));
      ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) 0.0f, num, (Func<float, float>) null));
      animationTracks2.Add((AnimationTrack) componentProperty2);
      List<AnimationTrack> animationTracks3 = animation3.AnimationTracks;
      AnimationTrackComponentProperty componentProperty3 = new AnimationTrackComponentProperty();
      componentProperty3.ComponentType = typeof (PointLightComponent);
      componentProperty3.Property = "AnimatedEnable";
      ((AnimationTrackProperty) componentProperty3).InterpolationMode = (AnimationInterpolationMode) 0;
      ((AnimationTrackProperty) componentProperty3).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) true, 0.0f, (Func<float, float>) null));
      ((AnimationTrackProperty) componentProperty3).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) false, num, (Func<float, float>) null));
      animationTracks3.Add((AnimationTrack) componentProperty3);
      Animation animation4 = animation3;
      AnimationPlayerComponent animationPlayerComponent = this.EnsureComp<AnimationPlayerComponent>(entityUid1);
      this._animPlayer.Stop(entityUid1, animationPlayerComponent, "muzzle-flash-light");
      this._animPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((entityUid1, animationPlayerComponent)), animation4, "muzzle-flash-light");
      CivMuzzleSmokeComponent muzzleSmokeComponent;
      if (!this.TryComp<CivMuzzleSmokeComponent>(gunUid, ref muzzleSmokeComponent))
        return;
      this._civParticles.EmitBurst(muzzleSmokeComponent.Preset, this.TransformSystem.GetMapCoordinates(gunUid, transformComponent), direction: new float?((float) ((Angle) ref message.Angle).Degrees));
    }
  }

  public override void ShootProjectile(
    EntityUid uid,
    Vector2 direction,
    Vector2 gunVelocity,
    EntityUid? gunUid,
    EntityUid? user = null,
    float speed = 20f)
  {
    this.EnsureComp<PredictedProjectileClientComponent>(uid);
    this.Physics.UpdateIsPredicted(new EntityUid?(uid), (PhysicsComponent) null);
    base.ShootProjectile(uid, direction, gunVelocity, gunUid, user, speed);
  }

  protected override void InitializeMagazine()
  {
    base.InitializeMagazine();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnMagazineAmmoUpdate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>((object) this, __methodptr(OnMagazineControl)), (Type[]) null, (Type[]) null);
  }

  private void OnMagazineAmmoUpdate(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    GunSystem.UpdateAmmoCounterEvent args)
  {
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    if (!magazineEntity.HasValue)
    {
      if (!(args.Control is GunSystem.DefaultStatusControl control))
        return;
      control.Update(0, 0);
    }
    else
      this.RaiseLocalEvent<GunSystem.UpdateAmmoCounterEvent>(magazineEntity.Value, args, false);
  }

  private void OnMagazineControl(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    GunSystem.AmmoCounterControlEvent args)
  {
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    if (!magazineEntity.HasValue)
      return;
    this.RaiseLocalEvent<GunSystem.AmmoCounterControlEvent>(magazineEntity.Value, args, false);
  }

  private void InitializeMagazineVisuals()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagazineVisualsComponent, ComponentInit>(new ComponentEventHandler<MagazineVisualsComponent, ComponentInit>((object) this, __methodptr(OnMagazineVisualsInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagazineVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<MagazineVisualsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnMagazineVisualsChange)), (Type[]) null, (Type[]) null);
  }

  private void OnMagazineVisualsInit(
    EntityUid uid,
    MagazineVisualsComponent component,
    ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    int num;
    if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) GunVisualLayers.Mag, ref num, false))
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) GunVisualLayers.Mag, RSI.StateId.op_Implicit($"{component.MagState}-{component.MagSteps - 1}"));
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) GunVisualLayers.Mag, false);
    }
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) GunVisualLayers.MagUnshaded, ref num, false))
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) GunVisualLayers.MagUnshaded, RSI.StateId.op_Implicit($"{component.MagState}-unshaded-{component.MagSteps - 1}"));
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) GunVisualLayers.MagUnshaded, false);
  }

  private void OnMagazineVisualsChange(
    EntityUid uid,
    MagazineVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    if (sprite == null)
      return;
    object obj;
    if (!args.AppearanceData.TryGetValue((Enum) AmmoVisuals.MagLoaded, out obj) || obj is bool flag && flag)
    {
      object magSteps1;
      if (!args.AppearanceData.TryGetValue((Enum) AmmoVisuals.AmmoMax, out magSteps1))
        magSteps1 = (object) component.MagSteps;
      object magSteps2;
      if (!args.AppearanceData.TryGetValue((Enum) AmmoVisuals.AmmoCount, out magSteps2))
        magSteps2 = (object) component.MagSteps;
      int num1 = ContentHelpers.RoundToLevels((double) (int) magSteps2, (double) (int) magSteps1, component.MagSteps);
      if (component.ZeroOnlyOnEmpty && num1 == 0 && (int) magSteps2 > 0)
        num1 = 1;
      if (num1 == 0 && !component.ZeroVisible)
      {
        int num2;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, ref num2, false))
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, false);
        if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, ref num2, false))
          return;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, false);
      }
      else
      {
        int num3;
        if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, ref num3, false))
        {
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, true);
          this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, RSI.StateId.op_Implicit($"{component.MagState}-{num1}"));
        }
        if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, ref num3, false))
          return;
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, true);
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, RSI.StateId.op_Implicit($"{component.MagState}-unshaded-{num1}"));
      }
    }
    else
    {
      int num;
      if (this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, ref num, false))
        this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.Mag, false);
      if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, ref num, false))
        return;
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) GunVisualLayers.MagUnshaded, false);
    }
  }

  protected override void InitializeRevolver()
  {
    base.InitializeRevolver();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, GunSystem.AmmoCounterControlEvent>((object) this, __methodptr(OnRevolverCounter)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, GunSystem.UpdateAmmoCounterEvent>((object) this, __methodptr(OnRevolverAmmoUpdate)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<RevolverAmmoProviderComponent, EntRemovedFromContainerMessage>((object) this, __methodptr(OnRevolverEntRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnRevolverEntRemove(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (((ContainerModifiedMessage) args).Container.ID != "revolver-ammo" || !this.IsClientSide(((ContainerModifiedMessage) args).Entity, (MetaDataComponent) null))
      return;
    this.QueueDel(new EntityUid?(((ContainerModifiedMessage) args).Entity));
  }

  private void OnRevolverAmmoUpdate(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    GunSystem.UpdateAmmoCounterEvent args)
  {
    if (!(args.Control is GunSystem.RevolverStatusControl control))
      return;
    control.Update(component.CurrentIndex, component.Chambers);
  }

  private void OnRevolverCounter(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    GunSystem.AmmoCounterControlEvent args)
  {
    args.Control = (Control) new GunSystem.RevolverStatusControl();
  }

  private void InitializeSpentAmmo()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SpentAmmoVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<SpentAmmoVisualsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnSpentAmmoAppearance)), (Type[]) null, (Type[]) null);
  }

  private void OnSpentAmmoAppearance(
    EntityUid uid,
    SpentAmmoVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    SpriteComponent sprite = args.Sprite;
    object obj;
    if (sprite == null || !args.AppearanceData.TryGetValue((Enum) AmmoVisuals.Spent, out obj))
      return;
    string str = !(bool) obj ? component.State : (component.Suffix ? component.State + "-spent" : "spent");
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) AmmoVisualLayers.Base, RSI.StateId.op_Implicit(str));
    this._sprite.RemoveLayer(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) AmmoVisualLayers.Tip, false);
  }

  public sealed class AmmoCounterControlEvent : EntityEventArgs
  {
    public Control? Control;
  }

  public sealed class UpdateAmmoCounterEvent : HandledEntityEventArgs
  {
    public Control Control;
    public int ArtificialIncrease;
  }

  private sealed class DefaultStatusControl : Control
  {
    private readonly BulletRender _bulletRender;

    public DefaultStatusControl()
    {
      this.MinHeight = 15f;
      this.HorizontalExpand = true;
      this.VerticalAlignment = (Control.VAlignment) 2;
      BulletRender bulletRender1 = new BulletRender();
      bulletRender1.HorizontalAlignment = (Control.HAlignment) 3;
      bulletRender1.VerticalAlignment = (Control.VAlignment) 3;
      BulletRender bulletRender2 = bulletRender1;
      this._bulletRender = bulletRender1;
      this.AddChild((Control) bulletRender2);
    }

    public void Update(int count, int capacity)
    {
      this._bulletRender.Count = count;
      this._bulletRender.Capacity = capacity;
      this._bulletRender.Type = capacity > 50 ? BulletRender.BulletType.Tiny : BulletRender.BulletType.Normal;
    }
  }

  public sealed class BoxesStatusControl : Control
  {
    private readonly BatteryBulletRenderer _bullets;
    private readonly Label _ammoCount;

    public BoxesStatusControl()
    {
      this.MinHeight = 15f;
      this.HorizontalExpand = true;
      this.VerticalAlignment = (Control.VAlignment) 2;
      BoxContainer boxContainer = new BoxContainer();
      boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
      Control.OrderedChildCollection children1 = ((Control) boxContainer).Children;
      BatteryBulletRenderer batteryBulletRenderer1 = new BatteryBulletRenderer();
      batteryBulletRenderer1.Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
      batteryBulletRenderer1.HorizontalExpand = true;
      BatteryBulletRenderer batteryBulletRenderer2 = batteryBulletRenderer1;
      this._bullets = batteryBulletRenderer1;
      BatteryBulletRenderer batteryBulletRenderer3 = batteryBulletRenderer2;
      children1.Add((Control) batteryBulletRenderer3);
      Control.OrderedChildCollection children2 = ((Control) boxContainer).Children;
      Label label1 = new Label();
      ((Control) label1).StyleClasses.Add("ItemStatus");
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 3;
      ((Control) label1).VerticalAlignment = (Control.VAlignment) 3;
      Label label2 = label1;
      this._ammoCount = label1;
      Label label3 = label2;
      children2.Add((Control) label3);
      this.AddChild((Control) boxContainer);
    }

    public void Update(int count, int max)
    {
      ((Control) this._ammoCount).Visible = true;
      this._ammoCount.Text = $"x{count:00}";
      this._bullets.Capacity = max;
      this._bullets.Count = count;
    }
  }

  private sealed class ChamberMagazineStatusControl : Control
  {
    private readonly BulletRender _bulletRender;
    private readonly TextureRect _chamberedBullet;
    private readonly Label _noMagazineLabel;
    private readonly Label _ammoCount;

    public ChamberMagazineStatusControl()
    {
      this.MinHeight = 15f;
      this.HorizontalExpand = true;
      this.VerticalAlignment = (Control.VAlignment) 2;
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer1).HorizontalExpand = true;
      Control.OrderedChildCollection children1 = ((Control) boxContainer1).Children;
      Control control = new Control();
      control.HorizontalExpand = true;
      control.Margin = new Thickness(0.0f, 0.0f, 5f, 0.0f);
      Control.OrderedChildCollection children2 = control.Children;
      BulletRender bulletRender1 = new BulletRender();
      bulletRender1.HorizontalAlignment = (Control.HAlignment) 3;
      bulletRender1.VerticalAlignment = (Control.VAlignment) 3;
      BulletRender bulletRender2 = bulletRender1;
      this._bulletRender = bulletRender1;
      BulletRender bulletRender3 = bulletRender2;
      children2.Add((Control) bulletRender3);
      Control.OrderedChildCollection children3 = control.Children;
      Label label1 = new Label();
      label1.Text = "No Magazine!";
      ((Control) label1).StyleClasses.Add("ItemStatus");
      Label label2 = label1;
      this._noMagazineLabel = label1;
      Label label3 = label2;
      children3.Add((Control) label3);
      children1.Add(control);
      Control.OrderedChildCollection children4 = ((Control) boxContainer1).Children;
      BoxContainer boxContainer2 = new BoxContainer();
      boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer2).VerticalAlignment = (Control.VAlignment) 3;
      ((Control) boxContainer2).Margin = new Thickness(0.0f, 0.0f, 0.0f, 2f);
      Control.OrderedChildCollection children5 = ((Control) boxContainer2).Children;
      Label label4 = new Label();
      ((Control) label4).StyleClasses.Add("ItemStatus");
      ((Control) label4).HorizontalAlignment = (Control.HAlignment) 3;
      Label label5 = label4;
      this._ammoCount = label4;
      Label label6 = label5;
      children5.Add((Control) label6);
      Control.OrderedChildCollection children6 = ((Control) boxContainer2).Children;
      TextureRect textureRect1 = new TextureRect();
      textureRect1.Texture = StaticIoC.ResC.GetTexture("/Textures/Interface/ItemStatus/Bullets/chambered.png");
      ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 1;
      TextureRect textureRect2 = textureRect1;
      this._chamberedBullet = textureRect1;
      TextureRect textureRect3 = textureRect2;
      children6.Add((Control) textureRect3);
      children4.Add((Control) boxContainer2);
      this.AddChild((Control) boxContainer1);
    }

    public void Update(bool chambered, bool magazine, int count, int capacity)
    {
      ((Control) this._chamberedBullet).ModulateSelfOverride = new Color?(chambered ? Color.FromHex((ReadOnlySpan<char>) "#d7df60", new Color?()) : Color.Black);
      if (!magazine)
      {
        this._bulletRender.Visible = false;
        ((Control) this._noMagazineLabel).Visible = true;
        ((Control) this._ammoCount).Visible = false;
      }
      else
      {
        this._bulletRender.Visible = true;
        ((Control) this._noMagazineLabel).Visible = false;
        ((Control) this._ammoCount).Visible = true;
        this._bulletRender.Count = count;
        this._bulletRender.Capacity = capacity;
        this._bulletRender.Type = capacity > 50 ? BulletRender.BulletType.Tiny : BulletRender.BulletType.Normal;
        this._ammoCount.Text = $"x{count:00}";
      }
    }

    public void PlayAlarmAnimation(Animation animation)
    {
      ((Control) this._noMagazineLabel).PlayAnimation(animation, "alarm");
    }
  }

  private sealed class RevolverStatusControl : Control
  {
    private readonly BoxContainer _bulletsList;

    public RevolverStatusControl()
    {
      this.MinHeight = 15f;
      this.HorizontalExpand = true;
      this.VerticalAlignment = (Control.VAlignment) 2;
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
      ((Control) boxContainer1).HorizontalExpand = true;
      ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
      boxContainer1.SeparationOverride = new int?(0);
      BoxContainer boxContainer2 = boxContainer1;
      this._bulletsList = boxContainer1;
      this.AddChild((Control) boxContainer2);
    }

    public void Update(int currentIndex, bool?[] bullets)
    {
      ((Control) this._bulletsList).RemoveAllChildren();
      int length = bullets.Length;
      Texture texture1 = StaticIoC.ResC.GetTexture(length > 20 ? (length > 30 ? "/Textures/Interface/ItemStatus/Bullets/tiny.png" : "/Textures/Interface/ItemStatus/Bullets/small.png") : "/Textures/Interface/ItemStatus/Bullets/normal.png");
      Texture texture2 = StaticIoC.ResC.GetTexture("/Textures/Interface/ItemStatus/Bullets/empty.png");
      this.FillBulletRow(currentIndex, bullets, (Control) this._bulletsList, texture1, texture2);
    }

    private void FillBulletRow(
      int currentIndex,
      bool?[] bullets,
      Control container,
      Texture texture,
      Texture emptyTexture)
    {
      int length = bullets.Length;
      Color color1 = Color.FromHex((ReadOnlySpan<char>) "#b68f0e", new Color?());
      Color color2 = Color.FromHex((ReadOnlySpan<char>) "#d7df60", new Color?());
      Color color3 = Color.FromHex((ReadOnlySpan<char>) "#b50e25", new Color?());
      Color color4 = Color.FromHex((ReadOnlySpan<char>) "#d3745f", new Color?());
      Color color5 = Color.FromHex((ReadOnlySpan<char>) "#000000", new Color?());
      Color color6 = Color.FromHex((ReadOnlySpan<char>) "#222222", new Color?());
      bool flag = false;
      float num = 1.3f;
      for (int index = 0; index < length; ++index)
      {
        bool? bullet = bullets[index];
        Control control1 = new Control()
        {
          MinSize = Vector2i.op_Multiply(texture.Size, num)
        };
        if (index == currentIndex)
        {
          Control control2 = control1;
          TextureRect textureRect = new TextureRect();
          textureRect.Texture = texture;
          textureRect.TextureScale = new Vector2(num, num);
          ((Control) textureRect).ModulateSelfOverride = new Color?(Color.LimeGreen);
          control2.AddChild((Control) textureRect);
        }
        Texture texture1 = texture;
        Color color7;
        if (bullet.HasValue)
        {
          if (bullet.Value)
          {
            color7 = flag ? color1 : color2;
          }
          else
          {
            color7 = flag ? color3 : color4;
            texture1 = emptyTexture;
          }
        }
        else
          color7 = flag ? color5 : color6;
        Control control3 = control1;
        TextureRect textureRect1 = new TextureRect();
        textureRect1.Stretch = (TextureRect.StretchMode) 4;
        textureRect1.Texture = texture1;
        ((Control) textureRect1).ModulateSelfOverride = new Color?(color7);
        control3.AddChild((Control) textureRect1);
        flag = !flag;
        container.AddChild(control1);
      }
    }
  }
}
