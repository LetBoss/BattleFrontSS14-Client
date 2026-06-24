// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.InjectorComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class InjectorComponent : 
  Component,
  ISerializationGenerated<InjectorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public string SolutionName = "injector";
  [DataField(null, false, 1, false, false, null)]
  public bool InjectOnly;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreMobs;
  [DataField(null, false, 1, false, false, null)]
  public bool IgnoreClosed = true;
  [DataField("minTransferAmount", false, 1, false, false, null)]
  public FixedPoint2 MinimumTransferAmount = FixedPoint2.New(5);
  [DataField("maxTransferAmount", false, 1, false, false, null)]
  public FixedPoint2 MaximumTransferAmount = FixedPoint2.New(15);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2 TransferAmount = FixedPoint2.New(5);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan Delay = TimeSpan.FromSeconds(5L);
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DelayPerVolume = TimeSpan.FromSeconds(0.1);
  [AutoNetworkedField]
  [DataField(null, false, 1, false, false, null)]
  public InjectorToggleMode ToggleState = InjectorToggleMode.Draw;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<ReagentPrototype>>? ReagentWhitelist;
  [DataField(null, false, 1, false, false, null)]
  public bool NeedHand = true;
  [DataField(null, false, 1, false, false, null)]
  public bool BreakOnHandChange = true;
  [DataField(null, false, 1, false, false, null)]
  public float MovementThreshold = 0.1f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref InjectorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (InjectorComponent) component;
    if (serialization.TryCustomCopy<InjectorComponent>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.SolutionName == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.SolutionName, ref str, hookCtx, false, context))
      str = this.SolutionName;
    target.SolutionName = str;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.InjectOnly, ref flag1, hookCtx, false, context))
      flag1 = this.InjectOnly;
    target.InjectOnly = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreMobs, ref flag2, hookCtx, false, context))
      flag2 = this.IgnoreMobs;
    target.IgnoreMobs = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.IgnoreClosed, ref flag3, hookCtx, false, context))
      flag3 = this.IgnoreClosed;
    target.IgnoreClosed = flag3;
    FixedPoint2 fixedPoint2_1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinimumTransferAmount, ref fixedPoint2_1, hookCtx, false, context))
      fixedPoint2_1 = serialization.CreateCopy<FixedPoint2>(this.MinimumTransferAmount, hookCtx, context, false);
    target.MinimumTransferAmount = fixedPoint2_1;
    FixedPoint2 fixedPoint2_2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaximumTransferAmount, ref fixedPoint2_2, hookCtx, false, context))
      fixedPoint2_2 = serialization.CreateCopy<FixedPoint2>(this.MaximumTransferAmount, hookCtx, context, false);
    target.MaximumTransferAmount = fixedPoint2_2;
    FixedPoint2 fixedPoint2_3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref fixedPoint2_3, hookCtx, false, context))
      fixedPoint2_3 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context, false);
    target.TransferAmount = fixedPoint2_3;
    TimeSpan timeSpan1 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.Delay, ref timeSpan1, hookCtx, false, context))
      timeSpan1 = serialization.CreateCopy<TimeSpan>(this.Delay, hookCtx, context, false);
    target.Delay = timeSpan1;
    TimeSpan timeSpan2 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DelayPerVolume, ref timeSpan2, hookCtx, false, context))
      timeSpan2 = serialization.CreateCopy<TimeSpan>(this.DelayPerVolume, hookCtx, context, false);
    target.DelayPerVolume = timeSpan2;
    InjectorToggleMode injectorToggleMode = InjectorToggleMode.Inject;
    if (!serialization.TryCustomCopy<InjectorToggleMode>(this.ToggleState, ref injectorToggleMode, hookCtx, false, context))
      injectorToggleMode = this.ToggleState;
    target.ToggleState = injectorToggleMode;
    List<ProtoId<ReagentPrototype>> protoIdList = (List<ProtoId<ReagentPrototype>>) null;
    if (!serialization.TryCustomCopy<List<ProtoId<ReagentPrototype>>>(this.ReagentWhitelist, ref protoIdList, hookCtx, true, context))
      protoIdList = serialization.CreateCopy<List<ProtoId<ReagentPrototype>>>(this.ReagentWhitelist, hookCtx, context, false);
    target.ReagentWhitelist = protoIdList;
    bool flag4 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeedHand, ref flag4, hookCtx, false, context))
      flag4 = this.NeedHand;
    target.NeedHand = flag4;
    bool flag5 = false;
    if (!serialization.TryCustomCopy<bool>(this.BreakOnHandChange, ref flag5, hookCtx, false, context))
      flag5 = this.BreakOnHandChange;
    target.BreakOnHandChange = flag5;
    float num = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MovementThreshold, ref num, hookCtx, false, context))
      num = this.MovementThreshold;
    target.MovementThreshold = num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref InjectorComponent target,
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
    InjectorComponent target1 = (InjectorComponent) target;
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
    InjectorComponent target1 = (InjectorComponent) target;
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
    InjectorComponent target1 = (InjectorComponent) target;
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
  virtual InjectorComponent Component.Instantiate() => new InjectorComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class InjectorComponent_AutoState : IComponentState
  {
    public FixedPoint2 TransferAmount;
    public InjectorToggleMode ToggleState;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class InjectorComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<InjectorComponent, ComponentGetState>(new ComponentEventRefHandler<InjectorComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<InjectorComponent, ComponentHandleState>(new ComponentEventRefHandler<InjectorComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(EntityUid uid, InjectorComponent component, ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new InjectorComponent.InjectorComponent_AutoState()
      {
        TransferAmount = component.TransferAmount,
        ToggleState = component.ToggleState
      };
    }

    private void OnHandleState(
      EntityUid uid,
      InjectorComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is InjectorComponent.InjectorComponent_AutoState current))
        return;
      component.TransferAmount = current.TransferAmount;
      component.ToggleState = current.ToggleState;
    }
  }
}
