// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Components.InputMoverComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Movement.Components;

[RegisterComponent]
[NetworkedComponent]
public sealed class InputMoverComponent : 
  Component,
  ISerializationGenerated<InputMoverComponent>,
  ISerializationGenerated
{
  public GameTick LastInputTick;
  public ushort LastInputSubTick;
  public Vector2 CurTickWalkMovement;
  public Vector2 CurTickSprintMovement;
  public MoveButtons HeldMoveButtons;
  public Vector2 WishDir;
  public EntityUid? RelativeEntity;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle TargetRelativeRotation = Angle.Zero;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Angle RelativeRotation;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan LerpTarget;
  public const float LerpTime = 1f;
  [Robust.Shared.ViewVariables.ViewVariables(VVAccess.ReadWrite)]
  public bool CanMove = true;

  public bool HasDirectionalMovement => (this.HeldMoveButtons & MoveButtons.AnyDirection) != 0;

  public bool Sprinting => (this.HeldMoveButtons & MoveButtons.Walk) == MoveButtons.None;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InputMoverComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InputMoverComponent) target1;
    if (serialization.TryCustomCopy<InputMoverComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LerpTarget, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.LerpTarget, hookCtx, context);
    target.LerpTarget = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InputMoverComponent target,
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
    InputMoverComponent target1 = (InputMoverComponent) target;
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
    InputMoverComponent target1 = (InputMoverComponent) target;
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
    InputMoverComponent target1 = (InputMoverComponent) target;
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
  virtual InputMoverComponent Component.Instantiate() => new InputMoverComponent();
}
