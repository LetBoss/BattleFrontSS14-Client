// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.Rotting.RotIntoComponent
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
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Atmos.Rotting;

[RegisterComponent]
[NetworkedComponent]
public sealed class RotIntoComponent : 
  Component,
  ISerializationGenerated<RotIntoComponent>,
  ISerializationGenerated
{
  [DataField("entity", false, 1, true, false, typeof (PrototypeIdSerializer<EntityPrototype>))]
  [Robust.Shared.ViewVariables.ViewVariables]
  public string Entity = string.Empty;
  [DataField("stage", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public int Stage;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotIntoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (RotIntoComponent) component;
    if (serialization.TryCustomCopy<RotIntoComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Entity == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Entity, ref str, hookCtx, false, context))
      str = this.Entity;
    target.Entity = str;
    int num = 0;
    if (!serialization.TryCustomCopy<int>(this.Stage, ref num, hookCtx, false, context))
      num = this.Stage;
    target.Stage = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotIntoComponent target,
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
    RotIntoComponent target1 = (RotIntoComponent) target;
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
    RotIntoComponent target1 = (RotIntoComponent) target;
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
    RotIntoComponent target1 = (RotIntoComponent) target;
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
  virtual RotIntoComponent Component.Instantiate() => new RotIntoComponent();
}
