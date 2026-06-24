// Decompiled with JetBrains decompiler
// Type: Content.Shared.Prying.Components.PryingComponent
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
namespace Content.Shared.Prying.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class PryingComponent : 
  Component,
  ISerializationGenerated<PryingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public bool PryPowered;
  [DataField(null, false, 1, false, false, null)]
  public bool Force;
  [DataField(null, false, 1, false, false, null)]
  public float SpeedModifier = 1f;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? UseSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Items/crowbar.ogg");
  [DataField(null, false, 1, false, false, null)]
  public bool Enabled = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PryingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PryingComponent) target1;
    if (serialization.TryCustomCopy<PryingComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.PryPowered, ref target2, hookCtx, false, context))
      target2 = this.PryPowered;
    target.PryPowered = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Force, ref target3, hookCtx, false, context))
      target3 = this.Force;
    target.Force = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SpeedModifier, ref target4, hookCtx, false, context))
      target4 = this.SpeedModifier;
    target.SpeedModifier = target4;
    SoundSpecifier target5 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.UseSound, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<SoundSpecifier>(this.UseSound, hookCtx, context);
    target.UseSound = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.Enabled, ref target6, hookCtx, false, context))
      target6 = this.Enabled;
    target.Enabled = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PryingComponent target,
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
    PryingComponent target1 = (PryingComponent) target;
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
    PryingComponent target1 = (PryingComponent) target;
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
    PryingComponent target1 = (PryingComponent) target;
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
  virtual PryingComponent Component.Instantiate() => new PryingComponent();
}
