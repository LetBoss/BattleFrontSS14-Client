// Decompiled with JetBrains decompiler
// Type: Content.Shared.Examine.ExamineEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Content.Shared.Examine;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class ExamineEntry : ISerializationGenerated<ExamineEntry>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Component;
  [DataField(null, false, 1, false, false, null)]
  public float Priority;
  [DataField(null, false, 1, true, false, null)]
  public FormattedMessage Message;

  public ExamineEntry(string component, float priority, FormattedMessage message)
  {
    this.Component = component;
    this.Priority = priority;
    this.Message = message;
  }

  private ExamineEntry()
  {
    this.Message = (FormattedMessage) null;
    this.Component = (string) null;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ExamineEntry target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<ExamineEntry>(this, ref target, hookCtx, false, context))
      return;
    string target1 = (string) null;
    if (this.Component == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Component, ref target1, hookCtx, false, context))
      target1 = this.Component;
    target.Component = target1;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Priority, ref target2, hookCtx, false, context))
      target2 = this.Priority;
    target.Priority = target2;
    FormattedMessage target3 = (FormattedMessage) null;
    if (this.Message == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<FormattedMessage>(this.Message, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<FormattedMessage>(this.Message, hookCtx, context);
    target.Message = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ExamineEntry target,
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
    ExamineEntry target1 = (ExamineEntry) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public ExamineEntry Instantiate() => new ExamineEntry();
}
