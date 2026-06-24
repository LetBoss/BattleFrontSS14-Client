// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Components.StomachComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Whitelist;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Body.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (StomachSystem), typeof (FoodSystem)})]
public sealed class StomachComponent : 
  Component,
  ISerializationGenerated<StomachComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  public TimeSpan NextUpdate;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, null)]
  public float UpdateIntervalMultiplier = 1f;
  [Robust.Shared.ViewVariables.ViewVariables]
  public Entity<SolutionComponent>? Solution;
  [DataField(null, false, 1, false, false, null)]
  public string BodySolutionName = "chemicals";
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DigestionDelay = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  public EntityWhitelist? SpecialDigestible;
  [DataField(null, false, 1, false, false, null)]
  public bool IsSpecialDigestibleExclusive = true;
  [Robust.Shared.ViewVariables.ViewVariables]
  public readonly List<StomachComponent.ReagentDelta> ReagentDeltas = new List<StomachComponent.ReagentDelta>();

  [Robust.Shared.ViewVariables.ViewVariables]
  public TimeSpan AdjustedUpdateInterval
  {
    get => this.UpdateInterval * (double) this.UpdateIntervalMultiplier;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref StomachComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (StomachComponent) component;
    if (serialization.TryCustomCopy<StomachComponent>(this, ref target, hookCtx, false, context))
      return;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextUpdate, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.NextUpdate, hookCtx, context, false);
    target.NextUpdate = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UpdateInterval, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.UpdateInterval, hookCtx, context, false);
    target.UpdateInterval = timeSpan2;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.UpdateIntervalMultiplier, ref num, hookCtx, false, context))
      num = this.UpdateIntervalMultiplier;
    target.UpdateIntervalMultiplier = num;
    string str = (string) null;
    if (this.BodySolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.BodySolutionName, ref str, hookCtx, false, context))
      str = this.BodySolutionName;
    target.BodySolutionName = str;
    TimeSpan timeSpan3 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DigestionDelay, ref timeSpan3, hookCtx, false, context))
      timeSpan3 = serialization.CreateCopy<TimeSpan>(this.DigestionDelay, hookCtx, context, false);
    target.DigestionDelay = timeSpan3;
    EntityWhitelist entityWhitelist = (EntityWhitelist) null;
    if (!serialization.TryCustomCopy<EntityWhitelist>(this.SpecialDigestible, ref entityWhitelist, hookCtx, false, context))
    {
      if (this.SpecialDigestible == null)
        entityWhitelist = (EntityWhitelist) null;
      else
        serialization.CopyTo<EntityWhitelist>(this.SpecialDigestible, ref entityWhitelist, hookCtx, context, false);
    }
    target.SpecialDigestible = entityWhitelist;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IsSpecialDigestibleExclusive, ref flag, hookCtx, false, context))
      flag = this.IsSpecialDigestibleExclusive;
    target.IsSpecialDigestibleExclusive = flag;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref StomachComponent target,
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
    StomachComponent target1 = (StomachComponent) target;
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
    StomachComponent target1 = (StomachComponent) target;
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
    StomachComponent target1 = (StomachComponent) target;
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
  virtual StomachComponent Component.Instantiate() => new StomachComponent();

  public sealed class ReagentDelta
  {
    public readonly ReagentQuantity ReagentQuantity;

    public TimeSpan Lifetime { get; private set; }

    public ReagentDelta(ReagentQuantity reagentQuantity)
    {
      this.ReagentQuantity = reagentQuantity;
      this.Lifetime = TimeSpan.Zero;
    }

    public void Increment(TimeSpan delta) => this.Lifetime += delta;
  }
}
