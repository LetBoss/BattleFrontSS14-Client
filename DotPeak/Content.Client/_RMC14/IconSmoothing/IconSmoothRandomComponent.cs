// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.IconSmoothing.IconSmoothRandomComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14.IconSmoothing;

[RegisterComponent]
public sealed class IconSmoothRandomComponent : 
  Component,
  ISerializationGenerated<IconSmoothRandomComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public HashSet<string> Overrides = new HashSet<string>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref IconSmoothRandomComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (IconSmoothRandomComponent) component;
    if (serialization.TryCustomCopy<IconSmoothRandomComponent>(this, ref target, hookCtx, false, context))
      return;
    HashSet<string> stringSet = (HashSet<string>) null;
    if (this.Overrides == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<HashSet<string>>(this.Overrides, ref stringSet, hookCtx, true, context))
      stringSet = serialization.CreateCopy<HashSet<string>>(this.Overrides, hookCtx, context, false);
    target.Overrides = stringSet;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref IconSmoothRandomComponent target,
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
    IconSmoothRandomComponent target1 = (IconSmoothRandomComponent) target;
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
    IconSmoothRandomComponent target1 = (IconSmoothRandomComponent) target;
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
    IconSmoothRandomComponent target1 = (IconSmoothRandomComponent) target;
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
  virtual IconSmoothRandomComponent Component.Instantiate() => new IconSmoothRandomComponent();
}
