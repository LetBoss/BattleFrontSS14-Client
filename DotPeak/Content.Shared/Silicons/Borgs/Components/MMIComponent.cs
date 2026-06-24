// Decompiled with JetBrains decompiler
// Type: Content.Shared.Silicons.Borgs.Components.MMIComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Silicons.Borgs.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedBorgSystem)})]
public sealed class MMIComponent : 
  Component,
  ISerializationGenerated<MMIComponent>,
  ISerializationGenerated
{
  [DataField("brainSlotId", false, 1, false, false, null)]
  public string BrainSlotId = "brain_slot";
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public ItemSlot BrainSlot;
  [DataField("hasMindState", false, 1, false, false, null)]
  public string HasMindState = "mmi_alive";
  [DataField("noMindState", false, 1, false, false, null)]
  public string NoMindState = "mmi_dead";
  [DataField("noBrainState", false, 1, false, false, null)]
  public string NoBrainState = "mmi_off";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MMIComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MMIComponent) target1;
    if (serialization.TryCustomCopy<MMIComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.BrainSlotId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BrainSlotId, ref target2, hookCtx, false, context))
      target2 = this.BrainSlotId;
    target.BrainSlotId = target2;
    string target3 = (string) null;
    if (this.HasMindState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HasMindState, ref target3, hookCtx, false, context))
      target3 = this.HasMindState;
    target.HasMindState = target3;
    string target4 = (string) null;
    if (this.NoMindState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NoMindState, ref target4, hookCtx, false, context))
      target4 = this.NoMindState;
    target.NoMindState = target4;
    string target5 = (string) null;
    if (this.NoBrainState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.NoBrainState, ref target5, hookCtx, false, context))
      target5 = this.NoBrainState;
    target.NoBrainState = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MMIComponent target,
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
    MMIComponent target1 = (MMIComponent) target;
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
    MMIComponent target1 = (MMIComponent) target;
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
    MMIComponent target1 = (MMIComponent) target;
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
  virtual MMIComponent Component.Instantiate() => new MMIComponent();
}
