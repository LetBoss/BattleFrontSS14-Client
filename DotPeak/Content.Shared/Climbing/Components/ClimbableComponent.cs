// Decompiled with JetBrains decompiler
// Type: Content.Shared.Climbing.Components.ClimbableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Climbing.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class ClimbableComponent : 
  Component,
  ISerializationGenerated<ClimbableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float Range = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public bool Vaultable = true;
  [DataField("delay", false, 1, false, false, null)]
  public float ClimbDelay = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public float VaultPastDistance;
  [DataField("startClimbSound", false, 1, false, false, null)]
  public SoundSpecifier? StartClimbSound;
  [DataField("finishClimbSound", false, 1, false, false, null)]
  public SoundSpecifier? FinishClimbSound;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClimbableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ClimbableComponent) component;
    if (serialization.TryCustomCopy<ClimbableComponent>(this, ref target, hookCtx, false, context))
      return;
    float num1 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Range, ref num1, hookCtx, false, context))
      num1 = this.Range;
    target.Range = num1;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.Vaultable, ref flag, hookCtx, false, context))
      flag = this.Vaultable;
    target.Vaultable = flag;
    float num2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ClimbDelay, ref num2, hookCtx, false, context))
      num2 = this.ClimbDelay;
    target.ClimbDelay = num2;
    float num3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.VaultPastDistance, ref num3, hookCtx, false, context))
      num3 = this.VaultPastDistance;
    target.VaultPastDistance = num3;
    SoundSpecifier soundSpecifier1 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.StartClimbSound, ref soundSpecifier1, hookCtx, true, context))
      soundSpecifier1 = serialization.CreateCopy<SoundSpecifier>(this.StartClimbSound, hookCtx, context, false);
    target.StartClimbSound = soundSpecifier1;
    SoundSpecifier soundSpecifier2 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.FinishClimbSound, ref soundSpecifier2, hookCtx, true, context))
      soundSpecifier2 = serialization.CreateCopy<SoundSpecifier>(this.FinishClimbSound, hookCtx, context, false);
    target.FinishClimbSound = soundSpecifier2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClimbableComponent target,
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
    ClimbableComponent target1 = (ClimbableComponent) target;
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
    ClimbableComponent target1 = (ClimbableComponent) target;
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
    ClimbableComponent target1 = (ClimbableComponent) target;
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
  virtual ClimbableComponent Component.Instantiate() => new ClimbableComponent();
}
