// Decompiled with JetBrains decompiler
// Type: Content.Shared.GameTicking.Components.GameRuleComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Destructible.Thresholds;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.GameTicking.Components;

[RegisterComponent]
[EntityCategory(new string[] {"GameRules"})]
public sealed class GameRuleComponent : 
  Component,
  ISerializationGenerated<GameRuleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan ActivatedAt;
  [DataField(null, false, 1, false, false, null)]
  public int MinPlayers;
  [DataField(null, false, 1, false, false, null)]
  public bool CancelPresetOnTooFewPlayers = true;
  [DataField(null, false, 1, false, false, null)]
  public MinMax? Delay;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GameRuleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GameRuleComponent) target1;
    if (serialization.TryCustomCopy<GameRuleComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan target2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.ActivatedAt, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<TimeSpan>(this.ActivatedAt, hookCtx, context);
    target.ActivatedAt = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MinPlayers, ref target3, hookCtx, false, context))
      target3 = this.MinPlayers;
    target.MinPlayers = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.CancelPresetOnTooFewPlayers, ref target4, hookCtx, false, context))
      target4 = this.CancelPresetOnTooFewPlayers;
    target.CancelPresetOnTooFewPlayers = target4;
    MinMax? target5 = new MinMax?();
    if (!serialization.TryCustomCopy<MinMax?>(this.Delay, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<MinMax?>(this.Delay, hookCtx, context);
    target.Delay = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GameRuleComponent target,
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
    GameRuleComponent target1 = (GameRuleComponent) target;
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
    GameRuleComponent target1 = (GameRuleComponent) target;
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
    GameRuleComponent target1 = (GameRuleComponent) target;
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
  virtual GameRuleComponent Component.Instantiate() => new GameRuleComponent();
}
