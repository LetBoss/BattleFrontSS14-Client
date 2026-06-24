// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.SharedCMAutomatedVendorSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Animations;
using Content.Shared._RMC14.Cryostorage;
using Content.Shared._RMC14.Holiday;
using Content.Shared._RMC14.Inventory;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Rules;
using Content.Shared._RMC14.Scaling;
using Content.Shared._RMC14.TacticalMap;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Webbing;
using Content.Shared.Access;
using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Administration.Logs;
using Content.Shared.Clothing.Components;
using Content.Shared.Coordinates;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Item;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Content.Shared.Storage;
using Content.Shared.Storage.EntitySystems;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Wall;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

public abstract class SharedCMAutomatedVendorSystem : EntitySystem
{
  [Dependency]
  private AccessReaderSystem _accessReader;
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedCMInventorySystem _cmInventory;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private SharedJobSystem _job;
  [Dependency]
  private SharedMindSystem _mind;
  [Dependency]
  private SharedStorageSystem _storage;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCAnimationSystem _rmcAnimation;
  [Dependency]
  private SharedRMCHolidaySystem _rmcHoliday;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private RMCPlanetSystem _rmcPlanet;
  [Dependency]
  private SkillsSystem _skills;
  [Dependency]
  private SquadSystem _squads;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedWebbingSystem _webbing;
  [Dependency]
  private ThrowingSystem _throwingSystem;
  [Dependency]
  private SharedRankSystem _rank;
  public const string SpecialistPoints = "Specialist";
  private readonly Dictionary<EntProtoId, CMVendorEntry> _entries = new Dictionary<EntProtoId, CMVendorEntry>();
  private readonly List<CMVendorEntry> _boxEntries = new List<CMVendorEntry>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MarineScaleChangedEvent>(new EntityEventRefHandler<MarineScaleChangedEvent>(this.OnMarineScaleChanged));
    this.SubscribeLocalEvent<CMAutomatedVendorComponent, MapInitEvent>(new EntityEventRefHandler<CMAutomatedVendorComponent, MapInitEvent>(this.OnMapInit));
    this.SubscribeLocalEvent<CMAutomatedVendorComponent, ExaminedEvent>(new EntityEventRefHandler<CMAutomatedVendorComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<CMAutomatedVendorComponent, ActivatableUIOpenAttemptEvent>(new EntityEventRefHandler<CMAutomatedVendorComponent, ActivatableUIOpenAttemptEvent>(this.OnUIOpenAttempt));
    this.SubscribeLocalEvent<CMAutomatedVendorComponent, InteractUsingEvent>(new EntityEventRefHandler<CMAutomatedVendorComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.SubscribeLocalEvent<CMAutomatedVendorComponent, RMCAutomatedVendorHackDoAfterEvent>(new EntityEventRefHandler<CMAutomatedVendorComponent, RMCAutomatedVendorHackDoAfterEvent>(this.OnHack));
    this.SubscribeLocalEvent<CMAutomatedVendorComponent, DestructionEventArgs>(new EntityEventRefHandler<CMAutomatedVendorComponent, DestructionEventArgs>(this.OnVendorDestruction));
    this.SubscribeLocalEvent<RMCRecentlyVendedComponent, GotEquippedHandEvent>(new EntityEventRefHandler<RMCRecentlyVendedComponent, GotEquippedHandEvent>(this.OnRecentlyGotEquipped<GotEquippedHandEvent>));
    this.SubscribeLocalEvent<RMCRecentlyVendedComponent, GotEquippedEvent>(new EntityEventRefHandler<RMCRecentlyVendedComponent, GotEquippedEvent>(this.OnRecentlyGotEquipped<GotEquippedEvent>));
    this.SubscribeLocalEvent<RMCSpecCryoRefundComponent, EnteredCryostorageEvent>(new EntityEventRefHandler<RMCSpecCryoRefundComponent, EnteredCryostorageEvent>(this.OnSpecEnteredCryostorageEvent));
    this.Subs.BuiEvents<CMAutomatedVendorComponent>((object) CMAutomatedVendorUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<CMAutomatedVendorComponent>) (subs => subs.Event<CMVendorVendBuiMsg>(new EntityEventRefHandler<CMAutomatedVendorComponent, CMVendorVendBuiMsg>(this.OnVendBui))));
  }

  private void OnMarineScaleChanged(ref MarineScaleChangedEvent ev)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<CMAutomatedVendorComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CMAutomatedVendorComponent>();
    EntityUid uid;
    CMAutomatedVendorComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      if (comp1.Scaling)
      {
        bool flag = false;
        foreach (CMVendorSection section in comp1.Sections)
        {
          foreach (CMVendorEntry entry in section.Entries)
          {
            int? multiplier = entry.Multiplier;
            if (multiplier.HasValue)
            {
              int valueOrDefault1 = multiplier.GetValueOrDefault();
              int? max1 = entry.Max;
              if (max1.HasValue)
              {
                int valueOrDefault2 = max1.GetValueOrDefault();
                if (!entry.Box.HasValue)
                {
                  int num1 = (int) Math.Round(ev.New * (double) valueOrDefault1) - valueOrDefault2;
                  if (num1 > 0)
                  {
                    CMVendorEntry cmVendorEntry1 = entry;
                    int? amount = cmVendorEntry1.Amount;
                    int num2 = num1;
                    cmVendorEntry1.Amount = amount.HasValue ? new int?(amount.GetValueOrDefault() + num2) : new int?();
                    CMVendorEntry cmVendorEntry2 = entry;
                    int? max2 = cmVendorEntry2.Max;
                    int num3 = num1;
                    cmVendorEntry2.Max = max2.HasValue ? new int?(max2.GetValueOrDefault() + num3) : new int?();
                    flag = true;
                    this.AmountUpdated((Entity<CMAutomatedVendorComponent>) (uid, comp1), entry);
                  }
                }
              }
            }
          }
        }
        if (flag)
          this.Dirty(uid, (IComponent) comp1);
      }
    }
  }

  private void OnMapInit(Entity<CMAutomatedVendorComponent> ent, ref MapInitEvent args)
  {
    TransformComponent xform = this.Transform(ent.Owner);
    this._entries.Clear();
    this._boxEntries.Clear();
    foreach (CMVendorSection section in ent.Comp.Sections)
    {
      foreach (CMVendorEntry entry in section.Entries)
      {
        this._entries.TryAdd(entry.Id, entry);
        if (entry.Box.HasValue)
        {
          this._boxEntries.Add(entry);
        }
        else
        {
          entry.Multiplier = entry.Amount;
          entry.Max = entry.Amount;
          if (this._rmcPlanet.IsOnPlanet(xform))
          {
            int? amount = entry.Amount;
            if (amount.HasValue)
            {
              int valueOrDefault1 = amount.GetValueOrDefault();
              int? randomUnstockAmount = ent.Comp.RandomUnstockAmount;
              if (randomUnstockAmount.HasValue)
              {
                int valueOrDefault2 = randomUnstockAmount.GetValueOrDefault();
                entry.Amount = valueOrDefault2 != -1 ? new int?(this._random.Next(1, valueOrDefault2)) : new int?(this._random.Next(1, valueOrDefault1));
              }
              float? randomEmptyChance = ent.Comp.RandomEmptyChance;
              if (randomEmptyChance.HasValue && this._random.Prob(randomEmptyChance.GetValueOrDefault()))
                entry.Amount = new int?(0);
            }
          }
        }
      }
    }
    foreach (CMVendorEntry boxEntry in this._boxEntries)
    {
      EntProtoId? box = boxEntry.Box;
      CMVendorEntry entry;
      if (box.HasValue && this._entries.TryGetValue(box.GetValueOrDefault(), out entry))
        this.AmountUpdated(ent, entry);
    }
    if (this._boxEntries.Count <= 0)
      return;
    this.Dirty<CMAutomatedVendorComponent>(ent);
  }

  private void OnExamined(Entity<CMAutomatedVendorComponent> ent, ref ExaminedEvent args)
  {
    if (!this._skills.HasSkill((Entity<SkillsComponent>) args.Examiner, ent.Comp.HackSkill, ent.Comp.HackSkillLevel) || !ent.Comp.Hackable)
      return;
    using (args.PushGroup("CMAutomatedVendorComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-vending-machine-can-hack"));
  }

  private void OnUIOpenAttempt(
    Entity<CMAutomatedVendorComponent> vendor,
    ref ActivatableUIOpenAttemptEvent args)
  {
    if (args.Cancelled)
      return;
    CMVendorUserComponent comp1;
    RMCVendorUserRechargeComponent comp2;
    if (this.TryComp<CMVendorUserComponent>(args.User, out comp1) && this.TryComp<RMCVendorUserRechargeComponent>(args.User, out comp2))
    {
      int num = (int) Math.Floor((this._timing.CurTime - comp2.LastUpdate) / comp2.TimePerUpdate * (double) comp2.PointsPerUpdate);
      if (num > 0)
      {
        comp1.Points = Math.Min(comp2.MaxPoints, comp1.Points + num);
        comp2.LastUpdate = this._timing.CurTime;
        this.DirtyEntity(args.User);
      }
    }
    if (this.HasComp<BypassInteractionChecksComponent>(args.User))
      return;
    AccessReaderComponent comp3;
    if (this.TryComp<AccessReaderComponent>((EntityUid) vendor, out comp3) && comp3.Enabled && comp3.AccessLists.Count > 0)
    {
      foreach (EntityUid orInventoryEntity in this._inventory.GetHandOrInventoryEntities((Entity<HandsComponent, InventoryComponent>) args.User))
      {
        IdCardOwnerComponent comp4;
        if (this.HasComp<IdCardComponent>(orInventoryEntity) && this.TryComp<IdCardOwnerComponent>(orInventoryEntity, out comp4) && comp4.Id != args.User)
        {
          this._popup.PopupClient(this.Loc.GetString("cm-vending-machine-wrong-card"), (EntityUid) vendor, new EntityUid?(args.User));
          args.Cancel();
          return;
        }
      }
    }
    if (this.TryAuthorizeVendorUse(vendor, args.User, comp1))
      return;
    args.Cancel();
  }

  private void OnInteractUsing(Entity<CMAutomatedVendorComponent> ent, ref InteractUsingEvent args)
  {
    if (!this.HasComp<MultitoolComponent>(args.Used))
      return;
    args.Handled = true;
    if (!ent.Comp.Hackable)
      this._popup.PopupClient(this.Loc.GetString("rmc-vending-machine-cannot-hack", ("vendor", (object) ent)), (EntityUid) ent, new EntityUid?(args.User));
    else if (!this._skills.HasSkill((Entity<SkillsComponent>) args.User, ent.Comp.HackSkill, ent.Comp.HackSkillLevel))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-vending-machine-hack-no-skill", ("vendor", (object) ent)), (EntityUid) ent, new EntityUid?(args.User), PopupType.SmallCaution);
    }
    else
    {
      TimeSpan delay = ent.Comp.HackDelay * (double) this._skills.GetSkillDelayMultiplier((Entity<SkillsComponent>) args.User, ent.Comp.HackSkill);
      RMCAutomatedVendorHackDoAfterEvent @event = new RMCAutomatedVendorHackDoAfterEvent();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) ent), new EntityUid?((EntityUid) ent), new EntityUid?(args.Used))))
        return;
      this._popup.PopupClient(this.Loc.GetString("rmc-vending-machine-hack-start", ("vendor", (object) ent)), (EntityUid) ent, new EntityUid?(args.User));
    }
  }

  private void OnHack(
    Entity<CMAutomatedVendorComponent> ent,
    ref RMCAutomatedVendorHackDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
      return;
    args.Handled = true;
    ent.Comp.Hacked = !ent.Comp.Hacked;
    this.Dirty<CMAutomatedVendorComponent>(ent);
    this._popup.PopupClient(ent.Comp.Hacked ? this.Loc.GetString("rmc-vending-machine-hack-finish-remove", ("vendor", (object) ent)) : this.Loc.GetString("rmc-vending-machine-hack-finish-restore", ("vendor", (object) ent)), (EntityUid) ent, new EntityUid?(args.User));
    AccessReaderComponent comp;
    if (!this.TryComp<AccessReaderComponent>((EntityUid) ent, out comp))
      return;
    List<ProtoId<AccessLevelPrototype>> accesses = ent.Comp.Hacked ? new List<ProtoId<AccessLevelPrototype>>() : ent.Comp.Access;
    this._accessReader.SetAccesses((Entity<AccessReaderComponent>) ((EntityUid) ent, comp), accesses);
  }

  private void OnVendorDestruction(
    Entity<CMAutomatedVendorComponent> vendor,
    ref DestructionEventArgs args)
  {
    if (!vendor.Comp.EjectContentsOnDestruction)
      return;
    this.EjectAllVendorContents(vendor);
  }

  private void EjectAllVendorContents(Entity<CMAutomatedVendorComponent> vendor)
  {
    foreach ((EntProtoId entProtoId, int Amount) in this.GetAvailableInventoryWithAmounts(vendor.Comp))
    {
      for (int index = 0; index < Amount; ++index)
      {
        EntityCoordinates coordinates = this.Transform((EntityUid) vendor).Coordinates;
        this._throwingSystem.TryThrow(this.Spawn((string) entProtoId, coordinates), new Vector2(this._random.NextFloat(-1f, 1f), this._random.NextFloat(-1f, 1f)), this._random.NextFloat(1f, 7f));
      }
    }
  }

  private List<(EntProtoId Id, int Amount)> GetAvailableInventoryWithAmounts(
    CMAutomatedVendorComponent component)
  {
    List<(EntProtoId, int)> inventoryWithAmounts = new List<(EntProtoId, int)>();
    foreach (CMVendorSection section in component.Sections)
    {
      foreach (CMVendorEntry entry in section.Entries)
      {
        int? amount = entry.Amount;
        int num = 0;
        if (amount.GetValueOrDefault() > num & amount.HasValue)
          inventoryWithAmounts.Add((entry.Id, entry.Amount.Value));
      }
    }
    return inventoryWithAmounts;
  }

  private void OnRecentlyGotEquipped<T>(Entity<RMCRecentlyVendedComponent> ent, ref T args)
  {
    this.RemCompDeferred<WallMountComponent>((EntityUid) ent);
  }

  protected virtual void OnVendBui(
    Entity<CMAutomatedVendorComponent> vendor,
    ref CMVendorVendBuiMsg args)
  {
    this._audio.PlayPredicted(vendor.Comp.Sound, (EntityUid) vendor, new EntityUid?(args.Actor));
    this._rmcAnimation.TryFlick((Entity<RMCAnimationComponent>) vendor.Owner, vendor.Comp.AnimationSprite, vendor.Comp.BaseSprite);
    if (this._net.IsClient)
      return;
    CMAutomatedVendorComponent comp = vendor.Comp;
    int count1 = comp.Sections.Count;
    EntityUid actor = args.Actor;
    CMVendorUserComponent user = this.CompOrNull<CMVendorUserComponent>(actor);
    if (!this.TryAuthorizeVendorUse(vendor, actor, user))
      return;
    if (args.Section < 0 || args.Section >= count1)
    {
      this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} sent an invalid vend section: {args.Section}. Max: {count1}");
    }
    else
    {
      CMVendorSection section = comp.Sections[args.Section];
      int count2 = section.Entries.Count;
      if (args.Entry < 0 || args.Entry >= count2)
      {
        this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} sent an invalid vend entry: {args.Entry}. Max: {count2}");
      }
      else
      {
        CMVendorEntry entry1 = section.Entries[args.Entry];
        int? amount = entry1.Amount;
        if (amount.HasValue && amount.GetValueOrDefault() <= 0)
          return;
        EntityPrototype prototype;
        if (!this._prototypes.TryIndex(entry1.Id, out prototype))
        {
          this.Log.Error($"Tried to vend non-existent entity: {entry1.Id}");
        }
        else
        {
          string takeAll = section.TakeAll;
          if (takeAll != null)
          {
            user = this.EnsureComp<CMVendorUserComponent>(actor);
            if (!user.TakeAll.Add((takeAll, entry1.Id)))
            {
              this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to buy too many take-alls.");
              return;
            }
            this.Dirty(actor, (IComponent) user);
          }
          string takeOne = section.TakeOne;
          if (takeOne != null)
          {
            user = this.EnsureComp<CMVendorUserComponent>(actor);
            if (!user.TakeOne.Add(takeOne))
            {
              this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to buy too many take-ones.");
              return;
            }
            this.Dirty(actor, (IComponent) user);
          }
          bool flag1 = true;
          EntityUid mindId;
          if (this._mind.TryGetMind(args.Actor, out mindId, out MindComponent _))
          {
            foreach (ProtoId<JobPrototype> job in section.Jobs)
            {
              if (!this._job.MindHasJobWithId(new EntityUid?(mindId), job.Id))
              {
                flag1 = false;
              }
              else
              {
                flag1 = true;
                break;
              }
            }
          }
          bool flag2 = true;
          foreach (ProtoId<RankPrototype> rank1 in section.Ranks)
          {
            RankPrototype rank2 = this._rank.GetRank(actor);
            if (rank2 == null || rank1 != (ProtoId<RankPrototype>) rank2)
            {
              flag2 = false;
            }
            else
            {
              flag2 = true;
              break;
            }
          }
          if (!flag1 || !flag2)
            return;
          bool flag3 = section.Holidays.Count == 0;
          foreach (string holiday in section.Holidays)
          {
            if (this._rmcHoliday.IsActiveHoliday(holiday))
              flag3 = true;
          }
          if (!flag3)
            return;
          (string Id, int Amount)? choices = section.Choices;
          if (choices.HasValue)
          {
            (string Id, int Amount) valueOrDefault = choices.GetValueOrDefault();
            user = this.EnsureComp<CMVendorUserComponent>(actor);
            int num1;
            if (!user.Choices.TryGetValue(valueOrDefault.Id, out num1))
            {
              num1 = 0;
              user.Choices[valueOrDefault.Id] = num1;
              this.Dirty(actor, (IComponent) user);
            }
            if (num1 >= valueOrDefault.Amount)
            {
              this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to buy too many choices.");
              return;
            }
            int num2;
            user.Choices[valueOrDefault.Id] = num2 = num1 + 1;
            this.Dirty(actor, (IComponent) user);
          }
          int? nullable = section.SharedSpecLimit;
          if (nullable.HasValue)
          {
            nullable.GetValueOrDefault();
            if (!this.HasComp<IgnoreSpecLimitsComponent>(actor) && this.HasComp<RMCVendorSpecialistComponent>((EntityUid) vendor))
            {
              RMCVendorSpecialistComponent specialistComponent1 = this.Comp<RMCVendorSpecialistComponent>((EntityUid) vendor);
              int num3;
              if (specialistComponent1.GlobalSharedVends.TryGetValue(args.Entry, out num3))
              {
                int num4 = num3;
                nullable = section.SharedSpecLimit;
                int valueOrDefault = nullable.GetValueOrDefault();
                if (num4 >= valueOrDefault & nullable.HasValue)
                {
                  ResetChoices();
                  this._popup.PopupEntity(this.Loc.GetString("cm-vending-machine-specialist-max"), (EntityUid) vendor, actor);
                  return;
                }
              }
              Robust.Shared.GameObjects.EntityQueryEnumerator<RMCVendorSpecialistComponent> entityQueryEnumerator = this.EntityQueryEnumerator<RMCVendorSpecialistComponent>();
              int num5 = 0;
              if (specialistComponent1.GlobalSharedVends.TryGetValue(args.Entry, out num3))
                num5 = num3;
              EntityUid uid;
              while (entityQueryEnumerator.MoveNext(out uid, out RMCVendorSpecialistComponent _))
              {
                RMCVendorSpecialistComponent specialistComponent2 = this.EnsureComp<RMCVendorSpecialistComponent>(uid);
                foreach (int linkedEntry in args.LinkedEntries)
                {
                  int num6;
                  specialistComponent2.GlobalSharedVends.TryGetValue(linkedEntry, out num6);
                  num5 += num6;
                }
                if (specialistComponent2.GlobalSharedVends.TryGetValue(args.Entry, out num3))
                {
                  if (num3 > num5)
                    num5 = specialistComponent2.GlobalSharedVends[args.Entry];
                  else
                    specialistComponent2.GlobalSharedVends[args.Entry] = num5;
                }
                else
                  specialistComponent2.GlobalSharedVends.Add(args.Entry, num5);
                this.Dirty(uid, (IComponent) specialistComponent2);
              }
              specialistComponent1.GlobalSharedVends[args.Entry] = num5;
              int globalSharedVend = specialistComponent1.GlobalSharedVends[args.Entry];
              nullable = section.SharedSpecLimit;
              int valueOrDefault1 = nullable.GetValueOrDefault();
              if (globalSharedVend >= valueOrDefault1 & nullable.HasValue)
              {
                ResetChoices();
                this._popup.PopupEntity(this.Loc.GetString("cm-vending-machine-specialist-max"), vendor.Owner, actor);
                return;
              }
              ++specialistComponent1.GlobalSharedVends[args.Entry];
              this.Dirty((EntityUid) vendor, (IComponent) specialistComponent1);
              this.AddComp<RMCSpecCryoRefundComponent>(actor, new RMCSpecCryoRefundComponent()
              {
                Vendor = (EntityUid) vendor,
                Entry = args.Entry
              }, true);
            }
          }
          if (entry1.Points.HasValue)
          {
            if (user == null)
            {
              this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} tried to buy {entry1.Id} for {entry1.Points} points without having points.");
              return;
            }
            int num7;
            if (vendor.Comp.PointsType != null)
            {
              Dictionary<string, int> extraPoints = user.ExtraPoints;
              num7 = extraPoints != null ? extraPoints.GetValueOrDefault<string, int>(vendor.Comp.PointsType) : 0;
            }
            else
              num7 = user.Points;
            int num8 = num7;
            int num9 = num8;
            nullable = entry1.Points;
            int valueOrDefault = nullable.GetValueOrDefault();
            if (num9 < valueOrDefault & nullable.HasValue)
            {
              this.Log.Error($"{this.ToPrettyString((Entity<MetaDataComponent>) actor)} with {user.Points} tried to buy {entry1.Id} for {entry1.Points} points without having enough points.");
              return;
            }
            if (vendor.Comp.PointsType == null)
              user.Points -= entry1.Points.Value;
            else if (user.ExtraPoints != null)
              user.ExtraPoints[vendor.Comp.PointsType] = num8 - entry1.Points.GetValueOrDefault();
            this.Dirty(actor, (IComponent) user);
          }
          if (entry1.Amount.HasValue)
          {
            EntProtoId? box = entry1.Box;
            if (box.HasValue)
            {
              EntProtoId valueOrDefault = box.GetValueOrDefault();
              bool flag4 = false;
              foreach (CMVendorSection section1 in vendor.Comp.Sections)
              {
                foreach (CMVendorEntry entry2 in section1.Entries)
                {
                  if (!(entry2.Id != valueOrDefault))
                  {
                    CMVendorEntry cmVendorEntry = entry2;
                    nullable = cmVendorEntry.Amount;
                    int boxRemoveAmount = this.GetBoxRemoveAmount(entry1);
                    cmVendorEntry.Amount = nullable.HasValue ? new int?(nullable.GetValueOrDefault() - boxRemoveAmount) : new int?();
                    this.Dirty<CMAutomatedVendorComponent>(vendor);
                    this.AmountUpdated(vendor, entry2);
                    flag4 = true;
                    break;
                  }
                }
                if (flag4)
                  break;
              }
            }
            else
            {
              CMVendorEntry cmVendorEntry = entry1;
              nullable = cmVendorEntry.Amount;
              cmVendorEntry.Amount = nullable.HasValue ? new int?(nullable.GetValueOrDefault() - 1) : new int?();
              this.Dirty<CMAutomatedVendorComponent>(vendor);
              this.AmountUpdated(vendor, entry1);
            }
          }
          if (entry1.GiveSquadRoleName.HasValue || entry1.GiveIcon != null)
          {
            RMCVendorRoleOverrideComponent overrideComponent = this.EnsureComp<RMCVendorRoleOverrideComponent>(actor);
            overrideComponent.GiveSquadRoleName = entry1.GiveSquadRoleName;
            overrideComponent.IsAppendSquadRoleName = entry1.IsAppendSquadRoleName;
            overrideComponent.GiveIcon = entry1.GiveIcon;
            this.Dirty(actor, (IComponent) overrideComponent);
            this._squads.UpdateSquadTitle(actor);
          }
          if (entry1.GiveMapBlip != null)
          {
            MapBlipIconOverrideComponent overrideComponent = this.EnsureComp<MapBlipIconOverrideComponent>(actor);
            overrideComponent.Icon = entry1.GiveMapBlip;
            this.Dirty(actor, (IComponent) overrideComponent);
          }
          if (entry1.GivePrefix.HasValue)
          {
            JobPrefixComponent jobPrefixComponent = this.EnsureComp<JobPrefixComponent>(actor);
            if (entry1.IsAppendPrefix)
              jobPrefixComponent.AdditionalPrefix = entry1.GivePrefix;
            else
              jobPrefixComponent.Prefix = entry1.GivePrefix.Value;
            this.Dirty(actor, (IComponent) jobPrefixComponent);
          }
          Vector2 minOffset = comp.MinOffset;
          Vector2 maxOffset = comp.MaxOffset;
          for (int index = 0; index < entry1.Spawn; ++index)
          {
            Vector2 offset = this._random.NextVector2Box(minOffset.X, minOffset.Y, maxOffset.X, maxOffset.Y);
            CMVendorBundleComponent component;
            if (prototype.TryGetComponent<CMVendorBundleComponent>(out component, this._compFactory))
            {
              foreach (EntProtoId toVend in component.Bundle)
                this.Vend((EntityUid) vendor, actor, toVend, offset, entry1.ReplaceSlot);
            }
            else
              this.Vend((EntityUid) vendor, actor, entry1.Id, offset, entry1.ReplaceSlot);
          }
          CMChangeUserOnVendComponent component1;
          if (!prototype.TryGetComponent<CMChangeUserOnVendComponent>(out component1, this._compFactory) || component1.AddComponents == null)
            return;
          this.EntityManager.AddComponents(actor, component1.AddComponents, true);
        }
      }

      void ResetChoices()
      {
        (string Id, int Amount)? choices = section.Choices;
        if (choices.HasValue)
        {
          (string Id, int Amount) valueOrDefault = choices.GetValueOrDefault();
          if (user != null)
            user.Choices[valueOrDefault.Id]--;
        }
        string takeOne = section.TakeOne;
        if (takeOne == null || user == null)
          return;
        user.TakeOne.Remove(takeOne);
      }
    }
  }

  private bool TryAuthorizeVendorUse(
    Entity<CMAutomatedVendorComponent> vendor,
    EntityUid user,
    CMVendorUserComponent? vendorUser,
    bool showPopup = true)
  {
    CMAutomatedVendorAccessAttemptEvent args = new CMAutomatedVendorAccessAttemptEvent(user, showPopup);
    this.RaiseLocalEvent<CMAutomatedVendorAccessAttemptEvent>(vendor.Owner, args);
    if (args.Cancelled)
    {
      if (showPopup)
        this._popup.PopupClient(string.IsNullOrWhiteSpace(args.Reason) ? this.Loc.GetString("cm-vending-machine-access-denied") : args.Reason, (EntityUid) vendor, new EntityUid?(user), PopupType.SmallCaution);
      return false;
    }
    if (vendor.Comp.Hacked)
      return true;
    bool flag1 = false;
    if (vendor.Comp.Jobs.Count == 0)
    {
      flag1 = true;
    }
    else
    {
      EntityUid mindId;
      this._mind.TryGetMind(user, out mindId, out MindComponent _);
      foreach (ProtoId<JobPrototype> job in vendor.Comp.Jobs)
      {
        if (mindId.Valid && this._job.MindHasJobWithId(new EntityUid?(mindId), job.Id))
          flag1 = true;
        else if (vendorUser != null)
        {
          ProtoId<JobPrototype>? id = vendorUser.Id;
          ProtoId<JobPrototype> protoId = job;
          if ((id.HasValue ? (id.GetValueOrDefault() == protoId ? 1 : 0) : 0) != 0)
            flag1 = true;
        }
        if (flag1)
          break;
      }
    }
    bool flag2 = false;
    if (vendor.Comp.Ranks.Count == 0)
    {
      flag2 = true;
    }
    else
    {
      RankPrototype rank1 = this._rank.GetRank(user);
      if (rank1 != null)
      {
        foreach (ProtoId<RankPrototype> rank2 in vendor.Comp.Ranks)
        {
          if ((ProtoId<RankPrototype>) rank1 == rank2)
          {
            flag2 = true;
            break;
          }
        }
      }
    }
    if (flag1 & flag2)
      return true;
    if (showPopup)
      this._popup.PopupClient(this.Loc.GetString("cm-vending-machine-access-denied"), (EntityUid) vendor, new EntityUid?(user));
    return false;
  }

  private void Vend(
    EntityUid vendor,
    EntityUid player,
    EntProtoId toVend,
    Vector2 offset,
    SlotFlags? replaceSlot = null)
  {
    CMVendorMapToSquadComponent component;
    if (this._prototypes.Index(toVend).TryGetComponent<CMVendorMapToSquadComponent>(out component, this._compFactory))
    {
      SquadMemberComponent comp;
      if (this.TryComp<SquadMemberComponent>(player, out comp))
      {
        EntityUid? squad = comp.Squad;
        if (squad.HasValue)
        {
          EntityPrototype entityPrototype = this.CompOrNull<MetaDataComponent>(squad.GetValueOrDefault())?.EntityPrototype;
          EntProtoId entProtoId;
          if (entityPrototype != null && component.Map.TryGetValue((EntProtoId) entityPrototype.ID, out entProtoId))
          {
            toVend = entProtoId;
            goto label_7;
          }
        }
      }
      EntProtoId? nullable = component.Default;
      if (!nullable.HasValue)
        return;
      toVend = nullable.GetValueOrDefault();
    }
label_7:
    RMCRequisitionsVendorComponent comp1;
    Entity<RMCRequisitionsChairComponent> ent;
    if (this.TryComp<RMCRequisitionsVendorComponent>(vendor, out comp1) && comp1.Enabled && this._rmcMap.HasAnchoredEntityEnumerator<RMCRequisitionsChairComponent>(player.ToCoordinates(), out ent, facing: (DirectionFlag) 0))
    {
      Vector2 offsetItem = ent.Comp.OffsetItem;
      EntityCoordinates coordinates = ent.Owner.ToCoordinates().Offset(offsetItem);
      this.AfterVend(this.SpawnAtPosition((string) toVend, coordinates), player, vendor, offset, true, replaceSlot);
    }
    else
      this.AfterVend(this.SpawnNextToOrDrop((string) toVend, vendor), player, vendor, offset, replaceSlot: replaceSlot);
  }

  private void AfterVend(
    EntityUid spawn,
    EntityUid player,
    EntityUid vendor,
    Vector2 offset,
    bool vended = false,
    SlotFlags? replaceSlot = null)
  {
    RMCRecentlyVendedComponent recentlyVendedComponent = this.EnsureComp<RMCRecentlyVendedComponent>(spawn);
    RMCAnchoredEntitiesEnumerator entitiesEnumerator = this._rmcMap.GetAnchoredEntitiesEnumerator(spawn, facing: (DirectionFlag) 0);
    EntityUid uid;
    while (entitiesEnumerator.MoveNext(out uid))
      recentlyVendedComponent.PreventCollide.Add(uid);
    this.Dirty(spawn, (IComponent) recentlyVendedComponent);
    WallMountComponent wallMountComponent = this.EnsureComp<WallMountComponent>(spawn);
    wallMountComponent.Arc = Angle.FromDegrees(360.0);
    this.Dirty(spawn, (IComponent) wallMountComponent);
    TransformComponent comp;
    if (!vended && !this.Grab(player, spawn, replaceSlot) && this.TryComp(spawn, out comp))
      this._transform.SetLocalPosition(spawn, comp.LocalPosition + offset, comp);
    RMCAutomatedVendedUserEvent args = new RMCAutomatedVendedUserEvent(spawn, vendor);
    this.RaiseLocalEvent<RMCAutomatedVendedUserEvent>(player, ref args);
    ISharedAdminLogManager adminLog = this._adminLog;
    LogStringHandler logStringHandler = new LogStringHandler(21, 3);
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) player), "ToPrettyString(player)");
    logStringHandler.AppendLiteral(" vended ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) spawn), "ToPrettyString(spawn)");
    logStringHandler.AppendLiteral(" from vendor ");
    logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) vendor), "ToPrettyString(vendor)");
    ref LogStringHandler local = ref logStringHandler;
    adminLog.Add(LogType.RMCVend, ref local);
  }

  private bool Grab(EntityUid player, EntityUid item, SlotFlags? replaceSlot = null)
  {
    if (!this.HasComp<ItemComponent>(item))
      return false;
    if (this.TryAttachWebbing(player, item))
      return true;
    ClothingComponent comp;
    if (!this.TryComp<ClothingComponent>(item, out comp))
      return this._hands.TryPickupAnyHand(player, item);
    if (replaceSlot.HasValue)
    {
      EntityUid? uid = new EntityUid?();
      InventorySystem.InventorySlotEnumerator slotEnumerator = this._inventory.GetSlotEnumerator((Entity<InventoryComponent>) player, replaceSlot.Value);
      ContainerSlot container;
      EntityUid? nullable;
      while (slotEnumerator.MoveNext(out container))
      {
        nullable = container.ContainedEntity;
        if (nullable.HasValue)
        {
          uid = container.ContainedEntity;
          this._inventory.TryUnequip(player, container.ID, true);
          break;
        }
      }
      if (uid.HasValue && this.HasComp<StorageComponent>(item) && this.HasComp<StorageComponent>(uid))
      {
        SharedStorageSystem storage = this._storage;
        EntityUid source = uid.Value;
        EntityUid target = item;
        nullable = new EntityUid?();
        EntityUid? user = nullable;
        storage.TransferEntities(source, target, user);
      }
    }
    return this._cmInventory.TryEquipClothing(player, (Entity<ClothingComponent>) (item, comp), false) || this._hands.TryPickupAnyHand(player, item);
  }

  private bool TryAttachWebbing(EntityUid player, EntityUid item)
  {
    InventorySystem.InventorySlotEnumerator containerSlotEnumerator;
    if (this.HasComp<WebbingComponent>(item) && this._inventory.TryGetContainerSlotEnumerator((Entity<InventoryComponent>) player, out containerSlotEnumerator))
    {
      ContainerSlot container;
      while (containerSlotEnumerator.MoveNext(out container))
      {
        EntityUid? containedEntity = container.ContainedEntity;
        if (containedEntity.HasValue)
        {
          EntityUid valueOrDefault = containedEntity.GetValueOrDefault();
          WebbingClothingComponent comp;
          if (this.TryComp<WebbingClothingComponent>(valueOrDefault, out comp) && this._webbing.Attach((Entity<WebbingClothingComponent>) (valueOrDefault, comp), item, new EntityUid?(player), out bool _))
            return true;
        }
      }
    }
    return false;
  }

  public void SetPoints(Entity<CMVendorUserComponent> user, int points)
  {
    user.Comp.Points = points;
    this.Dirty<CMVendorUserComponent>(user);
  }

  public void SetExtraPoints(Entity<CMVendorUserComponent> user, string key, int points)
  {
    CMVendorUserComponent comp = user.Comp;
    if (comp.ExtraPoints == null)
      comp.ExtraPoints = new Dictionary<string, int>();
    user.Comp.ExtraPoints[key] = points;
    this.Dirty<CMVendorUserComponent>(user);
  }

  public void AmountUpdated(Entity<CMAutomatedVendorComponent> vendor, CMVendorEntry entry)
  {
    foreach (CMVendorSection section in vendor.Comp.Sections)
    {
      if (section.HasBoxes)
      {
        foreach (CMVendorEntry entry1 in section.Entries)
        {
          EntProtoId? box = entry1.Box;
          if (box.HasValue)
          {
            EntProtoId valueOrDefault = box.GetValueOrDefault();
            if (!(entry.Id != valueOrDefault))
            {
              CMVendorEntry cmVendorEntry = entry1;
              int? amount = entry.Amount;
              int boxRemoveAmount = this.GetBoxRemoveAmount(entry1);
              int? nullable = amount.HasValue ? new int?(amount.GetValueOrDefault() / boxRemoveAmount) : new int?();
              cmVendorEntry.Amount = nullable;
            }
          }
        }
      }
    }
  }

  public void SetSections(Entity<CMAutomatedVendorComponent?> vendor, List<CMVendorSection> sections)
  {
    if (!this.Resolve<CMAutomatedVendorComponent>((EntityUid) vendor, ref vendor.Comp, false))
      return;
    vendor.Comp.Sections = sections;
    this.Dirty<CMAutomatedVendorComponent>(vendor);
  }

  private int GetBoxRemoveAmount(CMVendorEntry entry)
  {
    int? boxSlots = entry.BoxSlots;
    int valueOrDefault;
    if (boxSlots.HasValue)
    {
      valueOrDefault = boxSlots.GetValueOrDefault();
    }
    else
    {
      EntityPrototype prototype;
      CMItemSlotsComponent component;
      if (this._prototypes.TryIndex(entry.Id, out prototype) && prototype.TryGetComponent<CMItemSlotsComponent>(out component, this._compFactory))
      {
        int? count = component.Count;
        if (count.HasValue)
        {
          valueOrDefault = count.GetValueOrDefault();
          goto label_6;
        }
      }
      return 1;
    }
label_6:
    int val2 = valueOrDefault;
    int? boxAmount = entry.BoxAmount;
    if (boxAmount.HasValue)
      val2 = boxAmount.GetValueOrDefault();
    return Math.Max(1, val2);
  }

  private void OnSpecEnteredCryostorageEvent(
    Entity<RMCSpecCryoRefundComponent> ent,
    ref EnteredCryostorageEvent args)
  {
    RMCVendorSpecialistComponent comp;
    int num;
    if (!this.TryComp<RMCVendorSpecialistComponent>(ent.Comp.Vendor, out comp) || !comp.GlobalSharedVends.TryGetValue(ent.Comp.Entry, out num) || num < 1)
      return;
    comp.GlobalSharedVends[ent.Comp.Entry] = num - 1;
    this.Dirty(ent.Comp.Vendor, (IComponent) comp);
  }
}
