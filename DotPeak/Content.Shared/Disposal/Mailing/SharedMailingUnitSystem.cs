// Decompiled with JetBrains decompiler
// Type: Content.Shared.Disposal.Mailing.SharedMailingUnitSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Configurable;
using Content.Shared.DeviceNetwork;
using Content.Shared.DeviceNetwork.Components;
using Content.Shared.DeviceNetwork.Events;
using Content.Shared.DeviceNetwork.Systems;
using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Unit;
using Content.Shared.Disposal.Unit.Events;
using Content.Shared.Interaction;
using Content.Shared.Power.Components;
using Content.Shared.Power.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Disposal.Mailing;

public abstract class SharedMailingUnitSystem : EntitySystem
{
  [Dependency]
  private SharedDeviceNetworkSystem _deviceNetworkSystem;
  [Dependency]
  private SharedPowerReceiverSystem _power;
  [Dependency]
  protected SharedUserInterfaceSystem UserInterfaceSystem;
  private const string MailTag = "mail";
  private const string TagConfigurationKey = "tag";
  private const string NetTag = "tag";
  private const string NetSrc = "src";
  private const string NetTarget = "target";
  private const string NetCmdSent = "mail_sent";
  private const string NetCmdRequest = "get_mailer_tag";
  private const string NetCmdResponse = "mailer_tag";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, ComponentInit>(new ComponentEventHandler<MailingUnitComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, DeviceNetworkPacketEvent>(new ComponentEventHandler<MailingUnitComponent, DeviceNetworkPacketEvent>((object) this, __methodptr(OnPacketReceived)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, BeforeDisposalFlushEvent>(new ComponentEventHandler<MailingUnitComponent, BeforeDisposalFlushEvent>((object) this, __methodptr(OnBeforeFlush)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, ConfigurationUpdatedEvent>(new ComponentEventHandler<MailingUnitComponent, ConfigurationUpdatedEvent>((object) this, __methodptr(OnConfigurationUpdated)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, ActivateInWorldEvent>(new ComponentEventHandler<MailingUnitComponent, ActivateInWorldEvent>((object) this, __methodptr(HandleActivate)), new Type[1]
    {
      typeof (SharedDisposalUnitSystem)
    }, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MailingUnitComponent, TargetSelectedMessage>(new ComponentEventHandler<MailingUnitComponent, TargetSelectedMessage>((object) this, __methodptr(OnTargetSelected)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, MailingUnitComponent component, ComponentInit args)
  {
    this.UpdateTargetList(uid, component);
  }

  private void OnPacketReceived(
    EntityUid uid,
    MailingUnitComponent component,
    DeviceNetworkPacketEvent args)
  {
    string str1;
    if (!args.Data.TryGetValue<string>("command", out str1) || !this._power.IsPowered(Entity<SharedApcPowerReceiverComponent>.op_Implicit(uid)))
      return;
    switch (str1)
    {
      case "get_mailer_tag":
        this.SendTagRequestResponse(uid, args, component.Tag);
        break;
      case "mailer_tag":
        string str2;
        if (!args.Data.TryGetValue<string>("tag", out str2))
          break;
        component.TargetList.Add(str2);
        this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
        break;
    }
  }

  private void SendTagRequestResponse(EntityUid uid, DeviceNetworkPacketEvent args, string? tag)
  {
    if (tag == null)
      return;
    NetworkPayload networkPayload = new NetworkPayload();
    networkPayload["command"] = (object) "mailer_tag";
    networkPayload[nameof (tag)] = (object) tag;
    NetworkPayload data = networkPayload;
    this._deviceNetworkSystem.QueuePacket(uid, args.Address, data, new uint?(args.Frequency));
  }

  private void OnBeforeFlush(
    EntityUid uid,
    MailingUnitComponent component,
    BeforeDisposalFlushEvent args)
  {
    if (string.IsNullOrEmpty(component.Target))
    {
      args.Cancel();
    }
    else
    {
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
      args.Tags.Add("mail");
      args.Tags.Add(component.Target);
      this.BroadcastSentMessage(uid, component);
    }
  }

  private void BroadcastSentMessage(
    EntityUid uid,
    MailingUnitComponent component,
    DeviceNetworkComponent? device = null)
  {
    if (string.IsNullOrEmpty(component.Tag) || string.IsNullOrEmpty(component.Target) || !this.Resolve<DeviceNetworkComponent>(uid, ref device, true))
      return;
    NetworkPayload networkPayload = new NetworkPayload();
    networkPayload["command"] = (object) "mail_sent";
    networkPayload["src"] = (object) component.Tag;
    networkPayload["target"] = (object) component.Target;
    NetworkPayload data = networkPayload;
    this._deviceNetworkSystem.QueuePacket(uid, (string) null, data, device: device);
  }

  private void UpdateTargetList(
    EntityUid uid,
    MailingUnitComponent component,
    DeviceNetworkComponent? device = null)
  {
    if (!this.Resolve<DeviceNetworkComponent>(uid, ref device, false))
      return;
    NetworkPayload networkPayload = new NetworkPayload();
    networkPayload["command"] = (object) "get_mailer_tag";
    NetworkPayload data = networkPayload;
    component.TargetList.Clear();
    this._deviceNetworkSystem.QueuePacket(uid, (string) null, data, device: device);
  }

  private void OnConfigurationUpdated(
    EntityUid uid,
    MailingUnitComponent component,
    ConfigurationUpdatedEvent args)
  {
    Dictionary<string, string> config = args.Configuration.Config;
    if (!config.ContainsKey("tag") || config["tag"] == string.Empty)
    {
      component.Tag = (string) null;
    }
    else
    {
      component.Tag = config["tag"];
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  private void HandleActivate(
    EntityUid uid,
    MailingUnitComponent component,
    ActivateInWorldEvent args)
  {
    ActorComponent actorComponent;
    if (args.Handled || !args.Complex || !this.TryComp<ActorComponent>(args.User, ref actorComponent))
      return;
    args.Handled = true;
    this.UpdateTargetList(uid, component);
    this.UserInterfaceSystem.OpenUi(Entity<UserInterfaceComponent>.op_Implicit(uid), (Enum) MailingUnitUiKey.Key, actorComponent.PlayerSession, false);
  }

  private void OnTargetSelected(
    EntityUid uid,
    MailingUnitComponent component,
    TargetSelectedMessage args)
  {
    component.Target = args.Target;
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }
}
