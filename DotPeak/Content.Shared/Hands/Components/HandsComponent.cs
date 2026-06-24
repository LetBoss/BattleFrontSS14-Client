// Decompiled with JetBrains decompiler
// Type: Content.Shared.Hands.Components.HandsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DisplacementMap;
using Content.Shared.Hands.EntitySystems;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Hands.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentPause]
[Access(new Type[] {typeof (SharedHandsSystem)})]
public sealed class HandsComponent : 
  Component,
  ISerializationGenerated<HandsComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string? ActiveHandId;
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, Hand> Hands = new Dictionary<string, Hand>();
  [DataField(null, false, 1, false, false, null)]
  public List<string> SortedHands = new List<string>();
  [DataField(null, false, 1, false, false, null)]
  public bool DisableExplosionRecursion;
  [DataField(null, false, 1, false, false, null)]
  public float BaseThrowspeed = 11f;
  [DataField(null, false, 1, false, false, null)]
  public float ThrowRange = 8f;
  [DataField(null, false, 1, false, false, null)]
  public bool ShowInHands = true;
  public readonly Dictionary<HandLocation, HashSet<string>> RevealedLayers = new Dictionary<HandLocation, HashSet<string>>();
  [DataField(null, false, 1, false, false, null)]
  [AutoPausedField]
  public TimeSpan NextThrowTime;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan ThrowCooldown = TimeSpan.FromSeconds(0.25);
  [DataField(null, false, 1, false, false, null)]
  public DisplacementData? HandDisplacement;
  [DataField(null, false, 1, false, false, null)]
  public DisplacementData? LeftHandDisplacement;
  [DataField(null, false, 1, false, false, null)]
  public DisplacementData? RightHandDisplacement;
  [DataField(null, false, 1, false, false, null)]
  public bool CanBeStripped = true;
  [DataField(null, false, 1, false, false, null)]
  public bool ExamineShowEmpty = true;

  [Robust.Shared.ViewVariables.ViewVariables]
  public int Count => this.Hands.Count;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref HandsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (HandsComponent) target1;
    if (serialization.TryCustomCopy<HandsComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (!serialization.TryCustomCopy<string>(this.ActiveHandId, ref target2, hookCtx, false, context))
      target2 = this.ActiveHandId;
    target.ActiveHandId = target2;
    Dictionary<string, Hand> target3 = (Dictionary<string, Hand>) null;
    if (this.Hands == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Hand>>(this.Hands, ref target3, hookCtx, true, context))
      target3 = serialization.CreateCopy<Dictionary<string, Hand>>(this.Hands, hookCtx, context);
    target.Hands = target3;
    List<string> target4 = (List<string>) null;
    if (this.SortedHands == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<string>>(this.SortedHands, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<string>>(this.SortedHands, hookCtx, context);
    target.SortedHands = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.DisableExplosionRecursion, ref target5, hookCtx, false, context))
      target5 = this.DisableExplosionRecursion;
    target.DisableExplosionRecursion = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.BaseThrowspeed, ref target6, hookCtx, false, context))
      target6 = this.BaseThrowspeed;
    target.BaseThrowspeed = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ThrowRange, ref target7, hookCtx, false, context))
      target7 = this.ThrowRange;
    target.ThrowRange = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowInHands, ref target8, hookCtx, false, context))
      target8 = this.ShowInHands;
    target.ShowInHands = target8;
    TimeSpan target9 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextThrowTime, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<TimeSpan>(this.NextThrowTime, hookCtx, context);
    target.NextThrowTime = target9;
    TimeSpan target10 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ThrowCooldown, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<TimeSpan>(this.ThrowCooldown, hookCtx, context);
    target.ThrowCooldown = target10;
    DisplacementData target11 = (DisplacementData) null;
    if (!serialization.TryCustomCopy<DisplacementData>(this.HandDisplacement, ref target11, hookCtx, false, context))
    {
      if (this.HandDisplacement == null)
        target11 = (DisplacementData) null;
      else
        serialization.CopyTo<DisplacementData>(this.HandDisplacement, ref target11, hookCtx, context);
    }
    target.HandDisplacement = target11;
    DisplacementData target12 = (DisplacementData) null;
    if (!serialization.TryCustomCopy<DisplacementData>(this.LeftHandDisplacement, ref target12, hookCtx, false, context))
    {
      if (this.LeftHandDisplacement == null)
        target12 = (DisplacementData) null;
      else
        serialization.CopyTo<DisplacementData>(this.LeftHandDisplacement, ref target12, hookCtx, context);
    }
    target.LeftHandDisplacement = target12;
    DisplacementData target13 = (DisplacementData) null;
    if (!serialization.TryCustomCopy<DisplacementData>(this.RightHandDisplacement, ref target13, hookCtx, false, context))
    {
      if (this.RightHandDisplacement == null)
        target13 = (DisplacementData) null;
      else
        serialization.CopyTo<DisplacementData>(this.RightHandDisplacement, ref target13, hookCtx, context);
    }
    target.RightHandDisplacement = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanBeStripped, ref target14, hookCtx, false, context))
      target14 = this.CanBeStripped;
    target.CanBeStripped = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.ExamineShowEmpty, ref target15, hookCtx, false, context))
      target15 = this.ExamineShowEmpty;
    target.ExamineShowEmpty = target15;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref HandsComponent target,
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
    HandsComponent target1 = (HandsComponent) target;
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
    HandsComponent target1 = (HandsComponent) target;
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
    HandsComponent target1 = (HandsComponent) target;
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
  virtual HandsComponent Component.Instantiate() => new HandsComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class HandsComponent_AutoPauseSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<HandsComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<HandsComponent, EntityUnpausedEvent>(this.OnEntityUnpaused));
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      HandsComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextThrowTime += args.PausedTime;
    }
  }
}
