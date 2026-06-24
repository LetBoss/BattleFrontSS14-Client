// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.RotationDrawDepthComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14;

[RegisterComponent]
public sealed class RotationDrawDepthComponent : 
  Component,
  ISerializationGenerated<RotationDrawDepthComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (ConstantSerializer<DrawDepth>))]
  public int DefaultDrawDepth;
  [DataField(null, false, 1, false, false, typeof (ConstantSerializer<DrawDepth>))]
  public int SouthDrawDepth;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RotationDrawDepthComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (RotationDrawDepthComponent) component;
    if (serialization.TryCustomCopy<RotationDrawDepthComponent>(this, ref target, hookCtx, false, context))
      return;
    int num1 = 0;
    if (!serialization.TryCustomCopy<int>(this.DefaultDrawDepth, ref num1, hookCtx, false, context))
      num1 = this.DefaultDrawDepth;
    target.DefaultDrawDepth = num1;
    int num2 = 0;
    if (!serialization.TryCustomCopy<int>(this.SouthDrawDepth, ref num2, hookCtx, false, context))
      num2 = this.SouthDrawDepth;
    target.SouthDrawDepth = num2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RotationDrawDepthComponent target,
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
    RotationDrawDepthComponent target1 = (RotationDrawDepthComponent) target;
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
    RotationDrawDepthComponent target1 = (RotationDrawDepthComponent) target;
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
    RotationDrawDepthComponent target1 = (RotationDrawDepthComponent) target;
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
  virtual RotationDrawDepthComponent Component.Instantiate() => new RotationDrawDepthComponent();
}
