// Decompiled with JetBrains decompiler
// Type: Content.Shared.Blocking.BlockingUserComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Physics;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Blocking;

[RegisterComponent]
public sealed class BlockingUserComponent : 
  Component,
  ISerializationGenerated<BlockingUserComponent>,
  ISerializationGenerated
{
  [DataField("blockingItem", false, 1, false, false, null)]
  public EntityUid? BlockingItem;
  [DataField("originalBodyType", false, 1, false, false, null)]
  public BodyType OriginalBodyType;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref BlockingUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (BlockingUserComponent) component;
    if (serialization.TryCustomCopy<BlockingUserComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? nullable = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.BlockingItem, ref nullable, hookCtx, false, context))
      nullable = serialization.CreateCopy<EntityUid?>(this.BlockingItem, hookCtx, context, false);
    target.BlockingItem = nullable;
    BodyType bodyType = (BodyType) 0;
    if (!serialization.TryCustomCopy<BodyType>(this.OriginalBodyType, ref bodyType, hookCtx, false, context))
      bodyType = this.OriginalBodyType;
    target.OriginalBodyType = bodyType;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref BlockingUserComponent target,
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
    BlockingUserComponent target1 = (BlockingUserComponent) target;
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
    BlockingUserComponent target1 = (BlockingUserComponent) target;
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
    BlockingUserComponent target1 = (BlockingUserComponent) target;
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
  virtual BlockingUserComponent Component.Instantiate() => new BlockingUserComponent();
}
