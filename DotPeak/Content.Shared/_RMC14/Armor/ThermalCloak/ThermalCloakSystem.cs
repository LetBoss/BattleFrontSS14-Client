// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Armor.ThermalCloak.ThermalCloakSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Chemistry;
using Content.Shared._RMC14.NightVision;
using Content.Shared._RMC14.Stealth;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared._RMC14.Xenonids.Projectile;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Explosion.EntitySystems;
using Content.Shared.Humanoid;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Systems;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Armor.ThermalCloak;

public sealed class ThermalCloakSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedHumanoidAppearanceSystem _humanoidSystem;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private INetManager _net;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<ThermalCloakComponent, GetItemActionsEvent>(new EntityEventRefHandler<ThermalCloakComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<ThermalCloakComponent, ThermalCloakTurnInvisibleActionEvent>(new EntityEventRefHandler<ThermalCloakComponent, ThermalCloakTurnInvisibleActionEvent>(this.OnCloakAction));
    this.SubscribeLocalEvent<ThermalCloakComponent, GotEquippedEvent>(new EntityEventRefHandler<ThermalCloakComponent, GotEquippedEvent>(this.OnEquipped));
    this.SubscribeLocalEvent<ThermalCloakComponent, GotUnequippedEvent>(new EntityEventRefHandler<ThermalCloakComponent, GotUnequippedEvent>(this.OnUnequipped));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, VaporHitEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, VaporHitEvent>(this.OnVaporHit));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, MobStateChangedEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, MobStateChangedEvent>(this.OnMobStateChanged));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, XenoDevouredEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, XenoDevouredEvent>(this.OnDevour));
    this.SubscribeLocalEvent<EntityActiveInvisibleComponent, XenoParasiteInfectEvent>(new EntityEventRefHandler<EntityActiveInvisibleComponent, XenoParasiteInfectEvent>(this.OnParasiteInfect));
    this.SubscribeLocalEvent<GunComponent, AttemptShootEvent>(new EntityEventRefHandler<GunComponent, AttemptShootEvent>(this.OnAttemptShoot));
    this.SubscribeLocalEvent<CancelUseWithCloakComponent, UseInHandEvent>(new EntityEventRefHandler<CancelUseWithCloakComponent, UseInHandEvent>(this.OnTimerUse), new Type[1]
    {
      typeof (SharedTriggerSystem)
    });
    this.SubscribeLocalEvent<UncloakOnHitComponent, ProjectileHitEvent>(new EntityEventRefHandler<UncloakOnHitComponent, ProjectileHitEvent>(this.OnAcidProjectile));
  }

  private void OnGetItemActions(Entity<ThermalCloakComponent> ent, ref GetItemActionsEvent args)
  {
    ThermalCloakComponent comp = ent.Comp;
    if (args.InHands || !this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), SlotFlags.BACK))
      return;
    args.AddAction(ref comp.Action, (string) comp.ActionId);
    this.Dirty<ThermalCloakComponent>(ent);
  }

  private void OnCloakAction(
    Entity<ThermalCloakComponent> ent,
    ref ThermalCloakTurnInvisibleActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    if (!this._whitelist.IsWhitelistPass(ent.Comp.Whitelist, args.Performer))
      this._popup.PopupClient(this.Loc.GetString("cm-gun-unskilled", ("gun", (object) ent.Owner)), args.Performer, new EntityUid?(args.Performer), PopupType.SmallCaution);
    else
      this.SetInvisibility(ent, args.Performer, !ent.Comp.Enabled, false);
  }

  private void OnEquipped(Entity<ThermalCloakComponent> ent, ref GotEquippedEvent args)
  {
    if (this._timing.ApplyingState || !this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), SlotFlags.BACK))
      return;
    EntityTurnInvisibleComponent invisibleComponent = this.EnsureComp<EntityTurnInvisibleComponent>(args.Equipee);
    invisibleComponent.RestrictWeapons = ent.Comp.RestrictWeapons;
    invisibleComponent.UncloakWeaponLock = ent.Comp.UncloakWeaponLock;
    this.Dirty(args.Equipee, (IComponent) invisibleComponent);
  }

  private void OnUnequipped(Entity<ThermalCloakComponent> ent, ref GotUnequippedEvent args)
  {
    if (this._timing.ApplyingState || this._inventory.InSlotWithFlags((Entity<TransformComponent, MetaDataComponent>) ((EntityUid) ent, (TransformComponent) null, (MetaDataComponent) null), SlotFlags.BACK))
      return;
    this.SetInvisibility(ent, args.Equipee, false, false);
    this.RemCompDeferred<EntityTurnInvisibleComponent>(args.Equipee);
  }

  public void SetInvisibility(
    Entity<ThermalCloakComponent> ent,
    EntityUid user,
    bool enabling,
    bool forced)
  {
    EntityTurnInvisibleComponent comp1;
    if (!this.TryComp<EntityTurnInvisibleComponent>(user, out comp1))
      return;
    if (enabling && !this.HasComp<EntityActiveInvisibleComponent>(user))
    {
      EntityActiveInvisibleComponent invisibleComponent = this.EnsureComp<EntityActiveInvisibleComponent>(user);
      invisibleComponent.Opacity = ent.Comp.Opacity;
      this.Dirty(user, (IComponent) invisibleComponent);
      ent.Comp.Enabled = true;
      comp1.Enabled = true;
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
      this.Dirty(user, (IComponent) comp1);
      ActionComponent comp2;
      if (this.HasComp<InstantActionComponent>(ent.Comp.Action) && this.TryComp<ActionComponent>(ent.Comp.Action, out comp2))
      {
        this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) (ent.Comp.Action.Value, comp2)), this._timing.CurTime, this._timing.CurTime + ent.Comp.Cooldown);
        this._actions.SetUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) (ent.Comp.Action.Value, comp2)), new TimeSpan?(ent.Comp.Cooldown));
      }
      if (ent.Comp.HideNightVision)
        this.RemCompDeferred<RMCNightVisionVisibleComponent>(user);
      if (ent.Comp.BlockFriendlyFire)
        this.EnsureComp<EntityIFFComponent>(user);
      comp1.UncloakTime = this._timing.CurTime;
      this.ToggleLayers(user, ent.Comp.CloakedHideLayers, false);
      this.SpawnCloakEffects(user, ent.Comp.CloakEffect);
      string othersMessage = this.Loc.GetString("rmc-cloak-activate-others", (nameof (user), (object) user));
      this._popup.PopupPredicted(this.Loc.GetString("rmc-cloak-activate-self"), othersMessage, user, new EntityUid?(user), PopupType.Medium);
      if (!this._net.IsServer)
        return;
      this._audio.PlayPvs(ent.Comp.CloakSound, user);
    }
    else
    {
      EntityActiveInvisibleComponent comp3;
      if (enabling || !this.TryComp<EntityActiveInvisibleComponent>(user, out comp3))
        return;
      comp3.Opacity = 1f;
      this.Dirty(user, (IComponent) comp3);
      ent.Comp.Enabled = false;
      comp1.Enabled = false;
      this.Dirty(ent.Owner, (IComponent) ent.Comp);
      this.Dirty(user, (IComponent) comp1);
      if (forced)
      {
        ActionComponent comp4;
        if (this.HasComp<InstantActionComponent>(ent.Comp.Action) && this.TryComp<ActionComponent>(ent.Comp.Action, out comp4))
        {
          this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) (ent.Comp.Action.Value, comp4)), this._timing.CurTime, this._timing.CurTime + ent.Comp.ForcedCooldown);
          this._actions.SetUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) (ent.Comp.Action.Value, comp4)), new TimeSpan?(ent.Comp.ForcedCooldown));
        }
        comp1.UncloakTime = this._timing.CurTime;
        string othersMessage = this.Loc.GetString("rmc-cloak-forced-deactivate-others", (nameof (user), (object) user));
        this._popup.PopupPredicted(this.Loc.GetString("rmc-cloak-forced-deactivate-self"), othersMessage, user, new EntityUid?(user), PopupType.Medium);
      }
      else
      {
        ActionComponent comp5;
        if (this.HasComp<InstantActionComponent>(ent.Comp.Action) && this.TryComp<ActionComponent>(ent.Comp.Action, out comp5))
        {
          this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) (ent.Comp.Action.Value, comp5)), this._timing.CurTime, this._timing.CurTime + ent.Comp.Cooldown);
          this._actions.SetUseDelay(new Entity<ActionComponent>?((Entity<ActionComponent>) (ent.Comp.Action.Value, comp5)), new TimeSpan?(ent.Comp.Cooldown));
        }
        comp1.UncloakTime = this._timing.CurTime;
        string othersMessage = this.Loc.GetString("rmc-cloak-deactivate-others", (nameof (user), (object) user));
        this._popup.PopupPredicted(this.Loc.GetString("rmc-cloak-deactivate-self"), othersMessage, user, new EntityUid?(user), PopupType.Medium);
      }
      this.ToggleLayers(user, ent.Comp.CloakedHideLayers, true);
      this.SpawnCloakEffects(user, ent.Comp.UncloakEffect);
      if (ent.Comp.HideNightVision)
        this.EnsureComp<RMCNightVisionVisibleComponent>(user);
      if (ent.Comp.BlockFriendlyFire)
        this.RemCompDeferred<EntityIFFComponent>(user);
      this.RemCompDeferred<EntityActiveInvisibleComponent>(user);
      if (!this._net.IsServer)
        return;
      this._audio.PlayPvs(ent.Comp.UncloakSound, user);
    }
  }

  public void TrySetInvisibility(
    EntityUid uid,
    bool enabling,
    bool forced,
    ThermalCloakComponent? component = null)
  {
    Entity<ThermalCloakComponent>? wornCloak = this.FindWornCloak(uid);
    if (!wornCloak.HasValue)
      return;
    this.SetInvisibility(wornCloak.Value, uid, enabling, forced);
  }

  private void OnAttemptShoot(Entity<GunComponent> ent, ref AttemptShootEvent args)
  {
    EntityTurnInvisibleComponent comp;
    if (args.Cancelled || !this.TryComp<EntityTurnInvisibleComponent>(args.User, out comp) || (!comp.RestrictWeapons || !comp.Enabled) && !(comp.UncloakTime + comp.UncloakWeaponLock > this._timing.CurTime))
      return;
    args.Cancelled = true;
    this._popup.PopupClient(this.Loc.GetString("rmc-cloak-attempt-shoot"), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnTimerUse(Entity<CancelUseWithCloakComponent> ent, ref UseInHandEvent args)
  {
    EntityTurnInvisibleComponent comp;
    if (args.Handled || !this.TryComp<EntityTurnInvisibleComponent>(args.User, out comp) || (!comp.RestrictWeapons || !comp.Enabled) && !(comp.UncloakTime + comp.UncloakWeaponLock > this._timing.CurTime))
      return;
    args.Handled = true;
    this._popup.PopupClient(this.Loc.GetString(ent.Comp.CancelMessage), args.User, new EntityUid?(args.User), PopupType.SmallCaution);
  }

  private void OnAcidProjectile(Entity<UncloakOnHitComponent> ent, ref ProjectileHitEvent args)
  {
    this.TrySetInvisibility(args.Target, false, true);
  }

  private void OnVaporHit(Entity<EntityActiveInvisibleComponent> ent, ref VaporHitEvent args)
  {
    this.TrySetInvisibility(ent.Owner, false, true);
  }

  private void OnMobStateChanged(
    Entity<EntityActiveInvisibleComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState != MobState.Dead)
      return;
    this.TrySetInvisibility(ent.Owner, false, true);
  }

  private void OnDevour(Entity<EntityActiveInvisibleComponent> ent, ref XenoDevouredEvent args)
  {
    this.TrySetInvisibility(ent.Owner, false, true);
  }

  private void OnParasiteInfect(
    Entity<EntityActiveInvisibleComponent> ent,
    ref XenoParasiteInfectEvent args)
  {
    this.TrySetInvisibility(ent.Owner, false, true);
  }

  public Entity<ThermalCloakComponent>? FindWornCloak(EntityUid player)
  {
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) player, SlotFlags.BACK);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      ThermalCloakComponent comp;
      if (this.TryComp<ThermalCloakComponent>(container.ContainedEntity, out comp))
        return new Entity<ThermalCloakComponent>?((Entity<ThermalCloakComponent>) (container.ContainedEntity.Value, comp));
    }
    return new Entity<ThermalCloakComponent>?();
  }

  private void ToggleLayers(
    EntityUid equipee,
    HashSet<HumanoidVisualLayers> layers,
    bool showLayers)
  {
    foreach (HumanoidVisualLayers layer in layers)
      this._humanoidSystem.SetLayerVisibility((Entity<HumanoidAppearanceComponent>) equipee, layer, showLayers);
  }

  public void SpawnCloakEffects(EntityUid user, EntProtoId cloakProtoId)
  {
    if (this._net.IsClient)
      return;
    MapCoordinates mapCoordinates = this._transform.GetMapCoordinates(user);
    Angle worldRotation = this._transform.GetWorldRotation(user);
    this.Spawn((string) cloakProtoId, mapCoordinates, rotation: worldRotation);
  }
}
