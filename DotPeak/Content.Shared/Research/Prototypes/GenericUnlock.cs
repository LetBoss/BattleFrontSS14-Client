// Decompiled with JetBrains decompiler
// Type: Content.Shared.Research.Prototypes.GenericUnlock
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Research.Prototypes;

[DataDefinition]
public record struct GenericUnlock : ISerializationGenerated<GenericUnlock>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public object? PurchaseEvent;
  [DataField(null, false, 1, false, false, null)]
  public string UnlockDescription;

  public GenericUnlock()
  {
    this.PurchaseEvent = (object) null;
    this.UnlockDescription = string.Empty;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GenericUnlock target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<GenericUnlock>(this, ref target, hookCtx, false, context))
      return;
    object target1 = (object) null;
    if (!serialization.TryCustomCopy<object>(this.PurchaseEvent, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy(this.PurchaseEvent, hookCtx, context);
    string target2 = (string) null;
    if (this.UnlockDescription == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.UnlockDescription, ref target2, hookCtx, false, context))
      target2 = this.UnlockDescription;
    target = target with
    {
      PurchaseEvent = target1,
      UnlockDescription = target2
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GenericUnlock target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    GenericUnlock target1 = (GenericUnlock) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public GenericUnlock Instantiate() => new GenericUnlock();

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<object>.Default.GetHashCode(this.PurchaseEvent) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.UnlockDescription);
  }

  [CompilerGenerated]
  public readonly bool Equals(GenericUnlock other)
  {
    return EqualityComparer<object>.Default.Equals(this.PurchaseEvent, other.PurchaseEvent) && EqualityComparer<string>.Default.Equals(this.UnlockDescription, other.UnlockDescription);
  }
}
