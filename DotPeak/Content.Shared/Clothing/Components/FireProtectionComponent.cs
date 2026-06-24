// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.Components.FireProtectionComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Clothing.Components;

[RegisterComponent]
[Access(new Type[] {typeof (FireProtectionSystem)})]
public sealed class FireProtectionComponent : 
  Component,
  ISerializationGenerated<FireProtectionComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public float Reduction;
  [DataField(null, false, 1, false, false, null)]
  public LocId ExamineMessage = LocId.op_Implicit("fire-protection-reduction-value");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref FireProtectionComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (FireProtectionComponent) component;
    if (serialization.TryCustomCopy<FireProtectionComponent>(this, ref target, hookCtx, false, context))
      return;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Reduction, ref num, hookCtx, false, context))
      num = this.Reduction;
    target.Reduction = num;
    LocId locId = new LocId();
    if (!serialization.TryCustomCopy<LocId>(this.ExamineMessage, ref locId, hookCtx, false, context))
      locId = serialization.CreateCopy<LocId>(this.ExamineMessage, hookCtx, context, false);
    target.ExamineMessage = locId;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref FireProtectionComponent target,
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
    FireProtectionComponent target1 = (FireProtectionComponent) target;
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
    FireProtectionComponent target1 = (FireProtectionComponent) target;
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
    FireProtectionComponent target1 = (FireProtectionComponent) target;
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
  virtual FireProtectionComponent Component.Instantiate() => new FireProtectionComponent();
}
