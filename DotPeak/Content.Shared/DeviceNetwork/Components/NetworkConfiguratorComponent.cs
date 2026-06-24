// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceNetwork.Components.NetworkConfiguratorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceNetwork.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.DeviceNetwork.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SharedNetworkConfiguratorSystem)})]
public sealed class NetworkConfiguratorComponent : 
  Component,
  ISerializationGenerated<NetworkConfiguratorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool LinkModeActive = true;
  public EntityUid? ActiveDeviceLink;
  public EntityUid? DeviceLinkTarget;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, EntityUid> Devices = new Dictionary<string, EntityUid>();
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan UseDelay = TimeSpan.FromSeconds(0.5);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan LastUseAttempt;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SoundNoAccess = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/custom_deny.ogg", new AudioParams?());
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier SoundSwitchMode = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/quickbeep.ogg", new AudioParams?());

  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? ActiveDeviceList { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NetworkConfiguratorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (NetworkConfiguratorComponent) component;
    if (serialization.TryCustomCopy<NetworkConfiguratorComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.LinkModeActive, ref flag, hookCtx, false, context))
      flag = this.LinkModeActive;
    target.LinkModeActive = flag;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.ActiveDeviceList, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.ActiveDeviceList, hookCtx, context, false);
    target.ActiveDeviceList = nullable;
    Dictionary<string, EntityUid> dictionary = (Dictionary<string, EntityUid>) null;
    if (this.Devices == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, EntityUid>>(this.Devices, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, EntityUid>>(this.Devices, hookCtx, context, false);
    target.Devices = dictionary;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UseDelay, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.UseDelay, hookCtx, context, false);
    target.UseDelay = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LastUseAttempt, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.LastUseAttempt, hookCtx, context, false);
    target.LastUseAttempt = timeSpan2;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (this.SoundNoAccess == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundNoAccess, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.SoundNoAccess, hookCtx, context, false);
    target.SoundNoAccess = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (this.SoundSwitchMode == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundSwitchMode, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.SoundSwitchMode, hookCtx, context, false);
    target.SoundSwitchMode = soundSpecifier2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NetworkConfiguratorComponent target,
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
    NetworkConfiguratorComponent target1 = (NetworkConfiguratorComponent) target;
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
    NetworkConfiguratorComponent target1 = (NetworkConfiguratorComponent) target;
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
    NetworkConfiguratorComponent target1 = (NetworkConfiguratorComponent) target;
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
  virtual NetworkConfiguratorComponent Component.Instantiate()
  {
    return new NetworkConfiguratorComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class NetworkConfiguratorComponent_AutoState : IComponentState
  {
    public bool LinkModeActive;
    public NetEntity? ActiveDeviceList;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class NetworkConfiguratorComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<NetworkConfiguratorComponent, ComponentGetState>(new ComponentEventRefHandler<NetworkConfiguratorComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<NetworkConfiguratorComponent, ComponentHandleState>(new ComponentEventRefHandler<NetworkConfiguratorComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      NetworkConfiguratorComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new NetworkConfiguratorComponent.NetworkConfiguratorComponent_AutoState()
      {
        LinkModeActive = component.LinkModeActive,
        ActiveDeviceList = this.GetNetEntity(component.ActiveDeviceList, (MetaDataComponent) null)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      NetworkConfiguratorComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is NetworkConfiguratorComponent.NetworkConfiguratorComponent_AutoState current))
        return;
      component.LinkModeActive = current.LinkModeActive;
      component.ActiveDeviceList = this.EnsureEntity<NetworkConfiguratorComponent>(current.ActiveDeviceList, uid);
    }
  }
}
