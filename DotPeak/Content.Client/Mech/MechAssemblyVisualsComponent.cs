// Decompiled with JetBrains decompiler
// Type: Content.Client.Mech.MechAssemblyVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Mech;

[RegisterComponent]
public sealed class MechAssemblyVisualsComponent : 
  Component,
  ISerializationGenerated<MechAssemblyVisualsComponent>,
  ISerializationGenerated
{
  [DataField("statePrefix", false, 1, true, false, null)]
  public string StatePrefix = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MechAssemblyVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (MechAssemblyVisualsComponent) component;
    if (serialization.TryCustomCopy<MechAssemblyVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.StatePrefix == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.StatePrefix, ref str, hookCtx, false, context))
      str = this.StatePrefix;
    target.StatePrefix = str;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MechAssemblyVisualsComponent target,
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
    MechAssemblyVisualsComponent target1 = (MechAssemblyVisualsComponent) target;
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
    MechAssemblyVisualsComponent target1 = (MechAssemblyVisualsComponent) target;
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
    MechAssemblyVisualsComponent target1 = (MechAssemblyVisualsComponent) target;
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
  virtual MechAssemblyVisualsComponent Component.Instantiate()
  {
    return new MechAssemblyVisualsComponent();
  }
}
