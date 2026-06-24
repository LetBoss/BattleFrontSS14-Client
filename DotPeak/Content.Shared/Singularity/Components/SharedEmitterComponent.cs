// Decompiled with JetBrains decompiler
// Type: Content.Shared.Singularity.Components.EmitterComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DeviceLinking;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace Content.Shared.Singularity.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EmitterComponent : 
  Component,
  ISerializationGenerated<EmitterComponent>,
  ISerializationGenerated
{
  public CancellationTokenSource? TimerCancel;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsOn;
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsPowered;
  [Robust.Shared.ViewVariables.ViewVariables]
  public int FireShotCounter;
  [DataField("boltType", false, 1, false, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  public string BoltType = "EmitterBolt";
  [DataField("selectableTypes", false, 1, false, false, typeof (PrototypeIdListSerializer<EntityPrototype>))]
  public System.Collections.Generic.List<string> SelectableTypes = new System.Collections.Generic.List<string>();
  [DataField("powerUseActive", false, 1, false, false, null)]
  public int PowerUseActive = 600;
  [DataField("fireBurstSize", false, 1, false, false, null)]
  public int FireBurstSize = 3;
  [DataField("fireInterval", false, 1, false, false, null)]
  public TimeSpan FireInterval = TimeSpan.FromSeconds(2L);
  [DataField("fireBurstDelayMin", false, 1, false, false, null)]
  public TimeSpan FireBurstDelayMin = TimeSpan.FromSeconds(4L);
  [DataField("fireBurstDelayMax", false, 1, false, false, null)]
  public TimeSpan FireBurstDelayMax = TimeSpan.FromSeconds(10L);
  [DataField("onState", false, 1, false, false, null)]
  public string? OnState = "beam";
  [DataField("underpoweredState", false, 1, false, false, null)]
  public string? UnderpoweredState = "underpowered";
  [DataField("onPort", false, 1, false, false, typeof (PrototypeIdSerializer<SinkPortPrototype>))]
  public string OnPort = "On";
  [DataField("offPort", false, 1, false, false, typeof (PrototypeIdSerializer<SinkPortPrototype>))]
  public string OffPort = "Off";
  [DataField("togglePort", false, 1, false, false, typeof (PrototypeIdSerializer<SinkPortPrototype>))]
  public string TogglePort = "Toggle";
  [DataField("setTypePorts", false, 1, false, false, typeof (PrototypeIdDictionarySerializer<string, SinkPortPrototype>))]
  public System.Collections.Generic.Dictionary<string, string> SetTypePorts = new System.Collections.Generic.Dictionary<string, string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EmitterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EmitterComponent) target1;
    if (serialization.TryCustomCopy<EmitterComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.BoltType == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BoltType, ref target2, hookCtx, false, context))
      target2 = this.BoltType;
    target.BoltType = target2;
    System.Collections.Generic.List<string> target3 = (System.Collections.Generic.List<string>) null;
    if (this.SelectableTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.List<string>>(this.SelectableTypes, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<System.Collections.Generic.List<string>>(this.SelectableTypes, hookCtx, context);
    target.SelectableTypes = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.PowerUseActive, ref target4, hookCtx, false, context))
      target4 = this.PowerUseActive;
    target.PowerUseActive = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.FireBurstSize, ref target5, hookCtx, false, context))
      target5 = this.FireBurstSize;
    target.FireBurstSize = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireInterval, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.FireInterval, hookCtx, context);
    target.FireInterval = target6;
    TimeSpan target7 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireBurstDelayMin, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<TimeSpan>(this.FireBurstDelayMin, hookCtx, context);
    target.FireBurstDelayMin = target7;
    TimeSpan target8 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.FireBurstDelayMax, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<TimeSpan>(this.FireBurstDelayMax, hookCtx, context);
    target.FireBurstDelayMax = target8;
    string target9 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.OnState, ref target9, hookCtx, false, context))
      target9 = this.OnState;
    target.OnState = target9;
    string target10 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.UnderpoweredState, ref target10, hookCtx, false, context))
      target10 = this.UnderpoweredState;
    target.UnderpoweredState = target10;
    string target11 = (string) null;
    if (this.OnPort == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OnPort, ref target11, hookCtx, false, context))
      target11 = this.OnPort;
    target.OnPort = target11;
    string target12 = (string) null;
    if (this.OffPort == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OffPort, ref target12, hookCtx, false, context))
      target12 = this.OffPort;
    target.OffPort = target12;
    string target13 = (string) null;
    if (this.TogglePort == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.TogglePort, ref target13, hookCtx, false, context))
      target13 = this.TogglePort;
    target.TogglePort = target13;
    System.Collections.Generic.Dictionary<string, string> target14 = (System.Collections.Generic.Dictionary<string, string>) null;
    if (this.SetTypePorts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<System.Collections.Generic.Dictionary<string, string>>(this.SetTypePorts, ref target14, hookCtx, true, context))
      target14 = serialization.CreateCopy<System.Collections.Generic.Dictionary<string, string>>(this.SetTypePorts, hookCtx, context);
    target.SetTypePorts = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EmitterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmitterComponent target1 = (EmitterComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmitterComponent target1 = (EmitterComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EmitterComponent target1 = (EmitterComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EmitterComponent Component.Instantiate() => new EmitterComponent();
}
