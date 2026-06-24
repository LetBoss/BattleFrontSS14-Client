// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceLinking.DeviceLinkSinkComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.DeviceLinking;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDeviceLinkSystem)})]
public sealed class DeviceLinkSinkComponent : 
  Component,
  ISerializationGenerated<DeviceLinkSinkComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<SinkPortPrototype>> Ports = new HashSet<ProtoId<SinkPortPrototype>>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public HashSet<EntityUid> LinkedSources = new HashSet<EntityUid>();
  [Access(new Type[] {typeof (SharedDeviceLinkSystem)})]
  public GameTick InvokeCounterTick;
  [DataField(null, false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedDeviceLinkSystem)})]
  public int InvokeCounter;
  [DataField(null, false, 1, false, false, null)]
  public int InvokeLimit = 10;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeviceLinkSinkComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeviceLinkSinkComponent) component;
    if (serialization.TryCustomCopy<DeviceLinkSinkComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<SinkPortPrototype>> protoIdSet = (HashSet<ProtoId<SinkPortPrototype>>) null;
    if (this.Ports == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<SinkPortPrototype>>>(this.Ports, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<SinkPortPrototype>>>(this.Ports, hookCtx, context, false);
    target.Ports = protoIdSet;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.InvokeCounter, ref num1, hookCtx, false, context))
      num1 = this.InvokeCounter;
    target.InvokeCounter = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.InvokeLimit, ref num2, hookCtx, false, context))
      num2 = this.InvokeLimit;
    target.InvokeLimit = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeviceLinkSinkComponent target,
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
    DeviceLinkSinkComponent target1 = (DeviceLinkSinkComponent) target;
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
    DeviceLinkSinkComponent target1 = (DeviceLinkSinkComponent) target;
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
    DeviceLinkSinkComponent target1 = (DeviceLinkSinkComponent) target;
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
  virtual DeviceLinkSinkComponent Component.Instantiate() => new DeviceLinkSinkComponent();
}
