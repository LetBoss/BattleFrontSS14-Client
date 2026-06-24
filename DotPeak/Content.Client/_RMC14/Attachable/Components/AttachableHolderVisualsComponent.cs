// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Attachable.Components.AttachableHolderVisualsComponent
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Attachable.Systems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client._RMC14.Attachable.Components;

[RegisterComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (AttachableHolderVisualsSystem)})]
public sealed class AttachableHolderVisualsComponent : 
  Component,
  ISerializationGenerated<AttachableHolderVisualsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  [AutoNetworkedField]
  public Dictionary<string, Vector2> Offsets = new Dictionary<string, Vector2>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AttachableHolderVisualsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AttachableHolderVisualsComponent) component;
    if (serialization.TryCustomCopy<AttachableHolderVisualsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<string, Vector2> dictionary = (Dictionary<string, Vector2>) null;
    if (this.Offsets == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Vector2>>(this.Offsets, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, Vector2>>(this.Offsets, hookCtx, context, false);
    target.Offsets = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AttachableHolderVisualsComponent target,
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
    AttachableHolderVisualsComponent target1 = (AttachableHolderVisualsComponent) target;
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
    AttachableHolderVisualsComponent target1 = (AttachableHolderVisualsComponent) target;
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
    AttachableHolderVisualsComponent target1 = (AttachableHolderVisualsComponent) target;
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
  virtual AttachableHolderVisualsComponent Component.Instantiate()
  {
    return new AttachableHolderVisualsComponent();
  }
}
