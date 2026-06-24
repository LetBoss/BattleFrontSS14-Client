// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Components.MachineBoardComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Stacks;
using Content.Shared.Tag;
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
namespace Content.Shared.Construction.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class MachineBoardComponent : 
  Component,
  ISerializationGenerated<MachineBoardComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<StackPrototype>, int> StackRequirements = new Dictionary<ProtoId<StackPrototype>, int>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<ProtoId<TagPrototype>, GenericPartInfo> TagRequirements = new Dictionary<ProtoId<TagPrototype>, GenericPartInfo>();
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, GenericPartInfo> ComponentRequirements = new Dictionary<string, GenericPartInfo>();
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Prototype;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MachineBoardComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (MachineBoardComponent) component;
    if (serialization.TryCustomCopy<MachineBoardComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<ProtoId<StackPrototype>, int> dictionary1 = (Dictionary<ProtoId<StackPrototype>, int>) null;
    if (this.StackRequirements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<StackPrototype>, int>>(this.StackRequirements, ref dictionary1, hookCtx, true, context))
      dictionary1 = serialization.CreateCopy<Dictionary<ProtoId<StackPrototype>, int>>(this.StackRequirements, hookCtx, context, false);
    target.StackRequirements = dictionary1;
    Dictionary<ProtoId<TagPrototype>, GenericPartInfo> dictionary2 = (Dictionary<ProtoId<TagPrototype>, GenericPartInfo>) null;
    if (this.TagRequirements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ProtoId<TagPrototype>, GenericPartInfo>>(this.TagRequirements, ref dictionary2, hookCtx, true, context))
      dictionary2 = serialization.CreateCopy<Dictionary<ProtoId<TagPrototype>, GenericPartInfo>>(this.TagRequirements, hookCtx, context, false);
    target.TagRequirements = dictionary2;
    Dictionary<string, GenericPartInfo> dictionary3 = (Dictionary<string, GenericPartInfo>) null;
    if (this.ComponentRequirements == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, GenericPartInfo>>(this.ComponentRequirements, ref dictionary3, hookCtx, true, context))
      dictionary3 = serialization.CreateCopy<Dictionary<string, GenericPartInfo>>(this.ComponentRequirements, hookCtx, context, false);
    target.ComponentRequirements = dictionary3;
    EntProtoId entProtoId = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref entProtoId, hookCtx, false, context))
      entProtoId = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context, false);
    target.Prototype = entProtoId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MachineBoardComponent target,
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
    MachineBoardComponent target1 = (MachineBoardComponent) target;
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
    MachineBoardComponent target1 = (MachineBoardComponent) target;
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
    MachineBoardComponent target1 = (MachineBoardComponent) target;
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
  virtual MachineBoardComponent Component.Instantiate() => new MachineBoardComponent();
}
