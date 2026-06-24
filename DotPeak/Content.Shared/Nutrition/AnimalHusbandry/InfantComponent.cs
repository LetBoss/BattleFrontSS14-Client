// Decompiled with JetBrains decompiler
// Type: Content.Shared.Nutrition.AnimalHusbandry.InfantComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Nutrition.AnimalHusbandry;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
public sealed class InfantComponent : 
  Component,
  ISerializationGenerated<InfantComponent>,
  ISerializationGenerated
{
  [DataField("infantDuration", false, 1, false, false, null)]
  public TimeSpan InfantDuration = TimeSpan.FromMinutes(3L);
  [DataField("defaultScale", false, 1, false, false, null)]
  public Vector2 DefaultScale = Vector2.One;
  [DataField("visualScale", false, 1, false, false, null)]
  public Vector2 VisualScale = new Vector2(0.5f, 0.5f);
  [DataField("infantEndTime", false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan InfantEndTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InfantComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (InfantComponent) target1;
    if (serialization.TryCustomCopy<InfantComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.InfantDuration, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.InfantDuration, hookCtx, context);
    target.InfantDuration = target2;
    Vector2 target3 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.DefaultScale, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Vector2>(this.DefaultScale, hookCtx, context);
    target.DefaultScale = target3;
    Vector2 target4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.VisualScale, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2>(this.VisualScale, hookCtx, context);
    target.VisualScale = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.InfantEndTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.InfantEndTime, hookCtx, context);
    target.InfantEndTime = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InfantComponent target,
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
    InfantComponent target1 = (InfantComponent) target;
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
    InfantComponent target1 = (InfantComponent) target;
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
    InfantComponent target1 = (InfantComponent) target;
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
  virtual InfantComponent Component.Instantiate() => new InfantComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InfantComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<InfantComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<InfantComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      InfantComponent component,
      ref EntityUnpausedEvent args)
    {
      component.InfantEndTime += args.PausedTime;
    }
  }
}
