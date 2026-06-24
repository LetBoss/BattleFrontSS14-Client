// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Components.PartAssemblyComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
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
public sealed class PartAssemblyComponent : 
  Component,
  ISerializationGenerated<PartAssemblyComponent>,
  ISerializationGenerated
{
  [DataField("parts", false, 1, true, false, null)]
  public Dictionary<string, List<string>> Parts = new Dictionary<string, List<string>>();
  [DataField("currentAssembly", false, 1, false, false, null)]
  public string? CurrentAssembly;
  [DataField("containerId", false, 1, false, false, null)]
  public string ContainerId = "part-container";
  [Robust.Shared.ViewVariables.ViewVariables]
  public Container PartsContainer;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PartAssemblyComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (PartAssemblyComponent) component;
    if (serialization.TryCustomCopy<PartAssemblyComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, List<string>> dictionary = (Dictionary<string, List<string>>) null;
    if (this.Parts == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, List<string>>>(this.Parts, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, List<string>>>(this.Parts, hookCtx, context, false);
    target.Parts = dictionary;
    string str1 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.CurrentAssembly, ref str1, hookCtx, false, context))
      str1 = this.CurrentAssembly;
    target.CurrentAssembly = str1;
    string str2 = (string) null;
    if (this.ContainerId == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ContainerId, ref str2, hookCtx, false, context))
      str2 = this.ContainerId;
    target.ContainerId = str2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PartAssemblyComponent target,
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
    PartAssemblyComponent target1 = (PartAssemblyComponent) target;
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
    PartAssemblyComponent target1 = (PartAssemblyComponent) target;
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
    PartAssemblyComponent target1 = (PartAssemblyComponent) target;
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
  virtual PartAssemblyComponent Component.Instantiate() => new PartAssemblyComponent();
}
