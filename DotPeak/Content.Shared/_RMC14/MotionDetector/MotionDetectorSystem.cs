// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.MotionDetector.MotionDetectorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Weapons.Ranged.Battery;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.MotionDetector;

public sealed class MotionDetectorSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private GunIFFSystem _gunIFF;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private MotionDetectorSystem _motionDetector;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCGunBatterySystem _rmcGunBattery;
  [Dependency]
  private SharedCMInventorySystem _rmcInventory;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  private Robust.Shared.GameObjects.EntityQuery<MotionDetectorComponent> _detectorQuery;
  private Robust.Shared.GameObjects.EntityQuery<StorageComponent> _storageQuery;
  private readonly HashSet<Entity<MotionDetectorTrackedComponent>> _toUpdate = new HashSet<Entity<MotionDetectorTrackedComponent>>();
  private readonly HashSet<Entity<MotionDetectorTrackedComponent>> _tracked = new HashSet<Entity<MotionDetectorTrackedComponent>>();

  public override void Initialize()
  {
    this._detectorQuery = this.GetEntityQuery<MotionDetectorComponent>();
    this._storageQuery = this.GetEntityQuery<StorageComponent>();
    this.SubscribeLocalEvent<XenoParasiteInfectEvent>(new EntityEventHandler<XenoParasiteInfectEvent>(this.OnXenoInfect));
    this.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged));
    this.SubscribeLocalEvent<XenoDevouredEvent>(new EntityEventRefHandler<XenoDevouredEvent>(this.OnMotionDetectorDevoured));
    this.SubscribeLocalEvent<MotionDetectorComponent, UseInHandEvent>(new EntityEventRefHandler<MotionDetectorComponent, UseInHandEvent>(this.OnMotionDetectorUseInHand));
    this.SubscribeLocalEvent<MotionDetectorComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<MotionDetectorComponent, GetVerbsEvent<AlternativeVerb>>(this.OnMotionDetectorGetVerbs));
    this.SubscribeLocalEvent<MotionDetectorComponent, DroppedEvent>(new EntityEventRefHandler<MotionDetectorComponent, DroppedEvent>(this.OnMotionDetectorDropped<DroppedEvent>));
    this.SubscribeLocalEvent<MotionDetectorComponent, RMCDroppedEvent>(new EntityEventRefHandler<MotionDetectorComponent, RMCDroppedEvent>(this.OnMotionDetectorDropped<RMCDroppedEvent>));
    this.SubscribeLocalEvent<MotionDetectorComponent, ExaminedEvent>(new EntityEventRefHandler<MotionDetectorComponent, ExaminedEvent>(this.OnMotionDetectorExamined));
    this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, GetItemActionsEvent>(new EntityEventRefHandler<ToggleableMotionDetectorComponent, GetItemActionsEvent>(this.OnGetItemActions));
    this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, ToggleableMotionDetectorActionEvent>(new EntityEventRefHandler<ToggleableMotionDetectorComponent, ToggleableMotionDetectorActionEvent>(this.OnToggleAction));
    this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, GunGetBatteryDrainEvent>(new EntityEventRefHandler<ToggleableMotionDetectorComponent, GunGetBatteryDrainEvent>(this.OnGetBatteryDrain));
    this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, GunUnpoweredEvent>(new EntityEventRefHandler<ToggleableMotionDetectorComponent, GunUnpoweredEvent>(this.OnGunUnpowered));
    this.SubscribeLocalEvent<ToggleableMotionDetectorComponent, MotionDetectorUpdatedEvent>(new EntityEventRefHandler<ToggleableMotionDetectorComponent, MotionDetectorUpdatedEvent>(this.OnMotionDetectorUpdated));
    this.SubscribeLocalEvent<MotionDetectorTrackedComponent, MoveEvent>(new EntityEventRefHandler<MotionDetectorTrackedComponent, MoveEvent>(this.OnMotionDetectorTracked));
  }

  private void OnXenoInfect(XenoParasiteInfectEvent ev) => this.DisableDetectorsOnMob(ev.Target);

  private void OnMobStateChanged(MobStateChangedEvent ev)
  {
    if (ev.NewMobState != MobState.Dead)
      return;
    this.DisableDetectorsOnMob(ev.Target);
  }

  private void OnMotionDetectorUseInHand(
    Entity<MotionDetectorComponent> ent,
    ref UseInHandEvent args)
  {
    if (!ent.Comp.HandToggleable || !this._hands.IsHolding((Entity<HandsComponent>) args.User, new EntityUid?((EntityUid) ent)))
      return;
    args.Handled = true;
    this.Toggle(ent);
    EntityUid user = args.User;
    ent.Comp.LastUser = new EntityUid?(user);
    this.Dirty<MotionDetectorComponent>(ent);
    this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(user));
  }

  private void OnMotionDetectorGetVerbs(
    Entity<MotionDetectorComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract || !ent.Comp.CanToggleRange)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = ent.Comp.Short ? "Change to long range mode" : "Change to short range mode";
    alternativeVerb.Act = (Action) (() =>
    {
      ent.Comp.Short = !ent.Comp.Short;
      this.Dirty<MotionDetectorComponent>(ent);
      this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(user));
      this._popup.PopupClient($"You change the {this.Name((EntityUid) ent)} to {(ent.Comp.Short ? "short" : "long")} range mode", (EntityUid) ent, new EntityUid?(user));
    });
    verbs.Add(alternativeVerb);
  }

  private void OnMotionDetectorDropped<T>(Entity<MotionDetectorComponent> ent, ref T args)
  {
    if (!ent.Comp.DeactivateOnDrop)
      return;
    ent.Comp.Enabled = false;
    this.Dirty<MotionDetectorComponent>(ent);
    this.UpdateAppearance(ent);
    this.MotionDetectorUpdated(ent);
  }

  private void OnMotionDetectorDevoured(ref XenoDevouredEvent ent)
  {
    this.DisableDetectorsOnMob(ent.Target);
  }

  private void OnMotionDetectorExamined(Entity<MotionDetectorComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("MotionDetectorComponent"))
    {
      string str = ent.Comp.Short ? "short" : "long";
      args.PushMarkup($"The motion detector is in [color=cyan]{str}[/color] scanning mode.");
    }
  }

  private void OnGetItemActions(
    Entity<ToggleableMotionDetectorComponent> ent,
    ref GetItemActionsEvent args)
  {
    if (ent.Comp.Slots != SlotFlags.All)
    {
      if (args.InHands)
        return;
      SlotFlags? slotFlags1 = args.SlotFlags;
      SlotFlags slots = ent.Comp.Slots;
      SlotFlags? nullable = slotFlags1.HasValue ? new SlotFlags?(slotFlags1.GetValueOrDefault() & slots) : new SlotFlags?();
      SlotFlags slotFlags2 = SlotFlags.NONE;
      if (nullable.GetValueOrDefault() == slotFlags2 & nullable.HasValue)
        return;
    }
    args.AddAction(ref ent.Comp.Action, (string) ent.Comp.ActionId);
    this.Dirty<ToggleableMotionDetectorComponent>(ent);
  }

  private void OnToggleAction(
    Entity<ToggleableMotionDetectorComponent> ent,
    ref ToggleableMotionDetectorActionEvent args)
  {
    EntityUid performer = args.Performer;
    MotionDetectorComponent comp;
    if (this.TryComp<MotionDetectorComponent>((EntityUid) ent, out comp))
    {
      this._motionDetector.Toggle((Entity<MotionDetectorComponent>) ((EntityUid) ent, comp));
      comp.LastUser = new EntityUid?(performer);
      this.Dirty<ToggleableMotionDetectorComponent>(ent);
    }
    this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(performer));
    this.DetectorUpdated(ent);
    this._popup.PopupClient(this._motionDetector.IsEnabled((Entity<MotionDetectorComponent>) ((EntityUid) ent, comp)) ? this.Loc.GetString("rmc-toggleable-motion-detector-on", ("gun", (object) ent)) : this.Loc.GetString("rmc-toggleable-motion-detector-off", ("gun", (object) ent)), performer, new EntityUid?(performer), PopupType.Large);
  }

  private void OnGetBatteryDrain(
    Entity<ToggleableMotionDetectorComponent> ent,
    ref GunGetBatteryDrainEvent args)
  {
    if (!this._motionDetector.IsEnabled((Entity<MotionDetectorComponent>) ent.Owner))
      return;
    args.Drain += ent.Comp.BatteryDrain;
  }

  private void OnGunUnpowered(
    Entity<ToggleableMotionDetectorComponent> ent,
    ref GunUnpoweredEvent args)
  {
    MotionDetectorComponent comp;
    if (!this.TryComp<MotionDetectorComponent>((EntityUid) ent, out comp))
      return;
    this._motionDetector.Disable((Entity<MotionDetectorComponent>) ((EntityUid) ent, comp));
    this.DetectorUpdated(ent);
  }

  private void OnMotionDetectorUpdated(
    Entity<ToggleableMotionDetectorComponent> ent,
    ref MotionDetectorUpdatedEvent args)
  {
    this.DetectorUpdated(ent);
  }

  private void DetectorUpdated(Entity<ToggleableMotionDetectorComponent> ent)
  {
    bool flag = false;
    MotionDetectorComponent comp;
    if (this.TryComp<MotionDetectorComponent>((EntityUid) ent, out comp))
      flag = this._motionDetector.IsEnabled((Entity<MotionDetectorComponent>) ((EntityUid) ent, comp));
    this._rmcGunBattery.RefreshBatteryDrain((Entity<GunDrainBatteryOnShootComponent>) ent.Owner);
    SharedActionsSystem actions = this._actions;
    EntityUid? action1 = ent.Comp.Action;
    Entity<ActionComponent>? action2 = action1.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) action1.GetValueOrDefault()) : new Entity<ActionComponent>?();
    int num = flag ? 1 : 0;
    actions.SetToggled(action2, num != 0);
  }

  private void OnMotionDetectorTracked(
    Entity<MotionDetectorTrackedComponent> ent,
    ref MoveEvent args)
  {
    if (args.OldPosition == args.NewPosition)
      return;
    this._toUpdate.Add(ent);
  }

  private void UpdateAppearance(Entity<MotionDetectorComponent> ent)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) MotionDetectorLayer.Setting, (object) (MotionDetectorSetting) (ent.Comp.Short ? 0 : 1));
    int num = Math.Min(ent.Comp.Blips.Count, 9);
    if (!ent.Comp.Enabled)
      num = -1;
    this._appearance.SetData((EntityUid) ent, (Enum) MotionDetectorLayer.Number, (object) num);
  }

  private void DisableMotionDetectors(EntityUid ent)
  {
    MotionDetectorComponent component1;
    if (this._detectorQuery.TryComp(ent, out component1))
    {
      component1.Enabled = false;
      this.Dirty(ent, (IComponent) component1);
      this.UpdateAppearance((Entity<MotionDetectorComponent>) (ent, component1));
      this.MotionDetectorUpdated((Entity<MotionDetectorComponent>) (ent, component1));
    }
    StorageComponent component2;
    if (!this._storageQuery.TryComp(ent, out component2))
      return;
    foreach (EntityUid key in component2.StoredItems.Keys)
      this.DisableMotionDetectors(key);
  }

  private void DisableDetectorsOnMob(EntityUid uid)
  {
    foreach (EntityUid ent in this._hands.EnumerateHeld((Entity<HandsComponent>) uid))
      this.DisableMotionDetectors(ent);
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) uid);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      if (containedEntity.HasValue)
        this.DisableMotionDetectors(containedEntity.GetValueOrDefault());
    }
  }

  private TimeSpan GetRefreshRate(Entity<MotionDetectorComponent> ent)
  {
    return !ent.Comp.Short ? ent.Comp.LongRefresh : ent.Comp.ShortRefresh;
  }

  private void MotionDetectorUpdated(Entity<MotionDetectorComponent> ent)
  {
    MotionDetectorUpdatedEvent args = new MotionDetectorUpdatedEvent(ent.Comp.Enabled);
    this.RaiseLocalEvent<MotionDetectorUpdatedEvent>((EntityUid) ent, ref args);
  }

  public void Toggle(Entity<MotionDetectorComponent> ent)
  {
    ref bool local = ref ent.Comp.Enabled;
    local = !local;
    if (local)
      ent.Comp.NextScanAt = this._timing.CurTime + this.GetRefreshRate(ent);
    ent.Comp.Blips.Clear();
    this.Dirty<MotionDetectorComponent>(ent);
    this.UpdateAppearance(ent);
    this.MotionDetectorUpdated(ent);
  }

  public void Disable(Entity<MotionDetectorComponent> ent)
  {
    if (!ent.Comp.Enabled)
      return;
    this.Toggle(ent);
  }

  public bool IsEnabled(Entity<MotionDetectorComponent?> ent)
  {
    return this.Resolve<MotionDetectorComponent>((EntityUid) ent, ref ent.Comp, false) && ent.Comp.Enabled;
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    try
    {
      foreach (Entity<MotionDetectorTrackedComponent> entity in this._toUpdate)
        entity.Comp.LastMove = curTime;
    }
    finally
    {
      this._toUpdate.Clear();
    }
    Robust.Shared.GameObjects.EntityQueryEnumerator<MotionDetectorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<MotionDetectorComponent>();
    EntityUid uid;
    MotionDetectorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Enabled && !(curTime < comp1.NextScanAt))
      {
        comp1.LastScan = curTime;
        comp1.NextScanAt = curTime + this.GetRefreshRate((Entity<MotionDetectorComponent>) (uid, comp1));
        this.Dirty(uid, (IComponent) comp1);
        int range = comp1.Short ? comp1.ShortRange : comp1.LongRange;
        this._tracked.Clear();
        this._entityLookup.GetEntitiesInRange<MotionDetectorTrackedComponent>(uid.ToCoordinates(), (float) range, this._tracked, LookupFlags.Uncontained);
        comp1.Blips.Clear();
        foreach (Entity<MotionDetectorTrackedComponent> entity in this._tracked)
        {
          EntityUid owner = entity.Owner;
          EntityUid? lastUser = comp1.LastUser;
          if ((lastUser.HasValue ? (owner == lastUser.GetValueOrDefault() ? 1 : 0) : 0) == 0 && !(entity.Comp.LastMove < curTime - comp1.MoveTime))
          {
            lastUser = comp1.LastUser;
            EntProtoId<IFFFactionComponent> faction;
            if (!lastUser.HasValue || !this._gunIFF.TryGetFaction((Entity<UserIFFComponent>) lastUser.GetValueOrDefault(), out faction) || !this._gunIFF.IsInFaction(entity.Owner, faction))
              comp1.Blips.Add(new Blip(this._transform.GetMapCoordinates((EntityUid) entity), entity.Comp.IsQueenEye));
          }
        }
        this.UpdateAppearance((Entity<MotionDetectorComponent>) (uid, comp1));
        if (comp1.Blips.Count == 0)
        {
          EntityUid user;
          if (this._rmcInventory.TryGetUserHoldingOrStoringItem(uid, out user))
            this._audio.PlayEntity(comp1.ScanEmptySound, user, uid);
        }
        else
          this._audio.PlayPvs(comp1.ScanSound, uid);
      }
    }
  }
}
