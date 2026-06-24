// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Slow.RMCSpeciesSlowdownModifierComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Slow;

[RegisterComponent]
[NetworkedComponent]
public sealed class RMCSpeciesSlowdownModifierComponent : 
  Component,
  ISerializationGenerated<RMCSpeciesSlowdownModifierComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float SlowModifier;
  [DataField(null, false, 1, false, false, null)]
  public float SuperSlowModifier;
  [DataField(null, false, 1, false, false, null)]
  public float DurationMultiplier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public string[] StatusesToUpdateOn = new string[3]
  {
    "Stun",
    "KnockedDown",
    "Unconscious"
  };

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCSpeciesSlowdownModifierComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCSpeciesSlowdownModifierComponent) target1;
    if (serialization.TryCustomCopy<RMCSpeciesSlowdownModifierComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SlowModifier, ref target2, hookCtx, false, context))
      target2 = this.SlowModifier;
    target.SlowModifier = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SuperSlowModifier, ref target3, hookCtx, false, context))
      target3 = this.SuperSlowModifier;
    target.SuperSlowModifier = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.DurationMultiplier, ref target4, hookCtx, false, context))
      target4 = this.DurationMultiplier;
    target.DurationMultiplier = target4;
    string[] target5 = (string[]) null;
    if (this.StatusesToUpdateOn == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string[]>(this.StatusesToUpdateOn, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<string[]>(this.StatusesToUpdateOn, hookCtx, context);
    target.StatusesToUpdateOn = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCSpeciesSlowdownModifierComponent target,
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
    RMCSpeciesSlowdownModifierComponent target1 = (RMCSpeciesSlowdownModifierComponent) target;
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
    RMCSpeciesSlowdownModifierComponent target1 = (RMCSpeciesSlowdownModifierComponent) target;
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
    RMCSpeciesSlowdownModifierComponent target1 = (RMCSpeciesSlowdownModifierComponent) target;
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
  virtual RMCSpeciesSlowdownModifierComponent Component.Instantiate()
  {
    return new RMCSpeciesSlowdownModifierComponent();
  }
}
