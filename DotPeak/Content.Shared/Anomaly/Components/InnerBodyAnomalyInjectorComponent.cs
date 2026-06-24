// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Components.InnerBodyAnomalyInjectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Anomaly.Effects;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Components;

[RegisterComponent]
[Access(new Type[] {typeof (SharedInnerBodyAnomalySystem)})]
public sealed class InnerBodyAnomalyInjectorComponent : 
  Component,
  ISerializationGenerated<InnerBodyAnomalyInjectorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? Whitelist;
  [DataField(null, false, 1, true, false, null)]
  public ComponentRegistry InjectionComponents;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InnerBodyAnomalyInjectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (InnerBodyAnomalyInjectorComponent) component;
    if (serialization.TryCustomCopy<InnerBodyAnomalyInjectorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityWhitelist entityWhitelist = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.Whitelist, ref entityWhitelist, hookCtx, false, context))
    {
      if (this.Whitelist == null)
        entityWhitelist = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.Whitelist, ref entityWhitelist, hookCtx, context, false);
    }
    target.Whitelist = entityWhitelist;
    ComponentRegistry componentRegistry = (ComponentRegistry) null;
    if (this.InjectionComponents == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<ComponentRegistry>(this.InjectionComponents, ref componentRegistry, hookCtx, false, context))
      componentRegistry = serialization.CreateCopy<ComponentRegistry>(this.InjectionComponents, hookCtx, context, false);
    target.InjectionComponents = componentRegistry;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InnerBodyAnomalyInjectorComponent target,
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
    InnerBodyAnomalyInjectorComponent target1 = (InnerBodyAnomalyInjectorComponent) target;
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
    InnerBodyAnomalyInjectorComponent target1 = (InnerBodyAnomalyInjectorComponent) target;
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
    InnerBodyAnomalyInjectorComponent target1 = (InnerBodyAnomalyInjectorComponent) target;
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
  virtual InnerBodyAnomalyInjectorComponent Component.Instantiate()
  {
    return new InnerBodyAnomalyInjectorComponent();
  }
}
