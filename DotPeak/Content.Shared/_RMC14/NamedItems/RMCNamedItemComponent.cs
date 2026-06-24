// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.NamedItems.RMCNamedItemComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.NamedItems;

[RegisterComponent]
[NetworkedComponent]
[Access(new System.Type[] {typeof (SharedRMCNamedItemSystem)})]
public sealed class RMCNamedItemComponent : 
  Component,
  ISerializationGenerated<RMCNamedItemComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? User;
  [DataField(null, false, 1, false, false, null)]
  public RMCNamedItemType? Type;
  [DataField(null, false, 1, false, false, null)]
  public string Name = string.Empty;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCNamedItemComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCNamedItemComponent) target1;
    if (serialization.TryCustomCopy<RMCNamedItemComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.User, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.User, hookCtx, context);
    target.User = target2;
    RMCNamedItemType? target3 = new RMCNamedItemType?();
    if (!serialization.TryCustomCopy<RMCNamedItemType?>(this.Type, ref target3, hookCtx, false, context))
      target3 = this.Type;
    target.Type = target3;
    string target4 = (string) null;
    if (this.Name == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Name, ref target4, hookCtx, false, context))
      target4 = this.Name;
    target.Name = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCNamedItemComponent target,
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
    RMCNamedItemComponent target1 = (RMCNamedItemComponent) target;
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
    RMCNamedItemComponent target1 = (RMCNamedItemComponent) target;
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
    RMCNamedItemComponent target1 = (RMCNamedItemComponent) target;
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
  virtual RMCNamedItemComponent Component.Instantiate() => new RMCNamedItemComponent();
}
