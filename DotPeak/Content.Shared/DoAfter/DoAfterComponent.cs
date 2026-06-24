// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.DoAfterComponent
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
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

#nullable enable
namespace Content.Shared.DoAfter;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedDoAfterSystem)})]
public sealed class DoAfterComponent : 
  Component,
  ISerializationGenerated<DoAfterComponent>,
  ISerializationGenerated
{
  [DataField("nextId", false, 1, false, false, null)]
  public ushort NextId;
  [DataField("doAfters", false, 1, false, false, null)]
  public Dictionary<ushort, Content.Shared.DoAfter.DoAfter> DoAfters = new Dictionary<ushort, Content.Shared.DoAfter.DoAfter>();
  public readonly Dictionary<ushort, TaskCompletionSource<DoAfterStatus>> AwaitedDoAfters = new Dictionary<ushort, TaskCompletionSource<DoAfterStatus>>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DoAfterComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (DoAfterComponent) component;
    if (serialization.TryCustomCopy<DoAfterComponent>(this, ref target, hookCtx, false, context))
      return;
    ushort num = 0;
    if (!serialization.TryCustomCopy<ushort>(this.NextId, ref num, hookCtx, false, context))
      num = this.NextId;
    target.NextId = num;
    Dictionary<ushort, Content.Shared.DoAfter.DoAfter> dictionary = (Dictionary<ushort, Content.Shared.DoAfter.DoAfter>) null;
    if (this.DoAfters == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<ushort, Content.Shared.DoAfter.DoAfter>>(this.DoAfters, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<ushort, Content.Shared.DoAfter.DoAfter>>(this.DoAfters, hookCtx, context, false);
    target.DoAfters = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DoAfterComponent target,
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
    DoAfterComponent target1 = (DoAfterComponent) target;
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
    DoAfterComponent target1 = (DoAfterComponent) target;
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
    DoAfterComponent target1 = (DoAfterComponent) target;
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
  virtual DoAfterComponent Component.Instantiate() => new DoAfterComponent();
}
