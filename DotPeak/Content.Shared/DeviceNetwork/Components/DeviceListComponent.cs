// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Components.DeviceListComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.DeviceNetwork.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedDeviceListSystem)})]
public sealed class DeviceListComponent : 
  Component,
  ISerializationGenerated<DeviceListComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public HashSet<EntityUid> Devices = new HashSet<EntityUid>();
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  public int DeviceLimit = 32 /*0x20*/;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool IsAllowList = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool HandleIncomingPackets;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedNetworkConfiguratorSystem)})]
  public HashSet<EntityUid> Configurators = new HashSet<EntityUid>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeviceListComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeviceListComponent) component;
    if (serialization.TryCustomCopy<DeviceListComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<EntityUid> entityUidSet1 = (HashSet<EntityUid>) null;
    if (this.Devices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Devices, ref entityUidSet1, hookCtx, true, context))
      entityUidSet1 = serialization.CreateCopy<HashSet<EntityUid>>(this.Devices, hookCtx, context, false);
    target.Devices = entityUidSet1;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.DeviceLimit, ref num, hookCtx, false, context))
      num = this.DeviceLimit;
    target.DeviceLimit = num;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsAllowList, ref flag1, hookCtx, false, context))
      flag1 = this.IsAllowList;
    target.IsAllowList = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.HandleIncomingPackets, ref flag2, hookCtx, false, context))
      flag2 = this.HandleIncomingPackets;
    target.HandleIncomingPackets = flag2;
    HashSet<EntityUid> entityUidSet2 = (HashSet<EntityUid>) null;
    if (this.Configurators == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<EntityUid>>(this.Configurators, ref entityUidSet2, hookCtx, true, context))
      entityUidSet2 = serialization.CreateCopy<HashSet<EntityUid>>(this.Configurators, hookCtx, context, false);
    target.Configurators = entityUidSet2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeviceListComponent target,
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
    DeviceListComponent target1 = (DeviceListComponent) target;
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
    DeviceListComponent target1 = (DeviceListComponent) target;
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
    DeviceListComponent target1 = (DeviceListComponent) target;
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
  virtual DeviceListComponent Component.Instantiate() => new DeviceListComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DeviceListComponent_AutoState : IComponentState
  {
    public HashSet<NetEntity> Devices;
    public bool IsAllowList;
    public bool HandleIncomingPackets;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DeviceListComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeviceListComponent, ComponentGetState>(new ComponentEventRefHandler<DeviceListComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<DeviceListComponent, ComponentHandleState>(new ComponentEventRefHandler<DeviceListComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      DeviceListComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new DeviceListComponent.DeviceListComponent_AutoState()
      {
        Devices = this.GetNetEntitySet(component.Devices),
        IsAllowList = component.IsAllowList,
        HandleIncomingPackets = component.HandleIncomingPackets
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DeviceListComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is DeviceListComponent.DeviceListComponent_AutoState current))
        return;
      this.EnsureEntitySet<DeviceListComponent>(current.Devices, uid, component.Devices);
      component.IsAllowList = current.IsAllowList;
      component.HandleIncomingPackets = current.HandleIncomingPackets;
    }
  }
}
