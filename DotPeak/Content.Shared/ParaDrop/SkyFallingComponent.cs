// Decompiled with JetBrains decompiler
// Type: Content.Shared.ParaDrop.SkyFallingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.ParaDrop;

[RegisterComponent]
[NetworkedComponent]
public sealed class SkyFallingComponent : 
  Component,
  ISerializationGenerated<SkyFallingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float RemainingTime = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 OriginalScale;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 AnimationScale = new Vector2(0.01f, 0.01f);
  [DataField(null, false, 1, false, false, null)]
  public EntityCoordinates? TargetCoordinates;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SkyFallingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SkyFallingComponent) target1;
    if (serialization.TryCustomCopy<SkyFallingComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RemainingTime, ref target2, hookCtx, false, context))
      target2 = this.RemainingTime;
    target.RemainingTime = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OriginalScale, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.OriginalScale, hookCtx, context);
    target.OriginalScale = target3;
    Vector2 target4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.AnimationScale, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2>(this.AnimationScale, hookCtx, context);
    target.AnimationScale = target4;
    EntityCoordinates? target5 = new EntityCoordinates?();
    if (!serialization.TryCustomCopy<EntityCoordinates?>(this.TargetCoordinates, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<EntityCoordinates?>(this.TargetCoordinates, hookCtx, context);
    target.TargetCoordinates = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SkyFallingComponent target,
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
    SkyFallingComponent target1 = (SkyFallingComponent) target;
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
    SkyFallingComponent target1 = (SkyFallingComponent) target;
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
    SkyFallingComponent target1 = (SkyFallingComponent) target;
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
  virtual SkyFallingComponent Component.Instantiate() => new SkyFallingComponent();
}
