// Decompiled with JetBrains decompiler
// Type: Content.Shared.DeviceLinking.DeviceLinkSourceComponent
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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.DeviceLinking;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDeviceLinkSystem)})]
public sealed class DeviceLinkSourceComponent : 
  Component,
  ISerializationGenerated<DeviceLinkSourceComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<SourcePortPrototype>> Ports = new HashSet<ProtoId<SourcePortPrototype>>();
  [Robust.Shared.ViewVariables.ViewVariables]
  public Dictionary<ProtoId<SourcePortPrototype>, HashSet<EntityUid>> Outputs = new Dictionary<ProtoId<SourcePortPrototype>, HashSet<EntityUid>>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<SourcePortPrototype>, bool> LastSignals = new Dictionary<ProtoId<SourcePortPrototype>, bool>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype> Source, ProtoId<SinkPortPrototype> Sink)>> LinkedPorts = new Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>();
  [DataField(null, false, 1, false, false, null)]
  public float Range = 30f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DeviceLinkSourceComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DeviceLinkSourceComponent) component;
    if (serialization.TryCustomCopy<DeviceLinkSourceComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<ProtoId<SourcePortPrototype>> protoIdSet = (HashSet<ProtoId<SourcePortPrototype>>) null;
    if (this.Ports == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<SourcePortPrototype>>>(this.Ports, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<SourcePortPrototype>>>(this.Ports, hookCtx, context, false);
    target.Ports = protoIdSet;
    Dictionary<ProtoId<SourcePortPrototype>, bool> dictionary1 = (Dictionary<ProtoId<SourcePortPrototype>, bool>) null;
    if (this.LastSignals == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<SourcePortPrototype>, bool>>(this.LastSignals, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<ProtoId<SourcePortPrototype>, bool>>(this.LastSignals, hookCtx, context, false);
    target.LastSignals = dictionary1;
    Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>> dictionary2 = (Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>) null;
    if (this.LinkedPorts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>>(this.LinkedPorts, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<EntityUid, HashSet<(ProtoId<SourcePortPrototype>, ProtoId<SinkPortPrototype>)>>>(this.LinkedPorts, hookCtx, context, false);
    target.LinkedPorts = dictionary2;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref num, hookCtx, false, context))
      num = this.Range;
    target.Range = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DeviceLinkSourceComponent target,
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
    DeviceLinkSourceComponent target1 = (DeviceLinkSourceComponent) target;
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
    DeviceLinkSourceComponent target1 = (DeviceLinkSourceComponent) target;
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
    DeviceLinkSourceComponent target1 = (DeviceLinkSourceComponent) target;
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
  virtual DeviceLinkSourceComponent Component.Instantiate() => new DeviceLinkSourceComponent();
}
