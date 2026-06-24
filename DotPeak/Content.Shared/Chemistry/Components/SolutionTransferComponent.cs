// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.Components.SolutionTransferComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Chemistry.Components;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class SolutionTransferComponent : 
  Component,
  ISerializationGenerated<SolutionTransferComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public FixedPoint2[]? TransferAmounts;

  [DataField("transferAmount", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public FixedPoint2 TransferAmount { get; set; } = FixedPoint2.New(5);

  [DataField("minTransferAmount", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 MinimumTransferAmount { get; set; } = FixedPoint2.New(5);

  [DataField("maxTransferAmount", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public FixedPoint2 MaximumTransferAmount { get; set; } = FixedPoint2.New(100);

  [DataField("canReceive", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool CanReceive { get; set; } = true;

  [DataField("canSend", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool CanSend { get; set; } = true;

  [DataField("canChangeTransferAmount", false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public bool CanChangeTransferAmount { get; set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SolutionTransferComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (SolutionTransferComponent) component;
    if (serialization.TryCustomCopy<SolutionTransferComponent>(this, ref target, hookCtx, false, context))
      return;
    FixedPoint2 fixedPoint2_1 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.TransferAmount, ref fixedPoint2_1, hookCtx, false, context))
      fixedPoint2_1 = serialization.CreateCopy<FixedPoint2>(this.TransferAmount, hookCtx, context, false);
    target.TransferAmount = fixedPoint2_1;
    FixedPoint2 fixedPoint2_2 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MinimumTransferAmount, ref fixedPoint2_2, hookCtx, false, context))
      fixedPoint2_2 = serialization.CreateCopy<FixedPoint2>(this.MinimumTransferAmount, hookCtx, context, false);
    target.MinimumTransferAmount = fixedPoint2_2;
    FixedPoint2 fixedPoint2_3 = new FixedPoint2();
    if (!serialization.TryCustomCopy<FixedPoint2>(this.MaximumTransferAmount, ref fixedPoint2_3, hookCtx, false, context))
      fixedPoint2_3 = serialization.CreateCopy<FixedPoint2>(this.MaximumTransferAmount, hookCtx, context, false);
    target.MaximumTransferAmount = fixedPoint2_3;
    bool flag1 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanReceive, ref flag1, hookCtx, false, context))
      flag1 = this.CanReceive;
    target.CanReceive = flag1;
    bool flag2 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanSend, ref flag2, hookCtx, false, context))
      flag2 = this.CanSend;
    target.CanSend = flag2;
    bool flag3 = false;
    if (!serialization.TryCustomCopy<bool>(this.CanChangeTransferAmount, ref flag3, hookCtx, false, context))
      flag3 = this.CanChangeTransferAmount;
    target.CanChangeTransferAmount = flag3;
    FixedPoint2[] fixedPoint2Array = (FixedPoint2[]) null;
    if (!serialization.TryCustomCopy<FixedPoint2[]>(this.TransferAmounts, ref fixedPoint2Array, hookCtx, true, context))
      fixedPoint2Array = serialization.CreateCopy<FixedPoint2[]>(this.TransferAmounts, hookCtx, context, false);
    target.TransferAmounts = fixedPoint2Array;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SolutionTransferComponent target,
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
    SolutionTransferComponent target1 = (SolutionTransferComponent) target;
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
    SolutionTransferComponent target1 = (SolutionTransferComponent) target;
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
    SolutionTransferComponent target1 = (SolutionTransferComponent) target;
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
  virtual SolutionTransferComponent Component.Instantiate() => new SolutionTransferComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class SolutionTransferComponent_AutoState : IComponentState
  {
    public FixedPoint2 TransferAmount;
    public FixedPoint2[]? TransferAmounts;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class SolutionTransferComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionTransferComponent, ComponentGetState>(new ComponentEventRefHandler<SolutionTransferComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<SolutionTransferComponent, ComponentHandleState>(new ComponentEventRefHandler<SolutionTransferComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      SolutionTransferComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new SolutionTransferComponent.SolutionTransferComponent_AutoState()
      {
        TransferAmount = component.TransferAmount,
        TransferAmounts = component.TransferAmounts
      };
    }

    private void OnHandleState(
      EntityUid uid,
      SolutionTransferComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is SolutionTransferComponent.SolutionTransferComponent_AutoState current))
        return;
      component.TransferAmount = current.TransferAmount;
      component.TransferAmounts = current.TransferAmounts;
    }
  }
}
