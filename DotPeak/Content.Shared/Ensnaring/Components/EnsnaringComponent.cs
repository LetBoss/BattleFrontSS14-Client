// Decompiled with JetBrains decompiler
// Type: Content.Shared.Ensnaring.Components.EnsnaringComponent
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
namespace Content.Shared.Ensnaring.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class EnsnaringComponent : 
  Component,
  ISerializationGenerated<EnsnaringComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float FreeTime = 3.5f;
  [DataField(null, false, 1, false, false, null)]
  public float BreakoutTime = 30f;
  [DataField(null, false, 1, false, false, null)]
  public float WalkSpeed = 0.9f;
  [DataField(null, false, 1, false, false, null)]
  public float SprintSpeed = 0.9f;
  [DataField(null, false, 1, false, false, null)]
  public float StaminaDamage = 55f;
  [DataField(null, false, 1, false, false, null)]
  public float MaxEnsnares = 1f;
  [DataField(null, false, 1, false, false, null)]
  public bool CanThrowTrigger;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Ensnared;
  [DataField(null, false, 1, false, false, null)]
  public bool CanMoveBreakout;
  [DataField(null, false, 1, false, false, null)]
  public SoundSpecifier? EnsnareSound = (SoundSpecifier) new SoundPathSpecifier("/Audio/Effects/snap.ogg");

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EnsnaringComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EnsnaringComponent) target1;
    if (serialization.TryCustomCopy<EnsnaringComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FreeTime, ref target2, hookCtx, false, context))
      target2 = this.FreeTime;
    target.FreeTime = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BreakoutTime, ref target3, hookCtx, false, context))
      target3 = this.BreakoutTime;
    target.BreakoutTime = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.WalkSpeed, ref target4, hookCtx, false, context))
      target4 = this.WalkSpeed;
    target.WalkSpeed = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.SprintSpeed, ref target5, hookCtx, false, context))
      target5 = this.SprintSpeed;
    target.SprintSpeed = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.StaminaDamage, ref target6, hookCtx, false, context))
      target6 = this.StaminaDamage;
    target.StaminaDamage = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxEnsnares, ref target7, hookCtx, false, context))
      target7 = this.MaxEnsnares;
    target.MaxEnsnares = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanThrowTrigger, ref target8, hookCtx, false, context))
      target8 = this.CanThrowTrigger;
    target.CanThrowTrigger = target8;
    EntityUid? target9 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Ensnared, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<EntityUid?>(this.Ensnared, hookCtx, context);
    target.Ensnared = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanMoveBreakout, ref target10, hookCtx, false, context))
      target10 = this.CanMoveBreakout;
    target.CanMoveBreakout = target10;
    SoundSpecifier target11 = (SoundSpecifier) null;
    if (!serialization.TryCustomCopy<SoundSpecifier>(this.EnsnareSound, ref target11, hookCtx, true, context))
      target11 = serialization.CreateCopy<SoundSpecifier>(this.EnsnareSound, hookCtx, context);
    target.EnsnareSound = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EnsnaringComponent target,
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
    EnsnaringComponent target1 = (EnsnaringComponent) target;
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
    EnsnaringComponent target1 = (EnsnaringComponent) target;
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
    EnsnaringComponent target1 = (EnsnaringComponent) target;
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
  virtual EnsnaringComponent Component.Instantiate() => new EnsnaringComponent();
}
