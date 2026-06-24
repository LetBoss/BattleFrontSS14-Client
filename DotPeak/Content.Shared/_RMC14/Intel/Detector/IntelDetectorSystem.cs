// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Intel.Detector.IntelDetectorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.MotionDetector;
using Content.Shared._RMC14.Xenonids.Parasite;
using Content.Shared.Coordinates;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Mobs;
using Content.Shared.Storage;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Intel.Detector;

public sealed class IntelDetectorSystem : EntitySystem
{
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedCMInventorySystem _rmcInventory;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  private Robust.Shared.GameObjects.EntityQuery<IntelDetectorComponent> _detectorQuery;
  private Robust.Shared.GameObjects.EntityQuery<StorageComponent> _storageQuery;
  private readonly HashSet<Entity<IntelDetectorTrackedComponent>> _tracked = new HashSet<Entity<IntelDetectorTrackedComponent>>();

  public override void Initialize()
  {
    this._detectorQuery = this.GetEntityQuery<IntelDetectorComponent>();
    this._storageQuery = this.GetEntityQuery<StorageComponent>();
    this.SubscribeLocalEvent<XenoParasiteInfectEvent>(new EntityEventHandler<XenoParasiteInfectEvent>(this.OnXenoInfect));
    this.SubscribeLocalEvent<MobStateChangedEvent>(new EntityEventHandler<MobStateChangedEvent>(this.OnMobStateChanged));
    this.SubscribeLocalEvent<IntelDetectorComponent, UseInHandEvent>(new EntityEventRefHandler<IntelDetectorComponent, UseInHandEvent>(this.OnUseInHand));
    this.SubscribeLocalEvent<IntelDetectorComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<IntelDetectorComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetVerbs));
    this.SubscribeLocalEvent<IntelDetectorComponent, DroppedEvent>(new EntityEventRefHandler<IntelDetectorComponent, DroppedEvent>(this.OnDisable<DroppedEvent>));
    this.SubscribeLocalEvent<IntelDetectorComponent, RMCDroppedEvent>(new EntityEventRefHandler<IntelDetectorComponent, RMCDroppedEvent>(this.OnDisable<RMCDroppedEvent>));
    this.SubscribeLocalEvent<IntelDetectorComponent, ExaminedEvent>(new EntityEventRefHandler<IntelDetectorComponent, ExaminedEvent>(this.OnExamined));
  }

  private void OnXenoInfect(XenoParasiteInfectEvent ev) => this.DisableDetectorsOnMob(ev.Target);

  private void OnMobStateChanged(MobStateChangedEvent ev)
  {
    if (ev.NewMobState != MobState.Dead)
      return;
    this.DisableDetectorsOnMob(ev.Target);
  }

  private void OnUseInHand(Entity<IntelDetectorComponent> ent, ref UseInHandEvent args)
  {
    args.Handled = true;
    this.Toggle(ent);
    this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(args.User));
  }

  private void OnGetVerbs(
    Entity<IntelDetectorComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    if (!args.CanAccess || !args.CanInteract)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = ent.Comp.Short ? "Change to long range mode" : "Change to short range mode";
    alternativeVerb.Act = (Action) (() =>
    {
      ent.Comp.Short = !ent.Comp.Short;
      this.Dirty<IntelDetectorComponent>(ent);
      this._audio.PlayPredicted(ent.Comp.ToggleSound, (EntityUid) ent, new EntityUid?(user));
    });
    verbs.Add(alternativeVerb);
  }

  private void OnDisable<T>(Entity<IntelDetectorComponent> ent, ref T args)
  {
    ent.Comp.Enabled = false;
    this.Dirty<IntelDetectorComponent>(ent);
    this.UpdateAppearance(ent);
  }

  private void DisableIntelDetectors(EntityUid ent)
  {
    IntelDetectorComponent component1;
    if (this._detectorQuery.TryComp(ent, out component1))
    {
      component1.Enabled = false;
      this.Dirty(ent, (IComponent) component1);
      this.UpdateAppearance((Entity<IntelDetectorComponent>) (ent, component1));
    }
    StorageComponent component2;
    if (!this._storageQuery.TryComp(ent, out component2))
      return;
    foreach (EntityUid key in component2.StoredItems.Keys)
      this.DisableIntelDetectors(key);
  }

  private void DisableDetectorsOnMob(EntityUid uid)
  {
    foreach (EntityUid ent in this._hands.EnumerateHeld((Entity<HandsComponent>) uid))
      this.DisableIntelDetectors(ent);
    InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) uid);
    ContainerSlot container;
    while (slotEnumerator.MoveNext(out container))
    {
      EntityUid? containedEntity = container.ContainedEntity;
      if (containedEntity.HasValue)
        this.DisableIntelDetectors(containedEntity.GetValueOrDefault());
    }
  }

  private void OnExamined(Entity<IntelDetectorComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("IntelDetectorComponent"))
    {
      string str = ent.Comp.Short ? "short" : "long";
      args.PushMarkup($"The motion detector is in [color=cyan]{str}[/color] scanning mode.");
    }
  }

  private void Toggle(Entity<IntelDetectorComponent> ent)
  {
    ref bool local = ref ent.Comp.Enabled;
    local = !local;
    if (local)
      ent.Comp.NextScanAt = this._timing.CurTime + this.GetRefreshRate(ent);
    ent.Comp.Blips.Clear();
    this.Dirty<IntelDetectorComponent>(ent);
    this.UpdateAppearance(ent);
  }

  private TimeSpan GetRefreshRate(Entity<IntelDetectorComponent> ent)
  {
    return !ent.Comp.Short ? ent.Comp.LongRefresh : ent.Comp.ShortRefresh;
  }

  private void UpdateAppearance(Entity<IntelDetectorComponent> ent)
  {
    this._appearance.SetData((EntityUid) ent, (Enum) IntelDetectorLayer.State, (object) ent.Comp.Enabled);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<IntelDetectorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<IntelDetectorComponent>();
    EntityUid uid;
    IntelDetectorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Enabled && !(curTime < comp1.NextScanAt))
      {
        comp1.LastScan = curTime;
        comp1.NextScanAt = curTime + this.GetRefreshRate((Entity<IntelDetectorComponent>) (uid, comp1));
        this.Dirty(uid, (IComponent) comp1);
        int range = comp1.Short ? comp1.ShortRange : comp1.LongRange;
        this._tracked.Clear();
        this._entityLookup.GetEntitiesInRange<IntelDetectorTrackedComponent>(uid.ToCoordinates(), (float) range, this._tracked);
        comp1.Blips.Clear();
        foreach (Entity<IntelDetectorTrackedComponent> entity in this._tracked)
          comp1.Blips.Add(new Blip(this._transform.GetMapCoordinates((EntityUid) entity), false));
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
