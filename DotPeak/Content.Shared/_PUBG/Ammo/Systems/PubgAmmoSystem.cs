// Decompiled with JetBrains decompiler
// Type: Content.Shared._PUBG.Ammo.Systems.PubgAmmoSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Ammo.Components;
using Content.Shared._PUBG.Input;
using Content.Shared._PUBG.Loadout;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Stacks;
using Content.Shared.Tag;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._PUBG.Ammo.Systems;

public sealed class PubgAmmoSystem : EntitySystem
{
  [Dependency]
  private IPrototypeManager _proto;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private INetManager _netMan;
  [Dependency]
  private SharedStackSystem _stack;
  [Dependency]
  private TagSystem _tag;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private PubgWeaponModulesSystem _modules;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, TakeAmmoEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, TakeAmmoEvent>(this.OnTakeAmmo));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, GetAmmoCountEvent>(new ComponentEventRefHandler<PubgAmmoProviderComponent, GetAmmoCountEvent>(this.OnGetAmmoCount));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, PubgReloadDoAfterEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, PubgReloadDoAfterEvent>(this.OnReloadComplete));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, PubgUnloadDoAfterEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, PubgUnloadDoAfterEvent>(this.OnUnloadComplete));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, GunShotEvent>(new ComponentEventRefHandler<PubgAmmoProviderComponent, GunShotEvent>(this.OnGunShot));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, GotEquippedHandEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, GotEquippedHandEvent>(this.OnEquippedHand));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, GotUnequippedHandEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, GotUnequippedHandEvent>(this.OnUnequippedHand));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, HandSelectedEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, HandSelectedEvent>(this.OnHandSelected));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, HandDeselectedEvent>(new ComponentEventHandler<PubgAmmoProviderComponent, HandDeselectedEvent>(this.OnHandDeselected));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, EntInsertedIntoContainerMessage>(new ComponentEventHandler<PubgAmmoProviderComponent, EntInsertedIntoContainerMessage>(this.OnWeaponContainerInserted));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, EntRemovedFromContainerMessage>(new ComponentEventHandler<PubgAmmoProviderComponent, EntRemovedFromContainerMessage>(this.OnWeaponContainerRemoved));
    this.SubscribeLocalEvent<EntInsertedIntoContainerMessage>(new EntityEventHandler<EntInsertedIntoContainerMessage>(this.OnItemInserted));
    this.SubscribeLocalEvent<EntRemovedFromContainerMessage>(new EntityEventHandler<EntRemovedFromContainerMessage>(this.OnItemRemoved));
    this.SubscribeLocalEvent<StackComponent, StackCountChangedEvent>(new ComponentEventHandler<StackComponent, StackCountChangedEvent>(this.OnStackCountChanged));
    this.SubscribeLocalEvent<PubgAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<PubgAmmoProviderComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<PubgMagazineModuleAmmoComponent, GetVerbsEvent<AlternativeVerb>>(new ComponentEventHandler<PubgMagazineModuleAmmoComponent, GetVerbsEvent<AlternativeVerb>>(this.OnMagazineVerbs));
    this.SubscribeNetworkEvent<PubgReloadRequestEvent>(new EntitySessionEventHandler<PubgReloadRequestEvent>(this.OnReloadRequest));
    this.SubscribeNetworkEvent<PubgUnloadRequestEvent>(new EntitySessionEventHandler<PubgUnloadRequestEvent>(this.OnUnloadRequest));
    this.SubscribeNetworkEvent<PubgAmmoRefreshRequestEvent>(new EntitySessionEventHandler<PubgAmmoRefreshRequestEvent>(this.OnRefreshRequest));
    CommandBinds.Builder.Bind(PubgKeyFunctions.PubgReload, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      if (!this._netMan.IsClient)
        return;
      this.RaiseNetworkEvent((EntityEventArgs) new PubgReloadRequestEvent());
    }), handle: false)).Bind(PubgKeyFunctions.PubgUnload, InputCmdHandler.FromDelegate((StateInputCmdDelegate) (session =>
    {
      if (!this._netMan.IsClient)
        return;
      this.RaiseNetworkEvent((EntityEventArgs) new PubgUnloadRequestEvent());
    }), handle: false)).Register<PubgAmmoSystem>();
  }

  public override void Shutdown()
  {
    base.Shutdown();
    CommandBinds.Unregister<PubgAmmoSystem>();
  }

  private void OnReloadRequest(PubgReloadRequestEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid? uid;
    PubgAmmoProviderComponent comp;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) valueOrDefault, out uid) || !this.TryComp<PubgAmmoProviderComponent>(uid, out comp))
      return;
    this.TryStartReload(uid.Value, valueOrDefault, comp);
  }

  private void OnUnloadRequest(PubgUnloadRequestEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    EntityUid? uid;
    PubgAmmoProviderComponent comp;
    if (!this._hands.TryGetActiveItem((Entity<HandsComponent>) valueOrDefault, out uid) || !this.TryComp<PubgAmmoProviderComponent>(uid, out comp))
      return;
    this.TryStartUnload(uid.Value, valueOrDefault, comp);
  }

  private void OnRefreshRequest(PubgAmmoRefreshRequestEvent msg, EntitySessionEventArgs args)
  {
    EntityUid? attachedEntity = args.SenderSession.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    this.SendAmmoUiState(attachedEntity.GetValueOrDefault());
  }

  public void SendAmmoUiState(EntityUid userUid)
  {
    if (!this._netMan.IsServer)
      return;
    EntityUid? nullable;
    PubgAmmoProviderComponent comp;
    if (this._hands.TryGetActiveItem((Entity<HandsComponent>) userUid, out nullable) && this.TryComp<PubgAmmoProviderComponent>(nullable.Value, out comp))
      this.UpdateAmmoUI(userUid, nullable.Value, comp);
    else
      this.RaiseNetworkEvent((EntityEventArgs) new PubgAmmoUpdateEvent(0, 0, 0), userUid);
  }

  private void OnGetVerbs(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) args.User);
    EntityUid entityUid = uid;
    if ((activeItem.HasValue ? (activeItem.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return;
    int ammoCount;
    if (this.TryGetWeaponUnloadInfo(uid, component, out ammoCount, out string _) && ammoCount > 0)
    {
      SortedSet<AlternativeVerb> verbs = args.Verbs;
      AlternativeVerb alternativeVerb = new AlternativeVerb();
      alternativeVerb.Act = (Action) (() => this.TryStartUnload(uid, args.User, component));
      alternativeVerb.Text = this.Loc.GetString("pubg-loadout-verb-unload-weapon");
      alternativeVerb.Priority = 1;
      verbs.Add(alternativeVerb);
    }
    if (!this._modules.HasRequiredModulesForReload(uid))
      return;
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(uid, component);
    if (this.GetCurrentAmmoCount(uid, component) >= effectiveMaxAmmo || this.CountAmmoInInventory(args.User, component.AmmoTag) == 0)
      return;
    AlternativeVerb alternativeVerb1 = new AlternativeVerb();
    alternativeVerb1.Act = (Action) (() => this.TryStartReload(uid, args.User, component));
    alternativeVerb1.Text = "Перезарядить";
    alternativeVerb1.Priority = 2;
    AlternativeVerb alternativeVerb2 = alternativeVerb1;
    args.Verbs.Add(alternativeVerb2);
  }

  private void OnMagazineVerbs(
    EntityUid uid,
    PubgMagazineModuleAmmoComponent component,
    GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || component.CurrentAmmo <= 0 || string.IsNullOrWhiteSpace(this.GetAmmoStackPrototype(component)))
      return;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Act = (Action) (() => this.UnloadMagazine(uid, args.User, component));
    alternativeVerb.Text = this.Loc.GetString("pubg-loadout-verb-unload-magazine");
    alternativeVerb.Priority = 1;
    verbs.Add(alternativeVerb);
  }

  private void OnTakeAmmo(EntityUid uid, PubgAmmoProviderComponent component, TakeAmmoEvent args)
  {
    int currentAmmoCount = this.GetCurrentAmmoCount(uid, component);
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(uid, component);
    int num = Math.Min(args.Shots, currentAmmoCount);
    if (num == 0)
    {
      args.Reason = $"No ammo in gun ({currentAmmoCount}/{effectiveMaxAmmo})";
    }
    else
    {
      for (int index = 0; index < num; ++index)
      {
        if (!this._proto.TryIndex<EntityPrototype>(component.AmmoPrototype, out EntityPrototype _))
        {
          args.Reason = "Unknown ammo prototype: " + component.AmmoPrototype;
          return;
        }
        EntityUid uid1 = this.Spawn(component.AmmoPrototype, args.Coordinates);
        args.Ammo.Add((new EntityUid?(uid1), this._gun.EnsureShootable(uid1)));
      }
      this.SetCurrentAmmoCount(uid, currentAmmoCount - num, component);
      if (!args.User.HasValue)
        return;
      this.UpdateAmmoUI(args.User.Value, uid, component);
    }
  }

  private void OnGetAmmoCount(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    ref GetAmmoCountEvent args)
  {
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(uid, component);
    args.Count = Math.Min(this.GetCurrentAmmoCount(uid, component), effectiveMaxAmmo);
    args.Capacity = effectiveMaxAmmo;
  }

  private void OnGunShot(EntityUid uid, PubgAmmoProviderComponent component, ref GunShotEvent args)
  {
    if (component.IsReloading)
    {
      component.IsReloading = false;
      DoAfterId? activeReloadDoAfter = component.ActiveReloadDoAfter;
      if (activeReloadDoAfter.HasValue)
        this._doAfter.Cancel(new DoAfterId?(activeReloadDoAfter.GetValueOrDefault()));
      component.ActiveReloadDoAfter = new DoAfterId?();
      this.Dirty(uid, (IComponent) component);
    }
    if (!component.IsUnloading)
      return;
    component.IsUnloading = false;
    DoAfterId? activeUnloadDoAfter = component.ActiveUnloadDoAfter;
    if (activeUnloadDoAfter.HasValue)
      this._doAfter.Cancel(new DoAfterId?(activeUnloadDoAfter.GetValueOrDefault()));
    component.ActiveUnloadDoAfter = new DoAfterId?();
    this.Dirty(uid, (IComponent) component);
  }

  public int GetCurrentAmmoCount(EntityUid gunUid, PubgAmmoProviderComponent? component = null)
  {
    if (!this.Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
      return 0;
    int currentAmmo = this.ResolveCurrentAmmo(gunUid, component);
    this.SyncWeaponCurrentAmmo(gunUid, component, currentAmmo);
    return currentAmmo;
  }

  public int GetMaxAmmoCount(EntityUid gunUid, PubgAmmoProviderComponent? component = null)
  {
    return !this.Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false) ? 0 : this.GetEffectiveMaxAmmo(gunUid, component);
  }

  public void SetCurrentAmmoCount(
    EntityUid gunUid,
    int currentAmmo,
    PubgAmmoProviderComponent? component = null)
  {
    if (!this.Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
      return;
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
    int currentAmmo1 = Math.Clamp(currentAmmo, 0, effectiveMaxAmmo);
    EntityUid moduleUid;
    PubgMagazineModuleAmmoComponent magazineAmmo;
    if (this.TryGetMagazineAmmoModule(gunUid, component, out moduleUid, out magazineAmmo))
    {
      bool flag = false;
      if (magazineAmmo.AmmoTag != component.AmmoTag)
      {
        magazineAmmo.AmmoTag = component.AmmoTag;
        flag = true;
      }
      if (magazineAmmo.AmmoStackPrototype != component.AmmoTag)
      {
        magazineAmmo.AmmoStackPrototype = component.AmmoTag;
        flag = true;
      }
      if (magazineAmmo.Capacity != effectiveMaxAmmo)
      {
        magazineAmmo.Capacity = effectiveMaxAmmo;
        flag = true;
      }
      if (magazineAmmo.CurrentAmmo != currentAmmo1)
      {
        magazineAmmo.CurrentAmmo = currentAmmo1;
        flag = true;
      }
      if (flag && this._netMan.IsServer)
        this.Dirty(moduleUid, (IComponent) magazineAmmo);
      this.SyncWeaponCurrentAmmo(gunUid, component, currentAmmo1);
      this.RefreshAmmoVisuals(gunUid, component, new int?(currentAmmo1));
    }
    else if (this.UsesMagazineModuleAmmo(gunUid))
    {
      this.SyncWeaponCurrentAmmo(gunUid, component, 0);
      this.RefreshAmmoVisuals(gunUid, component, new int?(0));
    }
    else
      this.SyncWeaponCurrentAmmo(gunUid, component, currentAmmo1);
  }

  public void RefreshHeldWeaponAmmoUi(EntityUid gunUid, PubgAmmoProviderComponent? component = null)
  {
    if (!this.Resolve<PubgAmmoProviderComponent>(gunUid, ref component, false))
      return;
    this.UpdateHeldWeaponAmmoUi(gunUid, component);
  }

  public bool TryStartReload(
    EntityUid gunUid,
    EntityUid userUid,
    PubgAmmoProviderComponent? component = null)
  {
    if (!this.Resolve<PubgAmmoProviderComponent>(gunUid, ref component) || !this._modules.HasRequiredModulesForReload(gunUid) || component.IsReloading)
      return false;
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
    if (this.GetCurrentAmmoCount(gunUid, component) >= effectiveMaxAmmo || this.CountAmmoInInventory(userUid, component.AmmoTag) == 0)
      return false;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) userUid);
    EntityUid entityUid = gunUid;
    if ((activeItem.HasValue ? (activeItem.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return false;
    float reloadTimeMultiplier = this._modules.GetReloadTimeMultiplier(gunUid);
    float num = MathF.Max(0.1f, component.ReloadTime * reloadTimeMultiplier);
    DoAfterId? id;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userUid, TimeSpan.FromSeconds((double) num), (DoAfterEvent) new PubgReloadDoAfterEvent(), new EntityUid?(gunUid), new EntityUid?(gunUid), new EntityUid?(gunUid))
    {
      BreakOnMove = false,
      BreakOnDamage = false,
      NeedHand = true
    }, out id))
      return false;
    component.IsReloading = true;
    component.ActiveReloadDoAfter = id;
    this.Dirty(gunUid, (IComponent) component);
    if (component.ReloadSound != null && this._netMan.IsServer)
      this._audio.PlayPvs(component.ReloadSound, gunUid);
    return true;
  }

  private void OnReloadComplete(
    EntityUid gunUid,
    PubgAmmoProviderComponent component,
    PubgReloadDoAfterEvent args)
  {
    component.ActiveReloadDoAfter = new DoAfterId?();
    if (!component.IsReloading)
    {
      this.Dirty(gunUid, (IComponent) component);
    }
    else
    {
      component.IsReloading = false;
      if (args.Cancelled || args.Handled)
        this.Dirty(gunUid, (IComponent) component);
      else if (!this._modules.HasRequiredModulesForReload(gunUid))
      {
        this.Dirty(gunUid, (IComponent) component);
        this.UpdateAmmoUI(args.User, gunUid, component);
      }
      else
      {
        args.Handled = true;
        int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
        int currentAmmoCount = this.GetCurrentAmmoCount(gunUid, component);
        int val2 = Math.Max(0, effectiveMaxAmmo - currentAmmoCount);
        int amount = component.ReloadAmount > 0 ? Math.Min(component.ReloadAmount, val2) : val2;
        int ammoFromInventory = this.TakeAmmoFromInventory(args.User, component.AmmoTag, amount);
        this.SetCurrentAmmoCount(gunUid, currentAmmoCount + ammoFromInventory, component);
        this.UpdateAmmoUI(args.User, gunUid, component);
        if (component.ReloadAmount <= 0 || ammoFromInventory <= 0 || currentAmmoCount + ammoFromInventory >= effectiveMaxAmmo || this.CountAmmoInInventory(args.User, component.AmmoTag) <= 0)
          return;
        EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) args.User);
        EntityUid entityUid = gunUid;
        if ((activeItem.HasValue ? (activeItem.GetValueOrDefault() == entityUid ? 1 : 0) : 0) == 0)
          return;
        this.TryStartReload(gunUid, args.User, component);
      }
    }
  }

  private void UpdateAmmoUI(
    EntityUid userUid,
    EntityUid gunUid,
    PubgAmmoProviderComponent component)
  {
    if (!this._netMan.IsServer)
      return;
    int reserveAmmo = this.CountAmmoInInventory(userUid, component.AmmoTag);
    string ammoTypeDisplay = component.GetAmmoTypeDisplay();
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAmmoUpdateEvent(Math.Min(this.GetCurrentAmmoCount(gunUid, component), effectiveMaxAmmo), effectiveMaxAmmo, reserveAmmo, ammoTypeDisplay), userUid);
  }

  private void OnEquippedHand(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    GotEquippedHandEvent args)
  {
    this.UpdateAmmoUI(args.User, uid, component);
  }

  private void OnUnequippedHand(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    GotUnequippedHandEvent args)
  {
    if (!this._netMan.IsServer)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAmmoUpdateEvent(0, 0, 0), args.User);
  }

  private void OnHandSelected(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    HandSelectedEvent args)
  {
    this.UpdateAmmoUI(args.User, uid, component);
  }

  private void OnHandDeselected(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    HandDeselectedEvent args)
  {
    if (!this._netMan.IsServer)
      return;
    this.RaiseNetworkEvent((EntityEventArgs) new PubgAmmoUpdateEvent(0, 0, 0), args.User);
  }

  private void OnWeaponContainerInserted(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    EntInsertedIntoContainerMessage args)
  {
    if (!this.TryGetAmmoStorageSlot(uid, args.Container.ID, out PubgWeaponModuleSlotDefinition _))
      return;
    this.PrepareInsertedMagazine(uid, component, args.Entity);
    this.SyncWeaponCurrentAmmo(uid, component, this.ResolveCurrentAmmo(uid, component));
    this.RefreshAmmoVisuals(uid, component);
    this.UpdateHeldWeaponAmmoUi(uid, component);
  }

  private void OnWeaponContainerRemoved(
    EntityUid uid,
    PubgAmmoProviderComponent component,
    EntRemovedFromContainerMessage args)
  {
    if (!this.TryGetAmmoStorageSlot(uid, args.Container.ID, out PubgWeaponModuleSlotDefinition _))
      return;
    this.SyncWeaponCurrentAmmo(uid, component, this.ResolveCurrentAmmo(uid, component));
    this.RefreshAmmoVisuals(uid, component);
    this.UpdateHeldWeaponAmmoUi(uid, component);
  }

  private void OnItemInserted(EntInsertedIntoContainerMessage args)
  {
    if (!this.HasComp<StackComponent>(args.Entity))
      return;
    EntityUid owner = args.Container.Owner;
    EntityUid? nullable1 = new EntityUid?();
    for (int index = 0; owner.IsValid() && index < 10; ++index)
    {
      if (this.HasComp<InventoryComponent>(owner))
      {
        nullable1 = new EntityUid?(owner);
        break;
      }
      BaseContainer container;
      if (this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) owner, out container))
        owner = container.Owner;
      else
        break;
    }
    HandsComponent comp1;
    if (!nullable1.HasValue || !this.TryComp<HandsComponent>(nullable1.Value, out comp1))
      return;
    EntityUid? nullable2 = new EntityUid?();
    PubgAmmoProviderComponent component = (PubgAmmoProviderComponent) null;
    foreach (string key in comp1.Hands.Keys)
    {
      EntityUid? held;
      PubgAmmoProviderComponent comp2;
      if (this._hands.TryGetHeldItem((Entity<HandsComponent>) nullable1.Value, key, out held) && this.TryComp<PubgAmmoProviderComponent>(held.Value, out comp2))
      {
        nullable2 = new EntityUid?(held.Value);
        component = comp2;
        break;
      }
    }
    if (!nullable2.HasValue || component == null || !this._tag.HasTag(args.Entity, (ProtoId<TagPrototype>) component.AmmoTag))
      return;
    this.UpdateAmmoUI(nullable1.Value, nullable2.Value, component);
  }

  private void OnItemRemoved(EntRemovedFromContainerMessage args)
  {
    if (!this.HasComp<StackComponent>(args.Entity))
      return;
    EntityUid owner = args.Container.Owner;
    EntityUid? nullable1 = new EntityUid?();
    BaseContainer container;
    for (; owner.IsValid(); owner = container.Owner)
    {
      if (this.HasComp<InventoryComponent>(owner))
      {
        nullable1 = new EntityUid?(owner);
        break;
      }
      if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) owner, out container))
        break;
    }
    HandsComponent comp1;
    if (!nullable1.HasValue || !this.TryComp<HandsComponent>(nullable1.Value, out comp1))
      return;
    EntityUid? nullable2 = new EntityUid?();
    PubgAmmoProviderComponent component = (PubgAmmoProviderComponent) null;
    foreach (string key in comp1.Hands.Keys)
    {
      EntityUid? held;
      PubgAmmoProviderComponent comp2;
      if (this._hands.TryGetHeldItem((Entity<HandsComponent>) nullable1.Value, key, out held) && this.TryComp<PubgAmmoProviderComponent>(held.Value, out comp2))
      {
        nullable2 = new EntityUid?(held.Value);
        component = comp2;
        break;
      }
    }
    if (!nullable2.HasValue || component == null || !this._tag.HasTag(args.Entity, (ProtoId<TagPrototype>) component.AmmoTag))
      return;
    this.UpdateAmmoUI(nullable1.Value, nullable2.Value, component);
  }

  private void OnStackCountChanged(
    EntityUid uid,
    StackComponent component,
    StackCountChangedEvent args)
  {
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) uid, out container))
      return;
    for (EntityUid owner = container.Owner; owner.IsValid(); owner = container.Owner)
    {
      if (this.HasComp<InventoryComponent>(owner))
      {
        HandsComponent comp1;
        if (!this.TryComp<HandsComponent>(owner, out comp1))
          break;
        using (Dictionary<string, Hand>.KeyCollection.Enumerator enumerator = comp1.Hands.Keys.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            string current = enumerator.Current;
            EntityUid? held;
            PubgAmmoProviderComponent comp2;
            if (this._hands.TryGetHeldItem((Entity<HandsComponent>) owner, current, out held) && this.TryComp<PubgAmmoProviderComponent>(held.Value, out comp2))
            {
              if (!this._tag.HasTag(uid, (ProtoId<TagPrototype>) comp2.AmmoTag))
                break;
              this.UpdateAmmoUI(owner, held.Value, comp2);
              break;
            }
          }
          break;
        }
      }
      if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) owner, out container))
        break;
    }
  }

  private bool TryGetAmmoStorageSlot(
    EntityUid gunUid,
    string containerId,
    out PubgWeaponModuleSlotDefinition slotDefinition)
  {
    slotDefinition = (PubgWeaponModuleSlotDefinition) null;
    PubgWeaponModulesComponent comp;
    return this.TryComp<PubgWeaponModulesComponent>(gunUid, out comp) && this._modules.TryGetSlotDefinition(gunUid, containerId, out slotDefinition, comp) && slotDefinition.StoresAmmo;
  }

  private bool UsesMagazineModuleAmmo(EntityUid gunUid)
  {
    PubgWeaponModulesComponent comp;
    PubgWeaponModuleSlotDefinition slotDefinition;
    return this.TryComp<PubgWeaponModulesComponent>(gunUid, out comp) && this._modules.TryGetSlotDefinition(gunUid, PubgModuleSlotType.Magazine, out slotDefinition, comp) && slotDefinition.StoresAmmo;
  }

  private bool TryGetWeaponUnloadInfo(
    EntityUid weaponUid,
    PubgAmmoProviderComponent component,
    out int ammoCount,
    out string ammoPrototype)
  {
    ammoCount = 0;
    ammoPrototype = string.Empty;
    PubgMagazineModuleAmmoComponent magazineAmmo;
    if (this.TryGetMagazineAmmoModule(weaponUid, component, out EntityUid _, out magazineAmmo))
    {
      ammoCount = magazineAmmo.CurrentAmmo;
      ammoPrototype = this.GetAmmoStackPrototype(magazineAmmo);
      return ammoCount > 0 && !string.IsNullOrWhiteSpace(ammoPrototype);
    }
    ammoCount = this.GetCurrentAmmoCount(weaponUid, component);
    ammoPrototype = component.AmmoTag;
    return ammoCount > 0 && !string.IsNullOrWhiteSpace(ammoPrototype);
  }

  private bool TryGetMagazineAmmoModule(
    EntityUid gunUid,
    PubgAmmoProviderComponent component,
    out EntityUid moduleUid,
    out PubgMagazineModuleAmmoComponent magazineAmmo)
  {
    moduleUid = EntityUid.Invalid;
    magazineAmmo = (PubgMagazineModuleAmmoComponent) null;
    PubgWeaponModulesComponent comp1;
    PubgWeaponModuleSlotDefinition slotDefinition;
    PubgMagazineModuleAmmoComponent comp2;
    if (!this.TryComp<PubgWeaponModulesComponent>(gunUid, out comp1) || !this._modules.TryGetInstalledModule(gunUid, PubgModuleSlotType.Magazine, out moduleUid, out slotDefinition, comp1) || !slotDefinition.StoresAmmo || !this.TryComp<PubgMagazineModuleAmmoComponent>(moduleUid, out comp2))
      return false;
    magazineAmmo = comp2;
    return true;
  }

  private bool TryGetOwningWeapon(
    EntityUid moduleUid,
    out EntityUid weaponUid,
    out PubgAmmoProviderComponent ammoProvider)
  {
    weaponUid = EntityUid.Invalid;
    ammoProvider = (PubgAmmoProviderComponent) null;
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) moduleUid, out container))
      return false;
    weaponUid = container.Owner;
    PubgAmmoProviderComponent comp;
    if (!this.TryComp<PubgAmmoProviderComponent>(weaponUid, out comp) || comp == null)
      return false;
    ammoProvider = comp;
    return true;
  }

  private string GetAmmoStackPrototype(PubgMagazineModuleAmmoComponent component)
  {
    return !string.IsNullOrWhiteSpace(component.AmmoStackPrototype) ? component.AmmoStackPrototype : component.AmmoTag;
  }

  private void PrepareInsertedMagazine(
    EntityUid gunUid,
    PubgAmmoProviderComponent component,
    EntityUid moduleUid)
  {
    PubgMagazineModuleAmmoComponent comp;
    if (!this.TryComp<PubgMagazineModuleAmmoComponent>(moduleUid, out comp))
      return;
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
    bool flag = false;
    int num = Math.Clamp(comp.CurrentAmmo, 0, effectiveMaxAmmo);
    if (comp.CurrentAmmo != num)
    {
      comp.CurrentAmmo = num;
      flag = true;
    }
    if (string.IsNullOrEmpty(comp.AmmoTag) && comp.CurrentAmmo == 0 && component.CurrentAmmo > 0)
    {
      comp.CurrentAmmo = Math.Clamp(component.CurrentAmmo, 0, effectiveMaxAmmo);
      flag = true;
    }
    if (!comp.AmmoTag.Equals(component.AmmoTag, StringComparison.Ordinal))
    {
      if (!string.IsNullOrEmpty(comp.AmmoTag) && comp.CurrentAmmo != 0)
        comp.CurrentAmmo = 0;
      comp.AmmoTag = component.AmmoTag;
      flag = true;
    }
    if (comp.AmmoStackPrototype != component.AmmoTag)
    {
      comp.AmmoStackPrototype = component.AmmoTag;
      flag = true;
    }
    if (comp.Capacity != effectiveMaxAmmo)
    {
      comp.Capacity = effectiveMaxAmmo;
      flag = true;
    }
    if (!flag || !this._netMan.IsServer)
      return;
    this.Dirty(moduleUid, (IComponent) comp);
  }

  public bool TryStartUnload(
    EntityUid gunUid,
    EntityUid userUid,
    PubgAmmoProviderComponent? component = null)
  {
    int ammoCount;
    if (!this.Resolve<PubgAmmoProviderComponent>(gunUid, ref component) || component.IsUnloading || !this.TryGetWeaponUnloadInfo(gunUid, component, out ammoCount, out string _) || ammoCount <= 0)
      return false;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) userUid);
    EntityUid entityUid = gunUid;
    if ((activeItem.HasValue ? (activeItem.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return false;
    float unloadTime = component.UnloadTime;
    DoAfterId? id;
    if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, userUid, TimeSpan.FromSeconds((double) unloadTime), (DoAfterEvent) new PubgUnloadDoAfterEvent(), new EntityUid?(gunUid), new EntityUid?(gunUid), new EntityUid?(gunUid))
    {
      BreakOnMove = false,
      BreakOnDamage = false,
      NeedHand = true
    }, out id))
      return false;
    component.IsUnloading = true;
    component.ActiveUnloadDoAfter = id;
    this.Dirty(gunUid, (IComponent) component);
    SoundSpecifier sound = component.UnloadSound ?? component.ReloadSound;
    if (sound != null && this._netMan.IsServer)
      this._audio.PlayPvs(sound, gunUid);
    return true;
  }

  private void OnUnloadComplete(
    EntityUid gunUid,
    PubgAmmoProviderComponent component,
    PubgUnloadDoAfterEvent args)
  {
    component.ActiveUnloadDoAfter = new DoAfterId?();
    if (!component.IsUnloading)
    {
      this.Dirty(gunUid, (IComponent) component);
    }
    else
    {
      component.IsUnloading = false;
      if (args.Cancelled || args.Handled)
      {
        this.Dirty(gunUid, (IComponent) component);
      }
      else
      {
        args.Handled = true;
        this.UnloadWeapon(gunUid, args.User, component);
      }
    }
  }

  private void UnloadWeapon(
    EntityUid weaponUid,
    EntityUid userUid,
    PubgAmmoProviderComponent component)
  {
    int ammoCount;
    string ammoPrototype;
    if (!this.TryGetWeaponUnloadInfo(weaponUid, component, out ammoCount, out ammoPrototype) || ammoCount <= 0 || !this.TryReturnAmmoToUser(userUid, ammoPrototype, ammoCount))
      return;
    this.SetCurrentAmmoCount(weaponUid, 0, component);
    this.RefreshHeldWeaponAmmoUi(weaponUid, component);
  }

  private void UnloadMagazine(
    EntityUid magazineUid,
    EntityUid userUid,
    PubgMagazineModuleAmmoComponent component)
  {
    if (component.CurrentAmmo <= 0)
      return;
    string ammoStackPrototype = this.GetAmmoStackPrototype(component);
    if (string.IsNullOrWhiteSpace(ammoStackPrototype) || !this.TryReturnAmmoToUser(userUid, ammoStackPrototype, component.CurrentAmmo))
      return;
    EntityUid weaponUid;
    PubgAmmoProviderComponent ammoProvider;
    if (this.TryGetOwningWeapon(magazineUid, out weaponUid, out ammoProvider))
    {
      this.SetCurrentAmmoCount(weaponUid, 0, ammoProvider);
      this.RefreshHeldWeaponAmmoUi(weaponUid, ammoProvider);
    }
    else
    {
      component.CurrentAmmo = 0;
      if (!this._netMan.IsServer)
        return;
      this.Dirty(magazineUid, (IComponent) component);
    }
  }

  private bool TryReturnAmmoToUser(EntityUid userUid, string ammoTag, int amount)
  {
    if (amount <= 0 || string.IsNullOrWhiteSpace(ammoTag))
      return false;
    int amount1 = amount;
    int val2 = amount1 - this.TryMergeAmmoIntoUser(userUid, ammoTag, amount1);
    while (val2 > 0)
    {
      string ammoEntityByTag = this.FindAmmoEntityByTag(ammoTag);
      if (string.IsNullOrWhiteSpace(ammoEntityByTag))
        return false;
      EntityUid uid = this.Spawn(ammoEntityByTag, this.Transform(userUid).Coordinates);
      int amount2 = 1;
      StackComponent comp;
      if (this.TryComp<StackComponent>(uid, out comp))
      {
        amount2 = Math.Min(this._stack.GetMaxCount(comp), val2);
        this._stack.SetCount(uid, amount2, comp);
      }
      val2 -= amount2;
      this._stack.TryMergeToHands(uid, userUid);
    }
    return true;
  }

  private string? FindAmmoEntityByTag(string ammoTag)
  {
    foreach (EntityPrototype enumeratePrototype in this._proto.EnumeratePrototypes<EntityPrototype>())
    {
      TagComponent component;
      if (enumeratePrototype.TryGetComponent<TagComponent>(out component, this.EntityManager.ComponentFactory) && enumeratePrototype.TryGetComponent<StackComponent>(out StackComponent _, this.EntityManager.ComponentFactory))
      {
        foreach (ProtoId<TagPrototype> tag in component.Tags)
        {
          if (tag.ToString().Equals(ammoTag, StringComparison.OrdinalIgnoreCase))
            return enumeratePrototype.ID;
        }
      }
    }
    return (string) null;
  }

  private int TryMergeAmmoIntoUser(EntityUid userUid, string ammoTag, int amount)
  {
    if (amount <= 0)
      return 0;
    int num = 0;
    HashSet<EntityUid> visited = new HashSet<EntityUid>();
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) userUid, out containerSlotEnumerator))
    {
      ContainerSlot container;
      while (containerSlotEnumerator.MoveNext(out container) && num < amount)
      {
        EntityUid? containedEntity = container.ContainedEntity;
        if (containedEntity.HasValue)
        {
          EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
          num += this.TryMergeAmmoIntoEntity(valueOrDefault, ammoTag, amount - num, visited);
        }
      }
    }
    HandsComponent comp;
    if (this.TryComp<HandsComponent>(userUid, out comp))
    {
      foreach (EntityUid entity in this._hands.EnumerateHeld((Entity<HandsComponent>) (userUid, comp)))
      {
        if (num < amount)
          num += this.TryMergeAmmoIntoEntity(entity, ammoTag, amount - num, visited);
        else
          break;
      }
    }
    return num;
  }

  private int TryMergeAmmoIntoEntity(
    EntityUid entity,
    string ammoTag,
    int amount,
    HashSet<EntityUid> visited)
  {
    if (amount <= 0 || !visited.Add(entity))
      return 0;
    int num1 = 0;
    StackComponent comp1;
    if (this.TryComp<StackComponent>(entity, out comp1) && this._tag.HasTag(entity, (ProtoId<TagPrototype>) ammoTag))
    {
      int num2 = Math.Min(this._stack.GetAvailableSpace(comp1), amount);
      if (num2 > 0)
      {
        this._stack.SetCount(entity, comp1.Count + num2, comp1);
        num1 += num2;
      }
    }
    ContainerManagerComponent comp2;
    if (num1 >= amount || !this.TryComp<ContainerManagerComponent>(entity, out comp2))
      return num1;
    foreach (BaseContainer allContainer in this._container.GetAllContainers(entity, comp2))
    {
      foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) allContainer.ContainedEntities)
      {
        if (num1 < amount)
          num1 += this.TryMergeAmmoIntoEntity(containedEntity, ammoTag, amount - num1, visited);
        else
          break;
      }
      if (num1 >= amount)
        break;
    }
    return num1;
  }

  private int ResolveCurrentAmmo(EntityUid gunUid, PubgAmmoProviderComponent component)
  {
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
    PubgMagazineModuleAmmoComponent magazineAmmo;
    if (this.TryGetMagazineAmmoModule(gunUid, component, out EntityUid _, out magazineAmmo))
      return Math.Clamp(magazineAmmo.CurrentAmmo, 0, effectiveMaxAmmo);
    return this.UsesMagazineModuleAmmo(gunUid) ? 0 : Math.Clamp(component.CurrentAmmo, 0, effectiveMaxAmmo);
  }

  private void SyncWeaponCurrentAmmo(
    EntityUid gunUid,
    PubgAmmoProviderComponent component,
    int currentAmmo)
  {
    if (component.CurrentAmmo == currentAmmo)
      return;
    component.CurrentAmmo = currentAmmo;
    if (!this._netMan.IsServer)
      return;
    this.Dirty(gunUid, (IComponent) component);
  }

  private void RefreshAmmoVisuals(
    EntityUid gunUid,
    PubgAmmoProviderComponent component,
    int? currentAmmo = null)
  {
    AppearanceComponent comp;
    if (!this.UsesMagazineModuleAmmo(gunUid) || !this.TryComp<AppearanceComponent>(gunUid, out comp))
      return;
    int effectiveMaxAmmo = this.GetEffectiveMaxAmmo(gunUid, component);
    int num = Math.Clamp(currentAmmo ?? this.ResolveCurrentAmmo(gunUid, component), 0, effectiveMaxAmmo);
    bool magazineAmmoModule = this.TryGetMagazineAmmoModule(gunUid, component, out EntityUid _, out PubgMagazineModuleAmmoComponent _);
    this._appearance.SetData(gunUid, (Enum) AmmoVisuals.MagLoaded, (object) magazineAmmoModule, comp);
    this._appearance.SetData(gunUid, (Enum) AmmoVisuals.HasAmmo, (object) (num > 0), comp);
    this._appearance.SetData(gunUid, (Enum) AmmoVisuals.AmmoCount, (object) num, comp);
    this._appearance.SetData(gunUid, (Enum) AmmoVisuals.AmmoMax, (object) effectiveMaxAmmo, comp);
  }

  private bool TryGetHoldingUser(EntityUid gunUid, out EntityUid userUid)
  {
    userUid = EntityUid.Invalid;
    BaseContainer container;
    if (!this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) gunUid, out container))
      return false;
    EntityUid owner = container.Owner;
    HandsComponent comp;
    if (!this.TryComp<HandsComponent>(owner, out comp) || !this._hands.IsHolding((Entity<HandsComponent>) (owner, comp), new EntityUid?(gunUid)))
      return false;
    userUid = owner;
    return true;
  }

  private void UpdateHeldWeaponAmmoUi(EntityUid gunUid, PubgAmmoProviderComponent component)
  {
    EntityUid userUid;
    if (!this._netMan.IsServer || !this.TryGetHoldingUser(gunUid, out userUid))
      return;
    EntityUid? activeItem = this._hands.GetActiveItem((Entity<HandsComponent>) userUid);
    EntityUid entityUid = gunUid;
    if ((activeItem.HasValue ? (activeItem.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
      return;
    this.UpdateAmmoUI(userUid, gunUid, component);
  }

  private int GetEffectiveMaxAmmo(EntityUid gunUid, PubgAmmoProviderComponent component)
  {
    int maxAmmo = component.MaxAmmo;
    PubgWeaponModulesComponent comp;
    if (this.TryComp<PubgWeaponModulesComponent>(gunUid, out comp))
      maxAmmo += this._modules.GetMagazineCapacityBonus(gunUid, comp);
    return Math.Max(1, maxAmmo);
  }

  private int CountAmmoInInventory(EntityUid userUid, string ammoTag)
  {
    int num1 = 0;
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) userUid, out containerSlotEnumerator))
    {
      ContainerSlot container;
      while (containerSlotEnumerator.MoveNext(out container))
      {
        EntityUid? containedEntity = container.ContainedEntity;
        if (containedEntity.HasValue)
        {
          int num2 = num1;
          containedEntity = container.ContainedEntity;
          int num3 = this.CountAmmoInEntity(containedEntity.Value, ammoTag);
          num1 = num2 + num3;
        }
      }
    }
    HandsComponent comp;
    if (this.TryComp<HandsComponent>(userUid, out comp))
    {
      foreach (string key in comp.Hands.Keys)
      {
        EntityUid? held;
        if (this._hands.TryGetHeldItem((Entity<HandsComponent>) userUid, key, out held) && held.HasValue)
          num1 += this.CountAmmoInEntity(held.Value, ammoTag);
      }
    }
    return num1;
  }

  private int CountAmmoInEntity(EntityUid entity, string ammoTag)
  {
    int num = 0;
    StackComponent comp1;
    if (this._tag.HasTag(entity, (ProtoId<TagPrototype>) ammoTag) && this.TryComp<StackComponent>(entity, out comp1))
      num += comp1.Count;
    ContainerManagerComponent comp2;
    if (this.TryComp<ContainerManagerComponent>(entity, out comp2))
    {
      foreach (BaseContainer allContainer in this._container.GetAllContainers(entity, comp2))
      {
        foreach (EntityUid containedEntity in (IEnumerable<EntityUid>) allContainer.ContainedEntities)
          num += this.CountAmmoInEntity(containedEntity, ammoTag);
      }
    }
    return num;
  }

  private int TakeAmmoFromInventory(EntityUid userUid, string ammoTag, int amount)
  {
    int ammoFromInventory = 0;
    HandsComponent comp;
    if (this.TryComp<HandsComponent>(userUid, out comp))
    {
      foreach (string key in comp.Hands.Keys)
      {
        EntityUid? held;
        if (this._hands.TryGetHeldItem((Entity<HandsComponent>) userUid, key, out held) && held.HasValue)
        {
          ammoFromInventory += this.TakeAmmoFromEntity(held.Value, ammoTag, amount - ammoFromInventory);
          if (ammoFromInventory >= amount)
            return ammoFromInventory;
        }
      }
    }
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) userUid, out containerSlotEnumerator))
    {
      ContainerSlot container;
      while (containerSlotEnumerator.MoveNext(out container) && ammoFromInventory < amount)
      {
        if (container.ContainedEntity.HasValue)
          ammoFromInventory += this.TakeAmmoFromEntity(container.ContainedEntity.Value, ammoTag, amount - ammoFromInventory);
      }
    }
    return ammoFromInventory;
  }

  private int TakeAmmoFromEntity(EntityUid entity, string ammoTag, int amount)
  {
    int ammoFromEntity = 0;
    StackComponent comp1;
    if (this._tag.HasTag(entity, (ProtoId<TagPrototype>) ammoTag) && this.TryComp<StackComponent>(entity, out comp1))
    {
      int num = Math.Min(amount, comp1.Count);
      this._stack.SetCount(entity, comp1.Count - num);
      ammoFromEntity += num;
      if (comp1.Count <= 0 && this._netMan.IsServer)
        this.QueueDel(new EntityUid?(entity));
      if (ammoFromEntity >= amount)
        return ammoFromEntity;
    }
    ContainerManagerComponent comp2;
    if (this.TryComp<ContainerManagerComponent>(entity, out comp2))
    {
      foreach (BaseContainer allContainer in this._container.GetAllContainers(entity, comp2))
      {
        foreach (EntityUid entity1 in new List<EntityUid>((IEnumerable<EntityUid>) allContainer.ContainedEntities))
        {
          ammoFromEntity += this.TakeAmmoFromEntity(entity1, ammoTag, amount - ammoFromEntity);
          if (ammoFromEntity >= amount)
            return ammoFromEntity;
        }
      }
    }
    return ammoFromEntity;
  }
}
