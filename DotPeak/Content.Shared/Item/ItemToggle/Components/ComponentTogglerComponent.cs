// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemToggle.Components.ComponentTogglerComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Item.ItemToggle.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (ComponentTogglerSystem)})]
public sealed class ComponentTogglerComponent : 
  Component,
  ISerializationGenerated<ComponentTogglerComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry Components = new ComponentRegistry();
  [DataField(null, false, 1, false, false, null)]
  public ComponentRegistry? RemoveComponents;
  [DataField(null, false, 1, false, false, null)]
  public bool Parent;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Target;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ComponentTogglerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ComponentTogglerComponent) target1;
    if (serialization.TryCustomCopy<ComponentTogglerComponent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (this.Components == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Components, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.Components, hookCtx, context);
    target.Components = target2;
    ComponentRegistry target3 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.RemoveComponents, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<ComponentRegistry>(this.RemoveComponents, hookCtx, context);
    target.RemoveComponents = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.Parent, ref target4, hookCtx, false, context))
      target4 = this.Parent;
    target.Parent = target4;
    EntityUid? target5 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Target, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityUid?>(this.Target, hookCtx, context);
    target.Target = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ComponentTogglerComponent target,
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
    ComponentTogglerComponent target1 = (ComponentTogglerComponent) target;
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
    ComponentTogglerComponent target1 = (ComponentTogglerComponent) target;
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
    ComponentTogglerComponent target1 = (ComponentTogglerComponent) target;
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
  virtual ComponentTogglerComponent Component.Instantiate() => new ComponentTogglerComponent();
}
