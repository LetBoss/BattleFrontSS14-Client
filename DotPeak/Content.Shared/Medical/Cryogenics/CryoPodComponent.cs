// Decompiled with JetBrains decompiler
// Type: Content.Shared.Medical.Cryogenics.CryoPodComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Medical.Cryogenics;

[RegisterComponent]
[NetworkedComponent]
public sealed class CryoPodComponent : 
  Component,
  ISerializationGenerated<CryoPodComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("beakerTransferTime", false, 1, false, false, null)]
  public float BeakerTransferTime = 1f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("nextInjectionTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan? NextInjectionTime;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("beakerTransferAmount", false, 1, false, false, null)]
  public float BeakerTransferAmount = 1f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("entryDelay", false, 1, false, false, null)]
  public float EntryDelay = 2f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("pryDelay", false, 1, false, false, null)]
  public float PryDelay = 5f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public ContainerSlot BodyContainer;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("port", false, 1, false, false, null)]
  public string PortName { get; set; } = "port";

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("solutionContainerName", false, 1, false, false, null)]
  public string SolutionContainerName { get; set; } = "beakerSlot";

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("locked", false, 1, false, false, null)]
  public bool Locked { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("permaLocked", false, 1, false, false, null)]
  public bool PermaLocked { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CryoPodComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CryoPodComponent) target1;
    if (serialization.TryCustomCopy<CryoPodComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.PortName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.PortName, ref target2, hookCtx, false, context))
      target2 = this.PortName;
    target.PortName = target2;
    string target3 = (string) null;
    if (this.SolutionContainerName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionContainerName, ref target3, hookCtx, false, context))
      target3 = this.SolutionContainerName;
    target.SolutionContainerName = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BeakerTransferTime, ref target4, hookCtx, false, context))
      target4 = this.BeakerTransferTime;
    target.BeakerTransferTime = target4;
    TimeSpan? target5 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextInjectionTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan?>(this.NextInjectionTime, hookCtx, context);
    target.NextInjectionTime = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BeakerTransferAmount, ref target6, hookCtx, false, context))
      target6 = this.BeakerTransferAmount;
    target.BeakerTransferAmount = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.EntryDelay, ref target7, hookCtx, false, context))
      target7 = this.EntryDelay;
    target.EntryDelay = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PryDelay, ref target8, hookCtx, false, context))
      target8 = this.PryDelay;
    target.PryDelay = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.Locked, ref target9, hookCtx, false, context))
      target9 = this.Locked;
    target.Locked = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.PermaLocked, ref target10, hookCtx, false, context))
      target10 = this.PermaLocked;
    target.PermaLocked = target10;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CryoPodComponent target,
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
    CryoPodComponent target1 = (CryoPodComponent) target;
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
    CryoPodComponent target1 = (CryoPodComponent) target;
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
    CryoPodComponent target1 = (CryoPodComponent) target;
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
  virtual CryoPodComponent Component.Instantiate() => new CryoPodComponent();

  [NetSerializable]
  [Serializable]
  public enum CryoPodVisuals : byte
  {
    ContainsEntity,
    IsOn,
  }
}
