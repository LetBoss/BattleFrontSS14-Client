// Decompiled with JetBrains decompiler
// Type: Content.Shared.Payload.Components.PayloadTriggerComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
namespace Content.Shared.Payload.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PayloadTriggerComponent : 
  Component,
  ISerializationGenerated<PayloadTriggerComponent>,
  ISerializationGenerated
{
  public bool Active;
  [DataField("components", true, 1, false, true, null)]
  public ComponentRegistry? Components;
  [DataField("grantedComponents", false, 1, false, true, null)]
  public HashSet<Type> GrantedComponents = new HashSet<Type>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PayloadTriggerComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PayloadTriggerComponent) target1;
    if (serialization.TryCustomCopy<PayloadTriggerComponent>(this, ref target, hookCtx, false, context))
      return;
    ComponentRegistry target2 = (ComponentRegistry) null;
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.Components, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<ComponentRegistry>(this.Components, hookCtx, context);
    target.Components = target2;
    HashSet<Type> target3 = (HashSet<Type>) null;
    if (this.GrantedComponents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<Type>>(this.GrantedComponents, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<HashSet<Type>>(this.GrantedComponents, hookCtx, context);
    target.GrantedComponents = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PayloadTriggerComponent target,
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
    PayloadTriggerComponent target1 = (PayloadTriggerComponent) target;
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
    PayloadTriggerComponent target1 = (PayloadTriggerComponent) target;
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
    PayloadTriggerComponent target1 = (PayloadTriggerComponent) target;
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
  virtual PayloadTriggerComponent Component.Instantiate() => new PayloadTriggerComponent();
}
