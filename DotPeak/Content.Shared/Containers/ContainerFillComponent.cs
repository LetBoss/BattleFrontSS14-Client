// Decompiled with JetBrains decompiler
// Type: Content.Shared.Containers.ContainerFillComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Containers;

[RegisterComponent]
public sealed class ContainerFillComponent : 
  Component,
  ISerializationGenerated<ContainerFillComponent>,
  ISerializationGenerated
{
  [DataField("containers", false, 1, false, false, typeof (ContainerFillSerializer))]
  public Dictionary<string, List<string>> Containers = new Dictionary<string, List<string>>();
  [DataField("ignoreConstructionSpawn", false, 1, false, false, null)]
  public bool IgnoreConstructionSpawn = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ContainerFillComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ContainerFillComponent) component;
    if (serialization.TryCustomCopy<ContainerFillComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, List<string>> dictionary = (Dictionary<string, List<string>>) null;
    if (this.Containers == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(this.Containers, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, List<string>>>(this.Containers, hookCtx, context, false);
    target.Containers = dictionary;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreConstructionSpawn, ref flag, hookCtx, false, context))
      flag = this.IgnoreConstructionSpawn;
    target.IgnoreConstructionSpawn = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ContainerFillComponent target,
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
    ContainerFillComponent target1 = (ContainerFillComponent) target;
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
    ContainerFillComponent target1 = (ContainerFillComponent) target;
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
    ContainerFillComponent target1 = (ContainerFillComponent) target;
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
  virtual ContainerFillComponent Component.Instantiate() => new ContainerFillComponent();
}
