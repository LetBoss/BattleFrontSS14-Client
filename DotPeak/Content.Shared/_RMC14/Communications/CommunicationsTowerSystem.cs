// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Communications.CommunicationsTowerSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Dialog;
using Content.Shared._RMC14.Intel;
using Content.Shared._RMC14.Power;
using Content.Shared._RMC14.Tools;
using Content.Shared._RMC14.Weapons.Ranged.IFF;
using Content.Shared._RMC14.Xenonids.ManageHive.Boons;
using Content.Shared.Administration.Logs;
using Content.Shared.Damage;
using Content.Shared.Database;
using Content.Shared.Destructible;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Power;
using Content.Shared.Radio;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Communications;

public sealed class CommunicationsTowerSystem : EntitySystem
{
  [Dependency]
  private ISharedAdminLogManager _adminLog;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private IComponentFactory _compFactory;
  [Dependency]
  private DialogSystem _dialog;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private HiveBoonSystem _hiveBoon;
  [Dependency]
  private IntelSystem _intel;
  [Dependency]
  private GunIFFSystem _gunIFF;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private IPrototypeManager _prototypes;
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedRMCPowerSystem _rmcPower;
  [Dependency]
  private SharedTransformSystem _transform;
  private readonly Dictionary<EntProtoId, List<Entity<CommunicationsTowerSpawnerComponent>>> _spawners = new Dictionary<EntProtoId, List<Entity<CommunicationsTowerSpawnerComponent>>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<CommunicationsTowerComponent, MapInitEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, MapInitEvent>(this.OnTowerMapInit));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, DamageChangedEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, DamageChangedEvent>(this.OnTowerDamageChanged));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, BreakageEventArgs>(new EntityEventRefHandler<CommunicationsTowerComponent, BreakageEventArgs>(this.OnTowerBreakage));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, ExaminedEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, ExaminedEvent>(this.OnTowerExamined));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, InteractUsingEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, InteractUsingEvent>(this.OnTowerInteractUsing));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, DialogChosenEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, DialogChosenEvent>(this.OnTowerDialogChosen));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, CommunicationsTowerWipeDoAfterEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, CommunicationsTowerWipeDoAfterEvent>(this.OnTowerDialogWipeDoAfter));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, CommunicationsTowerAddDoAfterEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, CommunicationsTowerAddDoAfterEvent>(this.OnTowerDialogAddDoAfter));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, InteractHandEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, InteractHandEvent>(this.OnTowerInteractHand));
    this.SubscribeLocalEvent<CommunicationsTowerComponent, PowerChangedEvent>(new EntityEventRefHandler<CommunicationsTowerComponent, PowerChangedEvent>(this.OnTowerPowerChangedEvent));
  }

  private void OnTowerMapInit(Entity<CommunicationsTowerComponent> ent, ref MapInitEvent args)
  {
    this.UpdateAppearance(ent);
  }

  private void OnTowerDamageChanged(
    Entity<CommunicationsTowerComponent> ent,
    ref DamageChangedEvent args)
  {
    if (args.Damageable.TotalDamage > FixedPoint2.Zero || ent.Comp.State != CommunicationsTowerState.Broken)
      return;
    this.ChangeState(ent, CommunicationsTowerState.Off);
  }

  private void OnTowerBreakage(Entity<CommunicationsTowerComponent> ent, ref BreakageEventArgs args)
  {
    this.ChangeState(ent, CommunicationsTowerState.Broken);
  }

  private void OnTowerExamined(Entity<CommunicationsTowerComponent> ent, ref ExaminedEvent args)
  {
    using (args.PushGroup("CommunicationsTowerComponent"))
    {
      string markup = $"[color=cyan]If placed {(int) this._hiveBoon.CommunicationTowerXenoTakeoverTime.TotalMinutes} minutes into the round, a hive cluster will turn into a hive pylon when its weeds take over this![/color]";
      args.PushMarkup(markup);
      if (ent.Comp.State != CommunicationsTowerState.Broken)
        return;
      args.PushMarkup("[color=red]It is damaged and needs a welder for repairs![/color]");
    }
  }

  private void OnTowerInteractUsing(
    Entity<CommunicationsTowerComponent> ent,
    ref InteractUsingEvent args)
  {
    if (ent.Comp.State == CommunicationsTowerState.Broken)
      return;
    RMCDeviceBreakerComponent comp;
    if (this.TryComp<RMCDeviceBreakerComponent>(args.Used, out comp) && ent.Comp.State != CommunicationsTowerState.Broken)
    {
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.User, comp.DoAfterTime, (DoAfterEvent) new RMCDeviceBreakerDoAfterEvent(), new EntityUid?(args.Used), new EntityUid?(args.Target), new EntityUid?(args.Used))
      {
        BreakOnMove = true,
        RequireCanInteract = true,
        BreakOnHandChange = true,
        DuplicateCondition = DuplicateConditions.SameTool
      });
    }
    else
    {
      if (!this.HasComp<MultitoolComponent>(args.Used))
        return;
      List<DialogOption> options = new List<DialogOption>()
      {
        new DialogOption("Wipe communication frequencies"),
        new DialogOption("Add your faction's frequencies")
      };
      this._dialog.OpenOptions((EntityUid) ent, args.User, "TC-3T comms tower", options);
    }
  }

  private void OnTowerDialogChosen(
    Entity<CommunicationsTowerComponent> ent,
    ref DialogChosenEvent args)
  {
    TimeSpan delay = TimeSpan.Zero;
    DoAfterEvent @event;
    if (args.Index == 0)
    {
      @event = (DoAfterEvent) new CommunicationsTowerWipeDoAfterEvent();
    }
    else
    {
      @event = (DoAfterEvent) new CommunicationsTowerAddDoAfterEvent();
      delay = TimeSpan.FromSeconds(1L);
    }
    this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, args.Actor, delay, @event, new EntityUid?((EntityUid) ent))
    {
      BreakOnMove = true
    });
  }

  private void OnTowerDialogWipeDoAfter(
    Entity<CommunicationsTowerComponent> ent,
    ref CommunicationsTowerWipeDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled || ent.Comp.State == CommunicationsTowerState.Broken)
      return;
    args.Handled = true;
    this._popup.PopupClient($"You wipe the preexisting frequencies from the {this.Name((EntityUid) ent)}.", (EntityUid) ent, new EntityUid?(args.User), PopupType.Medium);
  }

  private void OnTowerDialogAddDoAfter(
    Entity<CommunicationsTowerComponent> ent,
    ref CommunicationsTowerAddDoAfterEvent args)
  {
    if (args.Handled || args.Cancelled || ent.Comp.State == CommunicationsTowerState.Broken)
      return;
    EntProtoId<IFFFactionComponent> faction;
    Robust.Shared.Prototypes.EntityPrototype prototype;
    FactionFrequenciesComponent component;
    if (this._gunIFF.TryGetFaction((Entity<UserIFFComponent>) args.User, out faction) && this._prototypes.TryIndex((EntProtoId) faction, out prototype) && prototype.TryGetComponent<FactionFrequenciesComponent>(out component, this._compFactory))
    {
      ent.Comp.Channels.UnionWith((IEnumerable<ProtoId<RadioChannelPrototype>>) component.Channels);
      this.Dirty<CommunicationsTowerComponent>(ent);
    }
    args.Handled = true;
    this._popup.PopupClient($"You add your faction's communication frequencies to the {this.Name((EntityUid) ent)}'s comm list.", (EntityUid) ent, new EntityUid?(args.User), PopupType.Medium);
  }

  private void OnTowerInteractHand(
    Entity<CommunicationsTowerComponent> ent,
    ref InteractHandEvent args)
  {
    if (ent.Comp.State == CommunicationsTowerState.Broken)
      this._popup.PopupClient(this.Name((EntityUid) ent) + " needs repairs to be turned back on!", (EntityUid) ent, new EntityUid?(args.User), PopupType.MediumCaution);
    else if (!this._rmcPower.IsPowered((EntityUid) ent))
    {
      this._popup.PopupClient(this.Name((EntityUid) ent) + " makes a small plaintful beep, and nothing happens. It seems to be out of power.", (EntityUid) ent, new EntityUid?(args.User), PopupType.MediumCaution);
    }
    else
    {
      CommunicationsTowerState communicationsTowerState;
      switch (ent.Comp.State)
      {
        case CommunicationsTowerState.Off:
          communicationsTowerState = CommunicationsTowerState.On;
          break;
        case CommunicationsTowerState.On:
          communicationsTowerState = CommunicationsTowerState.Off;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      CommunicationsTowerState newState = communicationsTowerState;
      ISharedAdminLogManager adminLog = this._adminLog;
      LogStringHandler logStringHandler = new LogStringHandler(10, 3);
      logStringHandler.AppendFormatted<EntityStringRepresentation>(this.ToPrettyString((Entity<MetaDataComponent>) args.User), "user", "ToPrettyString(args.User)");
      logStringHandler.AppendLiteral(" turned ");
      logStringHandler.AppendFormatted<EntityStringRepresentation?>(this.ToPrettyString(new EntityUid?((EntityUid) ent)), "tower", "ToPrettyString(ent)");
      logStringHandler.AppendLiteral(" ");
      logStringHandler.AppendFormatted<CommunicationsTowerState>(newState, "state");
      logStringHandler.AppendLiteral(".");
      ref LogStringHandler local = ref logStringHandler;
      adminLog.Add(LogType.RMCCommunicationsTower, ref local);
      this.ChangeState(ent, newState);
      if (ent.Comp.State != CommunicationsTowerState.On)
        return;
      this._intel.RestoreColonyCommunications();
    }
  }

  private void OnTowerPowerChangedEvent(
    Entity<CommunicationsTowerComponent> ent,
    ref PowerChangedEvent args)
  {
    if (ent.Comp.State != CommunicationsTowerState.On)
      return;
    if (args.Powered)
      this._intel.RestoreColonyCommunications();
    else
      this.ChangeState(ent, CommunicationsTowerState.Off);
  }

  private void ChangeState(
    Entity<CommunicationsTowerComponent> tower,
    CommunicationsTowerState newState)
  {
    tower.Comp.State = newState;
    this.Dirty<CommunicationsTowerComponent>(tower);
    CommunicationsTowerStateChangedEvent args = new CommunicationsTowerStateChangedEvent(tower);
    this.RaiseLocalEvent<CommunicationsTowerStateChangedEvent>((EntityUid) tower, args);
    this.UpdateAppearance(tower);
  }

  public bool CanTransmit(ProtoId<RadioChannelPrototype> channel)
  {
    Robust.Shared.GameObjects.EntityQueryEnumerator<CommunicationsTowerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CommunicationsTowerComponent>();
    CommunicationsTowerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out comp1))
    {
      if (comp1.State == CommunicationsTowerState.On && comp1.Channels.Contains(channel))
        return true;
    }
    return false;
  }

  public void UpdateAppearance(Entity<CommunicationsTowerComponent> tower)
  {
    this._appearance.SetData((EntityUid) tower, (Enum) CommunicationsTowerLayers.Layer, (object) tower.Comp.State);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    this._spawners.Clear();
    Robust.Shared.GameObjects.EntityQueryEnumerator<CommunicationsTowerSpawnerComponent> entityQueryEnumerator = this.EntityQueryEnumerator<CommunicationsTowerSpawnerComponent>();
    EntityUid uid1;
    CommunicationsTowerSpawnerComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid1, out comp1))
    {
      if (!this.TerminatingOrDeleted(uid1) && !this.EntityManager.IsQueuedForDeletion(uid1))
      {
        this.QueueDel(new EntityUid?(uid1));
        this._spawners.GetOrNew<EntProtoId, List<Entity<CommunicationsTowerSpawnerComponent>>>(comp1.Group).Add((Entity<CommunicationsTowerSpawnerComponent>) (uid1, comp1));
      }
    }
    foreach (List<Entity<CommunicationsTowerSpawnerComponent>> list in this._spawners.Values)
    {
      if (list.Count != 0)
      {
        Entity<CommunicationsTowerSpawnerComponent> uid2 = RandomExtensions.Pick<Entity<CommunicationsTowerSpawnerComponent>>(this._random, (IReadOnlyList<Entity<CommunicationsTowerSpawnerComponent>>) list);
        this.Spawn((string) uid2.Comp.Spawn, this._transform.GetMoverCoordinates((EntityUid) uid2));
      }
    }
  }
}
