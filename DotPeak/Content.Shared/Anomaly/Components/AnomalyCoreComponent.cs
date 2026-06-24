// Decompiled with JetBrains decompiler
// Type: Content.Shared.Anomaly.Components.AnomalyCoreComponent
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
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Anomaly.Components;

[RegisterComponent]
[NetworkedComponent]
[Access(new Type[] {typeof (SharedAnomalyCoreSystem)})]
[AutoGenerateComponentState(false, false)]
public sealed class AnomalyCoreComponent : 
  Component,
  ISerializationGenerated<AnomalyCoreComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public double TimeToDecay = 600.0;
  [DataField(null, false, 1, false, false, typeof (TimeOffsetSerializer))]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public TimeSpan DecayMoment;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public double StartPrice = 10000.0;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  public double EndPrice = 200.0;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public bool IsDecayed;
  [DataField(null, false, 1, false, false, null)]
  [Robust.Shared.ViewVariables.ViewVariables]
  [AutoNetworkedField]
  public int Charge = 5;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AnomalyCoreComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component component = (Component) target;
    this.InternalCopy(ref component, serialization, hookCtx, context);
    target = (AnomalyCoreComponent) component;
    if (serialization.TryCustomCopy<AnomalyCoreComponent>(this, ref target, hookCtx, false, context))
      return;
    double num1 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.TimeToDecay, ref num1, hookCtx, false, context))
      num1 = this.TimeToDecay;
    target.TimeToDecay = num1;
    TimeSpan timeSpan = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DecayMoment, ref timeSpan, hookCtx, false, context))
      timeSpan = serialization.CreateCopy<TimeSpan>(this.DecayMoment, hookCtx, context, false);
    target.DecayMoment = timeSpan;
    double num2 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.StartPrice, ref num2, hookCtx, false, context))
      num2 = this.StartPrice;
    target.StartPrice = num2;
    double num3 = 0.0;
    if (!serialization.TryCustomCopy<double>(this.EndPrice, ref num3, hookCtx, false, context))
      num3 = this.EndPrice;
    target.EndPrice = num3;
    bool flag = false;
    if (!serialization.TryCustomCopy<bool>(this.IsDecayed, ref flag, hookCtx, false, context))
      flag = this.IsDecayed;
    target.IsDecayed = flag;
    int num4 = 0;
    if (!serialization.TryCustomCopy<int>(this.Charge, ref num4, hookCtx, false, context))
      num4 = this.Charge;
    target.Charge = num4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AnomalyCoreComponent target,
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
    AnomalyCoreComponent target1 = (AnomalyCoreComponent) target;
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
    AnomalyCoreComponent target1 = (AnomalyCoreComponent) target;
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
    AnomalyCoreComponent target1 = (AnomalyCoreComponent) target;
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
  virtual AnomalyCoreComponent Component.Instantiate() => new AnomalyCoreComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class AnomalyCoreComponent_AutoState : IComponentState
  {
    public TimeSpan DecayMoment;
    public bool IsDecayed;
    public int Charge;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class AnomalyCoreComponent_AutoNetworkSystem : EntitySystem
  {
    public virtual void Initialize()
    {
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnomalyCoreComponent, ComponentGetState>(new ComponentEventRefHandler<AnomalyCoreComponent, ComponentGetState>((object) this, __methodptr(OnGetState)), (Type[]) null, (Type[]) null);
      // ISSUE: method pointer
      this.SubscribeLocalEvent<AnomalyCoreComponent, ComponentHandleState>(new ComponentEventRefHandler<AnomalyCoreComponent, ComponentHandleState>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    }

    private void OnGetState(
      EntityUid uid,
      AnomalyCoreComponent component,
      ref ComponentGetState args)
    {
      ((ComponentGetState) ref args).State = (IComponentState) new AnomalyCoreComponent.AnomalyCoreComponent_AutoState()
      {
        DecayMoment = component.DecayMoment,
        IsDecayed = component.IsDecayed,
        Charge = component.Charge
      };
    }

    private void OnHandleState(
      EntityUid uid,
      AnomalyCoreComponent component,
      ref ComponentHandleState args)
    {
      if (!(((ComponentHandleState) ref args).Current is AnomalyCoreComponent.AnomalyCoreComponent_AutoState current))
        return;
      component.DecayMoment = current.DecayMoment;
      component.IsDecayed = current.IsDecayed;
      component.Charge = current.Charge;
    }
  }
}
