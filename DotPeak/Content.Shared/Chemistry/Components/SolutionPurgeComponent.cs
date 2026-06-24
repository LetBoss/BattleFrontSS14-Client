// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionPurgeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[AutoGenerateComponentPause]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (SolutionPurgeSystem)})]
public sealed class SolutionPurgeComponent : 
  Component,
  ISerializationGenerated<SolutionPurgeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public string Solution = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ReagentPrototype>> Preserve = new List<ProtoId<ReagentPrototype>>();
  [DataField(null, false, 1, true, false, null)]
  public FixedPoint2 Quantity;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Duration = TimeSpan.FromSeconds(1L);
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [AutoPausedField]
  [AutoNetworkedField]
  public TimeSpan NextPurgeTime;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionPurgeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionPurgeComponent) component;
    if (serialization.TryCustomCopy<SolutionPurgeComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.Solution == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Solution, ref str, hookCtx, false, context))
      str = this.Solution;
    target.Solution = str;
    List<ProtoId<ReagentPrototype>> protoIdList = (List<ProtoId<ReagentPrototype>>) null;
    if (this.Preserve == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<ReagentPrototype>>>(this.Preserve, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<ReagentPrototype>>>(this.Preserve, hookCtx, context, false);
    target.Preserve = protoIdList;
    FixedPoint2 fixedPoint2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.Quantity, ref fixedPoint2, hookCtx, false, context))
      fixedPoint2 = serialization.CreateCopy<FixedPoint2>(this.Quantity, hookCtx, context, false);
    target.Quantity = fixedPoint2;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Duration, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.Duration, hookCtx, context, false);
    target.Duration = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextPurgeTime, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.NextPurgeTime, hookCtx, context, false);
    target.NextPurgeTime = timeSpan2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionPurgeComponent target,
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
    SolutionPurgeComponent target1 = (SolutionPurgeComponent) target;
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
    SolutionPurgeComponent target1 = (SolutionPurgeComponent) target;
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
    SolutionPurgeComponent target1 = (SolutionPurgeComponent) target;
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
  virtual SolutionPurgeComponent Component.Instantiate() => new SolutionPurgeComponent();

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionPurgeComponent_AutoPauseSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionPurgeComponent, EntityUnpausedEvent>(new ComponentEventRefHandler<SolutionPurgeComponent, EntityUnpausedEvent>((object) this, __methodptr(OnEntityUnpaused)), (Type[]) null, (Type[]) null);
    }

    private void OnEntityUnpaused(
      EntityUid uid,
      #nullable disable
      SolutionPurgeComponent component,
      ref EntityUnpausedEvent args)
    {
      component.NextPurgeTime += args.PausedTime;
      this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
    }
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SolutionPurgeComponent_AutoState : IComponentState
  {
    public TimeSpan NextPurgeTime;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionPurgeComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionPurgeComponent, ComponentGetState>(new ComponentEventRefHandler<SolutionPurgeComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionPurgeComponent, ComponentHandleState>(new ComponentEventRefHandler<SolutionPurgeComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      #nullable enable
      SolutionPurgeComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SolutionPurgeComponent.SolutionPurgeComponent_AutoState()
      {
        NextPurgeTime = component.NextPurgeTime
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SolutionPurgeComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SolutionPurgeComponent.SolutionPurgeComponent_AutoState current))
        return;
      component.NextPurgeTime = current.NextPurgeTime;
    }
  }
}
