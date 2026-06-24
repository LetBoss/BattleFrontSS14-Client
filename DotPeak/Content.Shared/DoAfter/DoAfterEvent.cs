// Decompiled with JetBrains decompiler
// Type: Content.Shared.DoAfter.DoAfterEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.DoAfter;

[NetSerializable]
[ImplicitDataDefinitionForInheritors]
[Serializable]
public abstract class DoAfterEvent : 
  HandledEntityEventArgs,
  ISerializationGenerated<DoAfterEvent>,
  ISerializationGenerated
{
  [NonSerialized]
  public Content.Shared.DoAfter.DoAfter DoAfter;
  public bool Repeat;

  public abstract DoAfterEvent Clone();

  public bool Cancelled => this.DoAfter.Cancelled;

  public EntityUid User => this.DoAfter.Args.User;

  public EntityUid? Target => this.DoAfter.Args.Target;

  public EntityUid? Used => this.DoAfter.Args.Used;

  public DoAfterArgs Args => this.DoAfter.Args;

  public virtual bool IsDuplicate(DoAfterEvent other)
  {
    return ((object) this).GetType() == ((object) other).GetType();
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    serialization.TryCustomCopy<DoAfterEvent>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref DoAfterEvent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DoAfterEvent target1 = (DoAfterEvent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual DoAfterEvent Instantiate() => throw new NotImplementedException();
}
