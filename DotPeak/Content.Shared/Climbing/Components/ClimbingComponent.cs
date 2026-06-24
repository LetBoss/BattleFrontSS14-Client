// Decompiled with JetBrains decompiler
// Type: Content.Shared.Climbing.Components.ClimbingComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.DoAfter;
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
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Climbing.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[AutoGenerateComponentPause]
public sealed class ClimbingComponent : 
  Component,
  ISerializationGenerated<ClimbingComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool CanClimb = true;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public bool IsClimbing;
  [DataField(null, false, 1, false, false, null)]
  public DoAfterId? DoAfter;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  public TimeSpan? NextTransition;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Direction;
  [DataField(null, false, 1, false, false, null)]
  public float TransitionRate = 5f;
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, int> DisabledFixtureMasks = new Dictionary<string, int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref ClimbingComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (ClimbingComponent) component;
    if (serialization.TryCustomCopy<ClimbingComponent>(this, ref target, hookCtx, false, context))
      return;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanClimb, ref flag1, hookCtx, false, context))
      flag1 = this.CanClimb;
    target.CanClimb = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IsClimbing, ref flag2, hookCtx, false, context))
      flag2 = this.IsClimbing;
    target.IsClimbing = flag2;
    DoAfterId? nullable1 = new DoAfterId?();
    if (!serialization.TryCustomCopy<DoAfterId?>(this.DoAfter, ref nullable1, hookCtx, false, context))
      nullable1 = serialization.CreateCopy<DoAfterId?>(this.DoAfter, hookCtx, context, false);
    target.DoAfter = nullable1;
    TimeSpan? nullable2 = new TimeSpan?();
    if (!serialization.TryCustomCopy<TimeSpan?>(this.NextTransition, ref nullable2, hookCtx, false, context))
      nullable2 = serialization.CreateCopy<TimeSpan?>(this.NextTransition, hookCtx, context, false);
    target.NextTransition = nullable2;
    Vector2 vector2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Direction, ref vector2, hookCtx, false, context))
      vector2 = serialization.CreateCopy<Vector2>(this.Direction, hookCtx, context, false);
    target.Direction = vector2;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.TransitionRate, ref num, hookCtx, false, context))
      num = this.TransitionRate;
    target.TransitionRate = num;
    Dictionary<string, int> dictionary = (Dictionary<string, int>) null;
    if (this.DisabledFixtureMasks == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, int>>(this.DisabledFixtureMasks, ref dictionary, hookCtx, true, context))
      dictionary = serialization.CreateCopy<Dictionary<string, int>>(this.DisabledFixtureMasks, hookCtx, context, false);
    target.DisabledFixtureMasks = dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref ClimbingComponent target,
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
    ClimbingComponent target1 = (ClimbingComponent) target;
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
    ClimbingComponent target1 = (ClimbingComponent) target;
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
    ClimbingComponent target1 = (ClimbingComponent) target;
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
  virtual ClimbingComponent Component.Instantiate() => new ClimbingComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ClimbingComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClimbingComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<ClimbingComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      ClimbingComponent component,
      ref EntityUnpausedEvent args)
    {
      if (component.NextTransition.HasValue)
        component.NextTransition = new TimeSpan?(component.NextTransition.Value + args.PausedTime);
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class ClimbingComponent_AutoState : IComponentState
  {
    public bool CanClimb;
    public bool IsClimbing;
    public TimeSpan? NextTransition;
    public Vector2 Direction;
    public 
    #nullable enable
    Dictionary<string, int> DisabledFixtureMasks;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ClimbingComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClimbingComponent, ComponentGetState>(new ComponentEventRefHandler<ClimbingComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<ClimbingComponent, ComponentHandleState>(new ComponentEventRefHandler<ClimbingComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, ClimbingComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new ClimbingComponent.ClimbingComponent_AutoState()
      {
        CanClimb = component.CanClimb,
        IsClimbing = component.IsClimbing,
        NextTransition = component.NextTransition,
        Direction = component.Direction,
        DisabledFixtureMasks = component.DisabledFixtureMasks
      };
    }

    private void OnHandleState(
      EntityUid uid,
      ClimbingComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is ClimbingComponent.ClimbingComponent_AutoState current))
        return;
      component.CanClimb = current.CanClimb;
      component.IsClimbing = current.IsClimbing;
      component.NextTransition = current.NextTransition;
      component.Direction = current.Direction;
      component.DisabledFixtureMasks = current.DisabledFixtureMasks == null ? (Dictionary<string, int>) null : new Dictionary<string, int>((IDictionary<string, int>) current.DisabledFixtureMasks);
    }
  }
}
