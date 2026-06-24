// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Projectiles.ProjectileBlowOnStopComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._CIV14merka.Projectiles;

[RegisterComponent]
[NetworkedComponent]
public sealed class ProjectileBlowOnStopComponent : 
  Component,
  ISerializationGenerated<ProjectileBlowOnStopComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float StopThreshold = 0.05f;
  [DataField(null, false, 1, false, false, null)]
  public int StallTicks = 3;
  [DataField(null, false, 1, false, false, null)]
  public float ArmDelay = 0.2f;
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntitySystem)})]
  public TimeSpan ArmAt;
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntitySystem)})]
  public int StallCounter;
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntitySystem)})]
  public Vector2 PrevPos;
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntitySystem)})]
  public bool PrevPosSet;
  [Robust.Shared.ViewVariables.ViewVariables]
  [Access(new Type[] {typeof (EntitySystem)})]
  public bool Triggered;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ProjectileBlowOnStopComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (ProjectileBlowOnStopComponent) target1;
    if (serialization.TryCustomCopy<ProjectileBlowOnStopComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StopThreshold, ref target2, hookCtx, false, context))
      target2 = this.StopThreshold;
    target.StopThreshold = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.StallTicks, ref target3, hookCtx, false, context))
      target3 = this.StallTicks;
    target.StallTicks = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ArmDelay, ref target4, hookCtx, false, context))
      target4 = this.ArmDelay;
    target.ArmDelay = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ProjectileBlowOnStopComponent target,
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
    ProjectileBlowOnStopComponent target1 = (ProjectileBlowOnStopComponent) target;
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
    ProjectileBlowOnStopComponent target1 = (ProjectileBlowOnStopComponent) target;
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
    ProjectileBlowOnStopComponent target1 = (ProjectileBlowOnStopComponent) target;
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
  virtual ProjectileBlowOnStopComponent Component.Instantiate()
  {
    return new ProjectileBlowOnStopComponent();
  }
}
