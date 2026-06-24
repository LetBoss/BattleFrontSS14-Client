// Decompiled with JetBrains decompiler
// Type: Content.Shared.VendingMachines.VendingMachineRestockComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.VendingMachines;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedVendingMachineSystem)})]
public sealed class VendingMachineRestockComponent : 
  Component,
  ISerializationGenerated<VendingMachineRestockComponent>,
  ISerializationGenerated
{
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("restockDelay", false, 1, false, false, null)]
  public TimeSpan RestockDelay;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("canRestock", false, 1, false, false, typeof (PrototypeIdHashSetSerializer<VendingMachineInventoryPrototype>))]
  public HashSet<string> CanRestock;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundRestockStart", false, 1, false, false, null)]
  public SoundSpecifier SoundRestockStart;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("soundRestockDone", false, 1, false, false, null)]
  public SoundSpecifier SoundRestockDone;

  public VendingMachineRestockComponent()
  {
    SoundPathSpecifier soundPathSpecifier = new SoundPathSpecifier("/Audio/Machines/vending_restock_start.ogg");
    soundPathSpecifier.Params = new AudioParams()
    {
      Volume = -2f,
      Variation = new float?(0.2f)
    };
    this.SoundRestockStart = (SoundSpecifier) soundPathSpecifier;
    this.SoundRestockDone = (SoundSpecifier) new SoundPathSpecifier("/Audio/Machines/vending_restock_done.ogg");
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VendingMachineRestockComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VendingMachineRestockComponent) target1;
    if (serialization.TryCustomCopy<VendingMachineRestockComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.RestockDelay, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.RestockDelay, hookCtx, context);
    target.RestockDelay = target2;
    HashSet<string> target3 = (HashSet<string>) null;
    if (this.CanRestock == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.CanRestock, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<string>>(this.CanRestock, hookCtx, context);
    target.CanRestock = target3;
    SoundSpecifier target4 = (SoundSpecifier) null;
    if (this.SoundRestockStart == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundRestockStart, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<SoundSpecifier>(this.SoundRestockStart, hookCtx, context);
    target.SoundRestockStart = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (this.SoundRestockDone == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.SoundRestockDone, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.SoundRestockDone, hookCtx, context);
    target.SoundRestockDone = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VendingMachineRestockComponent target,
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
    VendingMachineRestockComponent target1 = (VendingMachineRestockComponent) target;
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
    VendingMachineRestockComponent target1 = (VendingMachineRestockComponent) target;
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
    VendingMachineRestockComponent target1 = (VendingMachineRestockComponent) target;
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
  virtual VendingMachineRestockComponent Component.Instantiate()
  {
    return new VendingMachineRestockComponent();
  }
}
