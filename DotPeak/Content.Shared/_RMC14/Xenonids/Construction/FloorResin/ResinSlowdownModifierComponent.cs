// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Construction.FloorResin.ResinSlowdownModifierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Xenonids.Weeds;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Construction.FloorResin;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedXenoWeedsSystem)})]
public sealed class ResinSlowdownModifierComponent : 
  Component,
  ISerializationGenerated<ResinSlowdownModifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float OutsiderSpeedModifier;
  [DataField(null, false, 1, false, false, null)]
  public float OutsiderSpeedModifierArmor;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ResinSlowdownModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ResinSlowdownModifierComponent) target1;
    if (serialization.TryCustomCopy<ResinSlowdownModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OutsiderSpeedModifier, ref target2, hookCtx, false, context))
      target2 = this.OutsiderSpeedModifier;
    target.OutsiderSpeedModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.OutsiderSpeedModifierArmor, ref target3, hookCtx, false, context))
      target3 = this.OutsiderSpeedModifierArmor;
    target.OutsiderSpeedModifierArmor = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ResinSlowdownModifierComponent target,
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
    ResinSlowdownModifierComponent target1 = (ResinSlowdownModifierComponent) target;
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
    ResinSlowdownModifierComponent target1 = (ResinSlowdownModifierComponent) target;
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
    ResinSlowdownModifierComponent target1 = (ResinSlowdownModifierComponent) target;
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
  virtual ResinSlowdownModifierComponent Component.Instantiate()
  {
    return new ResinSlowdownModifierComponent();
  }
}
