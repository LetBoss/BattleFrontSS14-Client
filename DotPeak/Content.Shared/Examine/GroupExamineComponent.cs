// Decompiled with JetBrains decompiler
// Type: Content.Shared.Examine.GroupExamineComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Examine;

[RegisterComponent]
public sealed class GroupExamineComponent : 
  Component,
  ISerializationGenerated<GroupExamineComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public List<ExamineGroup> Group = new List<ExamineGroup>()
  {
    new ExamineGroup()
    {
      Components = new List<string>()
      {
        "Armor",
        "ClothingSpeedModifier"
      }
    }
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GroupExamineComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GroupExamineComponent) target1;
    if (serialization.TryCustomCopy<GroupExamineComponent>(this, ref target, hookCtx, false, context))
      return;
    List<ExamineGroup> target2 = (List<ExamineGroup>) null;
    if (this.Group == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ExamineGroup>>(this.Group, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<ExamineGroup>>(this.Group, hookCtx, context);
    target.Group = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GroupExamineComponent target,
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
    GroupExamineComponent target1 = (GroupExamineComponent) target;
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
    GroupExamineComponent target1 = (GroupExamineComponent) target;
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
    GroupExamineComponent target1 = (GroupExamineComponent) target;
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
  virtual GroupExamineComponent Component.Instantiate() => new GroupExamineComponent();
}
