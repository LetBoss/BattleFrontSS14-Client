// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Components.AtmosMonitoringConsoleDeviceComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class AtmosMonitoringConsoleDeviceComponent : 
  Component,
  ISerializationGenerated<AtmosMonitoringConsoleDeviceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public ProtoId<NavMapBlipPrototype>? NavMapBlip;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowAbsentConnections = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AtmosMonitoringConsoleDeviceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AtmosMonitoringConsoleDeviceComponent) component;
    if (serialization.TryCustomCopy<AtmosMonitoringConsoleDeviceComponent>(this, ref target, hookCtx, false, context))
      return;
    ProtoId<NavMapBlipPrototype>? nullable = new ProtoId<NavMapBlipPrototype>?();
    if (!serialization.TryCustomCopy<ProtoId<NavMapBlipPrototype>?>(this.NavMapBlip, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<ProtoId<NavMapBlipPrototype>?>(this.NavMapBlip, hookCtx, context, false);
    target.NavMapBlip = nullable;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowAbsentConnections, ref flag, hookCtx, false, context))
      flag = this.ShowAbsentConnections;
    target.ShowAbsentConnections = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AtmosMonitoringConsoleDeviceComponent target,
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
    AtmosMonitoringConsoleDeviceComponent target1 = (AtmosMonitoringConsoleDeviceComponent) target;
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
    AtmosMonitoringConsoleDeviceComponent target1 = (AtmosMonitoringConsoleDeviceComponent) target;
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
    AtmosMonitoringConsoleDeviceComponent target1 = (AtmosMonitoringConsoleDeviceComponent) target;
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
  virtual AtmosMonitoringConsoleDeviceComponent Component.Instantiate()
  {
    return new AtmosMonitoringConsoleDeviceComponent();
  }
}
