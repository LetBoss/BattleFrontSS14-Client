// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Components.DeviceNetworkComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.DeviceNetwork.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedDeviceNetworkSystem), typeof (DeviceNet)})]
public sealed class DeviceNetworkComponent : 
  Component,
  ISerializationGenerated<DeviceNetworkComponent>,
  ISerializationGenerated
{
  [DataField("receiveFrequency", false, 1, false, false, null)]
  public uint? ReceiveFrequency;
  [DataField("receiveFrequencyId", false, 1, false, false, typeof (PrototypeIdSerializer<DeviceFrequencyPrototype>))]
  public string? ReceiveFrequencyId;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("transmitFrequency", false, 1, false, false, null)]
  public uint? TransmitFrequency;
  [DataField("transmitFrequencyId", false, 1, false, false, typeof (PrototypeIdSerializer<DeviceFrequencyPrototype>))]
  public string? TransmitFrequencyId;
  [DataField("address", false, 1, false, false, null)]
  public string Address = string.Empty;
  [DataField("customAddress", false, 1, false, false, null)]
  public bool CustomAddress;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("prefix", false, 1, false, false, null)]
  public string? Prefix;
  [DataField("receiveAll", false, 1, false, false, null)]
  public bool ReceiveAll;
  [DataField("examinableAddress", false, 1, false, false, null)]
  public bool ExaminableAddress;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("autoConnect", false, 1, false, false, null)]
  public bool AutoConnect = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("sendBroadcastAttemptEvent", false, 1, false, false, null)]
  public bool SendBroadcastAttemptEvent;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField("savableAddress", false, 1, false, false, null)]
  public bool SavableAddress = true;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedDeviceListSystem)})]
  public HashSet<EntityUid> DeviceLists = new HashSet<EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedNetworkConfiguratorSystem)})]
  public HashSet<EntityUid> Configurators = new HashSet<EntityUid>();

  [DataField("deviceNetId", false, 1, false, false, null)]
  public DeviceNetworkComponent.DeviceNetIdDefaults NetIdEnum { get; set; }

  public int DeviceNetId => (int) this.NetIdEnum;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeviceNetworkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeviceNetworkComponent) component;
    if (serialization.TryCustomCopy<DeviceNetworkComponent>(this, ref target, hookCtx, false, context))
      return;
    DeviceNetworkComponent.DeviceNetIdDefaults deviceNetIdDefaults = DeviceNetworkComponent.DeviceNetIdDefaults.Private;
    if (!serialization.TryCustomCopy<DeviceNetworkComponent.DeviceNetIdDefaults>(this.NetIdEnum, ref deviceNetIdDefaults, hookCtx, false, context))
      deviceNetIdDefaults = this.NetIdEnum;
    target.NetIdEnum = deviceNetIdDefaults;
    uint? nullable1 = new uint?();
    if (!serialization.TryCustomCopy<uint?>(this.ReceiveFrequency, ref nullable1, hookCtx, false, context))
      nullable1 = this.ReceiveFrequency;
    target.ReceiveFrequency = nullable1;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ReceiveFrequencyId, ref str1, hookCtx, false, context))
      str1 = this.ReceiveFrequencyId;
    target.ReceiveFrequencyId = str1;
    uint? nullable2 = new uint?();
    if (!serialization.TryCustomCopy<uint?>(this.TransmitFrequency, ref nullable2, hookCtx, false, context))
      nullable2 = this.TransmitFrequency;
    target.TransmitFrequency = nullable2;
    string str2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.TransmitFrequencyId, ref str2, hookCtx, false, context))
      str2 = this.TransmitFrequencyId;
    target.TransmitFrequencyId = str2;
    string str3 = (string) null;
    if (this.Address == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Address, ref str3, hookCtx, false, context))
      str3 = this.Address;
    target.Address = str3;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.CustomAddress, ref flag1, hookCtx, false, context))
      flag1 = this.CustomAddress;
    target.CustomAddress = flag1;
    string str4 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.Prefix, ref str4, hookCtx, false, context))
      str4 = this.Prefix;
    target.Prefix = str4;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.ReceiveAll, ref flag2, hookCtx, false, context))
      flag2 = this.ReceiveAll;
    target.ReceiveAll = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExaminableAddress, ref flag3, hookCtx, false, context))
      flag3 = this.ExaminableAddress;
    target.ExaminableAddress = flag3;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoConnect, ref flag4, hookCtx, false, context))
      flag4 = this.AutoConnect;
    target.AutoConnect = flag4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.SendBroadcastAttemptEvent, ref flag5, hookCtx, false, context))
      flag5 = this.SendBroadcastAttemptEvent;
    target.SendBroadcastAttemptEvent = flag5;
    bool flag6 = false;
    if (!serialization.TryCustomCopy<bool>(this.SavableAddress, ref flag6, hookCtx, false, context))
      flag6 = this.SavableAddress;
    target.SavableAddress = flag6;
    HashSet<EntityUid> entityUidSet1 = (HashSet<EntityUid>) null;
    if (this.DeviceLists == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.DeviceLists, ref entityUidSet1, hookCtx, true, context))
      entityUidSet1 = serialization.CreateCopy<HashSet<EntityUid>>(this.DeviceLists, hookCtx, context, false);
    target.DeviceLists = entityUidSet1;
    HashSet<EntityUid> entityUidSet2 = (HashSet<EntityUid>) null;
    if (this.Configurators == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Configurators, ref entityUidSet2, hookCtx, true, context))
      entityUidSet2 = serialization.CreateCopy<HashSet<EntityUid>>(this.Configurators, hookCtx, context, false);
    target.Configurators = entityUidSet2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeviceNetworkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeviceNetworkComponent target1 = (DeviceNetworkComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeviceNetworkComponent target1 = (DeviceNetworkComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DeviceNetworkComponent target1 = (DeviceNetworkComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    base.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DeviceNetworkComponent Component.Instantiate() => new DeviceNetworkComponent();

  public enum DeviceNetIdDefaults
  {
    Private = 0,
    Wired = 1,
    Wireless = 2,
    Apc = 3,
    AtmosDevices = 4,
    Reserved = 100, // 0x00000064
  }
}
