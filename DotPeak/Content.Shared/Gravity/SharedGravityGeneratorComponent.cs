// Decompiled with JetBrains decompiler
// Type: Content.Shared.Gravity.SharedGravityGeneratorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Power;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.ViewVariables;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Gravity;

[NetworkedComponent]
[Virtual]
public class SharedGravityGeneratorComponent : 
  Component,
  ISerializationGenerated<SharedGravityGeneratorComponent>,
  ISerializationGenerated
{
  [DataField("spriteMap", false, 1, false, false, null)]
  [Access(new Type[] {typeof (SharedGravitySystem)})]
  public Dictionary<PowerChargeStatus, string> SpriteMap = new Dictionary<PowerChargeStatus, string>();
  [DataField("coreStartupState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string CoreStartupState = "startup";
  [DataField("coreIdleState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string CoreIdleState = "idle";
  [DataField("coreActivatingState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string CoreActivatingState = "activating";
  [DataField("coreActivatedState", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public string CoreActivatedState = "activated";

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref SharedGravityGeneratorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SharedGravityGeneratorComponent) target1;
    if (serialization.TryCustomCopy<SharedGravityGeneratorComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<PowerChargeStatus, string> target2 = (Dictionary<PowerChargeStatus, string>) null;
    if (this.SpriteMap == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<PowerChargeStatus, string>>(this.SpriteMap, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<PowerChargeStatus, string>>(this.SpriteMap, hookCtx, context);
    target.SpriteMap = target2;
    string target3 = (string) null;
    if (this.CoreStartupState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CoreStartupState, ref target3, hookCtx, false, context))
      target3 = this.CoreStartupState;
    target.CoreStartupState = target3;
    string target4 = (string) null;
    if (this.CoreIdleState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CoreIdleState, ref target4, hookCtx, false, context))
      target4 = this.CoreIdleState;
    target.CoreIdleState = target4;
    string target5 = (string) null;
    if (this.CoreActivatingState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CoreActivatingState, ref target5, hookCtx, false, context))
      target5 = this.CoreActivatingState;
    target.CoreActivatingState = target5;
    string target6 = (string) null;
    if (this.CoreActivatedState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.CoreActivatedState, ref target6, hookCtx, false, context))
      target6 = this.CoreActivatedState;
    target.CoreActivatedState = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref SharedGravityGeneratorComponent target,
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
    SharedGravityGeneratorComponent target1 = (SharedGravityGeneratorComponent) target;
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
    SharedGravityGeneratorComponent target1 = (SharedGravityGeneratorComponent) target;
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
    SharedGravityGeneratorComponent target1 = (SharedGravityGeneratorComponent) target;
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
  virtual SharedGravityGeneratorComponent Component.Instantiate()
  {
    return new SharedGravityGeneratorComponent();
  }
}
