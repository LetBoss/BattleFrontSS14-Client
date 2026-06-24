// Decompiled with JetBrains decompiler
// Type: Content.Shared.Stacks.StackComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Stacks;

[RegisterComponent]
[NetworkedComponent]
public sealed class StackComponent : 
  Component,
  ISerializationGenerated<StackComponent>,
  ISerializationGenerated
{
  [DataField("lingering", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool Lingering;
  [DataField("baseLayer", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string BaseLayer = "";
  [DataField("composite", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool IsComposite;
  [DataField("layerStates", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public List<string> LayerStates = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public StackLayerFunction LayerFunction;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  [DataField("stackType", false, 1, true, false, typeof (PrototypeIdSerializer<StackPrototype>))]
  public string StackTypeId { get; private set; }

  [DataField("count", false, 1, false, false, null)]
  public int Count { get; set; } = 30;

  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  [DataField("maxCountOverride", false, 1, false, false, null)]
  public int? MaxCountOverride { get; set; }

  [DataField("unlimited", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadOnly)]
  public bool Unlimited { get; set; }

  [DataField("throwIndividually", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool ThrowIndividually { get; set; }

  [Robust.Shared.ViewVariables.ViewVariables]
  public bool UiUpdateNeeded { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StackComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (StackComponent) target1;
    if (serialization.TryCustomCopy<StackComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.StackTypeId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StackTypeId, ref target2, hookCtx, false, context))
      target2 = this.StackTypeId;
    target.StackTypeId = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target3, hookCtx, false, context))
      target3 = this.Count;
    target.Count = target3;
    int? target4 = new int?();
    if (!serialization.TryCustomCopy<int?>(this.MaxCountOverride, ref target4, hookCtx, false, context))
      target4 = this.MaxCountOverride;
    target.MaxCountOverride = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.Unlimited, ref target5, hookCtx, false, context))
      target5 = this.Unlimited;
    target.Unlimited = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Lingering, ref target6, hookCtx, false, context))
      target6 = this.Lingering;
    target.Lingering = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.ThrowIndividually, ref target7, hookCtx, false, context))
      target7 = this.ThrowIndividually;
    target.ThrowIndividually = target7;
    string target8 = (string) null;
    if (this.BaseLayer == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BaseLayer, ref target8, hookCtx, false, context))
      target8 = this.BaseLayer;
    target.BaseLayer = target8;
    bool target9 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsComposite, ref target9, hookCtx, false, context))
      target9 = this.IsComposite;
    target.IsComposite = target9;
    List<string> target10 = (List<string>) null;
    if (this.LayerStates == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.LayerStates, ref target10, hookCtx, true, context))
      target10 = serialization.CreateCopy<List<string>>(this.LayerStates, hookCtx, context);
    target.LayerStates = target10;
    StackLayerFunction target11 = StackLayerFunction.None;
    if (!serialization.TryCustomCopy<StackLayerFunction>(this.LayerFunction, ref target11, hookCtx, false, context))
      target11 = this.LayerFunction;
    target.LayerFunction = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StackComponent target,
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
    StackComponent target1 = (StackComponent) target;
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
    StackComponent target1 = (StackComponent) target;
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
    StackComponent target1 = (StackComponent) target;
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
  virtual StackComponent Component.Instantiate() => new StackComponent();
}
