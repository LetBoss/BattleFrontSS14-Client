// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleAmmoLoaderSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Vehicle.Weapons;
using Content.Shared._RMC14.Weapons.Ranged.Ammo.BulletBox;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.DoAfter;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

public sealed class RMCVehicleAmmoLoaderSystem : EntitySystem
{
  [Dependency]
  private BulletBoxSystem _bulletBox;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private ItemSlotsSystem _itemSlots;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedUserInterfaceSystem _ui;
  [Dependency]
  private VehicleSystem _vehicleSystem;
  private readonly Dictionary<EntityUid, Dictionary<EntityUid, EntityUid>> _activeAmmoBoxes = new Dictionary<EntityUid, Dictionary<EntityUid, EntityUid>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, VehicleAmmoLoaderDoAfterEvent>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, VehicleAmmoLoaderDoAfterEvent>(this.OnLoadDoAfter));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, BoundUIOpenedEvent>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, BoundUIOpenedEvent>(this.OnUiOpened));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, BoundUIClosedEvent>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, BoundUIClosedEvent>(this.OnUiClosed));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderSelectMessage>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderSelectMessage>(this.OnUiSelect));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderRefreshMessage>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, RMCVehicleAmmoLoaderRefreshMessage>(this.OnUiRefresh));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ShellTypeSelectMessage>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, ShellTypeSelectMessage>(this.OnShellTypeSelect));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ShellSelectionCancelMessage>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, ShellSelectionCancelMessage>(this.OnShellSelectionCancel));
    this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ComponentShutdown>(new EntityEventRefHandler<RMCVehicleAmmoLoaderComponent, ComponentShutdown>(this.OnLoaderShutdown));
    this.SubscribeLocalEvent<ShellSelectionComponent, PlayerDetachedEvent>(new EntityEventRefHandler<ShellSelectionComponent, PlayerDetachedEvent>(this.OnPlayerDetached));
  }

  private void OnInteractUsing(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref InteractUsingEvent args)
  {
    BulletBoxComponent comp1;
    UserInterfaceComponent comp2;
    if (args.Handled || this._net.IsClient || !this.TryComp<BulletBoxComponent>(args.Used, out comp1) || !this.TryComp<UserInterfaceComponent>(ent.Owner, out comp2) || !this._ui.HasUi(ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, comp2))
      return;
    EntityUid? vehicle;
    if (!this._vehicleSystem.TryGetVehicleFromInterior(ent.Owner, out vehicle) || !vehicle.HasValue)
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-no-vehicle"), (EntityUid) ent, new EntityUid?(args.User));
    else if (comp1.Amount <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-empty", ("box", (object) args.Used)), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      if (ent.Comp.BulletType.HasValue)
      {
        EntProtoId? bulletType1 = ent.Comp.BulletType;
        EntProtoId bulletType2 = comp1.BulletType;
        if ((bulletType1.HasValue ? (bulletType1.GetValueOrDefault() != bulletType2 ? 1 : 0) : 1) != 0)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-wrong-ammo"), (EntityUid) ent, new EntityUid?(args.User));
          return;
        }
      }
      HardpointSlotsComponent comp3;
      ItemSlotsComponent comp4;
      if (!this.TryComp<HardpointSlotsComponent>(vehicle.Value, out comp3) || !this.TryComp<ItemSlotsComponent>(vehicle.Value, out comp4))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-no-hardpoint"), (EntityUid) ent, new EntityUid?(args.User));
      }
      else
      {
        this.TryFindAmmoProvider(vehicle.Value, comp3, comp4, ent.Comp, comp1, (string) null, out EntityUid _, out BallisticAmmoProviderComponent _, out RMCVehicleHardpointAmmoComponent _);
        if (!this.CanOpenUi(ent.Owner, args.User))
          return;
        this.TrySetActiveAmmoBox(ent.Owner, args.User, args.Used);
        if (ent.Comp.EnableShellSelection)
        {
          if (!this.CanOpenShellSelectionUi(ent.Owner, args.User))
            return;
          if (!this._ui.HasUi(ent.Owner, (Enum) ShellSelectionUiKey.Key, comp2))
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-shell-selection-unavailable"), (EntityUid) ent, new EntityUid?(args.User));
            return;
          }
          ShellSelectionComponent comp5;
          if (!this.TryComp<ShellSelectionComponent>(ent.Owner, out comp5))
            comp5 = this.AddComp<ShellSelectionComponent>(ent.Owner);
          if (comp5.AvailableShells.Count == 0)
            this.PopulateAvailableShells((Entity<ShellSelectionComponent>) (ent.Owner, comp5));
          if (ent.Comp.SelectedShellType.HasValue)
          {
            comp5.SelectedShellType = ent.Comp.SelectedShellType.Value;
            this.Dirty(ent.Owner, (IComponent) comp5);
          }
          this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(args.User));
          this.UpdateShellSelectionUi(ent.Owner, args.User, comp5);
        }
        else
        {
          this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, new EntityUid?(args.User));
          this.UpdateUi(ent.Owner, comp1);
        }
        args.Handled = true;
      }
    }
  }

  private void OnLoadDoAfter(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref VehicleAmmoLoaderDoAfterEvent args)
  {
    if (this._net.IsClient || args.Cancelled || args.Handled)
      return;
    EntityUid? used = args.Used;
    if (!used.HasValue)
      return;
    EntityUid valueOrDefault = used.GetValueOrDefault();
    BulletBoxComponent comp1;
    EntityUid boxUid;
    EntityUid ammoUid;
    BallisticAmmoProviderComponent ammo;
    RMCVehicleHardpointAmmoComponent hardpointAmmo;
    if (!this.TryComp<BulletBoxComponent>(valueOrDefault, out comp1) || !this.TryGetActiveAmmoBox(ent.Owner, args.User, out boxUid) || boxUid != valueOrDefault || string.IsNullOrWhiteSpace(args.SlotId) || !this.CanLoad(ent, args.User, (Entity<BulletBoxComponent>) (valueOrDefault, comp1), args.SlotId, out EntityUid _, out ammoUid, out ammo, out hardpointAmmo))
      return;
    int num = Math.Max(1, hardpointAmmo.MagazineSize);
    if (comp1.Amount < num)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-not-enough"), (EntityUid) ent, new EntityUid?(args.User));
    }
    else
    {
      if (!this._bulletBox.TryConsume((Entity<BulletBoxComponent>) (valueOrDefault, comp1), num))
        return;
      int count1 = ammo.Count;
      EntProtoId? nullable1 = new EntProtoId?();
      RefillableByBulletBoxComponent comp2;
      EntProtoId? nullable2;
      if (this.TryComp<RefillableByBulletBoxComponent>(ammoUid, out comp2) && !comp2.BulletType.HasValue)
      {
        nullable2 = this.GetProjectileProtoFromShell(new EntProtoId?(comp1.BulletType));
        if (!nullable2.HasValue)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-invalid-shell"), (EntityUid) ent, new EntityUid?(args.User));
          return;
        }
      }
      else
        nullable2 = ammo.Proto;
      if (count1 == 0)
      {
        if (nullable2.HasValue)
        {
          ammo.Proto = nullable2;
          this.Dirty(ammoUid, (IComponent) ammo);
        }
        int count2 = Math.Min(num, ammo.Capacity);
        this._gun.SetBallisticUnspawned((Entity<BallisticAmmoProviderComponent>) (ammoUid, ammo), count2);
      }
      else
      {
        ++hardpointAmmo.StoredMagazines;
        if (nullable2.HasValue)
          hardpointAmmo.MagazineProjectileQueue.Enqueue(nullable2.Value);
        this.Dirty(ammoUid, (IComponent) hardpointAmmo);
      }
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-loaded", ("amount", (object) num), ("target", (object) ammoUid)), (EntityUid) ent, new EntityUid?(args.User));
      this.UpdateUi(ent.Owner, comp1);
      args.Handled = true;
    }
  }

  private EntProtoId? GetProjectileProtoFromShell(EntProtoId? shellType)
  {
    if (!shellType.HasValue)
      return new EntProtoId?();
    string projectileProtoFromShell;
    switch (shellType.Value.Id)
    {
      case "PUBGVehicleAmmoBoxTankAPFSDS":
        projectileProtoFromShell = "PUBGVehicleCartridgeTankAPFSDS";
        break;
      case "PUBGVehicleAmmoBoxTankHE":
        projectileProtoFromShell = "PUBGVehicleCartridgeTankHE";
        break;
      case "PUBGVehicleAmmoBoxTankHEAT":
        projectileProtoFromShell = "PUBGVehicleCartridgeTankHEAT";
        break;
      default:
        projectileProtoFromShell = (string) null;
        break;
    }
    return (EntProtoId?) projectileProtoFromShell;
  }

  private string? GetLoadedAmmoTypeName(
    BallisticAmmoProviderComponent ammo,
    RefillableByBulletBoxComponent? refill)
  {
    if ((refill != null ? (!refill.BulletType.HasValue ? 1 : 0) : 1) != 0 && ammo.Proto.HasValue)
    {
      string loadedAmmoTypeName;
      switch (ammo.Proto.Value.Id)
      {
        case "PUBGVehicleCartridgeTankAPFSDS":
          loadedAmmoTypeName = "APFSDS";
          break;
        case "PUBGVehicleCartridgeTankHE":
          loadedAmmoTypeName = "HE";
          break;
        case "PUBGVehicleCartridgeTankHEAT":
          loadedAmmoTypeName = "HEAT";
          break;
        default:
          loadedAmmoTypeName = (string) null;
          break;
      }
      return loadedAmmoTypeName;
    }
    if (refill == null || !refill.BulletType.HasValue)
      return (string) null;
    string loadedAmmoTypeName1;
    switch (refill.BulletType.Value.Id)
    {
      case "PubgMagazineM2":
        loadedAmmoTypeName1 = "M2 Magazine";
        break;
      case "PubgMagazineKord":
        loadedAmmoTypeName1 = "KORD Magazine";
        break;
      default:
        loadedAmmoTypeName1 = refill.BulletType.Value.Id;
        break;
    }
    return loadedAmmoTypeName1;
  }

  private void OnUiOpened(Entity<RMCVehicleAmmoLoaderComponent> ent, ref BoundUIOpenedEvent args)
  {
    EntityUid boxUid;
    BulletBoxComponent comp;
    if (!object.Equals((object) args.UiKey, (object) RMCVehicleAmmoLoaderUiKey.Key) || this._net.IsClient || !this.TryGetActiveAmmoBox(ent.Owner, args.Actor, out boxUid) || !this.TryComp<BulletBoxComponent>(boxUid, out comp))
      return;
    this.UpdateUi(ent.Owner, comp);
  }

  private void OnUiClosed(Entity<RMCVehicleAmmoLoaderComponent> ent, ref BoundUIClosedEvent args)
  {
    if (!object.Equals((object) args.UiKey, (object) RMCVehicleAmmoLoaderUiKey.Key))
      return;
    this.ClearActiveAmmoBox(ent.Owner, args.Actor);
  }

  private void OnUiSelect(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref RMCVehicleAmmoLoaderSelectMessage args)
  {
    EntityUid? nullable1;
    if (!object.Equals((object) args.UiKey, (object) RMCVehicleAmmoLoaderUiKey.Key) || args.Actor == new EntityUid() || !this.Exists(args.Actor) || !this._hands.TryGetActiveItem((Entity<HandsComponent>) args.Actor, out nullable1))
      return;
    EntityUid boxUid;
    if (this.TryGetActiveAmmoBox(ent.Owner, args.Actor, out boxUid))
    {
      EntityUid? nullable2 = nullable1;
      EntityUid vehicle = boxUid;
      if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() != vehicle ? 1 : 0) : 1) == 0)
      {
        BulletBoxComponent comp;
        EntityUid ammoUid;
        BallisticAmmoProviderComponent ammo;
        RMCVehicleHardpointAmmoComponent hardpointAmmo;
        if (!this.TryComp<BulletBoxComponent>(boxUid, out comp) || !this.CanLoad(ent, args.Actor, (Entity<BulletBoxComponent>) (boxUid, comp), args.SlotId, out vehicle, out ammoUid, out ammo, out hardpointAmmo))
          return;
        int num = Math.Max(1, hardpointAmmo.MagazineSize);
        int count = ammo.Count;
        bool flag = hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines;
        if (comp.Amount < num)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-not-enough"), (EntityUid) ent, new EntityUid?(args.Actor));
          return;
        }
        if (count > 0 && !flag)
        {
          this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-full", ("target", (object) ammoUid)), (EntityUid) ent, new EntityUid?(args.Actor));
          return;
        }
        this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.Actor, ent.Comp.LoadDelay, (DoAfterEvent) new VehicleAmmoLoaderDoAfterEvent(args.SlotId), new EntityUid?(ent.Owner), new EntityUid?(ent.Owner), new EntityUid?(boxUid))
        {
          BreakOnMove = true,
          BreakOnDropItem = true,
          NeedHand = true
        });
        return;
      }
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-hold-ammo"), (EntityUid) ent, new EntityUid?(args.Actor));
  }

  private void OnUiRefresh(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref RMCVehicleAmmoLoaderRefreshMessage args)
  {
    if (!object.Equals((object) args.UiKey, (object) RMCVehicleAmmoLoaderUiKey.Key) || this._net.IsClient || args.Actor == new EntityUid() || !this.Exists(args.Actor))
      return;
    EntityUid boxUid;
    BulletBoxComponent comp1;
    if (this.TryGetActiveAmmoBox(ent.Owner, args.Actor, out boxUid) && this.TryComp<BulletBoxComponent>(boxUid, out comp1))
    {
      this.UpdateUi(ent.Owner, comp1);
    }
    else
    {
      EntityUid? uid;
      BulletBoxComponent comp2;
      if (this._hands.TryGetActiveItem((Entity<HandsComponent>) args.Actor, out uid) && this.TryComp<BulletBoxComponent>(uid, out comp2))
      {
        this.TrySetActiveAmmoBox(ent.Owner, args.Actor, uid.Value);
        this.UpdateUi(ent.Owner, comp2);
      }
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-hold-ammo"), (EntityUid) ent, new EntityUid?(args.Actor));
    }
  }

  private void OnShellTypeSelect(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref ShellTypeSelectMessage args)
  {
    if (!object.Equals((object) args.UiKey, (object) ShellSelectionUiKey.Key) || this._net.IsClient || args.Actor == new EntityUid() || !this.Exists(args.Actor))
      return;
    if (!this._prototypes.HasIndex<EntityPrototype>((string) args.SelectedShellType))
    {
      this.Log.Error($"Invalid shell type selected: {args.SelectedShellType} by user {args.Actor}. Defaulting to APFSDS.");
      this._popup.PopupClient(this.Loc.GetString("rmc-shell-selection-invalid"), (EntityUid) ent, new EntityUid?(args.Actor));
      string str = "PUBGVehicleAmmoBoxTankAPFSDS";
      if (this._prototypes.HasIndex<EntityPrototype>(str))
      {
        args = new ShellTypeSelectMessage((EntProtoId) str);
      }
      else
      {
        this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(args.Actor));
        return;
      }
    }
    if (!this.IsShellAvailable(args.Actor, args.SelectedShellType))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-shell-selection-unavailable"), (EntityUid) ent, new EntityUid?(args.Actor));
      this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(args.Actor));
    }
    else
    {
      ShellSelectionComponent comp1;
      if (this.TryComp<ShellSelectionComponent>(ent.Owner, out comp1))
      {
        comp1.SelectedShellType = args.SelectedShellType;
        this.Dirty(ent.Owner, (IComponent) comp1);
      }
      ent.Comp.SelectedShellType = new EntProtoId?(args.SelectedShellType);
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
      EntityUid? nullable = this.SpawnShellInHands(args.Actor, args.SelectedShellType);
      if (!nullable.HasValue)
      {
        this.Log.Error($"Failed to spawn shell {args.SelectedShellType} for user {args.Actor}");
        this._popup.PopupClient(this.Loc.GetString("rmc-shell-selection-invalid"), (EntityUid) ent, new EntityUid?(args.Actor));
        this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(args.Actor));
      }
      else
      {
        this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(args.Actor));
        BulletBoxComponent comp2;
        if (!this.TryComp<BulletBoxComponent>(nullable.Value, out comp2))
          return;
        this._ui.OpenUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, new EntityUid?(args.Actor));
        this.UpdateUi(ent.Owner, comp2);
      }
    }
  }

  private void OnShellSelectionCancel(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref ShellSelectionCancelMessage args)
  {
    if (!object.Equals((object) args.UiKey, (object) ShellSelectionUiKey.Key) || this._net.IsClient || args.Actor == new EntityUid() || !this.Exists(args.Actor))
      return;
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(args.Actor));
  }

  private bool CanOpenUi(EntityUid loader, EntityUid user)
  {
    using (IEnumerator<EntityUid> enumerator = this._ui.GetActors((Entity<UserInterfaceComponent>) loader, (Enum) RMCVehicleAmmoLoaderUiKey.Key).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        if (enumerator.Current == user)
          return true;
        this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-in-use"), loader, new EntityUid?(user));
        return false;
      }
    }
    return true;
  }

  private bool TrySetActiveAmmoBox(EntityUid loader, EntityUid user, EntityUid boxUid)
  {
    Dictionary<EntityUid, EntityUid> dictionary;
    if (!this._activeAmmoBoxes.TryGetValue(loader, out dictionary))
    {
      dictionary = new Dictionary<EntityUid, EntityUid>();
      this._activeAmmoBoxes[loader] = dictionary;
    }
    dictionary[user] = boxUid;
    return true;
  }

  private bool TryGetActiveAmmoBox(EntityUid loader, EntityUid user, out EntityUid boxUid)
  {
    boxUid = new EntityUid();
    Dictionary<EntityUid, EntityUid> dictionary;
    return this._activeAmmoBoxes.TryGetValue(loader, out dictionary) && dictionary.TryGetValue(user, out boxUid);
  }

  private void ClearActiveAmmoBox(EntityUid loader, EntityUid user)
  {
    Dictionary<EntityUid, EntityUid> dictionary;
    if (!this._activeAmmoBoxes.TryGetValue(loader, out dictionary))
      return;
    dictionary.Remove(user);
    if (dictionary.Count != 0)
      return;
    this._activeAmmoBoxes.Remove(loader);
  }

  private bool CanLoad(
    Entity<RMCVehicleAmmoLoaderComponent> loader,
    EntityUid user,
    Entity<BulletBoxComponent> box,
    string? slotId,
    out EntityUid vehicle,
    out EntityUid ammoUid,
    out BallisticAmmoProviderComponent ammo,
    out RMCVehicleHardpointAmmoComponent hardpointAmmo)
  {
    ammoUid = new EntityUid();
    ammo = (BallisticAmmoProviderComponent) null;
    hardpointAmmo = (RMCVehicleHardpointAmmoComponent) null;
    vehicle = new EntityUid();
    EntityUid? vehicle1;
    if (!this._vehicleSystem.TryGetVehicleFromInterior(loader.Owner, out vehicle1) || !vehicle1.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-no-vehicle"), (EntityUid) loader, new EntityUid?(user));
      return false;
    }
    vehicle = vehicle1.Value;
    if (box.Comp.Amount <= 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-empty", (nameof (box), (object) box.Owner)), (EntityUid) loader, new EntityUid?(user));
      return false;
    }
    if (loader.Comp.BulletType.HasValue)
    {
      EntProtoId? bulletType1 = loader.Comp.BulletType;
      EntProtoId bulletType2 = box.Comp.BulletType;
      if ((bulletType1.HasValue ? (bulletType1.GetValueOrDefault() != bulletType2 ? 1 : 0) : 1) != 0)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-wrong-ammo"), (EntityUid) loader, new EntityUid?(user));
        return false;
      }
    }
    HardpointSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if (!this.TryComp<HardpointSlotsComponent>(vehicle, out comp1) || !this.TryComp<ItemSlotsComponent>(vehicle, out comp2))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-no-hardpoint"), (EntityUid) loader, new EntityUid?(user));
      return false;
    }
    if (!this.TryFindAmmoProvider(vehicle, comp1, comp2, loader.Comp, box.Comp, slotId, out ammoUid, out ammo, out hardpointAmmo))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-no-hardpoint"), (EntityUid) loader, new EntityUid?(user));
      return false;
    }
    int count = ammo.Count;
    bool flag = hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines;
    if (count <= 0 || flag)
      return true;
    this._popup.PopupClient(this.Loc.GetString("rmc-vehicle-ammo-loader-full", ("target", (object) ammoUid)), (EntityUid) loader, new EntityUid?(user));
    return false;
  }

  private bool TryFindAmmoProvider(
    EntityUid vehicle,
    HardpointSlotsComponent hardpoints,
    ItemSlotsComponent itemSlots,
    RMCVehicleAmmoLoaderComponent loader,
    BulletBoxComponent box,
    string? slotId,
    out EntityUid ammoUid,
    out BallisticAmmoProviderComponent ammo,
    out RMCVehicleHardpointAmmoComponent hardpointAmmo)
  {
    ammoUid = new EntityUid();
    ammo = (BallisticAmmoProviderComponent) null;
    hardpointAmmo = (RMCVehicleHardpointAmmoComponent) null;
    string parentSlotId;
    string childSlotId;
    if (!string.IsNullOrWhiteSpace(slotId) && VehicleTurretSlotIds.TryParse(slotId, out parentSlotId, out childSlotId))
    {
      HardpointSlot turretSlot;
      EntityUid entityUid;
      return this.TryGetTurretSlot(vehicle, parentSlotId, childSlotId, itemSlots, out turretSlot, out entityUid) && (string.IsNullOrWhiteSpace(loader.HardpointType) || string.Equals(turretSlot.HardpointType, loader.HardpointType, StringComparison.OrdinalIgnoreCase)) && this.TryGetAmmoProviderFromItem(entityUid, box, out ammoUid, out ammo, out hardpointAmmo);
    }
    foreach (HardpointSlot slot1 in hardpoints.Slots)
    {
      ItemSlot itemSlot1;
      if (!string.IsNullOrWhiteSpace(slot1.Id) && (string.IsNullOrWhiteSpace(slotId) || string.Equals(slot1.Id, slotId, StringComparison.OrdinalIgnoreCase)) && this._itemSlots.TryGetSlot(vehicle, slot1.Id, out itemSlot1, itemSlots) && itemSlot1.HasItem)
      {
        EntityUid uid = itemSlot1.Item.Value;
        if ((string.IsNullOrWhiteSpace(loader.HardpointType) ? 1 : (string.Equals(slot1.HardpointType, loader.HardpointType, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) != 0 && this.TryGetAmmoProviderFromItem(uid, box, out ammoUid, out ammo, out hardpointAmmo))
          return true;
        HardpointSlotsComponent comp1;
        ItemSlotsComponent comp2;
        if (this.TryComp<HardpointSlotsComponent>(uid, out comp1) && this.TryComp<ItemSlotsComponent>(uid, out comp2))
        {
          foreach (HardpointSlot slot2 in comp1.Slots)
          {
            ItemSlot itemSlot2;
            if (!string.IsNullOrWhiteSpace(slot2.Id) && (string.IsNullOrWhiteSpace(loader.HardpointType) || string.Equals(slot2.HardpointType, loader.HardpointType, StringComparison.OrdinalIgnoreCase)) && this._itemSlots.TryGetSlot(uid, slot2.Id, out itemSlot2, comp2) && itemSlot2.HasItem && this.TryGetAmmoProviderFromItem(itemSlot2.Item.Value, box, out ammoUid, out ammo, out hardpointAmmo))
              return true;
          }
        }
      }
    }
    return false;
  }

  private void UpdateUi(EntityUid loader, BulletBoxComponent box)
  {
    RMCVehicleAmmoLoaderComponent comp1;
    EntityUid? vehicle;
    HardpointSlotsComponent comp2;
    ItemSlotsComponent comp3;
    if (this._net.IsClient || !this.TryComp<RMCVehicleAmmoLoaderComponent>(loader, out comp1) || !this._vehicleSystem.TryGetVehicleFromInterior(loader, out vehicle) || !vehicle.HasValue || !this.TryComp<HardpointSlotsComponent>(vehicle.Value, out comp2) || !this.TryComp<ItemSlotsComponent>(vehicle.Value, out comp3))
      return;
    List<RMCVehicleAmmoLoaderUiEntry> ammoLoaderUiEntryList = new List<RMCVehicleAmmoLoaderUiEntry>(comp2.Slots.Count);
    foreach (HardpointSlot slot in comp2.Slots)
    {
      ItemSlot itemSlot;
      if (!string.IsNullOrWhiteSpace(slot.Id) && this._itemSlots.TryGetSlot(vehicle.Value, slot.Id, out itemSlot, comp3) && itemSlot.HasItem)
      {
        EntityUid entityUid = itemSlot.Item.Value;
        BallisticAmmoProviderComponent ammo;
        RMCVehicleHardpointAmmoComponent hardpointAmmo;
        if ((string.IsNullOrWhiteSpace(comp1.HardpointType) ? 1 : (string.Equals(slot.HardpointType, comp1.HardpointType, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) != 0 && this.TryGetAmmoProviderFromItem(entityUid, box, out EntityUid _, out ammo, out hardpointAmmo))
        {
          int count = ammo.Count;
          int magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
          bool canLoad = box.Amount >= magazineSize && (count == 0 || hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines);
          RefillableByBulletBoxComponent comp4;
          this.TryComp<RefillableByBulletBoxComponent>(entityUid, out comp4);
          string loadedAmmoTypeName = this.GetLoadedAmmoTypeName(ammo, comp4);
          string installedName = this.Name(entityUid);
          ammoLoaderUiEntryList.Add(new RMCVehicleAmmoLoaderUiEntry(slot.Id, slot.HardpointType, installedName, new NetEntity?(this.GetNetEntity(entityUid)), count, magazineSize, hardpointAmmo.StoredMagazines, hardpointAmmo.MaxStoredMagazines, canLoad, loadedAmmoTypeName));
        }
        this.AppendTurretAmmoEntries(ammoLoaderUiEntryList, entityUid, slot.Id, comp1, box);
      }
    }
    this._ui.SetUiState((Entity<UserInterfaceComponent>) loader, (Enum) RMCVehicleAmmoLoaderUiKey.Key, (BoundUserInterfaceState) new RMCVehicleAmmoLoaderUiState(ammoLoaderUiEntryList, box.Amount, box.Max));
  }

  private void AppendTurretAmmoEntries(
    List<RMCVehicleAmmoLoaderUiEntry> entries,
    EntityUid turretUid,
    string parentSlotId,
    RMCVehicleAmmoLoaderComponent loaderComp,
    BulletBoxComponent box)
  {
    HardpointSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if (!this.TryComp<HardpointSlotsComponent>(turretUid, out comp1) || !this.TryComp<ItemSlotsComponent>(turretUid, out comp2))
      return;
    foreach (HardpointSlot slot in comp1.Slots)
    {
      ItemSlot itemSlot;
      if (!string.IsNullOrWhiteSpace(slot.Id) && (string.IsNullOrWhiteSpace(loaderComp.HardpointType) || string.Equals(slot.HardpointType, loaderComp.HardpointType, StringComparison.OrdinalIgnoreCase)) && this._itemSlots.TryGetSlot(turretUid, slot.Id, out itemSlot, comp2) && itemSlot.HasItem)
      {
        EntityUid uid = itemSlot.Item.Value;
        BallisticAmmoProviderComponent ammo;
        RMCVehicleHardpointAmmoComponent hardpointAmmo;
        if (this.TryGetAmmoProviderFromItem(uid, box, out EntityUid _, out ammo, out hardpointAmmo))
        {
          int count = ammo.Count;
          int magazineSize = Math.Max(1, hardpointAmmo.MagazineSize);
          bool canLoad = box.Amount >= magazineSize && (count == 0 || hardpointAmmo.StoredMagazines < hardpointAmmo.MaxStoredMagazines);
          RefillableByBulletBoxComponent comp3;
          this.TryComp<RefillableByBulletBoxComponent>(uid, out comp3);
          string loadedAmmoTypeName = this.GetLoadedAmmoTypeName(ammo, comp3);
          entries.Add(new RMCVehicleAmmoLoaderUiEntry(VehicleTurretSlotIds.Compose(parentSlotId, slot.Id), slot.HardpointType, this.Name(uid), new NetEntity?(this.GetNetEntity(uid)), count, magazineSize, hardpointAmmo.StoredMagazines, hardpointAmmo.MaxStoredMagazines, canLoad, loadedAmmoTypeName));
        }
      }
    }
  }

  private bool TryGetTurretSlot(
    EntityUid vehicle,
    string parentSlotId,
    string childSlotId,
    ItemSlotsComponent itemSlots,
    out HardpointSlot turretSlot,
    out EntityUid item)
  {
    turretSlot = (HardpointSlot) null;
    item = new EntityUid();
    ItemSlot itemSlot1;
    if (!this._itemSlots.TryGetSlot(vehicle, parentSlotId, out itemSlot1, itemSlots) || !itemSlot1.HasItem)
      return false;
    EntityUid uid = itemSlot1.Item.Value;
    HardpointSlotsComponent comp1;
    ItemSlotsComponent comp2;
    if (!this.TryComp<HardpointSlotsComponent>(uid, out comp1) || !this.TryComp<ItemSlotsComponent>(uid, out comp2))
      return false;
    foreach (HardpointSlot slot in comp1.Slots)
    {
      if (string.Equals(slot.Id, childSlotId, StringComparison.OrdinalIgnoreCase))
      {
        turretSlot = slot;
        ItemSlot itemSlot2;
        if (!this._itemSlots.TryGetSlot(uid, slot.Id, out itemSlot2, comp2) || !itemSlot2.HasItem)
          return false;
        item = itemSlot2.Item.Value;
        return true;
      }
    }
    return false;
  }

  private bool TryGetAmmoProviderFromItem(
    EntityUid item,
    BulletBoxComponent box,
    out EntityUid ammoUid,
    out BallisticAmmoProviderComponent ammo,
    out RMCVehicleHardpointAmmoComponent hardpointAmmo)
  {
    ammoUid = new EntityUid();
    ammo = (BallisticAmmoProviderComponent) null;
    hardpointAmmo = (RMCVehicleHardpointAmmoComponent) null;
    BallisticAmmoProviderComponent comp1;
    RMCVehicleHardpointAmmoComponent comp2;
    RefillableByBulletBoxComponent comp3;
    if (!this.TryComp<BallisticAmmoProviderComponent>(item, out comp1) || !this.TryComp<RMCVehicleHardpointAmmoComponent>(item, out comp2) || !this.TryComp<RefillableByBulletBoxComponent>(item, out comp3))
      return false;
    if (comp3.BulletType.HasValue)
    {
      EntProtoId? bulletType1 = comp3.BulletType;
      EntProtoId bulletType2 = box.BulletType;
      if ((bulletType1.HasValue ? (bulletType1.GetValueOrDefault() != bulletType2 ? 1 : 0) : 1) != 0)
        return false;
    }
    ammoUid = item;
    ammo = comp1;
    hardpointAmmo = comp2;
    return true;
  }

  public void PopulateAvailableShells(Entity<ShellSelectionComponent> shellSelection)
  {
    shellSelection.Comp.AvailableShells.Clear();
    (string, string, string, string)[] valueTupleArray = new (string, string, string, string)[3]
    {
      ("PUBGVehicleAmmoBoxTankAPFSDS", "APFSDS Shell", "Armor-Piercing Fin-Stabilized Discarding Sabot shell with high penetration", "APFSDS"),
      ("PUBGVehicleAmmoBoxTankHE", "HE Shell", "High Explosive shell with area damage and shrapnel", "HE"),
      ("PUBGVehicleAmmoBoxTankHEAT", "HEAT Shell", "High Explosive Anti-Tank shaped charge shell", "HEAT")
    };
    foreach ((string id, string str1, string str2, string str3) in valueTupleArray)
    {
      if (this._prototypes.HasIndex<EntityPrototype>(id))
        shellSelection.Comp.AvailableShells.Add(new ShellTypeInfo()
        {
          ProtoId = (EntProtoId) id,
          Name = str1,
          Description = str2,
          SpriteState = str3
        });
    }
    this.Dirty<ShellSelectionComponent>(shellSelection);
  }

  private void UpdateShellSelectionUi(
    EntityUid loader,
    EntityUid user,
    ShellSelectionComponent shellSelection)
  {
    if (this._net.IsClient)
      return;
    List<ShellTypeEntry> availableShells = new List<ShellTypeEntry>();
    foreach (ShellTypeInfo availableShell in shellSelection.AvailableShells)
    {
      bool isAvailable = this.IsShellAvailable(user, availableShell.ProtoId);
      availableShells.Add(new ShellTypeEntry(availableShell.ProtoId, availableShell.Name, availableShell.Description, isAvailable));
    }
    ShellSelectionUiState state = new ShellSelectionUiState(availableShells, shellSelection.SelectedShellType);
    this._ui.SetUiState((Entity<UserInterfaceComponent>) loader, (Enum) ShellSelectionUiKey.Key, (BoundUserInterfaceState) state);
  }

  public bool IsShellAvailable(EntityUid user, EntProtoId shellProtoId)
  {
    EntityUid? uid;
    BulletBoxComponent comp;
    return this._hands.TryGetActiveItem((Entity<HandsComponent>) user, out uid) && this.TryComp<BulletBoxComponent>(uid, out comp) && !(comp.BulletType != shellProtoId) && comp.Amount > 0;
  }

  public EntityUid? SpawnShellInHands(EntityUid user, EntProtoId shellProtoId)
  {
    if (!this._prototypes.HasIndex<EntityPrototype>((string) shellProtoId))
      return new EntityUid?();
    EntityCoordinates coordinates = this.Transform(user).Coordinates;
    EntityUid entity = this.Spawn((string) shellProtoId, coordinates);
    if (this._hands.TryPickupAnyHand(user, entity))
      return new EntityUid?(entity);
    this.QueueDel(new EntityUid?(entity));
    return new EntityUid?();
  }

  private bool CanOpenShellSelectionUi(EntityUid loader, EntityUid user)
  {
    using (IEnumerator<EntityUid> enumerator = this._ui.GetActors((Entity<UserInterfaceComponent>) loader, (Enum) ShellSelectionUiKey.Key).GetEnumerator())
    {
      if (enumerator.MoveNext())
      {
        if (enumerator.Current == user)
          return true;
        this._popup.PopupClient(this.Loc.GetString("rmc-shell-selection-in-use"), loader, new EntityUid?(user));
        return false;
      }
    }
    return true;
  }

  private void OnPlayerDetached(Entity<ShellSelectionComponent> ent, ref PlayerDetachedEvent args)
  {
    UserInterfaceComponent comp;
    if (!this.TryComp<RMCVehicleAmmoLoaderComponent>(ent.Owner, out RMCVehicleAmmoLoaderComponent _) || !this.TryComp<UserInterfaceComponent>(ent.Owner, out comp))
      return;
    if (this._ui.HasUi(ent.Owner, (Enum) ShellSelectionUiKey.Key, comp))
      this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, args.Player.AttachedEntity);
    if (this._ui.HasUi(ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, comp))
      this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, args.Player.AttachedEntity);
    EntityUid? attachedEntity = args.Player.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid owner = ent.Owner;
    attachedEntity = args.Player.AttachedEntity;
    EntityUid user = attachedEntity.Value;
    this.ClearActiveAmmoBox(owner, user);
  }

  private void OnLoaderShutdown(
    Entity<RMCVehicleAmmoLoaderComponent> ent,
    ref ComponentShutdown args)
  {
    UserInterfaceComponent comp;
    if (!this.TryComp<UserInterfaceComponent>(ent.Owner, out comp))
      return;
    if (this._ui.HasUi(ent.Owner, (Enum) ShellSelectionUiKey.Key, comp))
    {
      foreach (EntityUid entityUid in this._ui.GetActors((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key).ToArray<EntityUid>())
        this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) ShellSelectionUiKey.Key, new EntityUid?(entityUid));
    }
    if (this._ui.HasUi(ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, comp))
    {
      foreach (EntityUid entityUid in this._ui.GetActors((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key).ToArray<EntityUid>())
        this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) RMCVehicleAmmoLoaderUiKey.Key, new EntityUid?(entityUid));
    }
    if (!this._activeAmmoBoxes.ContainsKey(ent.Owner))
      return;
    this._activeAmmoBoxes.Remove(ent.Owner);
  }
}
