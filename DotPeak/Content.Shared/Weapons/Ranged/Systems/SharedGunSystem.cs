// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.SharedGunSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Loadout;
using Content.Shared._PUBG.Weapons;
using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Emplacements;
using Content.Shared._RMC14.Random;
using Content.Shared._RMC14.Stack;
using Content.Shared._RMC14.Vehicle;
using Content.Shared._RMC14.Weapons.Ranged;
using Content.Shared._RMC14.Weapons.Ranged.Flamer;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared._RMC14.Xenonids;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Administration.Logs;
using Content.Shared.Camera;
using Content.Shared.Chemistry.Components;
using Content.Shared.CombatMode;
using Content.Shared.Containers;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Damage;
using Content.Shared.Damage.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Database;
using Content.Shared.DoAfter;
using Content.Shared.Effects;
using Content.Shared.Examine;
using Content.Shared.Explosion.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Gravity;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Light.Components;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Stacks;
using Content.Shared.Storage;
using Content.Shared.Stunnable;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Timing;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Reflect;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Components;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

public abstract class SharedGunSystem : EntitySystem
{
  [Robust.Shared.IoC.Dependency]
  private SharedDoAfterSystem _doAfter;
  [Robust.Shared.IoC.Dependency]
  private SharedInteractionSystem _interaction;
  [Robust.Shared.IoC.Dependency]
  private SharedRMCStackSystem _rmcStack;
  protected const string ChamberSlot = "gun_chamber";
  [Robust.Shared.IoC.Dependency]
  private ActionBlockerSystem _actionBlockerSystem;
  [Robust.Shared.IoC.Dependency]
  protected IGameTiming Timing;
  [Robust.Shared.IoC.Dependency]
  protected IMapManager MapManager;
  [Robust.Shared.IoC.Dependency]
  protected SharedMapSystem MapSystem;
  [Robust.Shared.IoC.Dependency]
  private INetManager _netManager;
  [Robust.Shared.IoC.Dependency]
  protected IPrototypeManager ProtoManager;
  [Robust.Shared.IoC.Dependency]
  protected IRobustRandom Random;
  [Robust.Shared.IoC.Dependency]
  protected ISharedAdminLogManager Logs;
  [Robust.Shared.IoC.Dependency]
  protected DamageableSystem Damageable;
  [Robust.Shared.IoC.Dependency]
  protected ExamineSystemShared Examine;
  [Robust.Shared.IoC.Dependency]
  private SharedHandsSystem _hands;
  [Robust.Shared.IoC.Dependency]
  private ItemSlotsSystem _slots;
  [Robust.Shared.IoC.Dependency]
  private RechargeBasicEntityAmmoSystem _recharge;
  [Robust.Shared.IoC.Dependency]
  protected SharedActionsSystem Actions;
  [Robust.Shared.IoC.Dependency]
  protected SharedAppearanceSystem Appearance;
  [Robust.Shared.IoC.Dependency]
  protected SharedAudioSystem Audio;
  [Robust.Shared.IoC.Dependency]
  private SharedCombatModeSystem _combatMode;
  [Robust.Shared.IoC.Dependency]
  protected SharedContainerSystem Containers;
  [Robust.Shared.IoC.Dependency]
  private SharedGravitySystem _gravity;
  [Robust.Shared.IoC.Dependency]
  protected SharedPointLightSystem Lights;
  [Robust.Shared.IoC.Dependency]
  protected SharedPopupSystem PopupSystem;
  [Robust.Shared.IoC.Dependency]
  protected SharedPhysicsSystem Physics;
  [Robust.Shared.IoC.Dependency]
  protected SharedProjectileSystem Projectiles;
  [Robust.Shared.IoC.Dependency]
  protected SharedTransformSystem TransformSystem;
  [Robust.Shared.IoC.Dependency]
  protected TagSystem TagSystem;
  [Robust.Shared.IoC.Dependency]
  protected ThrowingSystem ThrowingSystem;
  [Robust.Shared.IoC.Dependency]
  private UseDelaySystem _useDelay;
  [Robust.Shared.IoC.Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  [Robust.Shared.IoC.Dependency]
  private SharedStaminaSystem _stamina;
  [Robust.Shared.IoC.Dependency]
  private SharedStunSystem _stun;
  [Robust.Shared.IoC.Dependency]
  private SharedColorFlashEffectSystem _color;
  [Robust.Shared.IoC.Dependency]
  private SharedCameraRecoilSystem _recoil;
  [Robust.Shared.IoC.Dependency]
  private IConfigurationManager _config;
  [Robust.Shared.IoC.Dependency]
  private INetConfigurationManager _netConfig;
  [Robust.Shared.IoC.Dependency]
  private ISharedPlayerManager _playerManager;
  [Robust.Shared.IoC.Dependency]
  private EntityLookupSystem _lookup;
  [Robust.Shared.IoC.Dependency]
  private AttachableHolderSystem _attachableHolder;
  [Robust.Shared.IoC.Dependency]
  private SharedRMCFlamerSystem _flamer;
  [Robust.Shared.IoC.Dependency]
  private VehicleSystem _rmcVehicle;
  [Robust.Shared.IoC.Dependency]
  private VehicleWeaponsSystem _rmcVehicleWeapons;
  [Robust.Shared.IoC.Dependency]
  private RMCSharedWeaponControllerSystem _rmcSharedWeaponController;
  [Robust.Shared.IoC.Dependency]
  private PubgWeaponModulesSystem _pubgWeaponModules;
  private const float InteractNextFire = 0.3f;
  private const double SafetyNextFire = 0.5;
  private const float EjectOffset = 0.4f;
  protected const string AmmoExamineColor = "yellow";
  protected const string FireRateExamineColor = "yellow";
  public const string ModeExamineColor = "cyan";
  public const float GunClumsyChance = 0.5f;
  private const float DamagePitchVariation = 0.05f;
  public const string MagazineSlot = "gun_magazine";
  protected const string RevolverContainer = "revolver-ammo";

  public void SetEnabled(EntityUid uid, AutoShootGunComponent component, bool status)
  {
    component.Enabled = status;
  }

  protected virtual void InitializeBallistic()
  {
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, ComponentInit>(new ComponentEventHandler<BallisticAmmoProviderComponent, ComponentInit>(this.OnBallisticInit));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, MapInitEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, MapInitEvent>(this.OnBallisticMapInit));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, TakeAmmoEvent>(this.OnBallisticTakeAmmo));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<BallisticAmmoProviderComponent, GetAmmoCountEvent>(this.OnBallisticAmmoCount));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, ExaminedEvent>(this.OnBallisticExamine));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, GetVerbsEvent<Verb>>(new ComponentEventHandler<BallisticAmmoProviderComponent, GetVerbsEvent<Verb>>(this.OnBallisticVerb));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, InteractUsingEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, InteractUsingEvent>(this.OnBallisticInteractUsing));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, AfterInteractEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, AfterInteractEvent>(this.OnBallisticAfterInteract));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, AmmoFillDoAfterEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, AmmoFillDoAfterEvent>(this.OnBallisticAmmoFillDoAfter));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, DelayedAmmoInsertDoAfterEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, DelayedAmmoInsertDoAfterEvent>(this.OnBallisticDelayedAmmoInsertDoAfter));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, DelayedCycleDoAfterEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, DelayedCycleDoAfterEvent>(this.OnBallisticDelayedCycleDoAfter));
    this.SubscribeLocalEvent<BallisticAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<BallisticAmmoProviderComponent, UseInHandEvent>(this.OnBallisticUse));
  }

  private void OnBallisticUse(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    UseInHandEvent args)
  {
    if (args.Handled || !component.Cycleable)
      return;
    if (component.CycleDelay > 0.0)
      this.BallisticCycleDelayCheck(uid, component, args.User);
    else
      this.ManualCycle(uid, component, this.TransformSystem.GetMapCoordinates(uid), new EntityUid?(args.User));
    args.Handled = true;
  }

  private void BallisticCycleDelayCheck(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    EntityUid user)
  {
    if (component.CycleDelay > 0.0)
    {
      this.Popup(this.Loc.GetString("gun-ballistic-cycle-delayed", ("entity", (object) uid)), new EntityUid?(uid), new EntityUid?(user));
      TimeSpan timeSpan = TimeSpan.FromSeconds(component.CycleDelay);
      SharedDoAfterSystem doAfter = this._doAfter;
      EntityManager entityManager = this.EntityManager;
      EntityUid user1 = user;
      TimeSpan delay = timeSpan;
      DelayedCycleDoAfterEvent @event = new DelayedCycleDoAfterEvent();
      EntityUid? nullable1 = new EntityUid?(uid);
      EntityUid? nullable2 = new EntityUid?(uid);
      EntityUid? eventTarget = new EntityUid?(uid);
      EntityUid? target = nullable2;
      EntityUid? used = nullable1;
      doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user1, delay, (DoAfterEvent) @event, eventTarget, target, used)
      {
        BreakOnMove = true,
        BreakOnDamage = false,
        NeedHand = true
      });
    }
    else
      this.ManualCycle(uid, component, this.TransformSystem.GetMapCoordinates(uid), new EntityUid?(user));
  }

  private void OnBallisticInteractUsing(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled || !this.TryAmmoInsert(uid, component, args.Used, args.User, args.Target, component.InsertDelay))
      return;
    args.Handled = true;
  }

  public bool TryAmmoInsert(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    EntityUid ammo,
    EntityUid loader,
    EntityUid weapon,
    double insertDelay)
  {
    if (this._whitelistSystem.IsWhitelistFailOrNull(component.Whitelist, ammo))
      return false;
    if (this.HasComp<ActiveTimerTriggerComponent>(ammo))
    {
      this.Popup(this.Loc.GetString("gun-ballistic-transfer-primed", ("ammoEntity", (object) ammo)), new EntityUid?(uid), new EntityUid?(loader));
      return false;
    }
    ExpendableLightComponent comp;
    if (this.GetBallisticShots(component) >= component.Capacity || this.TryComp<ExpendableLightComponent>(ammo, out comp) && comp.CurrentState != ExpendableLightState.BrandNew)
      return false;
    if (insertDelay > 0.0)
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds(insertDelay);
      SharedDoAfterSystem doAfter = this._doAfter;
      EntityManager entityManager = this.EntityManager;
      EntityUid user = loader;
      TimeSpan delay = timeSpan;
      DelayedAmmoInsertDoAfterEvent @event = new DelayedAmmoInsertDoAfterEvent();
      EntityUid? nullable1 = new EntityUid?(ammo);
      EntityUid? nullable2 = new EntityUid?(weapon);
      EntityUid? eventTarget = new EntityUid?(uid);
      EntityUid? target = nullable2;
      EntityUid? used = nullable1;
      doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, delay, (DoAfterEvent) @event, eventTarget, target, used)
      {
        BreakOnMove = true,
        BreakOnDamage = false,
        NeedHand = true
      });
    }
    else
      this.ManualLoad(uid, component, ammo, loader);
    return true;
  }

  private void OnBallisticAfterInteract(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    AfterInteractEvent args)
  {
    if (args.Handled || !component.MayTransfer || !this.Timing.IsFirstTimePredicted || !args.Target.HasValue)
      return;
    EntityUid used1 = args.Used;
    EntityUid? nullable = args.Target;
    BallisticAmmoProviderComponent comp;
    if ((nullable.HasValue ? (used1 == nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0 || this.Deleted(args.Target) || !this.TryComp<BallisticAmmoProviderComponent>(args.Target, out comp) || comp.Whitelist == null)
      return;
    SharedContainerSystem containers = this.Containers;
    nullable = args.Target;
    Entity<TransformComponent, MetaDataComponent> ent = (Entity<TransformComponent, MetaDataComponent>) (nullable.Value, (TransformComponent) null);
    BaseContainer baseContainer;
    ref BaseContainer local = ref baseContainer;
    if (containers.TryGetContainingContainer(ent, out local) && baseContainer.Owner != args.User && this.HasComp<StorageComponent>(baseContainer.Owner))
      return;
    args.Handled = true;
    SharedDoAfterSystem doAfter = this._doAfter;
    EntityManager entityManager = this.EntityManager;
    EntityUid user = args.User;
    TimeSpan fillDelay = component.FillDelay;
    AmmoFillDoAfterEvent @event = new AmmoFillDoAfterEvent();
    nullable = new EntityUid?(uid);
    EntityUid? target1 = args.Target;
    EntityUid? eventTarget = new EntityUid?(uid);
    EntityUid? target2 = target1;
    EntityUid? used2 = nullable;
    doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, fillDelay, (DoAfterEvent) @event, eventTarget, target2, used2)
    {
      BreakOnMove = true,
      BreakOnDamage = false,
      NeedHand = true
    });
  }

  private void OnBallisticAmmoFillDoAfter(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    AmmoFillDoAfterEvent args)
  {
    // ISSUE: variable of a compiler-generated type
    SharedGunSystem.\u003C\u003Ec__DisplayClass10_0 cDisplayClass100;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass100.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass100.args = args;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass100.args.Handled || cDisplayClass100.args.Cancelled)
      return;
    BallisticAmmoProviderComponent comp;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    if (this.Deleted(cDisplayClass100.args.Target) || !this.TryComp<BallisticAmmoProviderComponent>(cDisplayClass100.args.Target, out comp) || comp.Whitelist == null || cDisplayClass100.args.Cancelled)
    {
      // ISSUE: reference to a compiler-generated field
      this.Popup(this.Loc.GetString("gun-ballistic-transfer-cancelled", ("entity", (object) uid)), new EntityUid?(uid), new EntityUid?(cDisplayClass100.args.User));
    }
    else if (comp.Entities.Count + comp.UnspawnedCount == comp.Capacity)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.Popup(this.Loc.GetString("gun-ballistic-transfer-target-full", ("entity", (object) cDisplayClass100.args.Target)), cDisplayClass100.args.Target, new EntityUid?(cDisplayClass100.args.User));
    }
    else if (component.Entities.Count + component.UnspawnedCount == 0)
    {
      // ISSUE: reference to a compiler-generated field
      this.Popup(this.Loc.GetString("gun-ballistic-transfer-empty", ("entity", (object) uid)), new EntityUid?(uid), new EntityUid?(cDisplayClass100.args.User));
    }
    else
    {
      List<(EntityUid?, IShootable)> ammo1 = new List<(EntityUid?, IShootable)>();
      // ISSUE: reference to a compiler-generated field
      TakeAmmoEvent args1 = new TakeAmmoEvent(Math.Clamp(comp.Capacity - comp.Count, 0, 20), ammo1, this.Transform(uid).Coordinates, new EntityUid?(cDisplayClass100.args.User));
      this.RaiseLocalEvent<TakeAmmoEvent>(uid, args1);
      foreach ((EntityUid? nullable, IShootable _) in ammo1)
      {
        if (nullable.HasValue)
        {
          EntityUid? target;
          if (this._whitelistSystem.IsWhitelistFail(comp.Whitelist, nullable.Value))
          {
            ILocalizationManager loc = this.Loc;
            (string, object) valueTuple1 = ("ammoEntity", (object) nullable.Value);
            // ISSUE: reference to a compiler-generated field
            target = cDisplayClass100.args.Target;
            (string, object) valueTuple2 = ("targetEntity", (object) target.Value);
            // ISSUE: reference to a compiler-generated field
            this.Popup(loc.GetString("gun-ballistic-transfer-invalid", valueTuple1, valueTuple2), new EntityUid?(uid), new EntityUid?(cDisplayClass100.args.User));
            // ISSUE: reference to a compiler-generated method
            this.\u003COnBallisticAmmoFillDoAfter\u003Eg__SimulateInsertAmmo\u007C10_0(nullable.Value, uid, this.Transform(uid).Coordinates, ref cDisplayClass100);
          }
          else
          {
            // ISSUE: reference to a compiler-generated field
            this.Audio.PlayPredicted(component.SoundInsert, uid, new EntityUid?(cDisplayClass100.args.User));
            EntityUid ammo2 = nullable.Value;
            // ISSUE: reference to a compiler-generated field
            target = cDisplayClass100.args.Target;
            EntityUid ammoProvider = target.Value;
            // ISSUE: reference to a compiler-generated field
            target = cDisplayClass100.args.Target;
            EntityCoordinates coordinates = this.Transform(target.Value).Coordinates;
            ref SharedGunSystem.\u003C\u003Ec__DisplayClass10_0 local = ref cDisplayClass100;
            // ISSUE: reference to a compiler-generated method
            this.\u003COnBallisticAmmoFillDoAfter\u003Eg__SimulateInsertAmmo\u007C10_0(ammo2, ammoProvider, coordinates, ref local);
          }
          if (this.IsClientSide(nullable.Value))
            this.Del(new EntityUid?(nullable.Value));
        }
      }
      bool flag1 = comp.Entities.Count + comp.UnspawnedCount < comp.Capacity;
      bool flag2 = component.Entities.Count + component.UnspawnedCount > 0;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass100.args.Repeat = flag1 & flag2;
      if (!component.DeleteWhenEmpty || component.Entities.Count != 0)
        return;
      this.Del(new EntityUid?(uid));
    }
  }

  private void OnBallisticDelayedAmmoInsertDoAfter(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    DelayedAmmoInsertDoAfterEvent args)
  {
    BallisticAmmoProviderComponent comp;
    if (this.Deleted(args.Target) || !this.TryComp<BallisticAmmoProviderComponent>(args.Target, out comp) || comp.Whitelist == null || args.Cancelled)
      this.Popup(this.Loc.GetString("gun-ballistic-transfer-cancelled", ("entity", (object) uid)), new EntityUid?(uid), new EntityUid?(args.User));
    else if (comp.Entities.Count + comp.UnspawnedCount == comp.Capacity)
      this.Popup(this.Loc.GetString("gun-ballistic-transfer-target-full", ("entity", (object) args.Target)), args.Target, new EntityUid?(args.User));
    else if (args.Used.HasValue)
    {
      this.ManualLoad(uid, component, args.Used.Value, args.User);
      args.Handled = true;
    }
    else
      args.Handled = false;
  }

  private void ManualLoad(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    EntityUid used,
    EntityUid user)
  {
    StackComponent comp;
    if (this.TryComp<StackComponent>(used, out comp))
    {
      EntityCoordinates moverCoordinates = this.TransformSystem.GetMoverCoordinates(used);
      EntityUid? nullable = this._rmcStack.Split((Entity<StackComponent>) (used, comp), 1, moverCoordinates);
      if (nullable.HasValue)
        used = nullable.Value;
      SoundSpecifier soundInsert = this.CompOrNull<CartridgeAmmoComponent>(used)?.SoundInsert;
      if (soundInsert != null)
        this.Audio.PlayPredicted(soundInsert, uid, new EntityUid?(user));
      this.UpdateAmmoCount(uid, artificialIncrease: 1);
      if (this._netManager.IsClient)
        return;
    }
    component.Entities.Add(used);
    this.Containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) used, (BaseContainer) component.Container);
    this.Dirty(uid, (IComponent) component);
    this.Audio.PlayPredicted(component.SoundInsert, uid, new EntityUid?(user));
    this.UpdateBallisticAppearance(uid, component);
    this.UpdateAmmoCount(uid);
    this.DirtyField<BallisticAmmoProviderComponent>(uid, component, "Entities");
  }

  private void OnBallisticDelayedCycleDoAfter(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    DelayedCycleDoAfterEvent args)
  {
    if (this.Deleted(uid) || args.Cancelled)
      this.Popup(this.Loc.GetString("gun-ballistic-cycle-delayed-cancelled", ("entity", (object) uid)), new EntityUid?(uid), new EntityUid?(args.User));
    else if (component.Entities.Count + component.UnspawnedCount == 0)
    {
      this.Popup(this.Loc.GetString("gun-ballistic-cycle-delayed-empty", ("entity", (object) uid)), new EntityUid?(uid), new EntityUid?(args.User));
    }
    else
    {
      this.ManualCycle(uid, component, this.TransformSystem.GetMapCoordinates(uid), new EntityUid?(args.User));
      args.Handled = true;
    }
  }

  private void OnBallisticVerb(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    GetVerbsEvent<Verb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || !component.Cycleable || !component.Cycleable)
      return;
    args.Verbs.Add(new Verb()
    {
      Text = this.Loc.GetString("gun-ballistic-cycle"),
      Disabled = this.GetBallisticShots(component) == 0,
      Act = (Action) (() => this.BallisticCycleDelayCheck(uid, component, args.User))
    });
  }

  private void OnBallisticExamine(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    args.PushMarkup(this.Loc.GetString("gun-magazine-examine", ("color", (object) "yellow"), ("count", (object) this.GetBallisticShots(component))));
  }

  private void ManualCycle(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    MapCoordinates coordinates,
    EntityUid? user = null,
    GunComponent? gunComp = null)
  {
    if (!component.Cycleable)
      return;
    if (this.Resolve<GunComponent>(uid, ref gunComp, false) && gunComp != null && (double) gunComp.FireRateModified > 0.0 && !this.Paused(uid))
    {
      gunComp.NextFire = this.Timing.CurTime + TimeSpan.FromSeconds(1.0 / (double) gunComp.FireRateModified);
      this.DirtyField<GunComponent>(uid, gunComp, "NextFire");
    }
    this.Audio.PlayPredicted(component.SoundRack, uid, user);
    int ballisticShots = this.GetBallisticShots(component);
    this.Cycle(uid, component, coordinates);
    this.Popup(this.Loc.GetString(ballisticShots == 0 ? "gun-ballistic-cycled-empty" : "gun-ballistic-cycled"), new EntityUid?(uid), user);
    this.UpdateBallisticAppearance(uid, component);
    this.UpdateAmmoCount(uid);
  }

  protected abstract void Cycle(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    MapCoordinates coordinates);

  private void OnBallisticInit(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    ComponentInit args)
  {
    component.Container = this.Containers.EnsureContainer<Container>(uid, "ballistic-ammo");
    this.UpdateBallisticAppearance(uid, component);
  }

  private void OnBallisticMapInit(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    MapInitEvent args)
  {
    if (!component.Proto.HasValue)
      return;
    component.UnspawnedCount = Math.Max(0, component.Capacity - component.Container.ContainedEntities.Count);
    this.UpdateBallisticAppearance(uid, component);
    this.DirtyField<BallisticAmmoProviderComponent>(uid, component, "UnspawnedCount");
  }

  protected int GetBallisticShots(BallisticAmmoProviderComponent component)
  {
    return component.Entities.Count + component.UnspawnedCount;
  }

  private void OnBallisticTakeAmmo(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    for (int index = 0; index < args.Shots; ++index)
    {
      if (component.Entities.Count > 0)
      {
        List<EntityUid> entities = component.Entities;
        EntityUid entityUid = entities[entities.Count - 1];
        args.Ammo.Add((new EntityUid?(entityUid), this.EnsureShootable(entityUid)));
        component.Entities.RemoveAt(component.Entities.Count - 1);
        this.DirtyField<BallisticAmmoProviderComponent>(uid, component, "Entities");
        this.Containers.Remove((Entity<TransformComponent, MetaDataComponent>) entityUid, (BaseContainer) component.Container);
      }
      else if (component.UnspawnedCount > 0)
      {
        --component.UnspawnedCount;
        this.DirtyField<BallisticAmmoProviderComponent>(uid, component, "UnspawnedCount");
        EntProtoId? proto = component.Proto;
        EntityUid uid1 = this.Spawn(proto.HasValue ? (string) proto.GetValueOrDefault() : (string) null, args.Coordinates);
        args.Ammo.Add((new EntityUid?(uid1), this.EnsureShootable(uid1)));
      }
    }
    this.UpdateBallisticAppearance(uid, component);
  }

  private void OnBallisticAmmoCount(
    EntityUid uid,
    BallisticAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    args.Count = this.GetBallisticShots(component);
    args.Capacity = component.Capacity;
  }

  public void UpdateBallisticAppearance(EntityUid uid, BallisticAmmoProviderComponent component)
  {
    AppearanceComponent comp;
    if (!this.Timing.IsFirstTimePredicted || !this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoCount, (object) this.GetBallisticShots(component), comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoMax, (object) component.Capacity, comp);
  }

  public void SetBallisticUnspawned(Entity<BallisticAmmoProviderComponent> entity, int count)
  {
    if (entity.Comp.UnspawnedCount == count)
      return;
    entity.Comp.UnspawnedCount = count;
    this.UpdateBallisticAppearance(entity.Owner, entity.Comp);
    this.UpdateAmmoCount(entity.Owner);
    this.Dirty<BallisticAmmoProviderComponent>(entity);
  }

  protected virtual void InitializeBasicEntity()
  {
    this.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, MapInitEvent>(new ComponentEventHandler<BasicEntityAmmoProviderComponent, MapInitEvent>(this.OnBasicEntityMapInit));
    this.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<BasicEntityAmmoProviderComponent, TakeAmmoEvent>(this.OnBasicEntityTakeAmmo));
    this.SubscribeLocalEvent<BasicEntityAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<BasicEntityAmmoProviderComponent, GetAmmoCountEvent>(this.OnBasicEntityAmmoCount));
  }

  private void OnBasicEntityMapInit(
    EntityUid uid,
    BasicEntityAmmoProviderComponent component,
    MapInitEvent args)
  {
    if (!component.Count.HasValue)
    {
      component.Count = component.Capacity;
      this.Dirty(uid, (IComponent) component);
    }
    this.UpdateBasicEntityAppearance(uid, component);
  }

  private void OnBasicEntityTakeAmmo(
    EntityUid uid,
    BasicEntityAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    for (int index = 0; index < args.Shots; ++index)
    {
      int? count = component.Count;
      int num = 0;
      if (count.GetValueOrDefault() <= num & count.HasValue)
        return;
      if (component.Count.HasValue)
      {
        BasicEntityAmmoProviderComponent providerComponent = component;
        count = providerComponent.Count;
        providerComponent.Count = count.HasValue ? new int?(count.GetValueOrDefault() - 1) : new int?();
      }
      EntityUid uid1 = this.Spawn(component.Proto, args.Coordinates);
      args.Ammo.Add((new EntityUid?(uid1), this.EnsureShootable(uid1)));
    }
    this._recharge.Reset(uid);
    this.UpdateBasicEntityAppearance(uid, component);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnBasicEntityAmmoCount(
    EntityUid uid,
    BasicEntityAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    args.Capacity = component.Capacity ?? int.MaxValue;
    args.Count = component.Count ?? int.MaxValue;
  }

  private void UpdateBasicEntityAppearance(
    EntityUid uid,
    BasicEntityAmmoProviderComponent component)
  {
    AppearanceComponent comp;
    if (!this.Timing.IsFirstTimePredicted || !this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    SharedAppearanceSystem appearance = this.Appearance;
    EntityUid uid1 = uid;
    // ISSUE: variable of a boxed type
    __Boxed<AmmoVisuals> key = (Enum) AmmoVisuals.HasAmmo;
    int? count = component.Count;
    int num = 0;
    // ISSUE: variable of a boxed type
    __Boxed<bool> local = (ValueType) !(count.GetValueOrDefault() == num & count.HasValue);
    AppearanceComponent component1 = comp;
    appearance.SetData(uid1, (Enum) key, (object) local, component1);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoCount, (object) (component.Count ?? int.MaxValue), comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoMax, (object) (component.Capacity ?? int.MaxValue), comp);
  }

  public bool ChangeBasicEntityAmmoCount(
    EntityUid uid,
    int delta,
    BasicEntityAmmoProviderComponent? component = null)
  {
    return this.Resolve<BasicEntityAmmoProviderComponent>(uid, ref component, false) && component.Count.HasValue && this.UpdateBasicEntityAmmoCount(uid, component.Count.Value + delta, component);
  }

  public bool UpdateBasicEntityAmmoCount(
    EntityUid uid,
    int count,
    BasicEntityAmmoProviderComponent? component = null)
  {
    if (!this.Resolve<BasicEntityAmmoProviderComponent>(uid, ref component, false))
      return false;
    int num = count;
    int? capacity = component.Capacity;
    int valueOrDefault = capacity.GetValueOrDefault();
    if (num > valueOrDefault & capacity.HasValue)
      return false;
    component.Count = new int?(count);
    this.UpdateBasicEntityAppearance(uid, component);
    this.UpdateAmmoCount(uid);
    this.Dirty(uid, (IComponent) component);
    return true;
  }

  protected virtual void InitializeBattery()
  {
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, ComponentGetState>(this.OnBatteryGetState));
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, ComponentHandleState>(this.OnBatteryHandleState));
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, TakeAmmoEvent>(this.OnBatteryTakeAmmo));
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<HitscanBatteryAmmoProviderComponent, GetAmmoCountEvent>(this.OnBatteryAmmoCount));
    this.SubscribeLocalEvent<HitscanBatteryAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<HitscanBatteryAmmoProviderComponent, ExaminedEvent>(this.OnBatteryExamine));
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, ComponentGetState>(this.OnBatteryGetState));
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, ComponentHandleState>(this.OnBatteryHandleState));
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, TakeAmmoEvent>(this.OnBatteryTakeAmmo));
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<ProjectileBatteryAmmoProviderComponent, GetAmmoCountEvent>(this.OnBatteryAmmoCount));
    this.SubscribeLocalEvent<ProjectileBatteryAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<ProjectileBatteryAmmoProviderComponent, ExaminedEvent>(this.OnBatteryExamine));
  }

  private void OnBatteryHandleState(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is SharedGunSystem.BatteryAmmoProviderComponentState current))
      return;
    component.Shots = current.Shots;
    component.Capacity = current.MaxShots;
    component.FireCost = current.FireCost;
    this.UpdateAmmoCount(uid, false);
  }

  private void OnBatteryGetState(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new SharedGunSystem.BatteryAmmoProviderComponentState()
    {
      Shots = component.Shots,
      MaxShots = component.Capacity,
      FireCost = component.FireCost
    };
  }

  private void OnBatteryExamine(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("gun-battery-examine", ("color", (object) "yellow"), ("count", (object) component.Shots)));
  }

  private void OnBatteryTakeAmmo(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    int num = Math.Min(args.Shots, component.Shots);
    if (num == 0)
      return;
    for (int index = 0; index < num; ++index)
    {
      args.Ammo.Add(this.GetShootable(component, args.Coordinates));
      --component.Shots;
    }
    this.TakeCharge((Entity<BatteryAmmoProviderComponent>) (uid, component));
    this.UpdateBatteryAppearance(uid, component);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnBatteryAmmoCount(
    EntityUid uid,
    BatteryAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    args.Count = component.Shots;
    args.Capacity = component.Capacity;
  }

  protected virtual void TakeCharge(Entity<BatteryAmmoProviderComponent> entity)
  {
    this.UpdateAmmoCount((EntityUid) entity, false);
  }

  protected void UpdateBatteryAppearance(EntityUid uid, BatteryAmmoProviderComponent component)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.HasAmmo, (object) (component.Shots != 0), comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoCount, (object) component.Shots, comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoMax, (object) component.Capacity, comp);
  }

  private (EntityUid? Entity, IShootable) GetShootable(
    BatteryAmmoProviderComponent component,
    EntityCoordinates coordinates)
  {
    switch (component)
    {
      case ProjectileBatteryAmmoProviderComponent providerComponent1:
        EntityUid uid = this.Spawn(providerComponent1.Prototype, coordinates);
        return (new EntityUid?(uid), this.EnsureShootable(uid));
      case HitscanBatteryAmmoProviderComponent providerComponent2:
        return (new EntityUid?(), (IShootable) this.ProtoManager.Index<HitscanPrototype>(providerComponent2.Prototype));
      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  protected virtual void InitializeCartridge()
  {
  }

  protected virtual void InitializeChamberMagazine()
  {
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ComponentStartup>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ComponentStartup>(this.OnChamberStartup));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, TakeAmmoEvent>(this.OnChamberMagazineTakeAmmo));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<ChamberMagazineAmmoProviderComponent, GetAmmoCountEvent>(this.OnChamberAmmoCount));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<ActivationVerb>>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<ActivationVerb>>(this.OnChamberActivationVerb));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<InteractionVerb>>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<InteractionVerb>>(this.OnChamberInteractionVerb));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnMagazineVerb));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ActivateInWorldEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ActivateInWorldEvent>(this.OnChamberActivate));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, UseInHandEvent>(this.OnChamberUse));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(this.OnMagazineSlotChange));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnMagazineSlotChange));
    this.SubscribeLocalEvent<ChamberMagazineAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<ChamberMagazineAmmoProviderComponent, ExaminedEvent>(this.OnChamberMagazineExamine));
  }

  private void OnChamberStartup(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    ComponentStartup args)
  {
    if (!component.BoltClosed.HasValue)
      return;
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.BoltClosed, (object) component.BoltClosed.Value);
  }

  private void OnChamberActivate(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    args.Handled = true;
    this.ToggleBolt(uid, component, new EntityUid?(args.User));
  }

  private void OnChamberUse(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    UseInHandEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (component.CanRack)
      this.UseChambered(uid, component, new EntityUid?(args.User));
    else
      this.ToggleBolt(uid, component, new EntityUid?(args.User));
  }

  private void OnChamberActivationVerb(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    GetVerbsEvent<ActivationVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || args.Hands == null || !component.BoltClosed.HasValue || !component.CanRack)
      return;
    SortedSet<ActivationVerb> verbs = args.Verbs;
    ActivationVerb activationVerb = new ActivationVerb();
    activationVerb.Text = this.Loc.GetString("gun-chamber-rack");
    activationVerb.Act = (Action) (() => this.UseChambered(uid, component, new EntityUid?(args.User)));
    verbs.Add(activationVerb);
  }

  private void UseChambered(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    EntityUid? user = null)
  {
    bool? boltClosed1 = component.BoltClosed;
    bool flag1 = false;
    if (boltClosed1.GetValueOrDefault() == flag1 & boltClosed1.HasValue)
    {
      this.ToggleBolt(uid, component, user);
    }
    else
    {
      EntityUid? entity;
      if (this.TryTakeChamberEntity(uid, out entity))
      {
        if (this._netManager.IsServer)
          this.EjectCartridge(entity.Value);
        else
          this.TransformSystem.DetachEntity(entity.Value, this.Transform(entity.Value));
      }
      if (!this.CycleCartridge(uid, component, user))
        this.UpdateAmmoCount(uid);
      bool? boltClosed2 = component.BoltClosed;
      bool flag2 = false;
      if (boltClosed2.GetValueOrDefault() == flag2 & boltClosed2.HasValue)
        return;
      this.Audio.PlayPredicted(component.RackSound, uid, user);
    }
  }

  private void OnChamberInteractionVerb(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    GetVerbsEvent<InteractionVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || args.Hands == null || !component.BoltClosed.HasValue)
      return;
    SortedSet<InteractionVerb> verbs = args.Verbs;
    InteractionVerb interactionVerb = new InteractionVerb();
    interactionVerb.Text = component.BoltClosed.Value ? this.Loc.GetString("gun-chamber-bolt-open") : this.Loc.GetString("gun-chamber-bolt-close");
    interactionVerb.Act = (Action) (() => this.ToggleBolt(uid, component, new EntityUid?(args.User)));
    verbs.Add(interactionVerb);
  }

  public void SetBoltClosed(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    bool value,
    EntityUid? user = null,
    AppearanceComponent? appearance = null,
    ItemSlotsComponent? slots = null)
  {
    if (!component.BoltClosed.HasValue)
      return;
    int num1 = value ? 1 : 0;
    bool? boltClosed = component.BoltClosed;
    int num2 = boltClosed.GetValueOrDefault() ? 1 : 0;
    if (num1 == num2 & boltClosed.HasValue)
      return;
    this.Resolve<AppearanceComponent, ItemSlotsComponent>(uid, ref appearance, ref slots, false);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.BoltClosed, (object) value, appearance);
    if (value)
    {
      this.CycleCartridge(uid, component, user, appearance);
      if (user.HasValue)
        this.PopupSystem.PopupClient(this.Loc.GetString("gun-chamber-bolt-closed"), uid, new EntityUid?(user.Value));
      if (slots != null)
        this._slots.SetLock(uid, "gun_chamber", true, slots);
      this.Audio.PlayPredicted(component.BoltClosedSound, uid, user);
    }
    else
    {
      EntityUid? entity;
      if (this.TryTakeChamberEntity(uid, out entity))
      {
        if (this._netManager.IsServer)
          this.EjectCartridge(entity.Value);
        else
          this.TransformSystem.DetachEntity(entity.Value, this.Transform(entity.Value));
        this.UpdateAmmoCount(uid);
      }
      if (user.HasValue)
        this.PopupSystem.PopupClient(this.Loc.GetString("gun-chamber-bolt-opened"), uid, new EntityUid?(user.Value));
      if (slots != null)
        this._slots.SetLock(uid, "gun_chamber", false, slots);
      this.Audio.PlayPredicted(component.BoltOpenedSound, uid, user);
    }
    component.BoltClosed = new bool?(value);
    this.Dirty(uid, (IComponent) component);
  }

  private bool CycleCartridge(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    EntityUid? user = null,
    AppearanceComponent? appearance = null)
  {
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    EntityUid? chamberEntity = this.GetChamberEntity(uid);
    bool flag = false;
    if (magazineEntity.HasValue && !chamberEntity.HasValue)
    {
      TakeAmmoEvent args1 = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), this.Transform(uid).Coordinates, user);
      this.RaiseLocalEvent<TakeAmmoEvent>(magazineEntity.Value, args1);
      if (args1.Ammo.Count > 0)
      {
        EntityUid? entity = args1.Ammo[0].Entity;
        this.TryInsertChamber(uid, entity.Value);
        GetAmmoCountEvent args2 = new GetAmmoCountEvent();
        this.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref args2);
        this.FinaliseMagazineTakeAmmo(uid, (MagazineAmmoProviderComponent) component, args2.Count, args2.Capacity, user, appearance);
        this.UpdateAmmoCount(uid);
        if (this._netManager.IsClient)
        {
          foreach ((EntityUid? Entity, IShootable _) in args1.Ammo)
          {
            if (this.IsClientSide(Entity.Value))
              this.Del(new EntityUid?(Entity.Value));
          }
        }
      }
      else
        this.UpdateAmmoCount(uid);
      flag = true;
    }
    return flag;
  }

  public void ToggleBolt(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    EntityUid? user = null)
  {
    if (!component.BoltClosed.HasValue)
      return;
    this.SetBoltClosed(uid, component, !component.BoltClosed.Value, user);
  }

  private void OnChamberMagazineExamine(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    ExaminedEvent args)
  {
    if (!args.IsInDetailsRange)
      return;
    int num = this.GetChamberMagazineCountCapacity(uid, component).Item1;
    using (args.PushGroup("ChamberMagazineAmmoProviderComponent"))
    {
      if (component.BoltClosed.HasValue)
      {
        string str = !component.BoltClosed.GetValueOrDefault() ? this.Loc.GetString("gun-chamber-bolt-closed-state") : this.Loc.GetString("gun-chamber-bolt-open-state");
        args.PushMarkup(this.Loc.GetString("gun-chamber-bolt", ("bolt", (object) str), ("color", (object) (component.BoltClosed.Value ? Color.FromHex((ReadOnlySpan<char>) "#94e1f2", new Color?()) : Color.FromHex((ReadOnlySpan<char>) "#f29d94", new Color?())))));
      }
      args.PushMarkup(this.Loc.GetString("gun-magazine-examine", ("color", (object) "yellow"), ("count", (object) num)));
    }
  }

  private bool TryTakeChamberEntity(EntityUid uid, [NotNullWhen(true)] out EntityUid? entity)
  {
    BaseContainer container;
    if (!this.Containers.TryGetContainer(uid, "gun_chamber", out container) || !(container is ContainerSlot containerSlot))
    {
      entity = new EntityUid?();
      return false;
    }
    entity = containerSlot.ContainedEntity;
    if (!entity.HasValue)
      return false;
    this.Containers.Remove((Entity<TransformComponent, MetaDataComponent>) entity.Value, container);
    return true;
  }

  protected EntityUid? GetChamberEntity(EntityUid uid)
  {
    BaseContainer container;
    return !this.Containers.TryGetContainer(uid, "gun_chamber", out container) || !(container is ContainerSlot containerSlot) ? new EntityUid?() : containerSlot.ContainedEntity;
  }

  protected (int, int) GetChamberMagazineCountCapacity(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component)
  {
    int num1 = this.GetChamberEntity(uid).HasValue ? 1 : 0;
    (int num2, int num3) = this.GetMagazineCountCapacity(uid, (MagazineAmmoProviderComponent) component);
    int num4 = num2;
    return (num1 + num4, num3);
  }

  private bool TryInsertChamber(EntityUid uid, EntityUid ammo)
  {
    BaseContainer container1;
    return this.Containers.TryGetContainer(uid, "gun_chamber", out container1) && container1 is ContainerSlot container2 && this.Containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) ammo, (BaseContainer) container2);
  }

  private void OnChamberAmmoCount(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    this.OnMagazineAmmoCount(uid, (MagazineAmmoProviderComponent) component, ref args);
    ++args.Capacity;
    if (!this.GetChamberEntity(uid).HasValue)
      return;
    ++args.Count;
  }

  private void OnChamberMagazineTakeAmmo(
    EntityUid uid,
    ChamberMagazineAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    bool? boltClosed = component.BoltClosed;
    bool flag = false;
    if (boltClosed.GetValueOrDefault() == flag & boltClosed.HasValue)
    {
      args.Reason = this.Loc.GetString("gun-chamber-bolt-ammo");
    }
    else
    {
      AppearanceComponent comp;
      this.TryComp<AppearanceComponent>(uid, out comp);
      if (component.AutoCycle)
      {
        EntityUid? entity1;
        if (!this.TryTakeChamberEntity(uid, out entity1))
          return;
        args.Ammo.Add((new EntityUid?(entity1.Value), this.EnsureShootable(entity1.Value)));
        EntityUid? magazineEntity = this.GetMagazineEntity(uid);
        if (magazineEntity.HasValue)
        {
          TakeAmmoEvent args1 = new TakeAmmoEvent(args.Shots, new List<(EntityUid?, IShootable)>(), args.Coordinates, args.User);
          this.RaiseLocalEvent<TakeAmmoEvent>(magazineEntity.Value, args1);
          if (args1.Ammo.Count > 0)
          {
            List<(EntityUid? Entity, IShootable Shootable)> ammo = args1.Ammo;
            EntityUid? entity2 = ammo[ammo.Count - 1].Entity;
            this.TryInsertChamber(uid, entity2.Value);
          }
          for (int index = 0; index < args1.Ammo.Count - 1; ++index)
            args.Ammo.Add(args1.Ammo[index]);
          if (args1.Ammo.Count == 0)
            this.SetBoltClosed(uid, component, false, args.User, comp);
          GetAmmoCountEvent args2 = new GetAmmoCountEvent();
          this.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref args2);
          this.FinaliseMagazineTakeAmmo(uid, (MagazineAmmoProviderComponent) component, args2.Count, args2.Capacity, args.User, comp);
        }
        else
          this.Appearance.SetData(uid, (Enum) AmmoVisuals.MagLoaded, (object) false, comp);
      }
      else
      {
        BaseContainer container;
        if (!this.Containers.TryGetContainer(uid, "gun_chamber", out container) || !(container is ContainerSlot containerSlot) || !containerSlot.ContainedEntity.HasValue)
          return;
        EntityUid? containedEntity = containerSlot.ContainedEntity;
        args.Ammo.Add((new EntityUid?(containedEntity.Value), this.EnsureShootable(containedEntity.Value)));
      }
    }
  }

  private void InitializeClothing()
  {
    this.SubscribeLocalEvent<ClothingSlotAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ClothingSlotAmmoProviderComponent, TakeAmmoEvent>(this.OnClothingTakeAmmo));
    this.SubscribeLocalEvent<ClothingSlotAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<ClothingSlotAmmoProviderComponent, GetAmmoCountEvent>(this.OnClothingAmmoCount));
  }

  private void OnClothingTakeAmmo(
    EntityUid uid,
    ClothingSlotAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    GetConnectedContainerEvent args1 = new GetConnectedContainerEvent();
    this.RaiseLocalEvent<GetConnectedContainerEvent>(uid, ref args1);
    if (!args1.ContainerEntity.HasValue)
      return;
    this.RaiseLocalEvent<TakeAmmoEvent>(args1.ContainerEntity.Value, args);
  }

  private void OnClothingAmmoCount(
    EntityUid uid,
    ClothingSlotAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    GetConnectedContainerEvent args1 = new GetConnectedContainerEvent();
    this.RaiseLocalEvent<GetConnectedContainerEvent>(uid, ref args1);
    if (!args1.ContainerEntity.HasValue)
      return;
    this.RaiseLocalEvent<GetAmmoCountEvent>(args1.ContainerEntity.Value, ref args);
  }

  private void InitializeContainer()
  {
    this.SubscribeLocalEvent<ContainerAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<ContainerAmmoProviderComponent, TakeAmmoEvent>(this.OnContainerTakeAmmo));
    this.SubscribeLocalEvent<ContainerAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<ContainerAmmoProviderComponent, GetAmmoCountEvent>(this.OnContainerAmmoCount));
  }

  private void OnContainerTakeAmmo(
    EntityUid uid,
    ContainerAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    ContainerAmmoProviderComponent providerComponent = component;
    providerComponent.ProviderUid.GetValueOrDefault();
    if (!providerComponent.ProviderUid.HasValue)
    {
      EntityUid entityUid = uid;
      providerComponent.ProviderUid = new EntityUid?(entityUid);
    }
    BaseContainer container;
    if (!this.Containers.TryGetContainer(component.ProviderUid.Value, component.Container, out container))
      return;
    for (int index = 0; index < args.Shots && container.ContainedEntities.Any<EntityUid>(); ++index)
    {
      EntityUid containedEntity = container.ContainedEntities[0];
      if (this._netManager.IsServer)
        this.Containers.Remove((Entity<TransformComponent, MetaDataComponent>) containedEntity, container);
      args.Ammo.Add((new EntityUid?(containedEntity), this.EnsureShootable(containedEntity)));
    }
  }

  private void OnContainerAmmoCount(
    EntityUid uid,
    ContainerAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    ContainerAmmoProviderComponent providerComponent = component;
    providerComponent.ProviderUid.GetValueOrDefault();
    if (!providerComponent.ProviderUid.HasValue)
    {
      EntityUid entityUid = uid;
      providerComponent.ProviderUid = new EntityUid?(entityUid);
    }
    BaseContainer container;
    if (!this.Containers.TryGetContainer(component.ProviderUid.Value, component.Container, out container))
    {
      args.Capacity = 0;
      args.Count = 0;
    }
    else
    {
      args.Capacity = int.MaxValue;
      args.Count = container.ContainedEntities.Count;
    }
  }

  public bool GunPrediction { get; private set; }

  public override void Initialize()
  {
    this.SubscribeAllEvent<RequestStopShootEvent>(new EntitySessionEventHandler<RequestStopShootEvent>(this.OnStopShootRequest));
    this.SubscribeLocalEvent<GunComponent, MeleeHitEvent>(new ComponentEventHandler<GunComponent, MeleeHitEvent>(this.OnGunMelee));
    this.InitializeBallistic();
    this.InitializeBattery();
    this.InitializeCartridge();
    this.InitializeChamberMagazine();
    this.InitializeMagazine();
    this.InitializeRevolver();
    this.InitializeBasicEntity();
    this.InitializeClothing();
    this.InitializeContainer();
    this.InitializeSolution();
    this.SubscribeLocalEvent<GunComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<GunComponent, GetVerbsEvent<AlternativeVerb>>(this.OnAltVerb));
    this.SubscribeLocalEvent<GunComponent, ExaminedEvent>(new ComponentEventHandler<GunComponent, ExaminedEvent>(this.OnExamine));
    this.SubscribeLocalEvent<GunComponent, SharedGunSystem.CycleModeEvent>(new ComponentEventHandler<GunComponent, SharedGunSystem.CycleModeEvent>(this.OnCycleMode));
    this.SubscribeLocalEvent<GunComponent, HandSelectedEvent>(new ComponentEventHandler<GunComponent, HandSelectedEvent>(this.OnGunSelected));
    this.SubscribeLocalEvent<GunComponent, MapInitEvent>(new EntityEventRefHandler<GunComponent, MapInitEvent>(this.OnMapInit));
    this.Subs.CVar<bool>(this._config, RMCCVars.RMCGunPrediction, (Action<bool>) (v => this.GunPrediction = v), true);
  }

  private void OnMapInit(Entity<GunComponent> gun, ref MapInitEvent args)
  {
    this.RefreshModifiers((Entity<GunComponent>) ((EntityUid) gun, (GunComponent) gun));
  }

  private void OnGunMelee(EntityUid uid, GunComponent component, MeleeHitEvent args)
  {
    MeleeWeaponComponent comp;
    if (!this.TryComp<MeleeWeaponComponent>(uid, out comp) || !component.MeleeCooldownOnShoot || !(comp.NextAttack > component.NextFire))
      return;
    component.NextFire = comp.NextAttack;
    this.DirtyField<GunComponent>(uid, component, "NextFire");
  }

  private void OnStopShootRequest(RequestStopShootEvent ev, EntitySessionEventArgs args)
  {
    EntityUid entity = this.GetEntity(ev.Gun);
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    GunComponent comp;
    GunComponent gunComp;
    if (!this.TryComp<GunComponent>(entity, out comp) || !this.TryGetGun(valueOrDefault, out EntityUid _, out gunComp) || gunComp != comp)
      return;
    this.StopShooting(entity, comp);
  }

  public bool CanShoot(GunComponent component) => !(component.NextFire > this.Timing.CurTime);

  public EntityUid? SwapTarget(Entity<GunComponent> gun, EntityUid? target)
  {
    EntityUid? target1 = gun.Comp.Target;
    gun.Comp.Target = target;
    return target1;
  }

  public bool TryGetGun(EntityUid entity, out EntityUid gunEntity, [NotNullWhen(true)] out GunComponent? gunComp)
  {
    VehiclePortGunOperatorComponent comp1;
    EntityUid? nullable;
    if (this.TryComp<VehiclePortGunOperatorComponent>(entity, out comp1))
    {
      EntityUid? gun = comp1.Gun;
      if (gun.HasValue)
      {
        EntityUid valueOrDefault = gun.GetValueOrDefault();
        VehiclePortGunComponent comp2;
        if (this.TryComp<VehiclePortGunComponent>(valueOrDefault, out comp2))
        {
          nullable = comp2.Operator;
          EntityUid entityUid = entity;
          GunComponent comp3;
          if ((nullable.HasValue ? (nullable.GetValueOrDefault() == entityUid ? 1 : 0) : 0) != 0 && this.TryComp<GunComponent>(valueOrDefault, out comp3))
          {
            gunEntity = valueOrDefault;
            gunComp = comp3;
            return true;
          }
        }
      }
    }
    VehicleWeaponsOperatorComponent comp4;
    if (this.TryComp<VehicleWeaponsOperatorComponent>(entity, out comp4))
    {
      nullable = comp4.Vehicle;
      EntityUid weapon;
      GunComponent comp5;
      if (nullable.HasValue && this._rmcVehicleWeapons.TryGetSelectedWeaponForOperator(nullable.GetValueOrDefault(), entity, out weapon) && this.TryComp<GunComponent>(weapon, out comp5))
      {
        gunEntity = weapon;
        gunComp = comp5;
        return true;
      }
    }
    if (this._attachableHolder.TryGetInhandSupercedingGun(entity, out gunEntity, out gunComp))
      return true;
    gunEntity = new EntityUid();
    gunComp = (GunComponent) null;
    nullable = this._hands.GetActiveItem((Entity<HandsComponent>) entity);
    GunComponent gunComponent;
    if (nullable.HasValue)
    {
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      if (this.TryComp<GunComponent>(valueOrDefault, out gunComponent))
      {
        gunEntity = valueOrDefault;
        gunComp = gunComponent;
        return true;
      }
    }
    if (this.TryComp<GunComponent>(entity, out gunComponent))
    {
      gunEntity = entity;
      gunComp = gunComponent;
      return true;
    }
    EntityUid? controlledWeapon;
    if (!this._rmcSharedWeaponController.TryGetControlledWeapon(entity, out controlledWeapon, out gunComponent))
      return false;
    gunEntity = controlledWeapon.Value;
    gunComp = gunComponent;
    return true;
  }

  public void StopShooting(EntityUid uid, GunComponent gun)
  {
    if (gun.ShotCounter == 0)
      return;
    gun.ShotCounter = 0;
    gun.ShootCoordinates = new EntityCoordinates?();
    gun.Target = new EntityUid?();
    this.DirtyField<GunComponent>(uid, gun, "ShotCounter");
  }

  public List<EntityUid>? AttemptShoot(
    Entity<GunComponent> ent,
    EntityUid user,
    EntityCoordinates coordinates)
  {
    ent.Comp.ShootCoordinates = new EntityCoordinates?(coordinates);
    List<EntityUid> entityUidList = this.AttemptShoot(user, (EntityUid) ent, (GunComponent) ent);
    ent.Comp.ShotCounter = 0;
    this.DirtyField<GunComponent>(ent.Owner, ent.Comp, "ShotCounter");
    return entityUidList;
  }

  public void AttemptShoot(
    EntityUid user,
    EntityUid gunUid,
    GunComponent gun,
    EntityCoordinates toCoordinates)
  {
    gun.ShootCoordinates = new EntityCoordinates?(toCoordinates);
    this.AttemptShoot(user, gunUid, gun);
    gun.ShotCounter = 0;
    this.DirtyField<GunComponent>(gunUid, gun, "ShotCounter");
  }

  public void AttemptShoot(EntityUid gunUid, GunComponent gun)
  {
    EntityCoordinates entityCoordinates = new EntityCoordinates(gunUid, gun.DefaultDirection);
    gun.ShootCoordinates = new EntityCoordinates?(entityCoordinates);
    this.AttemptShoot(gunUid, gunUid, gun);
    gun.ShotCounter = 0;
  }

  public List<EntityUid>? AttemptShoot(
    EntityUid user,
    EntityUid gunUid,
    GunComponent gun,
    List<int>? predictedProjectiles = null,
    ICommonSession? userSession = null)
  {
    if ((double) gun.FireRateModified <= 0.0 || !this._actionBlockerSystem.CanAttack(user))
      return (List<EntityUid>) null;
    EntityCoordinates? ToCoordinates = gun.ShootCoordinates;
    if (!ToCoordinates.HasValue)
      return (List<EntityUid>) null;
    if (!this.TransformSystem.IsValid(ToCoordinates.Value))
      return (List<EntityUid>) null;
    TimeSpan curTime = this.Timing.CurTime;
    ShotAttemptedEvent args1 = new ShotAttemptedEvent()
    {
      User = user,
      Used = (Entity<GunComponent>) (gunUid, gun)
    };
    this.RaiseLocalEvent<ShotAttemptedEvent>(gunUid, ref args1);
    if (args1.Cancelled)
      return (List<EntityUid>) null;
    this.RaiseLocalEvent<ShotAttemptedEvent>(user, ref args1);
    if (args1.Cancelled)
      return (List<EntityUid>) null;
    if (gun.NextFire > curTime)
      return (List<EntityUid>) null;
    TimeSpan timeSpan = TimeSpan.FromSeconds(1.0 / (double) gun.FireRateModified);
    if (gun.SelectedMode == SelectiveFire.Burst || gun.BurstActivated)
      timeSpan = TimeSpan.FromSeconds(1.0 / (double) gun.BurstFireRate);
    if (gun.NextFire < curTime - timeSpan || gun.ShotCounter == 0 && gun.NextFire < curTime)
      gun.NextFire = curTime;
    int num = 0;
    TimeSpan nextFire = gun.NextFire;
    while (gun.NextFire <= curTime)
    {
      gun.NextFire += timeSpan;
      ++num;
    }
    this.DirtyField<GunComponent>(gunUid, gun, "NextFire");
    if (!gun.BurstActivated)
    {
      switch (gun.SelectedMode)
      {
        case SelectiveFire.SemiAuto:
          num = Math.Min(num, 1 - gun.ShotCounter);
          break;
        case SelectiveFire.Burst:
          num = Math.Min(num, gun.ShotsPerBurstModified - gun.ShotCounter);
          break;
        case SelectiveFire.FullAuto:
          break;
        default:
          throw new ArgumentOutOfRangeException($"No implemented shooting behavior for {gun.SelectedMode}!");
      }
    }
    else
      num = Math.Min(num, gun.ShotsPerBurstModified - gun.ShotCounter);
    EntityCoordinates entityCoordinates = this.Transform(this.HasComp<GunUseGunOriginComponent>(gunUid) ? gunUid : user).Coordinates;
    BeforeAttemptShootEvent args2 = new BeforeAttemptShootEvent(entityCoordinates, gun.ShootOriginOffset);
    this.RaiseLocalEvent<BeforeAttemptShootEvent>(user, ref args2);
    if (args2.Handled)
      entityCoordinates = args2.Origin;
    AttemptShootEvent args3 = new AttemptShootEvent(user, (string) null, entityCoordinates, ToCoordinates);
    this.RaiseLocalEvent<AttemptShootEvent>(gunUid, ref args3);
    if (args3.Cancelled)
    {
      if (args3.Message != null)
        this.PopupSystem.PopupClient(args3.Message, gunUid, new EntityUid?(user));
      gun.BurstActivated = false;
      gun.BurstShotsCount = 0;
      gun.NextFire = args3.ResetCooldown ? curTime : TimeSpan.FromSeconds(Math.Max(nextFire.TotalSeconds + 0.5, gun.NextFire.TotalSeconds));
      return (List<EntityUid>) null;
    }
    EntityCoordinates fromCoordinates = args3.FromCoordinates;
    ToCoordinates = args3.ToCoordinates;
    if (!ToCoordinates.HasValue)
      return (List<EntityUid>) null;
    if (!this.TransformSystem.IsValid(fromCoordinates) || !this.TransformSystem.IsValid(ToCoordinates.Value))
      return (List<EntityUid>) null;
    TakeAmmoEvent args4 = new TakeAmmoEvent(num, new List<(EntityUid?, IShootable)>(), fromCoordinates, new EntityUid?(user));
    if (num > 0)
      this.RaiseLocalEvent<TakeAmmoEvent>(gunUid, args4);
    this.UpdateAmmoCount(gunUid);
    gun.ShotCounter += num;
    this.DirtyField<GunComponent>(gunUid, gun, "ShotCounter");
    if (args4.Ammo.Count <= 0)
    {
      OnEmptyGunShotEvent args5 = new OnEmptyGunShotEvent();
      this.RaiseLocalEvent<OnEmptyGunShotEvent>(gunUid, ref args5);
      gun.BurstActivated = false;
      gun.BurstShotsCount = 0;
      gun.NextFire += TimeSpan.FromSeconds((double) gun.BurstCooldown);
      if (num <= 0)
        return (List<EntityUid>) null;
      this.PopupSystem.PopupCursor(args4.Reason ?? this.Loc.GetString("gun-magazine-fired-empty"));
      gun.NextFire = TimeSpan.FromSeconds(Math.Max(nextFire.TotalSeconds + 0.5, gun.NextFire.TotalSeconds));
      this.Audio.PlayPredicted(gun.SoundEmpty, gunUid, new EntityUid?(user));
      return (List<EntityUid>) null;
    }
    if (gun.SelectedMode == SelectiveFire.Burst)
      gun.BurstActivated = true;
    if (gun.BurstActivated)
    {
      gun.BurstShotsCount += num;
      if (gun.BurstShotsCount >= gun.ShotsPerBurstModified)
      {
        gun.NextFire += TimeSpan.FromSeconds((double) gun.BurstCooldown);
        gun.BurstActivated = false;
        gun.BurstShotsCount = 0;
      }
    }
    List<EntityUid> entityUidList = (List<EntityUid>) null;
    bool userImpulse = false;
    if (this.Timing.IsFirstTimePredicted)
      entityUidList = this.Shoot(gunUid, gun, args4.Ammo, fromCoordinates, ToCoordinates.Value, out userImpulse, new EntityUid?(user), args3.ThrowItems, predictedProjectiles, userSession);
    GunShotEvent args6 = new GunShotEvent(user, args4.Ammo, fromCoordinates, ToCoordinates.Value);
    this.RaiseLocalEvent<GunShotEvent>(gunUid, ref args6);
    PhysicsComponent comp;
    if (userImpulse && this.TryComp<PhysicsComponent>(user, out comp) && this._gravity.IsWeightless(user, comp))
      this.CauseImpulse(fromCoordinates, ToCoordinates.Value, user, comp);
    this.DirtyField<GunComponent>(gunUid, gun, "BurstActivated");
    this.Dirty(gunUid, (IComponent) gun);
    foreach ((EntityUid? nullable, IShootable _) in args4.Ammo)
    {
      if (nullable.HasValue && this.IsClientSide(nullable.Value) && (this.HasComp<GunIgnorePredictionComponent>(gunUid) || entityUidList == null || !entityUidList.Contains(nullable.Value)))
        this.Del(nullable);
    }
    return entityUidList;
  }

  public void Shoot(
    EntityUid gunUid,
    GunComponent gun,
    EntityUid ammo,
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates,
    out bool userImpulse,
    EntityUid? user = null,
    bool throwItems = false)
  {
    IShootable shootable = this.EnsureShootable(ammo);
    EntityUid gunUid1 = gunUid;
    GunComponent gun1 = gun;
    List<(EntityUid?, IShootable)> ammo1 = new List<(EntityUid?, IShootable)>(1);
    ammo1.Add((new EntityUid?(ammo), shootable));
    EntityCoordinates fromCoordinates1 = fromCoordinates;
    EntityCoordinates toCoordinates1 = toCoordinates;
    ref bool local = ref userImpulse;
    EntityUid? user1 = user;
    int num = throwItems ? 1 : 0;
    this.Shoot(gunUid1, gun1, ammo1, fromCoordinates1, toCoordinates1, out local, user1, num != 0);
  }

  public List<EntityUid>? Shoot(
    EntityUid gunUid,
    GunComponent gun,
    List<(EntityUid? Entity, IShootable Shootable)> ammo,
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates,
    out bool userImpulse,
    EntityUid? user = null,
    bool throwItems = false,
    List<int>? predictedProjectiles = null,
    ICommonSession? userSession = null)
  {
    userImpulse = true;
    if (user.HasValue)
    {
      SelfBeforeGunShotEvent args = new SelfBeforeGunShotEvent(user.Value, (Entity<GunComponent>) (gunUid, gun), ammo);
      this.RaiseLocalEvent<SelfBeforeGunShotEvent>(user.Value, args);
      if (args.Cancelled)
      {
        userImpulse = false;
        return (List<EntityUid>) null;
      }
    }
    MapCoordinates mapCoordinates1 = this.TransformSystem.ToMapCoordinates(fromCoordinates);
    Vector2 mapDirection = this.TransformSystem.ToMapCoordinates(toCoordinates).Position - mapCoordinates1.Position;
    Angle mapAngle = DirectionExtensions.ToAngle(mapDirection);
    Angle recoilAngle = this.GetRecoilAngle(this.Timing.CurTime, gunUid, gun, DirectionExtensions.ToAngle(mapDirection));
    EntityUid uid1;
    EntityCoordinates fromEnt = this.MapManager.TryFindGridAt(mapCoordinates1, out uid1, out MapGridComponent _) ? this.TransformSystem.WithEntityId(fromCoordinates, uid1) : new EntityCoordinates(this.MapSystem.GetMap(mapCoordinates1.MapId), mapCoordinates1.Position);
    mapDirection = mapCoordinates1.Position + ((Angle) ref recoilAngle).ToVec() * mapDirection.Length() - mapCoordinates1.Position;
    Vector2 gunVelocity = this.Physics.GetMapLinearVelocity(fromEnt);
    List<EntityUid> shotProjectiles = new List<EntityUid>(ammo.Count);
    foreach ((EntityUid? nullable1, IShootable Shootable) in ammo)
    {
      if (throwItems && nullable1.HasValue)
      {
        this.Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
        this.ShootOrThrow(nullable1.Value, mapDirection, gunVelocity, gun, gunUid, user);
      }
      else
      {
        switch (Shootable)
        {
          case CartridgeAmmoComponent cartridgeAmmoComponent:
            if (!cartridgeAmmoComponent.Spent)
            {
              if (this._netManager.IsServer || this.GunPrediction)
              {
                EntityUid uid = this.Spawn((string) cartridgeAmmoComponent.Prototype, fromEnt);
                CreateAndFireProjectiles(uid, (AmmoComponent) cartridgeAmmoComponent);
                if (this._netManager.IsClient && this.HasComp<GunIgnorePredictionComponent>(gunUid))
                {
                  predictedProjectiles?.RemoveAll((Predicate<int>) (i => i == uid.Id));
                  this.QueueDel(new EntityUid?(uid));
                }
                this.RaiseLocalEvent<AmmoShotEvent>(nullable1.Value, new AmmoShotEvent()
                {
                  FiredProjectiles = shotProjectiles
                });
                this.SetCartridgeSpent(nullable1.Value, cartridgeAmmoComponent, true);
                if (cartridgeAmmoComponent.DeleteOnSpawn && (this._netManager.IsServer || this.IsClientSide(nullable1.Value)))
                  this.Del(new EntityUid?(nullable1.Value));
              }
              else
              {
                this.MuzzleFlash(gunUid, (AmmoComponent) cartridgeAmmoComponent, DirectionExtensions.ToAngle(mapDirection), user);
                this.PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
              }
            }
            else
            {
              userImpulse = false;
              this.Audio.PlayPredicted(gun.SoundEmpty, gunUid, user);
            }
            this.Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
            if (!cartridgeAmmoComponent.DeleteOnSpawn && !this.Containers.IsEntityInContainer(nullable1.Value))
              this.EjectCartridge(nullable1.Value, new Angle?(recoilAngle));
            if (this.IsClientSide(nullable1.Value))
            {
              this.Del(new EntityUid?(nullable1.Value));
              continue;
            }
            this.Dirty(nullable1.Value, (IComponent) cartridgeAmmoComponent);
            continue;
          case AmmoComponent ammoComponent:
            if (this._netManager.IsServer || this.GunPrediction)
            {
              CreateAndFireProjectiles(nullable1.Value, ammoComponent);
            }
            else
            {
              this.MuzzleFlash(gunUid, ammoComponent, DirectionExtensions.ToAngle(mapDirection), user);
              this.PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
            }
            this.Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
            if (this._netManager.IsClient)
              this.RemoveShootable(nullable1.Value);
            MarkPredicted(nullable1.Value, 0);
            continue;
          case HitscanPrototype hitscan:
            EntityUid? nullable2 = new EntityUid?();
            MapCoordinates mapCoordinates2 = mapCoordinates1;
            EntityCoordinates entityCoordinates = fromCoordinates;
            Vector2 vector2 = Vector2Helpers.Normalized(mapDirection);
            EntityUid uid2 = user ?? gunUid;
            if (hitscan.Reflective != ReflectType.None)
            {
              for (int index = 0; index < 3; ++index)
              {
                CollisionRay ray = new CollisionRay(mapCoordinates2.Position, vector2, hitscan.CollisionMask);
                List<RayCastResults> list = this.Physics.IntersectRay(mapCoordinates2.MapId, ray, hitscan.MaxLength, new EntityUid?(uid2), false).ToList<RayCastResults>();
                if (list.Any<RayCastResults>())
                {
                  RayCastResults rayCastResults1 = list[0];
                  if (!this.Containers.IsEntityOrParentInContainer(uid2))
                  {
                    foreach (RayCastResults rayCastResults2 in list)
                    {
                      EntityUid hitEntity = rayCastResults2.HitEntity;
                      EntityUid? target = gun.Target;
                      if ((target.HasValue ? (hitEntity != target.GetValueOrDefault() ? 1 : 0) : 1) != 0)
                      {
                        RequireProjectileTargetComponent projectileTargetComponent = this.CompOrNull<RequireProjectileTargetComponent>(rayCastResults2.HitEntity);
                        if ((projectileTargetComponent != null ? (projectileTargetComponent.Active ? 1 : 0) : 0) != 0)
                          continue;
                      }
                      rayCastResults1 = rayCastResults2;
                      break;
                    }
                  }
                  EntityUid hitEntity1 = rayCastResults1.HitEntity;
                  nullable2 = new EntityUid?(hitEntity1);
                  this.FireEffects(entityCoordinates, rayCastResults1.Distance, DirectionExtensions.ToAngle(Vector2Helpers.Normalized(vector2)), hitscan, new EntityUid?(hitEntity1));
                  HitScanReflectAttemptEvent args = new HitScanReflectAttemptEvent(user, gunUid, hitscan.Reflective, vector2, false);
                  this.RaiseLocalEvent<HitScanReflectAttemptEvent>(hitEntity1, ref args);
                  if (args.Reflected)
                  {
                    entityCoordinates = this.Transform(hitEntity1).Coordinates;
                    mapCoordinates2 = this.TransformSystem.ToMapCoordinates(entityCoordinates);
                    vector2 = args.Direction;
                    uid2 = hitEntity1;
                  }
                  else
                    break;
                }
                else
                  break;
              }
            }
            if (nullable2.HasValue)
            {
              EntityUid entityUid = nullable2.Value;
              if ((double) hitscan.StaminaDamage > 0.0)
                this._stamina.TakeStaminaDamage(entityUid, hitscan.StaminaDamage, source: user);
              DamageSpecifier modifiedDamage = hitscan.Damage;
              EntityStringRepresentation prettyString = this.ToPrettyString((Entity<MetaDataComponent>) entityUid);
              if (modifiedDamage != null)
                modifiedDamage = this.Damageable.TryChangeDamage(new EntityUid?(entityUid), modifiedDamage * this.Damageable.UniversalHitscanDamageModifier, origin: user, tool: nullable1);
              if (modifiedDamage != null)
              {
                if (!this.Deleted(entityUid))
                {
                  Filter filter1 = Filter.Pvs(entityUid, entityManager: (IEntityManager) this.EntityManager);
                  if (this._netManager.IsServer && this.GunPrediction && userSession != null)
                    filter1.RemovePlayer(userSession);
                  if (modifiedDamage.AnyPositive())
                  {
                    SharedColorFlashEffectSystem color = this._color;
                    Color red = Color.Red;
                    List<EntityUid> entities = new List<EntityUid>();
                    entities.Add(entityUid);
                    Filter filter2 = Filter.Pvs(entityUid, entityManager: (IEntityManager) this.EntityManager);
                    color.RaiseEffect(red, entities, filter2);
                  }
                  this.PlayImpactSound(entityUid, modifiedDamage, hitscan.Sound, hitscan.ForceSound);
                }
                if (user.HasValue)
                {
                  ISharedAdminLogManager logs = this.Logs;
                  LogStringHandler logStringHandler = new LogStringHandler(37, 3);
                  logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) user.Value), nameof (user), "ToPrettyString(user.Value)");
                  logStringHandler.AppendLiteral(" hit ");
                  logStringHandler.AppendFormatted<EntityStringRepresentation>(prettyString, "target", "hitName");
                  logStringHandler.AppendLiteral(" using hitscan and dealt ");
                  logStringHandler.AppendFormatted<FixedPoint2>(modifiedDamage.GetTotal(), "damage", "dmg.GetTotal()");
                  logStringHandler.AppendLiteral(" damage");
                  ref LogStringHandler local = ref logStringHandler;
                  logs.Add(LogType.HitScanHit, ref local);
                }
                else
                {
                  ISharedAdminLogManager logs = this.Logs;
                  LogStringHandler logStringHandler = new LogStringHandler(31 /*0x1F*/, 2);
                  logStringHandler.AppendFormatted<EntityStringRepresentation>(prettyString, "target", "hitName");
                  logStringHandler.AppendLiteral(" hit by hitscan dealing ");
                  logStringHandler.AppendFormatted<FixedPoint2>(modifiedDamage.GetTotal(), "damage", "dmg.GetTotal()");
                  logStringHandler.AppendLiteral(" damage");
                  ref LogStringHandler local = ref logStringHandler;
                  logs.Add(LogType.HitScanHit, ref local);
                }
              }
            }
            else
              this.FireEffects(entityCoordinates, hitscan.MaxLength, DirectionExtensions.ToAngle(vector2), hitscan);
            this.PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
            this.Recoil(user, mapDirection, gun.CameraRecoilScalarModified);
            continue;
          case RMCFlamerAmmoProviderComponent providerComponent1:
            if (nullable1.HasValue)
            {
              this._flamer.ShootFlamer((Entity<RMCFlamerAmmoProviderComponent>) (nullable1.Value, providerComponent1), (Entity<GunComponent>) (gunUid, gun), user, fromCoordinates, toCoordinates);
              continue;
            }
            continue;
          case RMCSprayAmmoProviderComponent providerComponent2:
            if (nullable1.HasValue)
            {
              this._flamer.ShootSpray((Entity<RMCSprayAmmoProviderComponent>) (nullable1.Value, providerComponent2), (Entity<GunComponent>) (gunUid, gun), user, fromCoordinates, toCoordinates);
              continue;
            }
            continue;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
    this.RaiseLocalEvent<AmmoShotEvent>(gunUid, new AmmoShotEvent()
    {
      FiredProjectiles = shotProjectiles
    });
    ISharedAdminLogManager logs1 = this.Logs;
    LogStringHandler logStringHandler1 = new LogStringHandler(36, 4);
    logStringHandler1.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(user), "ToPrettyString(user)");
    logStringHandler1.AppendLiteral(" shot ");
    logStringHandler1.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) gunUid), "ToPrettyString(gunUid)");
    logStringHandler1.AppendLiteral(" with ");
    logStringHandler1.AppendFormatted<int>(shotProjectiles.Count, "shotProjectiles.Count");
    logStringHandler1.AppendLiteral(" projectiles aiming at ");
    logStringHandler1.AppendFormatted<MapCoordinates>(this.TransformSystem.ToMapCoordinates(toCoordinates), "TransformSystem.ToMapCoordinates(toCoordinates)");
    logStringHandler1.AppendLiteral(".");
    ref LogStringHandler local1 = ref logStringHandler1;
    logs1.Add(LogType.RMCGunShot, LogImpact.Low, ref local1);
    return shotProjectiles;

    void MarkPredicted(EntityUid projectile, int index)
    {
      int num;
      if (!this.GunPrediction || predictedProjectiles == null || userSession == null || !predictedProjectiles.TryGetValue<int>(index, out num))
        return;
      PredictedProjectileServerComponent component = new PredictedProjectileServerComponent()
      {
        Shooter = userSession,
        ClientId = num,
        ClientEnt = user
      };
      this.AddComp<PredictedProjectileServerComponent>(projectile, component, true);
      this.Dirty(projectile, (IComponent) component);
    }

    void CreateAndFireProjectiles(EntityUid ammoEnt, AmmoComponent ammoComp)
    {
      if (predictedProjectiles == null)
        predictedProjectiles = new List<int>();
      MarkPredicted(ammoEnt, 0);
      ProjectileSpreadComponent comp;
      if (this.TryComp<ProjectileSpreadComponent>(ammoEnt, out comp))
      {
        GunGetAmmoSpreadEvent args = new GunGetAmmoSpreadEvent(comp.Spread);
        this.RaiseLocalEvent<GunGetAmmoSpreadEvent>(gunUid, ref args);
        Angle[] angleArray = this.LinearSpread(Angle.op_Subtraction(mapAngle, Angle.op_Implicit(Angle.op_Implicit(args.Spread) / 2.0)), Angle.op_Addition(mapAngle, Angle.op_Implicit(Angle.op_Implicit(args.Spread) / 2.0)), comp.Count);
        this.ShootOrThrow(ammoEnt, ((Angle) ref angleArray[0]).ToVec(), gunVelocity, gun, gunUid, user);
        shotProjectiles.Add(ammoEnt);
        for (int index = 1; index < comp.Count; ++index)
        {
          EntityUid entityUid = this.Spawn((string) comp.Proto, fromEnt);
          this.ShootOrThrow(entityUid, ((Angle) ref angleArray[index]).ToVec(), gunVelocity, gun, gunUid, user);
          shotProjectiles.Add(entityUid);
          MarkPredicted(entityUid, index);
        }
      }
      else
      {
        this.ShootOrThrow(ammoEnt, mapDirection, gunVelocity, gun, gunUid, user);
        shotProjectiles.Add(ammoEnt);
      }
      this.MuzzleFlash(gunUid, ammoComp, DirectionExtensions.ToAngle(mapDirection), user);
      this.PlayGunshotAudio(gunUid, gun, user, fromCoordinates, toCoordinates);
    }
  }

  private void PlayGunshotAudio(
    EntityUid gunUid,
    GunComponent gun,
    EntityUid? user,
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates)
  {
    SpatialGunshotComponent comp1;
    if (this._netManager.IsClient || !this.TryComp<SpatialGunshotComponent>(gunUid, out comp1))
    {
      this.Audio.PlayPredicted(gun.SoundGunshotModified, gunUid, user);
    }
    else
    {
      if (gun.SoundGunshotModified == null)
        return;
      SoundSpecifier soundGunshotModified = gun.SoundGunshotModified;
      SoundSpecifier soundSpecifier = comp1.FarSound;
      float num1 = comp1.AudioRange;
      float x = comp1.NearAudioRange;
      float num2 = comp1.NearRange;
      float num3 = comp1.ConeAngle;
      float volume = comp1.NearVolume;
      bool flag = false;
      PubgWeaponModulesComponent comp2;
      if (this.TryComp<PubgWeaponModulesComponent>(gunUid, out comp2))
      {
        PubgSpatialGunshotModifiers gunshotModifiers = this._pubgWeaponModules.GetSpatialGunshotModifiers(gunUid, comp2);
        if (gunshotModifiers.FarSoundOverride != null)
          soundSpecifier = gunshotModifiers.FarSoundOverride;
        if (gunshotModifiers.DisableFarSound)
        {
          soundSpecifier = (SoundSpecifier) null;
          flag = true;
        }
        num1 = MathF.Max(1f, comp1.AudioRange * gunshotModifiers.AudioRangeMultiplier);
        x = Math.Clamp(comp1.NearAudioRange * gunshotModifiers.AudioRangeMultiplier, 1f, num1);
        num2 = Math.Clamp(comp1.NearRange * gunshotModifiers.NearRangeMultiplier, 1f, num1);
        num3 = Math.Clamp(comp1.ConeAngle * gunshotModifiers.ConeAngleMultiplier, 1f, 180f);
        volume = Math.Clamp(volume * gunshotModifiers.NearVolumeMultiplier, -12f, 12f);
      }
      float dist = MathF.Min(x, num1);
      if (!fromCoordinates.IsValid((IEntityManager) this.EntityManager) || !toCoordinates.IsValid((IEntityManager) this.EntityManager))
        return;
      MapCoordinates mapCoordinates1 = this.TransformSystem.ToMapCoordinates(fromCoordinates);
      MapCoordinates mapCoordinates2 = this.TransformSystem.ToMapCoordinates(toCoordinates);
      if (mapCoordinates1.MapId == MapId.Nullspace || mapCoordinates2.MapId == MapId.Nullspace)
        return;
      SoundSpecifier sound = soundSpecifier ?? (flag ? (SoundSpecifier) null : soundGunshotModified);
      Vector2 vector2_1 = mapCoordinates2.Position - mapCoordinates1.Position;
      Vector2 vector2_2 = (double) vector2_1.LengthSquared() > 9.9999999747524271E-07 ? Vector2Helpers.Normalized(vector2_1) : Vector2.Zero;
      Filter playerFilter1 = Filter.Empty();
      Filter playerFilter2 = Filter.Empty();
      foreach (ICommonSession recipient in Filter.Empty().AddInRange(mapCoordinates1, num1, this._playerManager, (IEntityManager) this.EntityManager).Recipients)
      {
        EntityUid? attachedEntity = recipient.AttachedEntity;
        if (attachedEntity.HasValue)
        {
          attachedEntity = recipient.AttachedEntity;
          EntityUid? nullable = user;
          if ((attachedEntity.HasValue == nullable.HasValue ? (attachedEntity.HasValue ? (attachedEntity.GetValueOrDefault() == nullable.GetValueOrDefault() ? 1 : 0) : 1) : 0) == 0)
          {
            MapCoordinates mapCoordinates3 = this.TransformSystem.GetMapCoordinates(recipient.AttachedEntity.Value);
            float num4 = (mapCoordinates3.Position - mapCoordinates1.Position).Length();
            if ((double) num4 <= (double) num2)
            {
              playerFilter1.AddPlayer(recipient);
            }
            else
            {
              Vector2 vector2_3 = Vector2Helpers.Normalized(mapCoordinates3.Position - mapCoordinates1.Position);
              float num5 = MathF.Acos(MathHelper.Clamp(Vector2.Dot(vector2_2, vector2_3), -1f, 1f)) * 57.2957764f;
              if (vector2_2 != Vector2.Zero && (double) num5 <= (double) num3 && (double) num4 <= (double) dist)
                playerFilter1.AddPlayer(recipient);
              else if (sound != null)
                playerFilter2.AddPlayer(recipient);
            }
          }
        }
      }
      if (playerFilter1.Count > 0)
      {
        AudioParams audioParams = AudioParams.Default.WithMaxDistance(dist).WithVolume(volume);
        this.Audio.PlayStatic(soundGunshotModified, playerFilter1, fromCoordinates, true, new AudioParams?(audioParams));
      }
      if (sound != null && playerFilter2.Count > 0)
      {
        AudioParams audioParams = AudioParams.Default.WithMaxDistance(num1);
        this.Audio.PlayStatic(sound, playerFilter2, fromCoordinates, true, new AudioParams?(audioParams));
      }
      this.RelayGunshotIntoVehicleInteriors(sound ?? soundGunshotModified, sound != null ? AudioParams.Default.WithMaxDistance(num1) : AudioParams.Default.WithMaxDistance(num1).WithVolume(volume), fromCoordinates);
    }
  }

  private void RelayGunshotIntoVehicleInteriors(
    SoundSpecifier sound,
    AudioParams baseParams,
    EntityCoordinates sourceCoords)
  {
    if (!sourceCoords.IsValid((IEntityManager) this.EntityManager))
      return;
    MapCoordinates mapCoordinates = this.TransformSystem.ToMapCoordinates(sourceCoords);
    if (mapCoordinates.MapId == MapId.Nullspace)
      return;
    Vector2 position = mapCoordinates.Position;
    Robust.Shared.GameObjects.EntityQueryEnumerator<RMCVehicleInteriorAudioRelayComponent, VehicleEnterComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCVehicleInteriorAudioRelayComponent, VehicleEnterComponent, TransformComponent>();
    EntityUid uid;
    RMCVehicleInteriorAudioRelayComponent comp1;
    TransformComponent comp3;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out VehicleEnterComponent _, out comp3))
    {
      if (!(comp3.MapID != mapCoordinates.MapId))
      {
        Box2 box2 = this._lookup.GetWorldAABB(uid);
        box2 = ((Box2) ref box2).Enlarged(comp1.ExteriorRange);
        EntityCoordinates coordinates;
        MapId mapId;
        if (((Box2) ref box2).Contains(position, true) && this._rmcVehicle.TryGetInteriorEntryCoordinates(uid, out coordinates) && this._rmcVehicle.TryGetInteriorMapId(uid, out mapId))
        {
          foreach (ICommonSession session in this._playerManager.Sessions)
          {
            EntityCoordinates relayCoords;
            if (this.TryGetVehicleRelayCoordinates(session, uid, mapCoordinates, mapId, coordinates, comp1, out relayCoords))
            {
              AudioParams audioParams1 = baseParams.AddVolume(comp1.InteriorVolumeOffset);
              audioParams1 = audioParams1.WithMaxDistance(comp1.InteriorMaxDistance);
              AudioParams audioParams2 = audioParams1.WithReferenceDistance(comp1.InteriorReferenceDistance);
              (EntityUid Entity, AudioComponent Component)? nullable = this.Audio.PlayStatic(sound, Filter.SinglePlayer(session), relayCoords, false, new AudioParams?(audioParams2));
              if (nullable.HasValue && comp1.InteriorNoOcclusion)
              {
                nullable.Value.Component.Flags |= AudioFlags.NoOcclusion;
                this.Dirty(nullable.Value.Entity, (IComponent) nullable.Value.Component);
              }
            }
          }
        }
      }
    }
  }

  private bool TryGetVehicleRelayCoordinates(
    ICommonSession session,
    EntityUid vehicle,
    MapCoordinates mapCoords,
    MapId interiorMapId,
    EntityCoordinates interiorCoords,
    RMCVehicleInteriorAudioRelayComponent relay,
    out EntityCoordinates relayCoords)
  {
    relayCoords = EntityCoordinates.Invalid;
    EntityUid? attachedEntity = session.AttachedEntity;
    if (attachedEntity.HasValue)
    {
      EntityUid valueOrDefault1 = attachedEntity.GetValueOrDefault();
      RMCVehicleInteriorOccupantComponent comp1;
      if (!this.TerminatingOrDeleted(valueOrDefault1) && this.TryComp<RMCVehicleInteriorOccupantComponent>(valueOrDefault1, out comp1))
      {
        EntityUid? nullable = comp1.Vehicle;
        EntityUid entityUid = vehicle;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0 || this.TransformSystem.GetMapId((Entity<TransformComponent>) valueOrDefault1) != interiorMapId)
          return false;
        EyeComponent comp2;
        if (this.TryComp<EyeComponent>(valueOrDefault1, out comp2))
        {
          nullable = comp2.Target;
          if (nullable.HasValue)
          {
            EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
            if (this.Exists(valueOrDefault2) && this.TransformSystem.GetMapId((Entity<TransformComponent>) valueOrDefault2) == mapCoords.MapId)
            {
              relayCoords = new EntityCoordinates(vehicle, Vector2.Zero);
              return relayCoords.IsValid((IEntityManager) this.EntityManager);
            }
          }
        }
        Vector2 worldPosition = this.TransformSystem.GetWorldPosition(vehicle);
        Angle worldRotation = this.TransformSystem.GetWorldRotation(vehicle);
        Vector2 vector2_1 = mapCoords.Position - worldPosition;
        Angle angle = Angle.op_UnaryNegation(worldRotation);
        Vector2 vector2_2 = ((Angle) ref angle).RotateVec(ref vector2_1) * relay.InsideScale;
        float num = relay.InsideClamp * relay.InsideClamp;
        if ((double) vector2_2.LengthSquared() > (double) num)
          vector2_2 = vector2_2 / vector2_2.Length() * relay.InsideClamp;
        relayCoords = interiorCoords.Offset(relay.InsideOffset + vector2_2);
        return relayCoords.IsValid((IEntityManager) this.EntityManager);
      }
    }
    return false;
  }

  private Angle GetRecoilAngle(
    TimeSpan curTime,
    EntityUid gunUid,
    GunComponent component,
    Angle direction)
  {
    double d1 = (curTime - component.LastFire).TotalSeconds;
    if (!double.IsFinite(d1) || d1 < 0.0)
      d1 = 0.0;
    double d2 = component.CurrentAngle.Theta;
    double d3 = component.AngleIncreaseModified.Theta;
    double d4 = component.AngleDecayModified.Theta;
    double d5 = component.MinAngleModified.Theta;
    double d6 = component.MaxAngleModified.Theta;
    if (!double.IsFinite(d2))
      d2 = 0.0;
    if (!double.IsFinite(d3))
      d3 = 0.0;
    if (!double.IsFinite(d4))
      d4 = 0.0;
    if (!double.IsFinite(d5))
      d5 = 0.0;
    if (!double.IsFinite(d6))
      d6 = d5;
    if (d5 > d6)
    {
      double num = d6;
      d6 = d5;
      d5 = num;
    }
    double d7 = MathHelper.Clamp(d2 + d3 - d4 * d1, d5, d6);
    if (!double.IsFinite(d7))
      d7 = d5;
    component.CurrentAngle = new Angle(d7);
    component.LastFire = component.NextFire;
    float num1 = new Xoroshiro64S((long) this.Timing.CurTick.Value << 32 /*0x20*/ | (long) (uint) this.GetNetEntity(gunUid).Id).NextFloat(-0.5f, 0.5f);
    double d8 = component.CurrentAngle.Theta * (double) num1;
    if (!double.IsFinite(d8))
      d8 = 0.0;
    double d9 = direction.Theta + d8;
    if (!double.IsFinite(d9))
      d9 = direction.Theta;
    return new Angle(d9);
  }

  private void ShootOrThrow(
    EntityUid uid,
    Vector2 mapDirection,
    Vector2 gunVelocity,
    GunComponent gun,
    EntityUid gunUid,
    EntityUid? user)
  {
    EntityUid? target = gun.Target;
    if (target.HasValue)
    {
      EntityUid valueOrDefault = target.GetValueOrDefault();
      if (!this.TerminatingOrDeleted(valueOrDefault))
      {
        TargetedProjectileComponent projectileComponent = this.EnsureComp<TargetedProjectileComponent>(uid);
        projectileComponent.Target = valueOrDefault;
        this.Dirty(uid, (IComponent) projectileComponent);
      }
    }
    if (!this.HasComp<ProjectileComponent>(uid))
    {
      this.RemoveShootable(uid);
      this.ThrowingSystem.TryThrow(uid, mapDirection, gun.ProjectileSpeedModified, user, recoil: false, rotate: false);
    }
    else
      this.ShootProjectile(uid, mapDirection, gunVelocity, new EntityUid?(gunUid), user, gun.ProjectileSpeedModified);
  }

  private void FireEffects(
    EntityCoordinates fromCoordinates,
    float distance,
    Angle mapDirection,
    HitscanPrototype hitscan,
    EntityUid? hitEntity = null)
  {
    List<(NetCoordinates, Angle, SpriteSpecifier, float)> valueTupleList = new List<(NetCoordinates, Angle, SpriteSpecifier, float)>();
    EntityUid? grid = this.TransformSystem.GetGrid(fromCoordinates);
    Angle angle = mapDirection;
    Robust.Shared.GameObjects.EntityQuery<TransformComponent> entityQuery = this.GetEntityQuery<TransformComponent>();
    TransformComponent component;
    if (entityQuery.TryGetComponent(grid, out component))
    {
      (Vector2 _, Angle WorldRotation, Matrix3x2 matrix3x2) = this.TransformSystem.GetWorldPositionRotationInvMatrix(component, entityQuery);
      fromCoordinates = new EntityCoordinates(grid.Value, Vector2.Transform(this.TransformSystem.ToMapCoordinates(fromCoordinates).Position, matrix3x2));
      angle = Angle.op_Subtraction(angle, WorldRotation);
    }
    if ((double) distance >= 1.0)
    {
      if (hitscan.MuzzleFlash != null)
      {
        NetCoordinates netCoordinates = this.GetNetCoordinates(fromCoordinates.Offset(Vector2Helpers.Normalized(((Angle) ref angle).ToVec()) / 2f));
        valueTupleList.Add((netCoordinates, angle, hitscan.MuzzleFlash, 1f));
      }
      if (hitscan.TravelFlash != null)
      {
        NetCoordinates netCoordinates = this.GetNetCoordinates(fromCoordinates.Offset(((Angle) ref angle).ToVec() * (distance + 0.5f) / 2f));
        valueTupleList.Add((netCoordinates, angle, hitscan.TravelFlash, distance - 1.5f));
      }
    }
    if (hitscan.ImpactFlash != null)
    {
      NetCoordinates netCoordinates = this.GetNetCoordinates(fromCoordinates.Offset(((Angle) ref angle).ToVec() * distance));
      valueTupleList.Add((netCoordinates, ((Angle) ref angle).FlipPositive(), hitscan.ImpactFlash, 1f));
    }
    if (!this._netManager.IsServer || valueTupleList.Count <= 0)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new SharedGunSystem.HitscanEvent()
    {
      Sprites = valueTupleList
    }, Filter.Pvs(fromCoordinates, entityMan: (IEntityManager) this.EntityManager));
  }

  private Angle[] LinearSpread(Angle start, Angle end, int intervals)
  {
    Angle[] angleArray = new Angle[intervals];
    for (int index = 0; index <= intervals - 1; ++index)
      angleArray[index] = new Angle(Angle.op_Implicit(Angle.op_Addition(start, Angle.op_Implicit(Angle.op_Implicit(Angle.op_Subtraction(end, start)) * (double) index / (double) (intervals - 1)))));
    return angleArray;
  }

  public void PlayImpactSound(
    EntityUid otherEntity,
    DamageSpecifier? modifiedDamage,
    SoundSpecifier? weaponSound,
    bool forceWeaponSound,
    Filter? filter = null,
    EntityUid? projectile = null)
  {
    if (this._netManager.IsClient && this.HasComp<PredictedProjectileServerComponent>(projectile))
      return;
    if (filter == null)
      filter = Filter.Pvs(otherEntity);
    bool flag = false;
    RangedDamageSoundComponent comp;
    if (!forceWeaponSound && modifiedDamage != null && modifiedDamage.GetTotal() > 0 && this.TryComp<RangedDamageSoundComponent>(otherEntity, out comp))
    {
      string highestDamageSound = SharedMeleeWeaponSystem.GetHighestDamageSound(modifiedDamage, this.ProtoManager);
      if (highestDamageSound != null)
      {
        Dictionary<string, SoundSpecifier> soundTypes = comp.SoundTypes;
        SoundSpecifier sound;
        // ISSUE: explicit non-virtual call
        if ((soundTypes != null ? (__nonvirtual (soundTypes.TryGetValue(highestDamageSound, out sound)) ? 1 : 0) : 0) != 0 && filter.Count > 0)
        {
          this.Audio.PlayEntity(sound, filter, otherEntity, true, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
          flag = true;
          goto label_11;
        }
      }
      if (highestDamageSound != null)
      {
        Dictionary<string, SoundSpecifier> soundGroups = comp.SoundGroups;
        SoundSpecifier sound;
        // ISSUE: explicit non-virtual call
        if ((soundGroups != null ? (__nonvirtual (soundGroups.TryGetValue(highestDamageSound, out sound)) ? 1 : 0) : 0) != 0 && filter.Count > 0)
        {
          this.Audio.PlayEntity(sound, filter, otherEntity, true, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f))));
          flag = true;
        }
      }
    }
label_11:
    if (flag || weaponSound == null || filter.Count <= 0)
      return;
    this.Audio.PlayEntity(weaponSound, filter, otherEntity, true);
  }

  private void Recoil(EntityUid? user, Vector2 recoil, float recoilScalar)
  {
    if (this._netManager.IsServer || !this.Timing.IsFirstTimePredicted || !user.HasValue || recoil == Vector2.Zero || (double) recoilScalar == 0.0 || this.HasComp<WeaponControllerComponent>(user.Value))
      return;
    this._recoil.KickCamera(user.Value, Vector2Helpers.Normalized(recoil) * 0.5f * recoilScalar);
  }

  public virtual void ShootProjectile(
    EntityUid uid,
    Vector2 direction,
    Vector2 gunVelocity,
    EntityUid? gunUid,
    EntityUid? user = null,
    float speed = 20f)
  {
    PhysicsComponent physicsComponent = this.EnsureComp<PhysicsComponent>(uid);
    this.Physics.SetBodyStatus(uid, physicsComponent, BodyStatus.InAir);
    Vector2 vector2 = gunVelocity + Vector2Helpers.Normalized(direction) * speed;
    Vector2 mapLinearVelocity = this.Physics.GetMapLinearVelocity(uid, physicsComponent);
    Vector2 velocity = physicsComponent.LinearVelocity + vector2 - mapLinearVelocity;
    this.Physics.SetLinearVelocity(uid, velocity, body: physicsComponent);
    ProjectileComponent component = this.EnsureComp<ProjectileComponent>(uid);
    component.Weapon = gunUid;
    EntityUid? nullable = user ?? gunUid;
    if (nullable.HasValue)
      this.Projectiles.SetShooter(uid, component, new EntityUid?(nullable.Value));
    this.TransformSystem.SetWorldRotationNoLerp((Entity<TransformComponent>) uid, Angle.op_Addition(DirectionExtensions.ToWorldAngle(direction), component.Angle));
  }

  protected abstract void Popup(string message, EntityUid? uid, EntityUid? user);

  public virtual void UpdateAmmoCount(EntityUid uid, bool prediction = true, int artificialIncrease = 0)
  {
  }

  protected void SetCartridgeSpent(EntityUid uid, CartridgeAmmoComponent cartridge, bool spent)
  {
    if (cartridge.Spent != spent)
      this.DirtyField<CartridgeAmmoComponent>(uid, cartridge, "Spent");
    cartridge.Spent = spent;
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.Spent, (object) spent);
    if (spent)
      this.TagSystem.AddTag(uid, (ProtoId<TagPrototype>) "HideContextMenu");
    else
      this.TagSystem.RemoveTag(uid, (ProtoId<TagPrototype>) "HideContextMenu");
  }

  protected void EjectCartridge(EntityUid entity, Angle? angle = null, bool playSound = true)
  {
    Vector2 position = this.Random.NextVector2(0.4f);
    TransformComponent xform = this.Transform(entity);
    EntityCoordinates entityCoordinates = xform.Coordinates;
    entityCoordinates = entityCoordinates.Offset(position);
    this.TransformSystem.SetLocalRotation(entity, this.Random.NextAngle(), xform);
    this.TransformSystem.SetCoordinates(entity, xform, entityCoordinates);
    if (angle.HasValue)
    {
      Angle angle1 = Angle.op_Addition(angle.Value, Angle.op_Implicit(3.7f));
      this.ThrowingSystem.TryThrow(entity, Vector2Helpers.Normalized(((Angle) ref angle1).ToVec()) / 100f, 5f, rotate: false);
    }
    CartridgeAmmoComponent comp;
    if (!playSound || !this.TryComp<CartridgeAmmoComponent>(entity, out comp))
      return;
    this.Audio.PlayPvs(comp.EjectSound, entity, new AudioParams?(AudioParams.Default.WithVariation(new float?(0.05f)).WithVolume(-1f)));
  }

  public IShootable EnsureShootable(EntityUid uid)
  {
    CartridgeAmmoComponent comp;
    return this.TryComp<CartridgeAmmoComponent>(uid, out comp) ? (IShootable) comp : (IShootable) this.EnsureComp<AmmoComponent>(uid);
  }

  protected void RemoveShootable(EntityUid uid)
  {
    this.RemCompDeferred<CartridgeAmmoComponent>(uid);
    this.RemCompDeferred<AmmoComponent>(uid);
  }

  protected void MuzzleFlash(
    EntityUid gun,
    AmmoComponent component,
    Angle worldAngle,
    EntityUid? user = null)
  {
    GunMuzzleFlashAttemptEvent args1 = new GunMuzzleFlashAttemptEvent();
    this.RaiseLocalEvent<GunMuzzleFlashAttemptEvent>(gun, ref args1);
    if (args1.Cancelled)
      return;
    EntProtoId? muzzleFlash = component.MuzzleFlash;
    if (!muzzleFlash.HasValue)
      return;
    Vector2 muzzleFlashOffset = component.MuzzleFlashOffset;
    Vector2 originOffset1 = Vector2.Zero;
    GunComponent comp;
    if (this.TryComp<GunComponent>(gun, out comp))
    {
      RMCBeforeMuzzleFlashEvent args2 = new RMCBeforeMuzzleFlashEvent(gun, comp.ShootOriginOffset);
      this.RaiseLocalEvent<RMCBeforeMuzzleFlashEvent>(gun, ref args2);
      gun = args2.Weapon;
      originOffset1 = args2.Offset;
    }
    NetEntity netEntity = this.GetNetEntity(gun);
    EntProtoId? nullable = muzzleFlash;
    string valueOrDefault = nullable.HasValue ? (string) nullable.GetValueOrDefault() : (string) null;
    Angle angle = worldAngle;
    Vector2 offset = muzzleFlashOffset;
    Vector2 originOffset2 = originOffset1;
    MuzzleFlashEvent message = new MuzzleFlashEvent(netEntity, valueOrDefault, angle, offset, originOffset2);
    this.CreateEffect(gun, message, new EntityUid?(gun), user, muzzleFlashOffset, originOffset1);
  }

  public void CauseImpulse(
    EntityCoordinates fromCoordinates,
    EntityCoordinates toCoordinates,
    EntityUid user,
    PhysicsComponent userPhysics)
  {
    Vector2 position = this.TransformSystem.ToMapCoordinates(fromCoordinates).Position;
    Vector2 vector2 = Vector2Helpers.Normalized(this.TransformSystem.ToMapCoordinates(toCoordinates).Position - position) * 25f;
    this.Physics.ApplyLinearImpulse(user, -vector2, body: userPhysics);
  }

  public void RefreshModifiers(Entity<GunComponent?> gun)
  {
    if (!this.Resolve<GunComponent>((EntityUid) gun, ref gun.Comp, false))
      return;
    GunComponent comp = gun.Comp;
    GunRefreshModifiersEvent args = new GunRefreshModifiersEvent((Entity<GunComponent>) ((EntityUid) gun, comp), comp.SoundGunshot, comp.CameraRecoilScalar, comp.AngleIncrease, comp.AngleDecay, comp.MaxAngle, comp.MinAngle, comp.ShotsPerBurst, comp.FireRate, comp.ProjectileSpeed);
    this.RaiseLocalEvent<GunRefreshModifiersEvent>((EntityUid) gun, ref args);
    if (comp.SoundGunshotModified != args.SoundGunshot)
    {
      comp.SoundGunshotModified = args.SoundGunshot;
      this.DirtyField<GunComponent>(gun, "SoundGunshotModified");
    }
    if (!MathHelper.CloseTo(comp.CameraRecoilScalarModified, args.CameraRecoilScalar, 1E-07f))
    {
      comp.CameraRecoilScalarModified = args.CameraRecoilScalar;
      this.DirtyField<GunComponent>(gun, "CameraRecoilScalarModified");
    }
    if (!((Angle) ref comp.AngleIncreaseModified).EqualsApprox(args.AngleIncrease))
    {
      comp.AngleIncreaseModified = args.AngleIncrease;
      this.DirtyField<GunComponent>(gun, "AngleIncreaseModified");
    }
    if (!((Angle) ref comp.AngleDecayModified).EqualsApprox(args.AngleDecay))
    {
      comp.AngleDecayModified = args.AngleDecay;
      this.DirtyField<GunComponent>(gun, "AngleDecayModified");
    }
    if (!((Angle) ref comp.MaxAngleModified).EqualsApprox(args.MaxAngle))
    {
      comp.MaxAngleModified = args.MaxAngle;
      this.DirtyField<GunComponent>(gun, "MaxAngleModified");
    }
    if (!((Angle) ref comp.MinAngleModified).EqualsApprox(args.MinAngle))
    {
      comp.MinAngleModified = args.MinAngle;
      this.DirtyField<GunComponent>(gun, "MinAngleModified");
    }
    if (comp.ShotsPerBurstModified != args.ShotsPerBurst)
    {
      comp.ShotsPerBurstModified = args.ShotsPerBurst;
      this.DirtyField<GunComponent>(gun, "ShotsPerBurstModified");
    }
    if (!MathHelper.CloseTo(comp.FireRateModified, args.FireRate, 1E-07f))
    {
      comp.FireRateModified = args.FireRate;
      this.DirtyField<GunComponent>(gun, "FireRateModified");
    }
    if (MathHelper.CloseTo(comp.ProjectileSpeedModified, args.ProjectileSpeed, 1E-07f))
      return;
    comp.ProjectileSpeedModified = args.ProjectileSpeed;
    this.DirtyField<GunComponent>(gun, "ProjectileSpeedModified");
  }

  protected abstract void CreateEffect(
    EntityUid gunUid,
    MuzzleFlashEvent message,
    EntityUid? user = null,
    EntityUid? player = null,
    Vector2 offset = default (Vector2),
    Vector2 originOffset = default (Vector2));

  private void OnExamine(EntityUid uid, GunComponent component, ExaminedEvent args)
  {
    if (!args.IsInDetailsRange || !component.ShowExamineText || this.HasComp<XenoComponent>(args.Examiner))
      return;
    using (args.PushGroup("GunComponent"))
    {
      args.PushMarkup(this.Loc.GetString("gun-selected-mode-examine", ("color", (object) "cyan"), ("mode", (object) this.GetLocSelector(component.SelectedMode))));
      args.PushMarkup(this.Loc.GetString("gun-fire-rate-examine", ("color", (object) "yellow"), ("fireRate", (object) $"{component.FireRateModified:0.0}")));
    }
  }

  private string GetLocSelector(SelectiveFire mode) => this.Loc.GetString("gun-" + mode.ToString());

  private void OnAltVerb(
    EntityUid uid,
    GunComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract || args.Hands == null || component.SelectedMode == component.AvailableModes || this.HasComp<XenoComponent>(args.User))
      return;
    SelectiveFire nextMode = this.GetNextMode(component);
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.SelectFire(uid, component, nextMode, new EntityUid?(args.User)));
    alternativeVerb1.Text = this.Loc.GetString("gun-selector-verb", ("mode", (object) this.GetLocSelector(nextMode)));
    alternativeVerb1.Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/fold.svg.192dpi.png"));
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  public SelectiveFire GetNextMode(GunComponent component)
  {
    List<SelectiveFire> selectiveFireList = new List<SelectiveFire>();
    foreach (SelectiveFire selectiveFire in Enum.GetValues<SelectiveFire>())
    {
      if ((selectiveFire & component.AvailableModes) != SelectiveFire.Invalid)
        selectiveFireList.Add(selectiveFire);
    }
    int num = selectiveFireList.IndexOf(component.SelectedMode);
    return selectiveFireList[(num + 1) % selectiveFireList.Count];
  }

  public void SelectFire(
    EntityUid uid,
    GunComponent component,
    SelectiveFire fire,
    EntityUid? user = null)
  {
    if (component.SelectedMode == fire)
      return;
    component.SelectedMode = fire;
    if (!this.Paused(uid))
    {
      TimeSpan curTime = this.Timing.CurTime;
      TimeSpan timeSpan = TimeSpan.FromSeconds(0.30000001192092896);
      if (component.NextFire < curTime)
        component.NextFire = curTime + timeSpan;
      else
        component.NextFire += timeSpan;
    }
    this.Audio.PlayPredicted(component.SoundMode, uid, user);
    this.Popup(this.Loc.GetString("gun-selected-mode", ("mode", (object) this.GetLocSelector(fire))), new EntityUid?(uid), user);
    RMCFireModeChangedEvent args = new RMCFireModeChangedEvent();
    this.RaiseLocalEvent<RMCFireModeChangedEvent>(uid, ref args);
    this.Dirty(uid, (IComponent) component);
  }

  public void CycleFire(EntityUid uid, GunComponent component, EntityUid? user = null)
  {
    if (component.SelectedMode == component.AvailableModes)
      return;
    SelectiveFire nextMode = this.GetNextMode(component);
    this.SelectFire(uid, component, nextMode, user);
  }

  private void OnCycleMode(
    EntityUid uid,
    GunComponent component,
    SharedGunSystem.CycleModeEvent args)
  {
    this.SelectFire(uid, component, args.Mode, new EntityUid?(args.Performer));
  }

  private void OnGunSelected(EntityUid uid, GunComponent component, HandSelectedEvent args)
  {
    if (this.Timing.ApplyingState || (double) component.FireRateModified <= 0.0)
      return;
    float num = 1f / component.FireRateModified;
    if (num.Equals(0.0f) || !component.ResetOnHandSelected || this.Paused(uid))
      return;
    TimeSpan timeSpan = this.Timing.CurTime + TimeSpan.FromSeconds((double) num);
    if (timeSpan < component.NextFire)
      return;
    component.NextFire = timeSpan;
    this.Dirty(uid, (IComponent) component);
  }

  protected virtual void InitializeMagazine()
  {
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, MapInitEvent>(new EntityEventRefHandler<MagazineAmmoProviderComponent, MapInitEvent>(this.OnMagazineMapInit));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, TakeAmmoEvent>(this.OnMagazineTakeAmmo));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<MagazineAmmoProviderComponent, GetAmmoCountEvent>(this.OnMagazineAmmoCount));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<MagazineAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnMagazineVerb));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<MagazineAmmoProviderComponent, EntInsertedIntoContainerMessage>(this.OnMagazineSlotChange));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<MagazineAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnMagazineSlotChange));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, UseInHandEvent>(this.OnMagazineUse));
    this.SubscribeLocalEvent<MagazineAmmoProviderComponent, ExaminedEvent>(new ComponentEventHandler<MagazineAmmoProviderComponent, ExaminedEvent>(this.OnMagazineExamine));
  }

  private void OnMagazineMapInit(Entity<MagazineAmmoProviderComponent> ent, ref MapInitEvent args)
  {
    if (this.UsesPubgStoredMagazineAmmo((EntityUid) ent))
      return;
    this.MagazineSlotChanged(ent);
  }

  private void OnMagazineExamine(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    ExaminedEvent args)
  {
    if (this.UsesPubgStoredMagazineAmmo(uid) || !args.IsInDetailsRange)
      return;
    int num = this.GetMagazineCountCapacity(uid, component).Item1;
    args.PushMarkup(this.Loc.GetString("gun-magazine-examine", ("color", (object) "yellow"), ("count", (object) num)));
  }

  private void OnMagazineUse(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    UseInHandEvent args)
  {
    if (this.UsesPubgStoredMagazineAmmo(uid))
      return;
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    if (!magazineEntity.HasValue)
      return;
    this.RaiseLocalEvent<UseInHandEvent>(magazineEntity.Value, args);
    this.UpdateAmmoCount(uid);
    this.UpdateMagazineAppearance(uid, component, magazineEntity.Value);
  }

  private void OnMagazineVerb(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (this.UsesPubgStoredMagazineAmmo(uid) || !args.CanInteract || !args.CanAccess)
      return;
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    if (!magazineEntity.HasValue)
      return;
    this.RaiseLocalEvent<GetVerbsEvent<AlternativeVerb>>(magazineEntity.Value, args);
    this.UpdateMagazineAppearance(magazineEntity.Value, component, magazineEntity.Value);
  }

  protected virtual void OnMagazineSlotChange(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    ContainerModifiedMessage args)
  {
    if (this.UsesPubgStoredMagazineAmmo(uid) || "gun_magazine" != args.Container.ID)
      return;
    this.MagazineSlotChanged((Entity<MagazineAmmoProviderComponent>) (uid, component));
  }

  private void MagazineSlotChanged(Entity<MagazineAmmoProviderComponent> ent)
  {
    this.UpdateAmmoCount((EntityUid) ent);
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>((EntityUid) ent, out comp))
      return;
    EntityUid? magazineEntity = this.GetMagazineEntity((EntityUid) ent);
    this.Appearance.SetData((EntityUid) ent, (Enum) AmmoVisuals.MagLoaded, (object) magazineEntity.HasValue, comp);
    if (!magazineEntity.HasValue)
      return;
    this.UpdateMagazineAppearance((EntityUid) ent, (MagazineAmmoProviderComponent) ent, magazineEntity.Value);
  }

  protected (int, int) GetMagazineCountCapacity(
    EntityUid uid,
    MagazineAmmoProviderComponent component)
  {
    int num1 = 0;
    int num2 = 1;
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    if (magazineEntity.HasValue)
    {
      GetAmmoCountEvent args = new GetAmmoCountEvent();
      this.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref args);
      num1 += args.Count;
      num2 += args.Capacity;
    }
    return (num1, num2);
  }

  protected EntityUid? GetMagazineEntity(EntityUid uid)
  {
    BaseContainer container;
    return !this.Containers.TryGetContainer(uid, "gun_magazine", out container) || !(container is ContainerSlot containerSlot) ? new EntityUid?() : containerSlot.ContainedEntity;
  }

  private void OnMagazineAmmoCount(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    if (this.UsesPubgStoredMagazineAmmo(uid))
      return;
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    if (!magazineEntity.HasValue)
      return;
    this.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref args);
  }

  private void OnMagazineTakeAmmo(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    if (this.UsesPubgStoredMagazineAmmo(uid))
      return;
    EntityUid? magazineEntity = this.GetMagazineEntity(uid);
    AppearanceComponent comp;
    this.TryComp<AppearanceComponent>(uid, out comp);
    if (!magazineEntity.HasValue)
    {
      this.Appearance.SetData(uid, (Enum) AmmoVisuals.MagLoaded, (object) false, comp);
    }
    else
    {
      this.RaiseLocalEvent<TakeAmmoEvent>(magazineEntity.Value, args);
      GetAmmoCountEvent args1 = new GetAmmoCountEvent();
      this.RaiseLocalEvent<GetAmmoCountEvent>(magazineEntity.Value, ref args1);
      this.FinaliseMagazineTakeAmmo(uid, component, args1.Count, args1.Capacity, args.User, comp);
    }
  }

  private void FinaliseMagazineTakeAmmo(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    int count,
    int capacity,
    EntityUid? user,
    AppearanceComponent? appearance)
  {
    bool flag = component.AutoEject && count == 0;
    ActorComponent comp;
    if (flag && this.TryComp<ActorComponent>(user, out comp))
      flag = this._netConfig.GetClientCVar<bool>(comp.PlayerSession.Channel, RMCCVars.RMCAutoEjectMagazines);
    if (flag)
    {
      this.EjectMagazine(uid, component, user);
      this.Audio.PlayPredicted(component.SoundAutoEject, uid, user);
    }
    this.UpdateMagazineAppearance(uid, appearance, !flag, count, capacity);
  }

  private void UpdateMagazineAppearance(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    EntityUid magEnt)
  {
    AppearanceComponent comp1;
    this.TryComp<AppearanceComponent>(uid, out comp1);
    int count = 0;
    int capacity = 0;
    AppearanceComponent comp2;
    if (this.TryComp<AppearanceComponent>(magEnt, out comp2))
    {
      int num1;
      this.Appearance.TryGetData<int>(magEnt, (Enum) AmmoVisuals.AmmoCount, out num1, comp2);
      int num2;
      this.Appearance.TryGetData<int>(magEnt, (Enum) AmmoVisuals.AmmoMax, out num2, comp2);
      count += num1;
      capacity += num2;
    }
    this.UpdateMagazineAppearance(uid, comp1, true, count, capacity);
  }

  private void UpdateMagazineAppearance(
    EntityUid uid,
    AppearanceComponent? appearance,
    bool magLoaded,
    int count,
    int capacity)
  {
    if (appearance == null)
      return;
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.MagLoaded, (object) magLoaded, appearance);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.HasAmmo, (object) (count != 0), appearance);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoCount, (object) count, appearance);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoMax, (object) capacity, appearance);
  }

  private void EjectMagazine(
    EntityUid uid,
    MagazineAmmoProviderComponent component,
    EntityUid? user)
  {
    if (!this.GetMagazineEntity(uid).HasValue)
      return;
    this._slots.TryEject(uid, "gun_magazine", user, out EntityUid? _, excludeUserAudio: true);
  }

  private bool UsesPubgStoredMagazineAmmo(EntityUid uid)
  {
    PubgWeaponModulesComponent comp;
    if (!this.TryComp<PubgAmmoProviderComponent>(uid, out PubgAmmoProviderComponent _) || !this.TryComp<PubgWeaponModulesComponent>(uid, out comp))
      return false;
    foreach (PubgWeaponModuleSlotDefinition slot in comp.Slots)
    {
      if (slot.Slot == PubgModuleSlotType.Magazine)
        return slot.StoresAmmo;
    }
    return false;
  }

  protected virtual void InitializeRevolver()
  {
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentGetState>(new ComponentEventRefHandler<RevolverAmmoProviderComponent, ComponentGetState>(this.OnRevolverGetState));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentHandleState>(new ComponentEventRefHandler<RevolverAmmoProviderComponent, ComponentHandleState>(this.OnRevolverHandleState));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, ComponentInit>(new ComponentEventHandler<RevolverAmmoProviderComponent, ComponentInit>(this.OnRevolverInit));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, TakeAmmoEvent>(this.OnRevolverTakeAmmo));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<RevolverAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnRevolverVerbs));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, InteractUsingEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, InteractUsingEvent>(this.OnRevolverInteractUsing));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<RevolverAmmoProviderComponent, GetAmmoCountEvent>(this.OnRevolverGetAmmoCount));
    this.SubscribeLocalEvent<RevolverAmmoProviderComponent, UseInHandEvent>(new ComponentEventHandler<RevolverAmmoProviderComponent, UseInHandEvent>(this.OnRevolverUse));
  }

  private void OnRevolverUse(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    UseInHandEvent args)
  {
    if (args.Handled || !this._useDelay.TryResetDelay(uid))
      return;
    args.Handled = true;
    this.Cycle(component);
    this.UpdateAmmoCount(uid, false);
    this.Dirty(uid, (IComponent) component);
  }

  private void OnRevolverGetAmmoCount(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    args.Count += this.GetRevolverCount(component);
    args.Capacity += component.Capacity;
  }

  private void OnRevolverInteractUsing(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    InteractUsingEvent args)
  {
    if (args.Handled || !this.TryRevolverInsert(uid, component, args.Used, new EntityUid?(args.User)))
      return;
    args.Handled = true;
  }

  private void OnRevolverGetState(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    ref ComponentGetState args)
  {
    args.State = (IComponentState) new SharedGunSystem.RevolverAmmoProviderComponentState()
    {
      CurrentIndex = component.CurrentIndex,
      AmmoSlots = this.GetNetEntityList(component.AmmoSlots),
      Chambers = component.Chambers
    };
  }

  private void OnRevolverHandleState(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    ref ComponentHandleState args)
  {
    if (!(args.Current is SharedGunSystem.RevolverAmmoProviderComponentState current))
      return;
    int currentIndex = component.CurrentIndex;
    component.CurrentIndex = current.CurrentIndex;
    component.Chambers = new bool?[current.Chambers.Length];
    for (int index = 0; index < component.AmmoSlots.Count; ++index)
    {
      component.AmmoSlots[index] = this.EnsureEntity<RevolverAmmoProviderComponent>(current.AmmoSlots[index], uid);
      component.Chambers[index] = current.Chambers[index];
    }
    if (currentIndex == current.CurrentIndex)
      return;
    this.UpdateAmmoCount(uid, false);
  }

  public bool TryRevolverInsert(
    EntityUid revolverUid,
    RevolverAmmoProviderComponent component,
    EntityUid uid,
    EntityUid? user)
  {
    if (this._whitelistSystem.IsWhitelistFail(component.Whitelist, uid))
      return false;
    if (this.HasComp<SpeedLoaderComponent>(uid))
    {
      int num = 0;
      for (int index = 0; index < component.Capacity; ++index)
      {
        if (!component.AmmoSlots[index].HasValue && !component.Chambers[index].HasValue)
          ++num;
      }
      if (num == 0)
      {
        this.Popup(this.Loc.GetString("gun-revolver-full"), new EntityUid?(revolverUid), user);
        return false;
      }
      TransformComponent component1 = this.GetEntityQuery<TransformComponent>().GetComponent(uid);
      List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>(num);
      TakeAmmoEvent args = new TakeAmmoEvent(num, ammo, component1.Coordinates, user);
      this.RaiseLocalEvent<TakeAmmoEvent>(uid, args);
      if (args.Ammo.Count == 0)
      {
        this.Popup(this.Loc.GetString("gun-speedloader-empty"), new EntityUid?(revolverUid), user);
        return false;
      }
      for (int index1 = 0; index1 < component.Capacity; ++index1)
      {
        int index2 = (component.CurrentIndex + index1) % component.Capacity;
        if (!component.AmmoSlots[index2].HasValue && !component.Chambers[index2].HasValue)
        {
          EntityUid? entity = args.Ammo.Last<(EntityUid?, IShootable)>().Entity;
          args.Ammo.RemoveAt(args.Ammo.Count - 1);
          if (!entity.HasValue)
          {
            this.Log.Error("Tried to load hitscan into a revolver which is unsupported");
          }
          else
          {
            component.AmmoSlots[index2] = new EntityUid?(entity.Value);
            this.Containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) entity.Value, (BaseContainer) component.AmmoContainer);
            this.SetChamber(index2, component, uid);
            if (args.Ammo.Count == 0)
              break;
          }
        }
      }
      this.UpdateRevolverAppearance(revolverUid, component);
      this.UpdateAmmoCount(revolverUid);
      this.Dirty(revolverUid, (IComponent) component);
      this.Audio.PlayPredicted(component.SoundInsert, revolverUid, user);
      this.Popup(this.Loc.GetString("gun-revolver-insert"), new EntityUid?(revolverUid), user);
      return true;
    }
    for (int index3 = 0; index3 < component.Capacity; ++index3)
    {
      int index4 = (component.CurrentIndex + index3) % component.Capacity;
      if (!component.AmmoSlots[index4].HasValue && !component.Chambers[index4].HasValue)
      {
        component.AmmoSlots[index4] = new EntityUid?(uid);
        this.Containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid, (BaseContainer) component.AmmoContainer);
        this.SetChamber(index4, component, uid);
        this.Audio.PlayPredicted(component.SoundInsert, revolverUid, user);
        this.Popup(this.Loc.GetString("gun-revolver-insert"), new EntityUid?(revolverUid), user);
        this.UpdateRevolverAppearance(revolverUid, component);
        this.UpdateAmmoCount(revolverUid);
        this.Dirty(revolverUid, (IComponent) component);
        return true;
      }
    }
    this.Popup(this.Loc.GetString("gun-revolver-full"), new EntityUid?(revolverUid), user);
    return false;
  }

  private void SetChamber(int index, RevolverAmmoProviderComponent component, EntityUid uid)
  {
    CartridgeAmmoComponent comp;
    if (this.TryComp<CartridgeAmmoComponent>(uid, out comp) && comp.Spent)
      component.Chambers[index] = new bool?(false);
    else
      component.Chambers[index] = new bool?(true);
  }

  private void OnRevolverVerbs(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || args.Hands == null)
      return;
    SortedSet<AlternativeVerb> verbs1 = args.Verbs;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Text = this.Loc.GetString("gun-revolver-empty");
    alternativeVerb1.Disabled = !this.AnyRevolverCartridges(component);
    alternativeVerb1.Act = (Action) (() => this.EmptyRevolver(uid, component, new EntityUid?(args.User)));
    alternativeVerb1.Priority = 1;
    verbs1.Add(alternativeVerb1);
    SortedSet<AlternativeVerb> verbs2 = args.Verbs;
    AlternativeVerb alternativeVerb2 = new AlternativeVerb();
    alternativeVerb2.Text = this.Loc.GetString("gun-revolver-spin");
    alternativeVerb2.Act = (Action) (() => this.SpinRevolver(uid, component, new EntityUid?(args.User)));
    verbs2.Add(alternativeVerb2);
  }

  private bool AnyRevolverCartridges(RevolverAmmoProviderComponent component)
  {
    for (int index = 0; index < component.Capacity; ++index)
    {
      if (component.Chambers[index].HasValue || component.AmmoSlots[index].HasValue)
        return true;
    }
    return false;
  }

  private int GetRevolverCount(RevolverAmmoProviderComponent component)
  {
    int revolverCount = 0;
    for (int index = 0; index < component.Capacity; ++index)
    {
      if (component.Chambers[index].HasValue || component.AmmoSlots[index].HasValue)
        ++revolverCount;
    }
    return revolverCount;
  }

  private int GetRevolverUnspentCount(RevolverAmmoProviderComponent component)
  {
    int revolverUnspentCount = 0;
    for (int index = 0; index < component.Capacity; ++index)
    {
      if (component.Chambers[index].GetValueOrDefault())
      {
        ++revolverUnspentCount;
      }
      else
      {
        CartridgeAmmoComponent comp;
        if (this.TryComp<CartridgeAmmoComponent>(component.AmmoSlots[index], out comp) && !comp.Spent)
          ++revolverUnspentCount;
      }
    }
    return revolverUnspentCount;
  }

  public void EmptyRevolver(
    EntityUid revolverUid,
    RevolverAmmoProviderComponent component,
    EntityUid? user = null)
  {
    MapCoordinates mapCoordinates = this.TransformSystem.GetMapCoordinates(revolverUid);
    bool flag = false;
    for (int index = 0; index < component.Capacity; ++index)
    {
      bool? chamber = component.Chambers[index];
      EntityUid? ammoSlot = component.AmmoSlots[index];
      if (!ammoSlot.HasValue)
      {
        if (chamber.HasValue)
        {
          if (!this._netManager.IsClient)
          {
            EntityUid entityUid = this.Spawn(component.FillPrototype, mapCoordinates, rotation: new Angle());
            CartridgeAmmoComponent comp;
            if (this.TryComp<CartridgeAmmoComponent>(entityUid, out comp))
              this.SetCartridgeSpent(entityUid, comp, !chamber.Value);
            this.EjectCartridge(entityUid);
          }
          component.Chambers[index] = new bool?();
          flag = true;
        }
      }
      else
      {
        component.AmmoSlots[index] = new EntityUid?();
        this.Containers.Remove((Entity<TransformComponent, MetaDataComponent>) ammoSlot.Value, (BaseContainer) component.AmmoContainer);
        component.Chambers[index] = new bool?();
        if (!this._netManager.IsClient)
          this.EjectCartridge(ammoSlot.Value);
        flag = true;
      }
    }
    if (!flag)
      return;
    this.Audio.PlayPredicted(component.SoundEject, revolverUid, user);
    this.UpdateAmmoCount(revolverUid, false);
    this.UpdateRevolverAppearance(revolverUid, component);
    this.Dirty(revolverUid, (IComponent) component);
  }

  private void UpdateRevolverAppearance(EntityUid uid, RevolverAmmoProviderComponent component)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    int revolverCount = this.GetRevolverCount(component);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.HasAmmo, (object) (revolverCount != 0), comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoCount, (object) revolverCount, comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoMax, (object) component.Capacity, comp);
  }

  protected virtual void SpinRevolver(
    EntityUid revolverUid,
    RevolverAmmoProviderComponent component,
    EntityUid? user = null)
  {
    this.Audio.PlayPredicted(component.SoundSpin, revolverUid, user);
    this.Popup(this.Loc.GetString("gun-revolver-spun"), new EntityUid?(revolverUid), user);
  }

  private void OnRevolverTakeAmmo(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    int currentIndex = component.CurrentIndex;
    this.Cycle(component, args.Shots);
    for (int index1 = 0; index1 < args.Shots; ++index1)
    {
      int index2 = (currentIndex + index1) % component.Capacity;
      bool? chamber = component.Chambers[index2];
      EntityUid? uid1 = new EntityUid?();
      EntityUid? nullable1 = component.AmmoSlots[index2];
      if (nullable1.HasValue)
      {
        uid1 = component.AmmoSlots[index2];
        component.Chambers[index2] = new bool?(false);
      }
      else if (chamber.HasValue && chamber.GetValueOrDefault())
      {
        uid1 = new EntityUid?(this.Spawn(component.FillPrototype, args.Coordinates));
        if (!this._netManager.IsClient)
        {
          component.AmmoSlots[index2] = uid1;
          this.Containers.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) uid1.Value, (BaseContainer) component.AmmoContainer);
        }
        component.Chambers[index2] = new bool?(false);
      }
      if (uid1.HasValue)
      {
        CartridgeAmmoComponent comp;
        if (this.TryComp<CartridgeAmmoComponent>(uid1, out comp))
        {
          if (!comp.Spent)
          {
            this.SetCartridgeSpent(uid1.Value, comp, true);
            EntityUid uid2 = this.Spawn((string) comp.Prototype, args.Coordinates);
            args.Ammo.Add((new EntityUid?(uid2), (IShootable) this.EnsureComp<AmmoComponent>(uid2)));
            if (comp.DeleteOnSpawn)
            {
              List<EntityUid?> ammoSlots = component.AmmoSlots;
              int index3 = index2;
              nullable1 = new EntityUid?();
              EntityUid? nullable2 = nullable1;
              ammoSlots[index3] = nullable2;
              component.Chambers[index2] = new bool?();
            }
          }
          else
            continue;
        }
        else
        {
          List<EntityUid?> ammoSlots = component.AmmoSlots;
          int index4 = index2;
          nullable1 = new EntityUid?();
          EntityUid? nullable3 = nullable1;
          ammoSlots[index4] = nullable3;
          component.Chambers[index2] = new bool?();
          args.Ammo.Add((new EntityUid?(uid1.Value), (IShootable) this.EnsureComp<AmmoComponent>(uid1.Value)));
        }
        if (this._netManager.IsClient && this.IsClientSide(uid1.Value))
          this.QueueDel(uid1);
      }
    }
    this.UpdateAmmoCount(uid, false);
    this.UpdateRevolverAppearance(uid, component);
    this.Dirty(uid, (IComponent) component);
  }

  private void Cycle(RevolverAmmoProviderComponent component, int count = 1)
  {
    component.CurrentIndex = (component.CurrentIndex + count) % component.Capacity;
  }

  private void OnRevolverInit(
    EntityUid uid,
    RevolverAmmoProviderComponent component,
    ComponentInit args)
  {
    component.AmmoContainer = this.Containers.EnsureContainer<Container>(uid, "revolver-ammo");
    component.AmmoSlots.EnsureCapacity(component.Capacity);
    int num = component.Capacity - component.AmmoSlots.Count;
    for (int index = 0; index < num; ++index)
      component.AmmoSlots.Add(new EntityUid?());
    component.Chambers = new bool?[component.Capacity];
    if (component.FillPrototype == null)
      return;
    for (int index = 0; index < component.Capacity; ++index)
    {
      EntityUid? ammoSlot = component.AmmoSlots[index];
      component.Chambers[index] = !ammoSlot.HasValue ? new bool?(true) : new bool?();
    }
  }

  protected virtual void InitializeSolution()
  {
    this.SubscribeLocalEvent<SolutionAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<SolutionAmmoProviderComponent, TakeAmmoEvent>(this.OnSolutionTakeAmmo));
    this.SubscribeLocalEvent<SolutionAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<SolutionAmmoProviderComponent, GetAmmoCountEvent>(this.OnSolutionAmmoCount));
  }

  private void OnSolutionTakeAmmo(
    EntityUid uid,
    SolutionAmmoProviderComponent component,
    TakeAmmoEvent args)
  {
    int num = Math.Min(args.Shots, component.Shots);
    if (num == 0)
      return;
    for (int index = 0; index < num; ++index)
    {
      List<(EntityUid?, IShootable)> ammo = args.Ammo;
      (EntityUid Entity, IShootable) solutionShot = this.GetSolutionShot(uid, component, args.Coordinates);
      (EntityUid?, IShootable) valueTuple = (new EntityUid?(solutionShot.Entity), solutionShot.Item2);
      ammo.Add(valueTuple);
      --component.Shots;
    }
    this.UpdateSolutionShots(uid, component);
    this.UpdateSolutionAppearance(uid, component);
  }

  private void OnSolutionAmmoCount(
    EntityUid uid,
    SolutionAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    args.Count = component.Shots;
    args.Capacity = component.MaxShots;
  }

  protected virtual void UpdateSolutionShots(
    EntityUid uid,
    SolutionAmmoProviderComponent component,
    Solution? solution = null)
  {
  }

  protected virtual (EntityUid Entity, IShootable) GetSolutionShot(
    EntityUid uid,
    SolutionAmmoProviderComponent component,
    EntityCoordinates position)
  {
    EntityUid uid1 = this.Spawn((string) component.Prototype, position);
    return (uid1, this.EnsureShootable(uid1));
  }

  protected void UpdateSolutionAppearance(EntityUid uid, SolutionAmmoProviderComponent component)
  {
    AppearanceComponent comp;
    if (!this.TryComp<AppearanceComponent>(uid, out comp))
      return;
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.HasAmmo, (object) (component.Shots != 0), comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoCount, (object) component.Shots, comp);
    this.Appearance.SetData(uid, (Enum) AmmoVisuals.AmmoMax, (object) component.MaxShots, comp);
  }

  [NetSerializable]
  [Serializable]
  private sealed class BatteryAmmoProviderComponentState : ComponentState
  {
    public int Shots;
    public int MaxShots;
    public float FireCost;
  }

  [NetSerializable]
  [Serializable]
  public sealed class HitscanEvent : EntityEventArgs
  {
    public List<(NetCoordinates coordinates, Angle angle, SpriteSpecifier Sprite, float Distance)> Sprites = new List<(NetCoordinates, Angle, SpriteSpecifier, float)>();
  }

  private sealed class CycleModeEvent : 
    InstantActionEvent,
    ISerializationGenerated<SharedGunSystem.CycleModeEvent>,
    ISerializationGenerated
  {
    public SelectiveFire Mode;

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref SharedGunSystem.CycleModeEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      InstantActionEvent target1 = (InstantActionEvent) target;
      this.InternalCopy(ref target1, serialization, hookCtx, context);
      target = (SharedGunSystem.CycleModeEvent) target1;
      serialization.TryCustomCopy<SharedGunSystem.CycleModeEvent>(this, ref target, hookCtx, false, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref SharedGunSystem.CycleModeEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref InstantActionEvent target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedGunSystem.CycleModeEvent target1 = (SharedGunSystem.CycleModeEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (InstantActionEvent) target1;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public override void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      SharedGunSystem.CycleModeEvent target1 = (SharedGunSystem.CycleModeEvent) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [PreserveBaseOverrides]
    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    virtual SharedGunSystem.CycleModeEvent InstantActionEvent.Instantiate()
    {
      return new SharedGunSystem.CycleModeEvent();
    }
  }

  [NetSerializable]
  [Serializable]
  protected sealed class RevolverAmmoProviderComponentState : ComponentState
  {
    public int CurrentIndex;
    public List<NetEntity?> AmmoSlots;
    public bool?[] Chambers;
  }

  public sealed class RevolverSpinEvent : EntityEventArgs
  {
  }
}
