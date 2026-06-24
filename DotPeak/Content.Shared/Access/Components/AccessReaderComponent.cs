// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.AccessReaderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Access.Systems;
using Content.Shared.StationRecords;
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
namespace Content.Shared.Access.Components;

[RegisterComponent]
[NetworkedComponent]
[Robust.Shared.Analyzers.Access(new Type[] {typeof (AccessReaderSystem)})]
public sealed class AccessReaderComponent : 
  Component,
  ISerializationGenerated<AccessReaderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled = true;
  [DataField(null, false, 1, false, false, null)]
  public HashSet<ProtoId<AccessLevelPrototype>> DenyTags = new HashSet<ProtoId<AccessLevelPrototype>>();
  [DataField("access", false, 1, false, false, null)]
  public List<HashSet<ProtoId<AccessLevelPrototype>>> AccessLists = new List<HashSet<ProtoId<AccessLevelPrototype>>>();
  [DataField(null, false, 1, false, false, null)]
  public HashSet<StationRecordKey> AccessKeys = new HashSet<StationRecordKey>();
  [DataField(null, false, 1, false, false, null)]
  public string? ContainerAccessProvider;
  [DataField(null, false, 1, false, false, null)]
  public Queue<AccessRecord> AccessLog = new Queue<AccessRecord>();
  [DataField(null, false, 1, false, false, null)]
  public int AccessLogLimit = 20;
  [DataField(null, false, 1, false, false, null)]
  public bool LoggingDisabled;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnAccessBreaker = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AccessReaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AccessReaderComponent) component;
    if (serialization.TryCustomCopy<AccessReaderComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref flag1, hookCtx, false, context))
      flag1 = this.Enabled;
    target.Enabled = flag1;
    HashSet<ProtoId<AccessLevelPrototype>> protoIdSet = (HashSet<ProtoId<AccessLevelPrototype>>) null;
    if (this.DenyTags == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.DenyTags, ref protoIdSet, hookCtx, true, context))
      protoIdSet = serialization.CreateCopy<HashSet<ProtoId<AccessLevelPrototype>>>(this.DenyTags, hookCtx, context, false);
    target.DenyTags = protoIdSet;
    List<HashSet<ProtoId<AccessLevelPrototype>>> protoIdSetList = (List<HashSet<ProtoId<AccessLevelPrototype>>>) null;
    if (this.AccessLists == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<HashSet<ProtoId<AccessLevelPrototype>>>>(this.AccessLists, ref protoIdSetList, hookCtx, true, context))
      protoIdSetList = serialization.CreateCopy<List<HashSet<ProtoId<AccessLevelPrototype>>>>(this.AccessLists, hookCtx, context, false);
    target.AccessLists = protoIdSetList;
    HashSet<StationRecordKey> stationRecordKeySet = (HashSet<StationRecordKey>) null;
    if (this.AccessKeys == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<StationRecordKey>>(this.AccessKeys, ref stationRecordKeySet, hookCtx, true, context))
      stationRecordKeySet = serialization.CreateCopy<HashSet<StationRecordKey>>(this.AccessKeys, hookCtx, context, false);
    target.AccessKeys = stationRecordKeySet;
    string str = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ContainerAccessProvider, ref str, hookCtx, false, context))
      str = this.ContainerAccessProvider;
    target.ContainerAccessProvider = str;
    Queue<AccessRecord> accessRecordQueue = (Queue<AccessRecord>) null;
    if (this.AccessLog == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Queue<AccessRecord>>(this.AccessLog, ref accessRecordQueue, hookCtx, true, context))
      accessRecordQueue = serialization.CreateCopy<Queue<AccessRecord>>(this.AccessLog, hookCtx, context, false);
    target.AccessLog = accessRecordQueue;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.AccessLogLimit, ref num, hookCtx, false, context))
      num = this.AccessLogLimit;
    target.AccessLogLimit = num;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.LoggingDisabled, ref flag2, hookCtx, false, context))
      flag2 = this.LoggingDisabled;
    target.LoggingDisabled = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnAccessBreaker, ref flag3, hookCtx, false, context))
      flag3 = this.BreakOnAccessBreaker;
    target.BreakOnAccessBreaker = flag3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AccessReaderComponent target,
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
    AccessReaderComponent target1 = (AccessReaderComponent) target;
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
    AccessReaderComponent target1 = (AccessReaderComponent) target;
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
    AccessReaderComponent target1 = (AccessReaderComponent) target;
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
  virtual AccessReaderComponent Component.Instantiate() => new AccessReaderComponent();
}
