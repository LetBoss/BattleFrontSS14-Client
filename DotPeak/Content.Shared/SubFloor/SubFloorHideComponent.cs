// Decompiled with JetBrains decompiler
// Type: Content.Shared.SubFloor.SubFloorHideComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.SubFloor;

[NetworkedComponent]
[RegisterComponent]
[Access(new Type[] {typeof (SharedSubFloorHideSystem)})]
public sealed class SubFloorHideComponent : 
  Component,
  ISerializationGenerated<SubFloorHideComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<Enum> VisibleLayers = new HashSet<Enum>();
  [DataField(null, false, 1, false, false, null)]
  public int? OriginalDrawDepth;

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool IsUnderCover { get; set; }

  [DataField(null, false, 1, false, false, null)]
  public bool BlockInteractions { get; set; } = true;

  [DataField(null, false, 1, false, false, null)]
  public bool BlockAmbience { get; set; } = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SubFloorHideComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SubFloorHideComponent) target1;
    if (serialization.TryCustomCopy<SubFloorHideComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockInteractions, ref target2, hookCtx, false, context))
      target2 = this.BlockInteractions;
    target.BlockInteractions = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.BlockAmbience, ref target3, hookCtx, false, context))
      target3 = this.BlockAmbience;
    target.BlockAmbience = target3;
    HashSet<Enum> target4 = (HashSet<Enum>) null;
    if (this.VisibleLayers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<Enum>>(this.VisibleLayers, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<HashSet<Enum>>(this.VisibleLayers, hookCtx, context);
    target.VisibleLayers = target4;
    int? target5 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.OriginalDrawDepth, ref target5, hookCtx, false, context))
      target5 = this.OriginalDrawDepth;
    target.OriginalDrawDepth = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SubFloorHideComponent target,
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
    SubFloorHideComponent target1 = (SubFloorHideComponent) target;
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
    SubFloorHideComponent target1 = (SubFloorHideComponent) target;
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
    SubFloorHideComponent target1 = (SubFloorHideComponent) target;
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
  virtual SubFloorHideComponent Component.Instantiate() => new SubFloorHideComponent();
}
