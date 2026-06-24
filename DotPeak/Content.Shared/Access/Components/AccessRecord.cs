// Decompiled with JetBrains decompiler
// Type: Content.Shared.Access.Components.AccessRecord
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;

#nullable enable
namespace Content.Shared.Access.Components;

[DataDefinition]
[NetSerializable]
[Serializable]
public readonly record struct AccessRecord(TimeSpan AccessTime, string Accessor) : 
  ISerializationGenerated<AccessRecord>,
  ISerializationGenerated
{
  public AccessRecord()
    : this(TimeSpan.Zero, string.Empty)
  {
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AccessRecord target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AccessRecord>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.AccessTime, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.AccessTime, hookCtx, context, false);
    string str = (string) null;
    if (this.Accessor == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Accessor, ref str, hookCtx, false, context))
      str = this.Accessor;
    target = target with
    {
      AccessTime = timeSpan,
      Accessor = str
    };
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AccessRecord target,
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
    AccessRecord target1 = (AccessRecord) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AccessRecord Instantiate() => new AccessRecord();
}
